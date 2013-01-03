using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using core.ChatMessageUtilities;
using core.Server;
using core.ServerInterface;
using core.TableStore;

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
        public CloudTable CredTable { get; set; }
        public Queue<ChatMessage> MessageQueue { get; set; }
        public Dictionary<string, DateTime> ServerMessages { get; set; } 

        /// <summary>
        ///     Constructs an instance of Core
        ///     Registers handlers to catch ChatMessage events
        /// </summary>
        public Core()
        {
            CommHandler = new CommHandler();
            CommHandler.CoreListener += MessageHandler;

            MessageQueue = new Queue<ChatMessage>();
            ServerMessages = new Dictionary<string, DateTime>();

            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            MessageTable = tableClient.GetTableReference("chatMessages");
            MessageTable.CreateIfNotExists();
        }

        // Implements ICore
        public void MessageHandler(object sender, ChatEventArgs e)
        {
            // Filter for server messages here - do not want the spam
            if (e != null)
            {
                if (e.ServerMessage.Speaker == "Server")
                {
                    if (ServerMessages.ContainsKey(e.ServerMessage.Text))
                    {
                        DateTime lastShown = ServerMessages[e.ServerMessage.Text];
                        DateTime now = DateTime.UtcNow;
                        TimeSpan ts = now - lastShown;
                        double diff = ts.TotalMinutes;
                        if (diff > 30)
                        {
                            // Message needs to be displayed again
                            EnqueueMessage(e.ServerMessage);

                            // Timestamp in Dictionary also needs to be updated
                            ServerMessages[e.ServerMessage.Text] = now;
                        }
                    }
                    else
                    {
                        // Message needs to be displayed, as it has not been seen
                        EnqueueMessage(e.ServerMessage);

                        // Also need to add it to the Dictionary
                        DateTime now = DateTime.UtcNow;
                        ServerMessages.Add(e.ServerMessage.Text, now);
                    }
                }
                else
                {
                    EnqueueMessage(e.ServerMessage);
                }
            }
        }

        private void EnqueueMessage(ChatMessage msg)
        {
            try
            {
                TableOperation insertOp = TableOperation.Insert(msg);
                MessageTable.Execute(insertOp);
                MessageQueue.Enqueue(msg);
                if (MessageQueue.Count > 100)
                {
                    MessageQueue.Dequeue();
                }
            }
            catch (Exception e)
            {
                
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

        // Implements ICore
        public void Connect(string address, int port, string password, string oldPass)
        {
            // Check for a last-saved connection - if present, oldPass must match
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CredTable = tableClient.GetTableReference("serverSettings");
            CredTable.CreateIfNotExists();
            TableOperation retrieveOp = TableOperation.Retrieve<ServerConfig>("Last", "Server");
            TableResult result = CredTable.Execute(retrieveOp);
            if (result.Result != null)
            {
                var settings = (ServerConfig)result.Result;
                // Only overwrite the last-saved connection if the passwords match
                if (settings.Password == oldPass)
                {
                    // Disconnect from current server
                    MessageQueue.Clear();
                    CommHandler.Disconnect();

                    settings.Address = address;
                    settings.Port = port;
                    settings.Password = password;
                    settings.PartitionKey = "Last";
                    settings.RowKey = "Server";
                    TableOperation updateOp = TableOperation.Replace(settings);
                    CredTable.Execute(updateOp);
                    try
                    {
                        CommHandler.Connect(address, port, password);
                    }
                    catch (Exception e) {}
                }
            }
            else
            {
                // Disconnect from current server
                MessageQueue.Clear();
                CommHandler.Disconnect();

                // If there is no last-saved connection, add one
                var settings = new ServerConfig(address, port, password);
                settings.PartitionKey = "Last";
                settings.RowKey = "Server";
                TableOperation insertOp = TableOperation.Insert(settings);
                CredTable.Execute(insertOp);
                try
                {
                    CommHandler.Connect(address, port, password);
                }
                catch (Exception e) {}
            }
        }

        public void LoadExistingConnection()
        {
            // Check for a last-saved connection
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CredTable = tableClient.GetTableReference("serverSettings");
            CredTable.CreateIfNotExists();
            TableOperation retrieveOp = TableOperation.Retrieve<ServerConfig>("Last", "Server");
            TableResult result = CredTable.Execute(retrieveOp);
            if (result.Result != null)
            {
                var settings = (ServerConfig)result.Result;
                try
                {
                    CommHandler.Connect(settings.Address, settings.Port, settings.Password);
                }
                catch (Exception e) {}
            }
        }
    }
}