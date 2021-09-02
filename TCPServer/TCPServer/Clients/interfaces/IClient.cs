using Interfaces;

namespace ChatService.Clients
{
    public interface IClient
    {
        int Handle { get; }

        void Disconnect(string caller = "");

        void Send(byte[] data);
        void Send<T>(T packet) where T : ServerToClient;

        void Close();
    }
}
