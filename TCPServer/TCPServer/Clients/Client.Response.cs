using Interfaces;

namespace ChatService.Clients
{
    public partial class Client : IClient
    {
        private void OnPacketReceive(PACKET_CS_ENTER packet)
        {
            _server.DoingJob(new AddPropagate(), this);
        }

        private void OnPacketReceive(PACKET_CS_CHAT packet)
        {
            var chat = new Chat {
                Sender = packet.Sender,
                Message = packet.Message,
            };

            _server.Message(chat);
        }
    }
}
