using System;
using ClientLib.Chat.Structures;

namespace ClientLib.Chat.Clients
{
    public partial class Client
    {
        public Action<ChatMessage> ReceiveChatEvent;
    }
}
