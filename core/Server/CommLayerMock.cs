namespace core.Server
{
    public class CommLayerMock : ICommLayer
    {
        // Implements ICommLayer
        /// <summary>
        ///     Constructs a CommLayer object with a predefined server
        /// </summary>
        /// <param name="server">Server for this CommLayer to hook to</param>
        public CommLayerMock(IServerMock server)
        {
            Server = server;
            if (server != null)
            {
                Server.MessageSent += NotifyCommHandler;
            }
        }

        public IServerMock Server { get; private set; }
        public event ChatEventHandler CommHandler;

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
        public void Disconnect()
        {
        }
    }
}