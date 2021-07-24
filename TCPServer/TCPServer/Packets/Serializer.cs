using System.Reflection;
using System.Text;
using Interfaces;
using Newtonsoft.Json;

namespace ChatService.Packets
{
    public static class Serializer
    {
        public static byte[] Serialize<Packet>(Packet packet) where Packet : PacketClientBase
        {
            var type = packet.GetType();
            var attr = type.GetCustomAttribute<PacketClientAttribute>();
            var key = attr.Name;
            var body = JsonConvert.SerializeObject(packet);
            var format = $"{key},{body}";
            var bytes = Encoding.UTF8.GetBytes(format + '\n');
            return bytes;
        }
    }
}
