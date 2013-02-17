using core.Server;
using core.ServerInterface;
using core.TableStoreEntities;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace core
{
    public interface ICore
    {
        // CommHandler that this Core instance is hooked to
        ICommHandler CommHandler { get; set; }

        // CloudTable object for ChatMessages
        CloudTable MessageTable { get; set; }

        // CloudTable object for LogMessages
        CloudTable LogTable { get; set; }

        // CloudTable object for CredMessages
        CloudTable CredTable { get; set; }

        // Current queue of messages
        Queue<ChatMessage> MessageQueue { get; set; }

        // ServerMessage dictionary for filtering
        Dictionary<string, DateTime> ServerMessages { get; set; } 

        /// <summary>
        ///     Process a received message
        ///     Triggered by CommHandler object
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="e">ChatEventArgs object containing the ChatMessage</param>
        void MessageHandler(object sender, ChatEventArgs e);

        /// <summary>
        ///     Retrieves the current message queue
        /// </summary>
        /// <returns>Current queue of ChatMessage objects</returns>
        IEnumerable<ChatMessage> GetMessageQueue();

        /// <summary>
        ///     Returns a larger chunk of messages from TableStore
        /// </summary>
        /// <param name="numMessages">Number of messages to return</param>
        /// <returns>List of last numMessages messages</returns>
        IEnumerable<ChatMessage> GetMoreMessages(int numMessages);

        /// <summary>
        ///     Returns the messages from a given day
        /// </summary>
        /// <param name="date">DateTime object specifying day (all other properties such as Time are ignored)</param>
        /// <returns>List of messages on that day</returns>
        IEnumerable<ChatMessage> GetMessagesFromDate(DateTime date);

        /// <summary>
        ///     Connect to a new server (wipes current cached MessageQueue)
        ///     Currently, this just replaces the CommLayer object with a new one
        /// </summary>
        /// <param name="address">Address of the server</param>
        /// <param name="port">Port to connect to</param>
        /// <param name="password">Password to use</param>
        /// <param name="oldpass">Old password to verify with</param>
        String Connect(string address, int port, string password, string oldPass);

        void LoadExistingConnection();

        /// <summary>
        ///     Query Azure Storage for settings related to the passed input.
        /// </summary>
        /// <param name="partitionKey">The primary IP of the server</param>
        /// <param name="rowKey">The unique port of the server</param>
        ServerConfig LoadServerSettings(string partitionKey, string rowKey);
    }
}
