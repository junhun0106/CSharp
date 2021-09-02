using System.Collections.Generic;
using ChatService.Clients;
using Interfaces;
using ChatService.Packets;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ChatService
{
    public class ClientManager {
        public int Count { get => _clients.Count; }

        private readonly ClientList _clients;
        public IEnumerable<IClient> Clients => _clients.Clients;

        public ClientManager()
        {
            _clients = new ClientList();
        }

        /// <summary> 클라이언트의 최초 접속 시 호출 된다 </summary>
        public void Add(IClient client)
        {
            _clients.Add(client);
        }

        /// <summary> 클라이언트의 접속 종료 시 호출 된다 </summary>
        public void Remove(int handle)
        {
            _clients.Remove(handle);
        }

        public void BroadCast<T>(T packet) where T : ServerToClient
        {
            var bytes = Serializer.Serialize(packet);
            _clients.ForeEach(forEach);
            
            // GC 방지
            void forEach(IClient client)
            {
                client.Send(bytes);
            }
        }

        public void Close()
        {
            _clients.Clear();
        }
    }
}
