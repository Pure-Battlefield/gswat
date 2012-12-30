using core.Server;

namespace core.ServerInterface
{
    public class CommHandler : ICommHandler
    {
        public event ChatEventHandler CoreListener;
        public ICommLayer CommLayer { get; set; }

        // Implements ICommHandler
        public void NotifyCore(object sender, ChatEventArgs e)
        {
            if (CoreListener != null)
            {
                CoreListener(this, e);
            }
        }

        // Implements ICommHandler
        public void Connect(string address, int port, string password)
        {
            CommLayer = new CommLayer();
            CommLayer.CommHandler += NotifyCore;
            CommLayer.Connect(address, port, password);
        }

        // Implements ICommHandler
        public void Disconnect()
        {
            if (CommLayer != null)
            {
                CommLayer.Disconnect();
                CommLayer.CommHandler -= NotifyCore;
            }
        }
    }
}