using core.ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.ChatMessageUtilities;

namespace core.Server
{
    public interface ICommLayer
    {
        // Mocked server that this ICommLayer is hooked to
        IServerMock Server { get; set; }

        // Event handler to pass ChatMessages to CommHandler
        event ChatEventHandler CommHandler;

        /// <summary>
        /// Connect to a given server
        /// </summary>
        /// <param name="address">IP address of the server</param>
        /// <param name="port">Port number to connect to</param>
        void Connect(string address, uint port);

        /// <summary>
        /// Notifies Core with a ChatMessage
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">ChatEventArgs that contains the ChatMessage to be sent</param>
        void NotifyCommHandler(object sender, ChatEventArgs e);

        /// <summary>
        /// Close server connection
        /// </summary>
        void close();
    }
}
