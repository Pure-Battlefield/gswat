using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using core.Logging;
using core.Roles;
using core.Server;
using core.TableStoreEntities;

namespace core
{
    // Handler for mocking ChatEvents
    public delegate void ChatEventHandler(object sender, ChatEventArgs e);

    public class Core : ICore
    {
        // Implements ICore
        public ICommLayer CommLayer { get; set; }
        public CloudTable MessageTable { get; set; }
        
        public CloudTable CredTable { get; set; }
        public Queue<ChatMessageEntity> MessageQueue { get; set; }
        public Dictionary<string, DateTime> ServerMessages { get; set; }
        public IPermissionsUtility PermissionsUtil { get; set; }

        /// <summary>
        ///     Constructs an instance of Core
        ///     Registers handlers to catch ChatMessage events
        /// </summary>
        public Core(IPermissionsUtility permsUtility)
        {
            CommLayer = new CommLayer();
            CommLayer.MessageEvents["player.onChat"] = MessageHandler;

            MessageQueue = new Queue<ChatMessageEntity>();
            ServerMessages = new Dictionary<string, DateTime>();

            // Connect to storage
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();

            // Create message table if it does not exist
            MessageTable = tableClient.GetTableReference("chatMessages");
            MessageTable.CreateIfNotExists();

            // Create cred table if it does not exist
            CredTable = tableClient.GetTableReference("serverSettings");
            CredTable.CreateIfNotExists();

            // Create role manager if it does not exist
            PermissionsUtil = permsUtility;
            PermissionsUtil.LoadPermissionsFromConfig();

            //Attempt to load existing connection.
            LoadExistingConnection();
        }

        public void SendAdminSay(string message, string playerName = null, string teamId = null, string squadId = null)
        {
            if (message.Length > 128)
            {
                throw new ArgumentException("Message length over maximum message size.");
            }
            var args = new Dictionary<string, string>();
            var playersSubset = "";

            if (playerName != null)
            {
                if (squadId != null || teamId != null)
                {
                    throw new ArgumentException("Cannot specify both playerName and Team/Squad.");
                }
                playersSubset = String.Format("player {0}", playerName);
            }
            else if (squadId != null)
            {
                if (teamId == null)
                {
                    throw new ArgumentException("teamId cannot be null when specifying squadId");
                }

                playersSubset = String.Format("sqaud {0} {1}", teamId, squadId);
            }
            else if (teamId != null)
            {
                playersSubset = String.Format("team {0}", teamId);
            }
            else
            {
                playersSubset = String.Format("all");
            }

            args.Add("players", playersSubset);
            args.Add("message", message);
            CommLayer.IssueRequest("admin.say", args, null);
        }

        // Implements ICore
        public void MessageHandler(object sender, Dictionary<string, string> packet)
        {
            // Filter for server messages here - do not want the spam
            if (packet != null)
            {
                ChatMessageEntity msg = new ChatMessageEntity(DateTime.UtcNow, packet["source soldier name"], packet["text"], packet["target players"]);
                if (msg.Speaker == "Server")
                {
                    if (ServerMessages.ContainsKey(msg.Text))
                    {
                        DateTime lastShown = ServerMessages[msg.Text];
                        DateTime now = DateTime.UtcNow;
                        TimeSpan ts = now - lastShown;
                        double diff = ts.TotalMinutes;
                        
                        if (diff > 30)
                        {
                            // Message needs to be displayed again
                            EnqueueMessage(msg);

                            // Timestamp in Dictionary also needs to be updated
                            ServerMessages[msg.Text] = now;
                        }
                    }
                    else
                    {
                        // Message needs to be displayed, as it has not been seen
                        EnqueueMessage(msg);

                        // Also need to add it to the Dictionary
                        DateTime now = DateTime.UtcNow;
                        ServerMessages.Add(msg.Text, now);
                    }
                }
                else
                {
                    EnqueueMessage(msg);
                }
            }
        }

