using core.Server;
using core.TableStoreEntities;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace core
{
    public interface ICore
    {
        // CommHandler that this Core instance is hooked to
        ICommLayer CommLayer { get; set; }

        // CloudTable object for ChatMessages
        CloudTable MessageTable { get; set; }

        // CloudTable object for CredMessages
        CloudTable CredTable { get; set; }

        // Current queue of messages
        Queue<ChatMessageEntity> MessageQueue { get; set; }

        // ServerMessage dictionary for filtering
        Dictionary<string, DateTime> ServerMessages { get; set; } 

        /// <summary>
        ///     Retrieves the current message queue
        /// </summary>
        /// <returns>Current queue of ChatMessage objects</returns>
        IEnumerable<ChatMessageEntity> GetMessageQueue();

        /// <summary>
        ///     Returns a larger chunk of messages from TableStore
        /// </summary>
        /// <param name="numMessages">Number of messages to return</param>
        /// <returns>List of last numMessages messages</returns>
        IEnumerable<ChatMessageEntity> GetMoreMessages(int numMessages);

        /// <summary>
        ///     Returns the messages from a given day
        /// </summary>
        /// <param name="date">DateTime object specifying day (all other properties such as Time are ignored)</param>
        /// <returns>List of messages on that day</returns>
        IEnumerable<ChatMessageEntity> GetMessagesFromDate(DateTime date);

        /// <summary>
        ///     Connect to a new server (wipes current cached MessageQueue)
        ///     Currently, this just replaces the CommLayer object with a new one
        /// </summary>
        /// <param name="address">Address of the server</param>
        /// <param name="port">Port to connect to</param>
        /// <param name="password">Password to use</param>
        /// <param name="oldPass">Old password to verify with</param>
        String Connect(string address, int port, string password, string oldPass);

        void LoadExistingConnection();

        /// <summary>
        ///     Query Azure Storage for settings related to the passed input.
        /// </summary>
        /// <param name="partitionKey">The primary IP of the server</param>
        /// <param name="rowKey">The unique port of the server</param>
        ServerSettingsEntity LoadServerSettings(string partitionKey, string rowKey);

        /// <summary>
        /// Bulk adds an enumerable of chat messages to table store.  
        /// NOTE:  This can cause data consistency issues, as records are replaced if duplicate primary key is found.  
        /// </summary>
        /// <param name="messages">The enumerable of messages to add to Table Store.</param>
        void StoreMessagesIntoTableStore(IList<ChatMessageEntity> messages);
    }
}
