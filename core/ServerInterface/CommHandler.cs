using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.ChatMessageUtilities;
using core.Server;

namespace core.ServerInterface
{
    public class CommHandler : ICommHandler
    {
        // Implements ICommHandler
        public event ChatEventHandler CoreListener;
        public ICommLayer CommLayer { get; set; }

        /// <summary>
        /// Constructs a CommHandler object with a predefined ICommLayer
        /// </summary>
        /// <param name="comm">ICommLayer to hook this CommHandler to</param>
        public CommHandler(ICommLayer comm)
        {
            CommLayer = comm;
            if (comm != null)
            {
                CommLayer.CommHandler += new ChatEventHandler(NotifyCore);
            }
        }

        public void NotifyCore(object sender, ChatEventArgs e)
        {
            if (CoreListener != null)
            {
                CoreListener(this, e);
            }
        }
    }
}
