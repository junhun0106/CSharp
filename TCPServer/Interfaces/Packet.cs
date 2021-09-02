using System;

namespace Interfaces
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class PacketServerAttribute : Attribute
    {
        public readonly string Name;

        public PacketServerAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class PacketClientAttribute : Attribute
    {
        public readonly string Name;

        public PacketClientAttribute(string name)
        {
            Name = name;
        }
    }

    public abstract class ClientToServer
    {
        public override string ToString()
        {
            return GetType().BaseType?.Name ?? nameof(ClientToServer);
        }
    }

    [PacketServer("Enter")]
    public class PACKET_CS_ENTER : ClientToServer
    {
        public string FamilyName;
    }

    [PacketServer("Chat")]
    public class PACKET_CS_CHAT : ClientToServer
    {
        public string Sender;
        public string Message;
    }

    public abstract class ServerToClient { }

    [PacketClient("Chat")]
    public class PACKET_SC_CHAT : ServerToClient
    {
        public string Sender;
        public string Message;
    }
}
