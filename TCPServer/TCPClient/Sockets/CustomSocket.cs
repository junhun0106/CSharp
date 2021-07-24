using System;
using System.Net.Sockets;
using System.Text;

namespace ClientLib.Chat.Sockets
{
    internal class CustomSocket : IDisposable {
        private const string _logger = "CustomSocket";

        private const int DefaultStackSize = 10;
        private const int DefaultSendBufferSize = 8192 + 4;
        private const int DefaultReceiveBufferSize = 65536 + 4;
        //private const int DefaultReceiveBufferSize = 20;
        //private const int DefaultReceiveBufferSize = 128;
        private const int DefaultReservedBufferSize = 512;

        private Socket _socket;
        private SaeaStack _sendStack;

        private readonly Action<string> _onReceive;
        private Action<SocketError> _onDisconnect;
        private Action<string> _extraLogging;

        private volatile bool _disconnected;
        private readonly object _disconnectObject = new object();
        private volatile bool _reqDisconnecting;
        private readonly string _id;
        public bool Connected => _socket?.Connected ?? false;

        internal CustomSocket(Socket socket, string id, Action<string> onReceive, Action<SocketError> onDisconnect, Action<string> extraLogging)
        {
            _id = id;

            _onReceive = onReceive;
            _onDisconnect = onDisconnect;

            socket.NoDelay = true;
            _socket = socket;

            _sendStack = new SaeaStack(DefaultStackSize, DefaultSendBufferSize);
            _sendStack.ForEach(saea => {
                saea.Completed += Event_IOComplete;
                saea.UserToken = new SendUserToken();
                saea.AcceptSocket = socket;
            });

            _extraLogging = extraLogging;
        }

        public void StartNetwork()
        {
            var recvSaea = new SocketAsyncEventArgs();
            recvSaea.SetBuffer(new byte[DefaultReceiveBufferSize], 0, DefaultReceiveBufferSize);
            recvSaea.SocketError = SocketError.Success;
            recvSaea.AcceptSocket = _socket;
            recvSaea.Completed += Event_IOComplete;
            recvSaea.UserToken = new ReceiveUserToken(DefaultReservedBufferSize);

            BeginReceive(recvSaea);
        }

        public void Send(string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg + '\n');
            Send(bytes);
        }

        private void Send(byte[] bytes)
        {
            if (_disconnected || _reqDisconnecting) {
                return;
            }

            var saea = _sendStack.Pop();
            if (saea == null) {
                saea = _sendStack.CreateNew();
                saea.Completed += Event_IOComplete;
                saea.UserToken = new SendUserToken();
                saea.AcceptSocket = _socket;
            }

            var buffer = saea.Buffer;
            Array.Copy(bytes, buffer, bytes.Length);

            var userToken = (SendUserToken)saea.UserToken;
            userToken.buffer = buffer;
            userToken.bytesSentAlready = 0;
            userToken.sendBytesRemaining = bytes.Length;
            BeginSend(saea);
        }

        public void Disconnect()
        {
            lock (_disconnectObject) {
                if (!_disconnected && !_reqDisconnecting) {
                    var saea = new SocketAsyncEventArgs {
                        DisconnectReuseSocket = false,
                    };
                    saea.Completed += Event_IOComplete;
                    _socket.Shutdown(SocketShutdown.Both);
                    if (!_socket.DisconnectAsync(saea)) {
                        try {
                            _socket?.Close();
                            _socket = null;
                        } catch {
                        }
                    }
                    _reqDisconnecting = true;
                }
            }
        }

        private void BeginReceive(SocketAsyncEventArgs saea)
        {
            if (saea.AcceptSocket == null || !saea.AcceptSocket.Connected) {
                Event_Disconnect(saea.SocketError, "BeginReceive");
                return;
            }

            saea.SetBuffer(0, DefaultReceiveBufferSize);

            bool willRaiseEvent = saea.AcceptSocket.ReceiveAsync(saea);
            if (!willRaiseEvent) {
                _extraLogging?.Invoke($"[{_logger}] BeginReceive Error - RaiseEvent Failed {saea.SocketError}");
                Event_ProcessReceive(saea);
            }
        }

