using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.ChatMessageUtilities
{
    public class ChatMessage : TableEntity
    {
        // TimeStamp for the message
        public DateTime MessageTimeStamp { get; set; }

        // Speaker name for the message
        public String Speaker { get; set; }

        // Full message text
        public String Text { get; set; }

        /// <summary>
        /// Construct an empty ChatMessage object
        /// MessageTimeStamp will be initiated to a default DateTime()
        /// Speaker and text will be empty strings
        /// </summary>
        public ChatMessage()
        {
            MessageTimeStamp = new DateTime();
            Speaker = "";
            Text = "";
            this.PartitionKey = "PlayerChatMessage";
            this.RowKey = "";
        }

        /// <summary>
        /// Construct a ChatMessage object with predefined fields
        /// Any null parameters will be initialized to empty strings or a default DateTime() object
        /// </summary>
        /// <param name="time">MessageTimeStamp of the ChatMessage</param>
        /// <param name="speaker">Speaker of the ChatMessage</param>
        /// <param name="text">Full text of the ChatMessage</param>
        public ChatMessage(DateTime time, String speaker, String text)
        {
            MessageTimeStamp = (time == null ? new DateTime() : time);
            Speaker = (speaker == null ? "" : speaker);
            Text = (text == null ? "" : text);
            this.PartitionKey = "PlayerChatMessage";
            this.RowKey = (DateTime.MaxValue.Ticks - time.Ticks).ToString();
        }

        /// <summary>
        /// Serialize this object for Azure storage
        /// </summary>
        /// <returns>Serialized object in string format</returns>
        public string SerializeMe()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
            StringWriter stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, this);
            return stringWriter.ToString();
        }

        /// <summary>
        /// Fetch a preformatted output version of this ChatMessage
        /// </summary>
        /// <returns>Formatted output string</returns>
        public override String ToString()
        {
            return "[" + MessageTimeStamp + "] " + Speaker + ": " + Text;
        }
    }
}
