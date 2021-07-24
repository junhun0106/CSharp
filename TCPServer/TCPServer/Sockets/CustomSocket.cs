using System;
using System.Linq;
using System.Net.Sockets;
using System.IO.Pipelines;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using JetBrains.Annotations;
using Interfaces;
using ChatService.Packets;
using ChatService.Clients;

namespace ChatService.Sockets
{
    public interface ICustomSocket
    {
        void SendPacket(byte[] data);
        void SendPacket<T>(T msg) where T : PacketClientBase;
        void Disconnect(string caller = "");
    }

    public class CustomSocket : ICustomSocket
    {
        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static readonly bool IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        [NotNull] private IClient _owner;
        [NotNull] private IDispatcher _dispatcher;
        [NotNull] private Socket _socket;
        [NotNull] private readonly SocketSender _sender;
        [NotNull] private readonly object _shutdownLock = new object();
        private volatile bool _socketDisposed;
        private volatile Exception _shutdownReason;
        [NotNull] private readonly ILogger _logger;

        [CanBeNull] public Action OnDisconnect { private get; set; }

        [NotNull]
        private readonly ConcurrentQueue<byte[]> _sendQueue = new ConcurrentQueue<byte[]>();

        public int Id { get; }

        public CustomSocket(IDispatcher dispatcher, IClient owner, Socket socket, ILogger logger)
        {
            _owner = owner;
            _dispatcher = dispatcher;
            _socket = socket;
            _sender = new SocketSender(_socket);
            _logger = logger;
            Task.Factory.StartNew(StartAsync);
        }

        private async Task StartAsync()
        {
            try {
                // connected

                var recvTask = DoReceiveTask();
                var sendTask = DoSendingTask();

                await recvTask.ConfigureAwait(false);
                await sendTask.ConfigureAwait(false);

                recvTask.Dispose();
                sendTask.Dispose();

                _sender.Dispose();

                // disconnected
                OnDisconnect?.Invoke();
            } catch (Exception ex) {
                _logger.LogError($"[{Id}] network thread error - {ex}");
            }
        }

        private async Task DoReceiveTask()
        {
            Exception error = null;
            var pipe = new Pipe();

            try {
                var writing = RecvFillPipeAsync(_socket, pipe.Writer);
                var reading = RecvReadPipeAsync(pipe.Reader);

                await Task.WhenAll(reading, writing).ConfigureAwait(false);
            } catch (SocketException ex) when (IsConnectionResetError(ex.SocketErrorCode)) {
                // This could be ignored if _shutdownReason is already set.
                error = new ConnectionResetException(ex.Message, ex);

                // There's still a small chance that both DoReceive() and DoSend() can log the same connection reset.
                // Both logs will have the same ConnectionId. I don't think it's worthwhile to lock just to avoid this.
                if (!_socketDisposed) {
                    _logger.LogError($"{ex}");
                }
            } catch (Exception ex)
                when ((ex is SocketException socketEx && IsConnectionAbortError(socketEx.SocketErrorCode))
                      || ex is ObjectDisposedException) {
                // This exception should always be ignored because _shutdownReason should be set.
                error = ex;

                if (!_socketDisposed) {
                    // This is unexpected if the socket hasn't been disposed yet.
                    _logger.LogError($"{ex}");
                }
            } catch (Exception ex) {
                // This is unexpected.
                error = ex;
                _logger.LogError($"{ex}");
            } finally {
                Shutdown(error);
                // If Shutdown() has already be called, assume that was the reason ProcessReceives() exited.
                await pipe.Writer.CompleteAsync(_shutdownReason ?? error).ConfigureAwait(false);
            }
        }

        private async Task RecvFillPipeAsync(Socket socket, PipeWriter writer)
        {
            const int minimumBufferSize = 512;

            while (!_socketDisposed) {
                try {
                    var memory = writer.GetMemory(minimumBufferSize);

                    int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None).ConfigureAwait(false);
                    if (bytesRead == 0) {
                        break;
                    }

                    writer.Advance(bytesRead);
                } catch {
                    break;
                }

                FlushResult result = await writer.FlushAsync().ConfigureAwait(false);
                if (result.IsCompleted) {
                    break;
                }
            }

