using core.Server;

namespace core.ServerInterface
{
    public interface ICommHandler
    {
        // Event handler for receiving Chat message events

        // ICommLayer that this ICommHandler is hooked to
        ICommLayer CommLayer { get; set; }
        event ChatEventHandler CoreListener;

        /// <summary>
        ///     Notifies Core of a ChatMessage received from CommLayer
        /// </summary>
        /// <param name="sender">Sender of the notification</param>
        /// <param name="e">ChatEventArgs containing message</param>
        void NotifyCore(object sender, ChatEventArgs e);
    }
}