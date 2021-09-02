using System.Text;
using Interfaces;
using System.Text.Json;
using JetBrains.Annotations;

namespace ChatService.Packets
{
    public static class Serializer
    {
        [CanBeNull]
        public static byte[] Serialize<Packet>(Packet packet) where Packet : ServerToClient
        {
            var type = packet.GetType();
            var key = ServerToClientPackets.Get(type);
            if (!string.IsNullOrEmpty(key)) {
                var body = JsonSerializer.Serialize(packet, options: JsonSerializerOption.Option);

                ValueStringBuilder sb = default;
                try {
                    sb = new ValueStringBuilder(200);
                    sb.Append(key);
                    sb.Append(',');
                    sb.Append(body);
                    sb.Append('\n');
                    return Encoding.UTF8.GetBytes(sb.ToString());
                } finally {
                    sb.Dispose();
                }
            }

            return null;
        }
    }
}
