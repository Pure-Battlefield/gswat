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
                
            }
        }

        public IServerMock Server { get; private set; }

        // Implements ICommLayer
        public override void Connect(string address, int port, string password)
        {
        }

        // Implements ICommLayer
        public override void Disconnect()
        {
        }
    }
}