using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.ChatMessageUtilities;

namespace core.Server
{
    public class ChatEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new ChatEventArgs object with a predefined ChatMessage
        /// </summary>
        /// <param name="msg">ChatMessage to pass along</param>
        public ChatEventArgs(ChatMessage msg)
        {
            ServerMessage = msg;
        }

        // ChatMessage contained in this ChatEventArgs object
        public ChatMessage ServerMessage { get; set; }
    }
}
