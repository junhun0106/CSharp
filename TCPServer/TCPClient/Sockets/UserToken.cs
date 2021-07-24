using System.Collections.Generic;

namespace ClientLib.Chat.Sockets {
    public class SendUserToken {
        public int sendBytesRemaining;
        public int bytesSentAlready;
        public byte[] buffer;

        public void Reset()
        {
            sendBytesRemaining = 0;
            bytesSentAlready = 0;
            buffer = null;
        }
    }

    public class ReceiveUserToken {
        /// <summary> 지금까지 받은 byte 리스트 </summary>
        public readonly List<byte> StoredBytes;

        public ReceiveUserToken(int reserved)
        {
            StoredBytes = new List<byte>(reserved);
        }

        public void Reset()
        {
            StoredBytes.Clear();
        }
    }
}
