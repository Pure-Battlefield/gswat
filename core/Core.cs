using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using core.ChatMessageUtilities;
using core.Server;
using core.ServerInterface;

namespace core
{
    // COMMENT!
    // Handler for mocking ChatEvents
    public delegate void ChatEventHandler(object sender, ChatEventArgs e);

    public class Core
    {
        // CommHandler that this Core instance is hooked to
        private readonly ICommHandler _commHandler;

        // CloudTable object for ChatMessages
        private readonly CloudTable _messageTable;

        // Current queue of messages
        private Queue<ChatMessage> _messageQueue;

        /// <summary>
        ///     Constructs an instance of Core
        ///     Registers handlers to catch ChatMessage events
        /// </summary>
        /// <param name="comm">CommHandler object to register with</param>
        public Core(ICommHandler comm)
        {
            _commHandler = comm;
            _messageQueue = new Queue<ChatMessage>();
            if (comm != null)
            {
                _commHandler.CoreListener += MessageHandler;
            }
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            CloudTableClient queueClient = storageAccount.CreateCloudTableClient();
            _messageTable = queueClient.GetTableReference("chatMessages");
            _messageTable.CreateIfNotExists();
        }

        /// <summary>
        ///     Process a received message
        ///     Triggered by CommHandler object
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="e">ChatEventArgs object containing the ChatMessage</param>
        private void MessageHandler(object sender, ChatEventArgs e)
        {
            if (e != null)
            {
                _messageQueue.Enqueue(e.ServerMessage);
                if (_messageQueue.Count > 10)
                {
                    _messageQueue.Dequeue();
                }
                TableOperation insertOp = TableOperation.Insert(e.ServerMessage);
                _messageTable.Execute(insertOp);
            }
        }

        /// <summary>
        ///     Retrieves the current message queue
        /// </summary>
        /// <returns>Current queue of ChatMessage objects</returns>
        public IEnumerable<ChatMessage> GetMessageQueue()
        {
            return _messageQueue.ToList<ChatMessage>();
        }

        /// <summary>
        ///     Returns a larger chunk of messages from TableStore
        /// </summary>
        /// <param name="numMessages">Number of messages to return</param>
        /// <returns>List of last numMessages messages</returns>
        public IEnumerable<ChatMessage> GetMoreMessages(int numMessages)
        {
            var query =
                new TableQuery<ChatMessage>().Take(numMessages);
            var output = _messageTable.ExecuteQuery(query).ToList();
            output.Reverse();
            return output;
        }

        /// <summary>
        ///     Returns the messages from a given day
        /// </summary>
        /// <param name="date">DateTime object specifying day (all other properties such as Time are ignored)</param>
        /// <returns>List of messages on that day</returns>
        public IEnumerable<ChatMessage> GetMessagesFromDate(DateTime date)
        {
            var query =
                new TableQuery<ChatMessage>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                                                                                       QueryComparisons.Equal,
                                                                                       date.Date.ToString("yyyyMMdd")));
            var output = _messageTable.ExecuteQuery(query).ToList();
            output.Reverse();
            return output;
        } 
    }
}