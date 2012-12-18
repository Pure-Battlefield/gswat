using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.ChatMessageUtilities;

namespace core
{
    public class Core
    {
        Queue<ChatMessage> _messageQueue;
        CommHandler _commHandler;
        IAPIHandler _iapiHandler;

        public Core()
        {
            _commHandler = new CommHandler();
            _iapiHandler = new IAPIHandler();
            _messageQueue = new Queue<ChatMessage>();
            _commHandler.MessageReceived += new CommHandler.ServerMessageEventHandler(MessageHandler);
        }

        public void MessageHandler(ChatMessage msg)
        {
            _messageQueue.Enqueue(msg);
        }

        public Queue<ChatMessage> GetMessageQueue()
        {
            return _messageQueue;
        }

        public void Test()
        {
            _commHandler.Test();
        }
    }
}
