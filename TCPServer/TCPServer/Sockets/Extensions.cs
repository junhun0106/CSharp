using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Sockets {
    internal static class SocketExtensions {
        public static Task<int> ReceiveAsync(this Socket socket, Memory<byte> memory, SocketFlags socketFlags)
        {
            var arraySegment = GetArray(memory);
            return SocketTaskExtensions.ReceiveAsync(socket, arraySegment, socketFlags);
        }

        public static string GetString(this Encoding encoding, ReadOnlyMemory<byte> memory)
        {
            var arraySegment = GetArray(memory);
            return encoding.GetString(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
        }

        private static ArraySegment<byte> GetArray(Memory<byte> memory)
        {
            return GetArray((ReadOnlyMemory<byte>)memory);
        }

        private static ArraySegment<byte> GetArray(ReadOnlyMemory<byte> memory)
        {
            if (!MemoryMarshal.TryGetArray(memory, out var result)) {
                throw new InvalidOperationException("Buffer backed by array was expected");
            }

            return result;
        }

        public static int Read(this MemoryStream stream, ArraySegment<byte> segment, int offset, int length)
        {
            if ((stream.Length - stream.Position) < length) {
                return 0;
            }

            if (segment.Count < length) {
                return 0;
            }

            Array.Copy(stream.GetBuffer(), (int)stream.Position, segment.Array, segment.Offset + offset, length);
            return length;
        }
    }

    public static class BufferExtensions {
        public static ArraySegment<byte> GetArray(this Memory<byte> memory)
        {
            return ((ReadOnlyMemory<byte>)memory).GetArray();
        }

        public static ArraySegment<byte> GetArray(this ReadOnlyMemory<byte> memory)
        {
            if (!MemoryMarshal.TryGetArray(memory, out var result)) {
                throw new InvalidOperationException("Buffer backed by array was expected");
            }
            return result;
        }
    }
}
