using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ClientLib.Chat.Sockets;

namespace ClientLib.Chat.Clients
{
    public partial class Client {
        private Socket _originSocket;
        private CustomSocket _socket;

        private EndPoint _endPoint;

        private IPAddress _address;
        private int _port;

        public bool IsConnected => ConnectState == State.Connect;

        public Action<string> EnteredResponse;

        public void InitNetwork(string ip = "192.168.0.171", int port = 15343)
        {
            var address = GetAddress(ip);
            if (address == null) {
                Logging(ELogLevel.Error, $"wrong ip protocol - {ip}");
                return;
            }

            _address = address;
            _port = port;
            _endPoint = new IPEndPoint(_address, port);
        }

        private IPAddress GetAddress(string hostNameOrAddress)
        {
            var addrs = Dns.GetHostAddresses(hostNameOrAddress);
            IPAddress addr = null;
            if (Socket.OSSupportsIPv6) {
                addr = Array.Find(addrs, x => x.AddressFamily == AddressFamily.InterNetworkV6);
            }
            return addr ?? Array.Find(addrs, x => x.AddressFamily == AddressFamily.InterNetwork);
        }

        public enum State
        {
            Connecting,
            Connect,
            Disconnect,
        }

        private int _connectState = (int)State.Disconnect;
        public State ConnectState {
            get => (State)_connectState;
            set {
                Interlocked.Exchange(ref _connectState, (int)value);
            }
        }

        private void Connect()
        {
            if (ConnectState != State.Disconnect) {
                Logging(ELogLevel.Warn, "duplicate connect");
                return;
            }

            ConnectState = State.Connecting;

            var saea = new SocketAsyncEventArgs {
                RemoteEndPoint = _endPoint,
            };
            saea.Completed += Event_Connect;
            _originSocket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try {
                _originSocket.ConnectAsync(saea);
            } catch (Exception ex) {
                EnteredResponse?.Invoke(ex.Message);
                Logging(ELogLevel.Error, ex);
            }
        }

        public void Reconnect()
        {
            if (IsConnected) {
                return;
            }

            if (this._socket != null) {
                return;
            }

            Connect();
        }

        private void Event_Connect(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success) {
                var socket = new CustomSocket(_originSocket, "test_client", ProcessReceive, ProcessDisconnect, msg => Logging(ELogLevel.Error, msg));
                socket.StartNetwork();
                
                ProcessConnect(socket);
                return;
            }
            ProcessDisconnect(args.SocketError);
        }

        public void EndNetwork()
        {
            _socket?.Disconnect();
            Close();
        }
    }
}
