using core.Server;

namespace core.ServerInterface
{
    public class CommHandler : ICommHandler
    {
        // Implements ICommHandler
        /// <summary>
        ///     Constructs a CommHandler object with a predefined ICommLayer
        /// </summary>
        /// <param name="comm">ICommLayer to hook this CommHandler to</param>
        public CommHandler(ICommLayer comm)
        {
            CommLayer = comm;
            if (comm != null)
            {
                CommLayer.CommHandler += NotifyCore;
            }
        }

        public event ChatEventHandler CoreListener;
        public ICommLayer CommLayer { get; set; }

        public void NotifyCore(object sender, ChatEventArgs e)
        {
            if (CoreListener != null)
            {
                CoreListener(this, e);
            }
        }
    }
}