using System.Collections.Generic;

namespace core.Server
{
    public interface ICommLayer
    {
        // Event handler to pass ChatMessages to CommHandler
        event ChatEventHandler CommHandler; 

        /// <summary>
        ///     Connect to a given server
        /// </summary>
        /// <param name="address">IP address of the server</param>
        /// <param name="port">Port number to connect to</param>
        /// <param name="password">Plaintext password to connect with</param>
        void Connect(string address, int port, string password);

        /// <summary>
        ///     Notifies Core with a ChatMessage
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">ChatEventArgs that contains the ChatMessage to be sent</param>
        void NotifyCommHandler(object sender, ChatEventArgs e);

        /// <summary>
        ///     Disconnect server connection
        /// </summary>
        void Disconnect();
    }
}