            await writer.CompleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// ServerType.ReadOnly인 경우 네트워크 스레드에서 곧바로 로직을 수행 한다
        /// </summary>
        private readonly static List<string> _directDispatch = new List<string> {
            "Chat",
            "DirectChat",
            "RoomChat",
            "RoomLoad",
            "RoomSearch",
        };

        private async Task RecvReadPipeAsync(PipeReader reader)
        {
            while (!_socketDisposed) {
                ReadResult result = await reader.ReadAsync().ConfigureAwait(false);

                ReadOnlySequence<byte> buffer = result.Buffer;
                SequencePosition? position;
                do {
                    // Look for a EOL in the buffer
                    position = buffer.PositionOf((byte)'\n');

                    if (position != null) {
                        // Process the line
                        var array = buffer.Slice(0, position.Value).ToArray();
                        var (packetId, packet) = GetPacket(array);
                        if (packet == null || string.IsNullOrEmpty(packetId)) {
                            Disconnect("RecvReadPipeAsync.InvalidPacketId");
                            break;
                        }

                        if (_directDispatch.Contains(packetId)) {
                            _dispatcher.Dispatcher.DispatchPacket(_owner, packetId, packet);
                        } else {
                            _dispatcher.AddPacket(_owner, packetId, packet);
                        }

                        // Skip the line + the \n character (basically position)
                        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                    }

                } while (position != null);

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted) {
                    break;
                }
            }

            await reader.CompleteAsync().ConfigureAwait(false);
        }

        private async Task DoSendingTask()
        {
            Exception shutdownReason = null;
            Exception unexpectedError = null;

            var pipe = new Pipe();

            try {
                var pooling = PoolingSocketDataAsync(pipe.Writer);
                var process = ProcessSocketDataAsync(pipe.Reader);

                await Task.WhenAll(pooling, process).ConfigureAwait(false);
            } catch (SocketException ex) when (IsConnectionResetError(ex.SocketErrorCode)) {
                shutdownReason = new ConnectionResetException(ex.Message, ex);
                _logger.LogError($"{ex}");
            } catch (Exception ex)
                when ((ex is SocketException socketEx && IsConnectionAbortError(socketEx.SocketErrorCode))
                      || ex is ObjectDisposedException) {
                // This should always be ignored since Shutdown() must have already been called by Abort().
                shutdownReason = ex;
            } catch (Exception ex) {
                shutdownReason = ex;
                unexpectedError = ex;
                _logger.LogError($"{ex}");
            } finally {
                Shutdown(shutdownReason);

                // Complete the output after disposing the socket
                pipe.Reader.Complete(unexpectedError);

                // Cancel any pending flushes so that the input loop is un-paused
                pipe.Writer.CancelPendingFlush();
            }
        }

        private async Task PoolingSocketDataAsync(PipeWriter writer)
        {
            while (!_socketDisposed) {
                await Task.Delay(10).ConfigureAwait(false);

                int advanced = 0;
                try {
                    while (_sendQueue.TryDequeue(out var data)) {
                        var size = data.Length;
                        var memory = writer.GetMemory(size);
                        var segment = memory.GetArray();
                        Array.Copy(data, 0, segment.Array, segment.Offset, size);
                        writer.Advance(size);
                        advanced += size;
                    }
                } catch (Exception ex) {
                    _logger.LogError($"{ex}");
                    break;
                }

                if (advanced > 0) {
                    FlushResult result = await writer.FlushAsync().ConfigureAwait(false);
                    if (result.IsCompleted) {
                        break;
                    }
                }
            }

            await writer.CompleteAsync().ConfigureAwait(false);
        }

