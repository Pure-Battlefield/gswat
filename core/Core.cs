using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using core.Server;
using core.ServerInterface;
using core.TableStoreEntities;

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
        public CloudTable LogTable { get; set; }
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

            // Create chatMessage table if it does not exist
            var storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();
            MessageTable = tableClient.GetTableReference("chatMessages");
            MessageTable.CreateIfNotExists();

            // Create log table if it does not exist
            LogTable = tableClient.GetTableReference("serverLogs");
            LogTable.CreateIfNotExists();

            // Create cred table if it does not exist
            CredTable = tableClient.GetTableReference("serverSettings");
            CredTable.CreateIfNotExists();
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
                        var lastShown = ServerMessages[e.ServerMessage.Text];
                        var now = DateTime.UtcNow;
                        var ts = now - lastShown;
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
                        var now = DateTime.UtcNow;
                        ServerMessages.Add(e.ServerMessage.Text, now);
                    }
                }
                else
                {
                    EnqueueMessage(e.ServerMessage);
                }
            }
        }

        /// <summary>
        /// Enqueues a message into both Table Store and the current cached queue
        /// </summary>
        /// <param name="msg"></param>
        private void EnqueueMessage(ChatMessage msg)
        {
            try
            {
                var insertOp = TableOperation.Insert(msg);
                MessageTable.Execute(insertOp);
                MessageQueue.Enqueue(msg);
                if (MessageQueue.Count > 100)
                {
                    MessageQueue.Dequeue();
                }
            }
            catch (Exception e)
            {
                Log(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
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
        public String Connect(string address, int port, string password, string oldPass)
        {
            // Check for a last-saved connection - if present, oldPass must match
            var Oldsettings = LoadServerSettings("Last", "Server");

            if (Oldsettings != null)
            {
                // Only overwrite the last-saved connection if the passwords match
                if (Oldsettings.Password == oldPass)
                {
                    try
                    {
                        // Disconnect from current server
                        MessageQueue.Clear();
                        CommHandler.Disconnect();

                        // Attempt to connect to the new server - if this fails, we leave the old "last server" entry in Table Store
                        CommHandler.Connect(address, port, password);

                        // Update the "last server" entry in Table Store
                        Oldsettings.Address = address;
                        Oldsettings.Port = port;
                        Oldsettings.Password = password;
                        Oldsettings.PartitionKey = "Last";
                        Oldsettings.RowKey = "Server";
                        var updateOp = TableOperation.Replace(Oldsettings);
                        CredTable.Execute(updateOp);

                        return "Connected to " + address;
                    }
                    catch (Exception e)
                    {
                        Log(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);

                        // Oops, the Connect failed - reconnect to our last known server
                        LoadExistingConnection();
                        throw new ArgumentException(e.Message);
                    }
                }
                else
                {
                    throw new ArgumentException("Old password was incorrect.");
                }
            }
            else
            {
                try
                {
                    // Disconnect from current server
                    MessageQueue.Clear();
                    CommHandler.Disconnect();
                    
                    // Attempt to connect to new server
                    // If this fails, we do not change the last-saved connection
                    CommHandler.Connect(address, port, password);

                    // Add the new "last server" entry in Table Store
                    var settings = new ServerConfig(address, port, password);
                    settings.PartitionKey = "Last";
                    settings.RowKey = "Server";
                    var insertOp = TableOperation.Insert(settings);
                    CredTable.Execute(insertOp);

                    return "Connected to " + address;
                }
                catch (Exception e)
                {
                    Log(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);

                    // Oops, the Connect failed - reconnect to our last known server
                    LoadExistingConnection();
                    throw new ArgumentException(e.Message);
                }
            }
        }

        public void LoadExistingConnection()
        {
            // Check for a last-saved connection
            ServerConfig settings = LoadServerSettings("Last", "Server");

            if (settings != null)
            {
                try
                {
                    CommHandler.Connect(settings.Address, settings.Port, settings.Password);
                }
                catch (Exception e) {
                    Log(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        /// <summary>
        /// Queries TableStore for server settings
        /// </summary>
        /// <param name="partitionKey">Partition key to query for</param>
        /// <param name="rowKey">Row key to query for</param>
        /// <returns>ServerConfig object containing server settings (or null if none was found)</returns>
        public ServerConfig LoadServerSettings(String partitionKey, String rowKey)
        {
            var retrieveOp = TableOperation.Retrieve<ServerConfig>(partitionKey, rowKey);
            var result = CredTable.Execute(retrieveOp);

            if (result.Result != null)
            {
                return (ServerConfig)result.Result;
            }
            return null;
        }

        /// <summary>
        /// Log 
        /// </summary>
        /// <param name="funcName">Current function on the stack</param>
        /// <param name="message">Message to log</param>
        public void Log(String funcName, String message)
        {
            try
            {
                var msg = new LogMessage(DateTime.UtcNow, funcName, message);
                var insertOp = TableOperation.Insert(msg);
                LogTable.Execute(insertOp);
            }
            catch (Exception e)
            {
                // If we somehow reach this point, we have failed as software developers
            }
        }
    }
}