using System;

namespace ChatService.Sockets {
    public class ConnectionAbortedException : OperationCanceledException {
        public ConnectionAbortedException() :
            this("The connection was aborted")
        {
        }

        public ConnectionAbortedException(string message) : base(message)
        {
        }

        public ConnectionAbortedException(string message, Exception inner) : base(message, inner)
        {
        }

        public ConnectionAbortedException(System.Threading.CancellationToken token) : base(token)
        {
        }

        public ConnectionAbortedException(string message, System.Threading.CancellationToken token) : base(message, token)
        {
        }

        public ConnectionAbortedException(string message, Exception innerException, System.Threading.CancellationToken token) : base(message, innerException, token)
        {
        }
    }
}
