using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

namespace ChatService.Sockets {
    public abstract class SocketSenderReceiverBase : IDisposable {
        protected readonly Socket _socket;
        protected readonly SocketAwaitableEventArgs _awaitableEventArgs;

        protected SocketSenderReceiverBase(Socket socket)
        {
            _socket = socket;
            _awaitableEventArgs = new SocketAwaitableEventArgs();
        }

        public void Dispose() => _awaitableEventArgs.Dispose();
    }

    public sealed class SocketSender : SocketSenderReceiverBase {
        private List<ArraySegment<byte>> _bufferList;

        public SocketSender(Socket socket) : base(socket)
        {
        }

        public SocketAwaitableEventArgs SendAsync(ReadOnlySequence<byte> buffers)
        {
            if (buffers.IsSingleSegment) {
                return SendAsync(buffers.First);
            }

            if (_awaitableEventArgs.Buffer != null) {
                _awaitableEventArgs.SetBuffer(null, 0, 0);
            }

            _awaitableEventArgs.BufferList = GetBufferList(buffers);

            if (!_socket.SendAsync(_awaitableEventArgs)) {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }

        private SocketAwaitableEventArgs SendAsync(ReadOnlyMemory<byte> memory)
        {
            // The BufferList getter is much less expensive then the setter.
            if (_awaitableEventArgs.BufferList != null) {
                _awaitableEventArgs.BufferList = null;
            }

            var segment = memory.GetArray();

            _awaitableEventArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            if (!_socket.SendAsync(_awaitableEventArgs)) {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }

        private List<ArraySegment<byte>> GetBufferList(ReadOnlySequence<byte> buffer)
        {
            Debug.Assert(!buffer.IsEmpty);
            Debug.Assert(!buffer.IsSingleSegment);

            if (_bufferList == null) {
                _bufferList = new List<ArraySegment<byte>>();
            } else {
                // Buffers are pooled, so it's OK to root them until the next multi-buffer write.
                _bufferList.Clear();
            }

            foreach (var b in buffer) {
                _bufferList.Add(b.GetArray());
            }

            return _bufferList;
        }
    }
}
