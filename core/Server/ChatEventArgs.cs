using System;
using core.ChatMessageUtilities;

namespace core.Server
{
    public class ChatEventArgs : EventArgs
    {
        /// <summary>
        ///     Constructs a new ChatEventArgs object with a predefined ChatMessage
        /// </summary>
        /// <param name="msg">ChatMessage to pass along</param>
        public ChatEventArgs(ChatMessage msg)
        {
            ServerMessage = msg;
        }

        // ChatMessage contained in this ChatEventArgs object
        public ChatMessage ServerMessage { get; private set; }
    }
}