        private async Task ProcessSocketDataAsync(PipeReader reader)
        {
            while (!_socketDisposed) {
                ReadResult result = await reader.ReadAsync().ConfigureAwait(false);

                if (result.IsCanceled) {
                    break;
                }

                var buffer = result.Buffer;

                var end = buffer.End;
                var isCompleted = result.IsCompleted;

                if (!buffer.IsEmpty) {
                    await _sender.SendAsync(buffer);
                }

                reader.AdvanceTo(end);

                if (isCompleted) {
                    break;
                }
            }

            await reader.CompleteAsync().ConfigureAwait(false);
        }

        public void SendPacket<T>(T msg) where T : PacketClientBase
        {
            if (_socketDisposed) return;
            var data = Serializer.Serialize(msg);
            SendPacket(data);
        }

        public void SendPacket(byte[] data)
        {
            if (_socketDisposed) return;
            _sendQueue.Enqueue(data);
        }

        public void Disconnect(string caller = "")
        {
            if (_socketDisposed) {
                return;
            }

            _logger.LogError($"[{Id}] disconnect call - {caller}");
            Shutdown(new Exception($"{caller} call disconnected"));
        }

        private void Shutdown(Exception shutdownReason)
        {
            lock (_shutdownLock) {
                if (_socketDisposed) {
                    return;
                }

                // Make sure to close the connection only after the _aborted flag is set.
                // Without this, the RequestsCanBeAbortedMidRead test will sometimes fail when
                // a BadHttpRequestException is thrown instead of a TaskCanceledException.
                _socketDisposed = true;

                // shutdownReason should only be null if the output was completed gracefully, so no one should ever
                // ever observe the nondescript ConnectionAbortedException except for connection middleware attempting
                // to half close the connection which is currently unsupported.
                _shutdownReason = shutdownReason ?? new ConnectionAbortedException("The Socket transport's send loop completed gracefully.");

                //_trace.ConnectionWriteFin(ConnectionId, _shutdownReason.Message);

                try {
                    // Try to gracefully close the socket even for aborts to match libuv behavior.
                    _socket.Shutdown(SocketShutdown.Both);
                } catch {
                    // Ignore any errors from Socket.Shutdown() since we're tearing down the connection anyway.
                }

                _socket.Dispose();
                _socket = null;

                _logger.LogDebug($"[{Id}] Shutdown - {_shutdownReason.Message}");
            }
        }

        private (string pksId, PacketServerBase pks) GetPacket(byte[] array)
        {
            var packetString = Encoding.UTF8.GetString(array);
            var split = packetString.Split(',');
            if (split.Length < 2) {
                _logger.LogError($"[{Id}] GetPacket - {packetString}");
                return (pksId: null, pks: null);
            }
            var pksId = split[0];
            var body = packetString.Remove(0, pksId.Length + 1);

            try {
                var type = ReceivePackets.Get(pksId);
                var obj = JsonConvert.DeserializeObject(body, type);
                var packet = obj as PacketServerBase;

                return (pksId, packet);
            } catch (Exception ex) {
                _logger.LogError($"[{Id}] GetPacket - {ex}");
            }

            return (pksId: null, pks: null);
        }

        private static bool IsConnectionResetError(SocketError errorCode)
        {
            // A connection reset can be reported as SocketError.ConnectionAborted on Windows.
            // ProtocolType can be removed once https://github.com/dotnet/corefx/issues/31927 is fixed.
            return errorCode == SocketError.ConnectionReset
                   || errorCode == SocketError.Shutdown
                   || (errorCode == SocketError.ConnectionAborted && IsWindows)
                   || (errorCode == SocketError.ProtocolType && IsMacOS);
        }

        private static bool IsConnectionAbortError(SocketError errorCode)
        {
            // Calling Dispose after ReceiveAsync can cause an "InvalidArgument" error on *nix.
            return errorCode == SocketError.OperationAborted
                   || errorCode == SocketError.Interrupted
                   || (errorCode == SocketError.InvalidArgument && !IsWindows);
        }
    }
}