        private void Event_ProcessReceive(SocketAsyncEventArgs saea)
        {
            var token = (ReceiveUserToken)saea.UserToken;
            bool socketError = (saea.SocketError != SocketError.Success && saea.SocketError != SocketError.ConnectionReset);
            if (saea.AcceptSocket == null || !saea.AcceptSocket.Connected || socketError || saea.BytesTransferred == 0) {
                token.Reset();
                Event_Disconnect(saea.SocketError, $"[{_id}] ProcessReceive connected:{saea.AcceptSocket.Connected}, bytesTransferred:{saea.BytesTransferred}");
                return;
            }

            int transferred;
            int remained= transferred = saea.BytesTransferred;
            var buffer = saea.Buffer;
            var offset = 0;
            var storedBytes = token.StoredBytes;
            while (remained > 0) {
                var indexPtr = buffer.IndexOf(offset, transferred, (byte)'\n');
                if (indexPtr != null) {
                    var index = indexPtr.Value;
                    buffer.CopyTo(offset, index, storedBytes);
                    var packetString = Encoding.UTF8.GetString(storedBytes.ToArray());

                    // process receive
                    _onReceive?.Invoke(packetString);

                    // 맨 뒤에 붙은 구분자['\n']는 건너 뛴다 
                    remained = transferred - (index + 1);
                    offset = index + 1;
                    token.Reset();
                } else {
                    buffer.CopyTo(offset, transferred, storedBytes);
                    remained = 0;
                }
            }

            BeginReceive(saea);
        }

        private void BeginSend(SocketAsyncEventArgs saea)
        {
            var sendToken = (SendUserToken)saea.UserToken;
            if (saea.AcceptSocket == null || !saea.AcceptSocket.Connected) {
                Event_Disconnect(saea.SocketError, "BeginSend");
                return;
            }

            saea.SetBuffer(sendToken.bytesSentAlready, sendToken.sendBytesRemaining);

            if (!saea.AcceptSocket.SendAsync(saea)) {
                Event_ProcessSend(saea);
            }
        }

        private void Event_ProcessSend(SocketAsyncEventArgs saea)
        {
            var sendToken = (SendUserToken)saea.UserToken;
            bool socketError = (saea.SocketError != SocketError.Success && saea.SocketError != SocketError.ConnectionReset);
            if (saea.AcceptSocket == null || !saea.AcceptSocket.Connected || socketError) {
                if (socketError) {
                    _extraLogging?.Invoke($"[{_logger}] Event_ProcessSend SocketError - {saea.SocketError} {socketError}");
                }
                Event_Disconnect(saea.SocketError, "Event_ProcessSend");
                return;
            }

            sendToken.sendBytesRemaining -= saea.BytesTransferred;

            // 더 이상 보낼 Byte가 없으면(0보다 같거나 작으면), SendPool에 반환해줍니다. Send Thread 에서 필요하다면 Pop하여 사용합니다.]
            if (sendToken.sendBytesRemaining <= 0) {
                if (sendToken.sendBytesRemaining < 0) {
                    //Log.Fatal(nameof(ServerStreamSocket), "Event_ProcessSend Error Remain Bytes Less then Zero");
                }
                sendToken.Reset();
                _sendStack?.Push(saea);
            } else {
                // So since (receiveSendToken.sendBytesRemaining == 0) is false,
                // we have more bytes to send for this message. So we need to 
                // call StartSend, so we can post another send message.
                sendToken.bytesSentAlready += saea.BytesTransferred;
                BeginSend(saea);
            }
        }

        private void Event_IOComplete(object sender, SocketAsyncEventArgs saea)
        {
            switch (saea.LastOperation) {
                case SocketAsyncOperation.Send: {
                        Event_ProcessSend(saea);
                        break;
                    }
                case SocketAsyncOperation.Receive: {
                        Event_ProcessReceive(saea);
                        break;
                    }
                case SocketAsyncOperation.Disconnect: {
                        Event_Disconnect(saea.SocketError, "Event_IOComplete");
                        break;
                    }

                default: {
                        throw new ArgumentException("\r\nError in I/O Completed");
                    }
            }
        }

        private void Event_Disconnect(SocketError error = SocketError.Success, string caller = "")
        {
            _extraLogging?.Invoke($"[{_logger}] Event_Disconnect {caller} {error}");

            lock (_disconnectObject) {
                if (_disconnected) {
                    return;
                }

                _disconnected = true;

                _extraLogging?.Invoke($"[{_logger}] Event_Disconnect First call {caller} {error}");
                try {
                    if (_socket != null && _socket.Connected) {
                        _socket.Shutdown(SocketShutdown.Both);
                        _socket.Close();
                    }
                } catch (Exception ex) {
                    _extraLogging?.Invoke($"[{_logger}] Event_Disconnect error {ex}");
                }
                _socket = null;
            }

            _sendStack?.ForEach(saea => {
                saea.Completed -= Event_IOComplete;
                saea.AcceptSocket = null;
                saea.SocketError = SocketError.Success;
            });
            _sendStack?.Dispose();
            _sendStack = null;

            _onDisconnect?.Invoke(error);
            _onDisconnect = null;
        }

        public void Dispose()
        {
            _socket?.Close();
            _sendStack?.Dispose();
        }
    }
}
