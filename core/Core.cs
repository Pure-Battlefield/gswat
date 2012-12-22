using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Table;
using core.ChatMessageUtilities;
using core.ServerInterface;
using core.Server;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics;

namespace core
{
    // COMMENT!
    // Handler for mocking ChatEvents
    public delegate void ChatEventHandler(object sender, ChatEventArgs e);

    public class Core
    {
        // Current queue of messages
        public Queue<ChatMessage> MessageQueue;

        // CommHandler that this Core instance is hooked to
        public ICommHandler CommHandler;

        // CloudQueue object for ChatMessages
        public CloudTable MessageTable;

        /// <summary>
        /// Constructs an instance of Core
        /// Registers handlers to catch ChatMessage events
        /// </summary>
        /// <param name="comm">CommHandler object to register with</param>
        /// <param name="storageConnectionString">Storage string for Azure settings</param>
        public Core(ICommHandler comm)
        {
            CommHandler = comm;
            MessageQueue = new Queue<ChatMessage>();
            if (comm != null)
            {
                CommHandler.CoreListener += new ChatEventHandler(MessageHandler);
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            CloudTableClient queueClient = storageAccount.CreateCloudTableClient();
            MessageTable = queueClient.GetTableReference("chatMessages");
            MessageTable.CreateIfNotExists();
        }

        /// <summary>
        /// Process a received message
        /// Triggered by CommHandler object
        /// </summary>
        /// <param name="msg">ChatMessage to process</param>
        public void MessageHandler(object sender, ChatEventArgs e)
        {
            if (e != null)
            {
                //MessageQueue.Enqueue(e.ServerMessage);
                //CloudQueueMessage msg = new CloudQueueMessage(e.ServerMessage.SerializeMe());
                TableOperation insertOp = TableOperation.Insert(e.ServerMessage);
                MessageTable.Execute(insertOp);
            }
        }

        /// <summary>
        /// Retrieves the current message queue
        /// </summary>
        /// <returns>Current queue of ChatMessage objects</returns>
        public IList<ChatMessage> GetMessageQueue()
        {
            string rowKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);
            TableQuery<ChatMessage> query =
                new TableQuery<ChatMessage>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                                                                                       QueryComparisons.Equal,
                                                                                       "PlayerChatMessage"));
            IList<ChatMessage> list = new List<ChatMessage>();
            foreach (ChatMessage msg in MessageTable.ExecuteQuery(query))
            {
                list.Add(msg);
            }
            return list;
            //return MessageQueue.ToList<ChatMessage>();
        }
    }
}
