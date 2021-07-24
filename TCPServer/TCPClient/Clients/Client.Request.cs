using Interfaces;
using Newtonsoft.Json;

namespace ClientLib.Chat.Clients
{
    public partial class Client {
        public void ReqChat(string message)
        {
            var json = JsonConvert.SerializeObject(new PACKET_CS_CHAT {
                Sender = Id,
                Message = message,
            });
            SendPacket($"Chat,{json}");
        }

        public void SendPacket(string msg)
        {
            _socket?.Send(msg);
        }
    }
}
