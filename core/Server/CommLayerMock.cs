using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.ChatMessageUtilities;

namespace core.Server
{
    public class CommLayerMock : ICommLayer
    {
        // Implements ICommLayer
        public IServerMock Server { get; set; }
        public event ChatEventHandler CommHandler;

        /// <summary>
        /// Constructs a CommLayer object with a predefined server
        /// </summary>
        /// <param name="server">Server for this CommLayer to hook to</param>
        public CommLayerMock(IServerMock server)
        {
            Server = server;
            if (server != null)
            {
                Server.MessageSent += new ChatEventHandler(NotifyCommHandler);
            }
        }

        // Implements ICommLayer
        public void Connect(string address, int port, string password)
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
