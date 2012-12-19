using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.ChatMessageUtilities;
using core.Server;

namespace core.ServerInterface
{
    public interface ICommHandler
    {
        // Event handler for receiving Chat message events
        event ChatEventHandler CoreListener;

        // ICommLayer that this ICommHandler is hooked to
        ICommLayer CommLayer { get; set; }

        /// <summary>
        /// Notifies Core of a ChatMessage received from CommLayer
        /// </summary>
        /// <param name="msg">ChatMessage to pass to Core</param>
        void NotifyCore(object sender, ChatEventArgs e);
    }
}
