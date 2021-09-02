using System;
using ChatService.Clients;
using Interfaces;
using ChatService.Packets;
using Microsoft.Extensions.Logging;

namespace ChatService
{
    public partial class Server : IServer
    {
        public volatile bool IsRunning;
        public PacketHandler Dispatcher { get; }

        public Listener Listen;

        public JobScheduler JobQueue;

        private readonly ClientManager ClientManager;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public Server(ILoggerFactory loggerFactory)
        {
            ClientManager = new ClientManager();
            JobQueue = new JobScheduler(this, loggerFactory);
            Dispatcher = new PacketHandler(loggerFactory);

            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Server>();
        }

        public void Start(int port)
        {
            IsRunning = true;

            // 패킷 핸들러
            Dispatcher.AddHandler(typeof(Client), "OnPacketReceive");
            Listen = new Listener(this, port, _loggerFactory);
            Listen.Start();
            JobQueue.Start();
        }

        public void AddPacket(IClient client, string packetId, ClientToServer packet)
        {
            AddJob(PacketJob.Create(client, packetId, packet));
        }

        public void AddJob(IJob job)
        {
            JobQueue?.AddJob(job);
        }

        public void DoingJob(EPropagate propagate, IClient client)
        {
            Propagate(propagate, client);
        }

        public void DoingJob(IPropagate propagate, IClient client)
        {
            Propagate(propagate, client);
        }

        public void AddWait(IClient client)
        {
            JobQueue?.AddWait(client);
        }

        public void Propagate(IPropagate propagate, IClient client)
        {
            switch (propagate.Propagate) {
                case EPropagate.Add: {
                        ClientManager.Add(client);
                    }
                    break;
                default:
                    Propagate(propagate.Propagate, client);
                    break;
            }
        }

        public void Propagate(EPropagate propagate, IClient client)
        {
            switch (propagate) {
                case EPropagate.Remove: {
                        ClientManager.Remove(client.Handle);
                        client.Close();
                    }
                    break;
                default:
                    _logger.LogError($"unknown propagate type : {propagate}"); break;
            }
        }

        public void Message(Chat chat)
        {
            ClientManager.BroadCast(new PACKET_SC_CHAT {
                Sender = chat.Sender,
                Message = chat.Message,
            });
        }

        public void Close()
        {
            IsRunning = false;

            Listen.Close();

            JobQueue?.Stop();
            JobQueue = null;

            ClientManager.Close();
            Console.WriteLine("exit...");
        }
    }
}
