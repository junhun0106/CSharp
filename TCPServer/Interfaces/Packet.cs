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

    public abstract class PacketServerBase
    {
        public override string ToString()
        {
            return GetType().BaseType?.Name ?? nameof(PacketServerBase);
        }
    }

    [PacketServer("Enter")]
    public class PACKET_CS_ENTER : PacketServerBase
    {
        public string FamilyName;
    }

    [PacketServer("Chat")]
    public class PACKET_CS_CHAT : PacketServerBase
    {
        public string Sender;
        public string Message;
    }

    public abstract class PacketClientBase { }

    [PacketClient("Chat")]
    public class PACKET_SC_CHAT : PacketClientBase
    {
        public string Sender;
        public string Message;
    }
}
