using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ChatService.Clients;

namespace ChatService {
    public class ClientList {
        public int Count => _clientHandleDic.Count;
        public IEnumerable<IClient> Clients => _clientHandleDic.Values;

        private readonly ConcurrentDictionary<int, IClient> _clientHandleDic = new ConcurrentDictionary<int, IClient>(Environment.ProcessorCount, 5000);

        public void Add(IClient client)
        {
            _clientHandleDic.TryAdd(client.Handle, client);
        }

        public void Remove(int handle)
        {
            _clientHandleDic.TryRemove(handle, out var _);
        }

        public void Clear()
        {
            _clientHandleDic.Clear();
        }

        public void ForeEach(Action<IClient> action)
        {
            foreach (var client in Clients) {
                action(client);
            }
        }
    }
}
