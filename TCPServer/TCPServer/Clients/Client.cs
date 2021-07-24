using System.Net.Sockets;
using Interfaces;
using ChatService.Sockets;
using Microsoft.Extensions.Logging;

namespace ChatService.Clients
{
    public partial class Client : IClient
    {
        private IServer _server;
        private ICustomSocket _socket;
        private readonly ILogger _logger;

        public int Handle { get; }
        public Client(int handle, Socket socket, IServer server, ILoggerFactory loggerFactory)
        {
            Handle = handle;
            _server = server;
            _socket = new CustomSocket(_server, this, socket, loggerFactory.CreateLogger<CustomSocket>()) {
                OnDisconnect = OnDisconnect,
            };

            _logger = loggerFactory.CreateLogger<Client>();
        }

        public void Disconnect(string caller = "")
        {
            _socket?.Disconnect(caller);
        }

        private void OnDisconnect()
        {
            _server.AddJob(new PropagateJob(EPropagate.Remove, this));
        }

        public void Close()
        {
            _server = null;
            _socket = null;
        }

        public void Send(byte[] data)
        {
            _socket?.SendPacket(data);
        }

        public void Send<T>(T packet) where T : PacketClientBase
        {
            _socket?.SendPacket(packet);
        }
    }
}
