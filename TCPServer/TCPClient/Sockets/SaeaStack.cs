using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ClientLib.Chat.Sockets {
    internal class SaeaStack : IDisposable {
        private readonly Stack<SocketAsyncEventArgs> _stack;
        private readonly int _defaultPoolSize;
        private readonly int _bufferSize;

        internal SaeaStack(int poolSize, int bufferSize)
        {
            _defaultPoolSize = poolSize;
            _bufferSize = bufferSize;
            _stack = new Stack<SocketAsyncEventArgs>(poolSize);

            for (int i = 0; i < poolSize; ++i) {
                var saea = new SocketAsyncEventArgs();
                if (bufferSize > 0) {
                    saea.SetBuffer(new byte[bufferSize], 0, bufferSize);
                }
                _stack.Push(saea);
            }
        }

        internal SocketAsyncEventArgs CreateNew()
        {
            var saea = new SocketAsyncEventArgs();
            if (_bufferSize > 0) {
                saea.SetBuffer(new byte[_bufferSize], 0, _bufferSize);
            }
            return saea;
        }

        internal void ForEach(Action<SocketAsyncEventArgs> action)
        {
            lock (_stack) {
                foreach (var saea in _stack) {
                    action(saea);
                }
            }
        }

        internal void Push(SocketAsyncEventArgs saea)
        {
            if (saea == null) {
                return;
            }

            lock (_stack) {
                if (_stack.Count > _defaultPoolSize) {
                    return;
                }
                _stack.Push(saea);
            }
        }

        internal SocketAsyncEventArgs Pop()
        {
            lock (_stack) {
                if (_stack.Count > 0) {
                    return _stack.Pop();
                }
            }
            return null;
        }

        public void Dispose()
        {
            lock (_stack) {
                foreach (var saea in _stack) {
                    saea.SetBuffer(buffer: null, 0, 0);
                    saea.Dispose();
                }
                _stack.Clear();
            }
        }
    }
}
