using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.ChatMessageUtilities;

namespace core
{
    public class CommHandler
    {
        public event ServerMessageEventHandler MessageReceived;
        public delegate void ServerMessageEventHandler(ChatMessage msg);

        public CommHandler()
        {

        }

        public void Test()
        {
            if (MessageReceived != null)
            {
                ChatMessage msg = new ChatMessage();
                msg.Speaker = "test speaker";
                msg.Text = "this is a sample text message";
                msg.Timestamp = new DateTime(2012, 12, 17);
                MessageReceived(msg);
            }
        }
    }
}
