using core.Roles;
using core.Server;
using core.TableStoreEntities;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace core
{
    public interface ICore
    {
        // ServerMessage dictionary for filtering
        ICommLayer CommLayer { get; set; }
        CloudTable MessageTable { get; set; }
        CloudTable CredTable { get; set; }
        Queue<ChatMessageEntity> MessageQueue { get; set; }
        Dictionary<string, DateTime> ServerMessages { get; set; }
        IPermissionsUtility PermissionsUtil { get; set; }
        void SendAdminSay(string message, string playerName = null, string teamId = null, string squadId = null);
        void MessageHandler(object sender, Dictionary<string, string> packet);

        /// <summary>
        /// Bulk adds an enumerable of chat messages to table store.  
        /// NOTE:  This can cause data consistency issues, as records are replaced if duplicate primary key is found.  
        /// </summary>
        /// <param name="messages">The enumerable of messages to add to Table Store.</param>
        void StoreMessagesIntoTableStore(IList<ChatMessageEntity> messages);

        IEnumerable<ChatMessageEntity> GetMessageQueue();
        IEnumerable<ChatMessageEntity> GetMoreMessages(int numMessages);
        IEnumerable<ChatMessageEntity> GetMessagesFromDate(DateTime date);
        String Connect(string address, int port, string password, string oldPass);
        void LoadExistingConnection();

        /// <summary>
        /// Queries TableStore for server settings
        /// </summary>
        /// <param name="partitionKey">Partition key to query for</param>
        /// <param name="rowKey">Row key to query for</param>
        /// <returns>ServerConfig object containing server settings (or null if none was found)</returns>
        ServerSettingsEntity LoadServerSettings(String partitionKey, String rowKey);
    }
}
