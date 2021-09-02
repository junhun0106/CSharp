using ChatService.Clients;
using Interfaces;
using System;

namespace ChatService
{
    public enum EJob
    {
        Packet,
        Action,
        Propagate,
    }
    public interface IJob
    {
        EJob Type { get; }
    }

    public class PacketJob : IJob
    {
        public readonly IClient Client;
        public readonly string PacketId;
        public readonly ClientToServer Packet;

        public EJob Type { get; } = EJob.Packet;

        public PacketJob(IClient client, string packetId, ClientToServer packet)
        {
            Client = client;
            PacketId = packetId;
            Packet = packet;
        }

        public static PacketJob Create(IClient client, string packetId, ClientToServer packet)
        {
            return new PacketJob(client, packetId, packet);
        }
    }

    public class ActionJob : IJob
    {
        public EJob Type { get; } = EJob.Action;

        public readonly Action Action;

        public ActionJob(Action action)
        {
            Action = action;
        }

        public static ActionJob Create(Action action)
        {
            return new ActionJob(action);
        }
    }

    public class PropagateJob : IJob
    {
        public EJob Type { get; } = EJob.Propagate;

        public readonly EPropagate Propagate;

        public readonly IClient Client;

        public PropagateJob(EPropagate propagate, IClient client)
        {
            Propagate = propagate;
            Client = client;
        }
    }
}
