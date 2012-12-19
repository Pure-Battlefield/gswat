using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.ChatMessageUtilities;

namespace core.Server
{
    public class CommLayer : ICommLayer
    {
        // Implements ICommLayer
        public IServerMock Server { get; set; }
        public event ChatEventHandler CommHandler;

        /// <summary>
        /// Constructs a CommLayer object with a predefined server
        /// </summary>
        /// <param name="server">Server for this CommLayer to hook to</param>
        public CommLayer(IServerMock server)
        {
            Server = server;
            if (server != null)
            {
                Server.MessageSent += new ChatEventHandler(NotifyCommHandler);
            }
        }

        // Implements ICommLayer
        public void Connect(string address, uint port)
        {

        }

        // Implements ICommLayer
        public void NotifyCommHandler(object sender, ChatEventArgs e)
        {
            if (CommHandler != null)
            {
                CommHandler(this, e);
            }
        }

        // Implements ICommLayer
        public void close()
        {

        }
    }
}
