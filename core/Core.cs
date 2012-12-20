using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.ChatMessageUtilities;
using core.ServerInterface;
using core.Server;

namespace core
{
    // Handler for mocking ChatEvents
    public delegate void ChatEventHandler(object sender, ChatEventArgs e);

    public class Core
    {
        // Static handler for web service access
        public static Core StaticInstance;

        // Current queue of messages
        public Queue<ChatMessage> MessageQueue;

        // CommHandler that this Core instance is hooked to
        public ICommHandler CommHandler;

        // IAPIHandler that this Core instance is hooked to
        public IAPI IAPIHandler;

        /// <summary>
        /// Constructs an instance of Core
        /// Registers handlers to catch ChatMessage events
        /// </summary>
        /// <param name="comm">CommHandler object to register with</param>
        public Core(ICommHandler comm)
        {
            CommHandler = comm;
            IAPIHandler = new IAPI();
            MessageQueue = new Queue<ChatMessage>();
            if (comm != null)
            {
                CommHandler.CoreListener += new ChatEventHandler(MessageHandler);
            }
            if (StaticInstance == null)
            {
                StaticInstance = this;
            }
        }

        /// <summary>
        /// Process a received message
        /// Triggered by CommHandler object
        /// </summary>
        /// <param name="msg">ChatMessage to process</param>
        public void MessageHandler(object sender, ChatEventArgs e)
        {
            if (e != null)
            {
                MessageQueue.Enqueue(e.ServerMessage);
            }
        }

        /// <summary>
        /// Retrieves the current message queue
        /// </summary>
        /// <returns>Current queue of ChatMessage objects</returns>
        public Queue<ChatMessage> GetMessageQueue()
        {
            return MessageQueue;
        }
    }
}
