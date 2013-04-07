using System.Collections.Generic;
using core.Server.RConn;

namespace core.Server
{
    public abstract class ICommLayer
    {
        // Event handler for all message types.
        public delegate void MessageEventHandler(object sender, Dictionary<string, string> message);
        // Directory of handlers for various message types.
        public Dictionary<string, MessageEventHandler> MessageEvents;

        /// <summary>
        ///     Connect to a given server
        /// </summary>
        /// <param name="address">IP address of the server</param>
        /// <param name="port">Port number to connect to</param>
        /// <param name="password">Plaintext password to connect with</param>
        public abstract void Connect(string address, int port, string password);

        /// <summary>
        ///     Disconnect server connection
        /// </summary>
        public abstract void Disconnect();

        public abstract void IssueRequest(string requestName, Dictionary<string, string> parameters, MessageEventHandler callback);
    }
}