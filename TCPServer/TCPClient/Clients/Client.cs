using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using ClientLib.Chat.Packets;
using ClientLib.Chat.Sockets;
using Interfaces;
using Newtonsoft.Json;

namespace ClientLib.Chat.Clients
{
    public sealed partial class Client : IDisposable {
        public readonly string Id;

        public Action<ELogLevel, string> ExtraLogging;

        private readonly PacketHandler _packetHandler;

        public Client(string id, string ip = "localhost", int port = 15344)
        {
            Id = id;

            InitNetwork(ip, port);

            _packetHandler = new PacketHandler();
            _packetHandler.AddHandler(typeof(Client), "OnPacketReceive");
        }

        public void AddHandler(Type type, string methodName)
        {
            _packetHandler?.AddHandler(type, methodName);
        }

        public enum ELogLevel {
            Debug,
            Warn,
            Error,
            Fatal,
        }

        private void Logging(ELogLevel level, object msg)
        {
            Logging(level, $"{msg}");
        }

        private void Logging(ELogLevel level, string msg)
        {
            if (ExtraLogging == null) {
                Console.WriteLine($"[{level}] {msg}");
            } else {
                ExtraLogging(level, msg);
            }
        }

        /// <summary>
        /// 네트워크 스레드
        /// </summary>
        private void ProcessConnect(CustomSocket socket)
        {
            // 네트워크 스레드에서 호출 시에 PostUpdate를 돌릴 수 없는 경우가 있으므로,
            // 곧바로 ConnectState를 변경해준다
            ConnectState = State.Connect;

            AddPostUpdate(() => {
                var json = JsonConvert.SerializeObject(new PACKET_CS_ENTER {
                    FamilyName = Id,
                });
                socket.Send($"Enter,{json}");

                EnteredResponse?.Invoke(obj: null);
                _socket = socket;
            });
        }

        /// <summary> 네트워크 스레드, 재접속을 위해서 패킷 핸들러는 남겨둠 </summary>
        private void ProcessDisconnect(SocketError error)
        {
            // 네트워크 스레드에서 호출 시에 PostUpdate를 돌릴 수 없는 경우가 있으므로,
            // 곧바로 ConnectState를 변경해준다
            ConnectState = State.Disconnect;

            AddPostUpdate(() => {
                Logging(ELogLevel.Error, $"[CHATTING] connect fail : {error} - {_address}:{_port}");
                EnteredResponse?.Invoke(error.ToString());
                Close(reset: false);
            });
        }

        /// <summary> 클라이언트가 완전히 꺼졌을 때 </summary>
        public void Close(bool reset = true)
        {
            if (reset) {
                _packetHandler?.Dispose();
            }

            _socket = null;

            ConnectState = State.Disconnect;
        }

        private readonly ConcurrentQueue<Action> _addPostUpdatedQ = new ConcurrentQueue<Action>();

        public void AddPostUpdate(Action action) => _addPostUpdatedQ.Enqueue(action);

        public void PostUpdate()
        {
            var capture = _addPostUpdatedQ.Count;
            var loopCount = 0;
            while (_addPostUpdatedQ.TryDequeue(out var action)) {
                action();
                loopCount++;
                if (capture <= loopCount) {
                    break;
                }
            }
        }

        public void Update()
        {
            //PostUpdate();

            var capture = _receiveQ.Count;
            var loopCount = 0;
            while (_receiveQ.TryDequeue(out var item)) {
                var id = item.id;
                var packet = item.pks;
                _packetHandler.DispatchPacket(this, id, packet, msg => Logging(ELogLevel.Error, msg));
                loopCount++;
                if (capture <= loopCount) {
                    break;
                }
            }
        }

        public void Dispose()
        {
            _packetHandler?.Dispose();
            _originSocket?.Close();
            _socket?.Dispose();
        }
    }
}
