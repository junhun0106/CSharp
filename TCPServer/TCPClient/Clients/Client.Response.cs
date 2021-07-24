using System;
using System.Collections.Concurrent;
using ClientLib.Chat.Structures;
using Interfaces;
using Newtonsoft.Json;

namespace ClientLib.Chat.Clients
{
    public partial class Client
    {
        private readonly ConcurrentQueue<(string id, PacketClientBase pks)> _receiveQ = new ConcurrentQueue<(string, PacketClientBase)>();

        internal void ProcessReceive(string packetString)
        {
            var split = packetString.TrimStart().Split(',');
            if (split.Length > 1) {
                var pksId = split[0];
                try {
                    var type = SendPackets.Get(pksId);
                    var body = packetString.Remove(0, pksId.Length + 1);
                    var obj = JsonConvert.DeserializeObject(body, type);
                    if (obj is PacketClientBase pks) {
                        _receiveQ.Enqueue((pksId, pks));
                        return;
                    }
                } catch (Exception e) {
                    Logging(ELogLevel.Error, $"[Client] error - packet parse - {e}");
                }
            }

            EndNetwork();
            Logging(ELogLevel.Error, $"[Client] error - not found packet - {packetString}");
        }

        private void OnPacketReceive(PACKET_SC_CHAT packet)
        {
            var chat = new ChatMessage {
                SenderName = packet.Sender,
                Message = packet.Message,
            };

            ReceiveChatEvent?.Invoke(chat);
        }
    }
}
