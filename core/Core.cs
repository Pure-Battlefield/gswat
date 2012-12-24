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

    public class Core : ICore
    {
        // Implements ICore
        public ICommHandler CommHandler { get; set; }
        public CloudTable MessageTable { get; set; }
        public Queue<ChatMessage> MessageQueue { get; set; }

        /// <summary>
        ///     Constructs an instance of Core
        ///     Registers handlers to catch ChatMessage events
        /// </summary>
        /// <param name="comm">CommHandler object to register with</param>
        public Core(ICommHandler comm)
        {
            CommHandler = comm;
            MessageQueue = new Queue<ChatMessage>();
            if (comm != null)
            {
                CommHandler.CoreListener += MessageHandler;
            }
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            CloudTableClient queueClient = storageAccount.CreateCloudTableClient();
            MessageTable = queueClient.GetTableReference("chatMessages");
            MessageTable.CreateIfNotExists();
        }

        // Implements ICore
        public void MessageHandler(object sender, ChatEventArgs e)
        {
            if (e != null)
            {
                MessageQueue.Enqueue(e.ServerMessage);
                if (MessageQueue.Count > 10)
                {
                    MessageQueue.Dequeue();
                }
                TableOperation insertOp = TableOperation.Insert(e.ServerMessage);
                MessageTable.Execute(insertOp);
            }
        }

        // Implements ICore
        public IEnumerable<ChatMessage> GetMessageQueue()
        {
            return MessageQueue.ToList<ChatMessage>();
        }

        // Implements ICore
        public IEnumerable<ChatMessage> GetMoreMessages(int numMessages)
        {
            var query =
                new TableQuery<ChatMessage>().Take(numMessages);
            var output = MessageTable.ExecuteQuery(query).ToList();
            output.Reverse();
            return output;
        }

        // Implements ICore
        public IEnumerable<ChatMessage> GetMessagesFromDate(DateTime date)
        {
            var query =
                new TableQuery<ChatMessage>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                                                                                       QueryComparisons.Equal,
                                                                                       date.Date.ToString("yyyyMMdd")));
            var output = MessageTable.ExecuteQuery(query).ToList();
            output.Reverse();
            return output;
        } 
    }
}