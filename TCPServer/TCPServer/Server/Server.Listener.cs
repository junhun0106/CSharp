using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatService.Clients;
using Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatService
{
    public class Listener {
        private int _handle;

        private readonly int _port;
        private Socket _listenSocket;

        private readonly IServer _server;

        private const int WaitListeningCount = 400;

        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public Listener(IServer server, int port, ILoggerFactory loggerFactory)
        {
            _server = server;
            _port = port;

            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Listener>();
        }

        public void Start()
        {
            // 패킷 파싱
            var count = ReceivePackets.Count;

            _logger.LogInformation($"Packet Init - count : {count}");

            ListenAsync(_port);
            _logger.LogInformation("listen...");
        }

        private Task ListenAsync(int port)
        {
            _listenSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            var endPoint = new IPEndPoint(IPAddress.IPv6Any, port);
            _listenSocket.Bind(endPoint);

            try {
                _listenSocket.Listen(WaitListeningCount);
            } catch (Exception ex) {
                _logger.LogError($"{ex}");
            }

            return Task.Factory.StartNew(AcceptAsync, _listenSocket);
        }

        private async Task AcceptAsync(object obj)
        {
            var listenSocket = (Socket)obj;
            while (true) {
                var socket = await listenSocket.AcceptAsync().ConfigureAwait(false);
                socket.NoDelay = true;
                Accept(socket);
                await Task.Delay(0).ConfigureAwait(false);
            }
        }

        private void Accept(Socket socket)
        {
            var client = new Client(Interlocked.Increment(ref _handle), socket, _server, _loggerFactory);
            _server.AddWait(client);
        }

        public void Close()
        {
            _listenSocket.Close();
        }
    }
}