        /// <summary>
        /// Enqueues a message into both Table Store and the current cached queue
        /// </summary>
        /// <param name="msg"></param>
        private void EnqueueMessage(ChatMessageEntity msg)
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
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
            }
        }

        /// <summary>
        /// Bulk adds an enumerable of chat messages to table store.  
        /// NOTE:  This can cause data consistency issues, as records are replaced if duplicate primary key is found.  
        /// </summary>
        /// <param name="messages">The enumerable of messages to add to Table Store.</param>
        public void StoreMessagesIntoTableStore(IList<ChatMessageEntity> messages)
        {
            var runs = 0;
            while (messages.Count / 100 >= runs)
            {
                var batchOp = new TableBatchOperation();
                for (var i = 0; i < Math.Min(100, messages.Count - (runs * 100)); i++)
                {
                    batchOp.InsertOrReplace(messages[runs*100 + i]);
                }
                ++runs;
                MessageTable.ExecuteBatch(batchOp);
            }
        }

        // Implements ICore
        public IEnumerable<ChatMessageEntity> GetMessageQueue()
        {
            return MessageQueue.ToList();
        }

        // Implements ICore
        public IEnumerable<ChatMessageEntity> GetMoreMessages(int numMessages)
        {
            var query =
                new TableQuery<ChatMessageEntity>().Take(numMessages);
            var output = MessageTable.ExecuteQuery(query).ToList();
            output.Reverse();
            return output;
        }

        // Implements ICore
        public IEnumerable<ChatMessageEntity> GetMessagesFromDate(DateTime date)
        {
            var query =
                new TableQuery<ChatMessageEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
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
            var oldsettings = LoadServerSettings("Last", "Server");

            if (oldsettings != null)
            {
                // Only overwrite the last-saved connection if the passwords match
                if (oldsettings.Password == oldPass)
                {
                    try
                    {
                        // Disconnect from current server
                        MessageQueue.Clear();
                        CommLayer.Disconnect();

                        // Attempt to connect to the new server - if this fails, we leave the old "last server" entry in Table Store
                        CommLayer.Connect(address, port, password);

                        // Update the "last server" entry in Table Store
                        oldsettings.Address = address;
                        oldsettings.Port = port;
                        oldsettings.Password = password;
                        oldsettings.PartitionKey = "Last";
                        oldsettings.RowKey = "Server";
                        var updateOp = TableOperation.Replace(oldsettings);
                        CredTable.Execute(updateOp);

                        return "Connected to " + address;
                    }
                    catch (Exception e)
                    {
                        LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);

                        // Oops, the Connect failed - reconnect to our last known server
                        LoadExistingConnection();
                        throw new ArgumentException(e.Message);
                    }
                }
                throw new ArgumentException("Old password was incorrect.");
            }
            try
            {
                // Disconnect from current server
                MessageQueue.Clear();
                CommLayer.Disconnect();
                    
                // Attempt to connect to new server
                // If this fails, we do not change the last-saved connection
                    CommLayer.Connect(address, port, password);

                // Add the new "last server" entry in Table Store
                var settings = new ServerSettingsEntity(address, port, password)
                    {
                        PartitionKey = "Last",
                        RowKey = "Server"
                    };
                var insertOp = TableOperation.Insert(settings);
                CredTable.Execute(insertOp);

                return "Connected to " + address;
            }
            catch (Exception e)
            {
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);

                // Oops, the Connect failed - reconnect to our last known server
                LoadExistingConnection();
                throw new ArgumentException(e.Message);
            }
        }

        public void LoadExistingConnection()
        {
            // Check for a last-saved connection
            ServerSettingsEntity settings = LoadServerSettings("Last", "Server");

            if (settings != null)
            {
                try
                {
                    CommLayer.Connect(settings.Address, settings.Port, settings.Password);
                }
                catch (Exception e) {
                    LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        /// <summary>
        /// Queries TableStore for server settings
        /// </summary>
        /// <param name="partitionKey">Partition key to query for</param>
        /// <param name="rowKey">Row key to query for</param>
        /// <returns>ServerConfig object containing server settings (or null if none was found)</returns>
        public ServerSettingsEntity LoadServerSettings(String partitionKey, String rowKey)
        {
            var retrieveOp = TableOperation.Retrieve<ServerSettingsEntity>(partitionKey, rowKey);
            var result = CredTable.Execute(retrieveOp);

            if (result.Result != null)
            {
                return (ServerSettingsEntity)result.Result;
            }
            return null;
        }

        public bool ValidateUser(string token, PermissionSetEntity permissionSet)
        {
            return PermissionsUtil.ValidateUser(token, permissionSet);
        }

        public void AddorUpdateUser(UserEntity user)
        {
            PermissionsUtil.AddorUpdateUser(user);
        }
    }
}