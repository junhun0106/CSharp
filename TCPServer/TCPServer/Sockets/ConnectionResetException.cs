using System;
using System.IO;

namespace ChatService.Sockets {
    public class ConnectionResetException : IOException {
        public ConnectionResetException()
        {
        }

        public ConnectionResetException(string message) : base(message)
        {
        }

        public ConnectionResetException(string message, Exception inner) : base(message, inner)
        {
        }

        public ConnectionResetException(string message, int hresult) : base(message, hresult)
        {
        }
    }
}
