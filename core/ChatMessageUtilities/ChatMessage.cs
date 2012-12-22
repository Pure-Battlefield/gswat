using System;
using System.Globalization;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.ChatMessageUtilities
{
    public class ChatMessage : TableEntity
    {
        private DateTime _messageTimeStamp;

        // TimeStamp for the message
        public DateTime MessageTimeStamp
        {
            get { return _messageTimeStamp; }
            set
            {
                PartitionKey = value.Day + "";
                RowKey = (DateTime.MaxValue.Ticks - value.Ticks).ToString(CultureInfo.InvariantCulture);
                _messageTimeStamp = value;
            }
        }

        // Speaker name for the message
        public String Speaker { get; set; }

        // Full message text
        public String Text { get; set; }

        // MessageType of the ChatMessage
        public String MessageType { get; set; }

        /// <summary>
        ///     Construct an empty ChatMessage object
        ///     MessageTimeStamp will be initiated to a default DateTime()
        ///     Speaker and text will be empty strings
        /// </summary>
        public ChatMessage()
        {
            MessageTimeStamp = new DateTime();
            Speaker = "";
            Text = "";
            PartitionKey = "";
            RowKey = "";
        }

        /// <summary>
        ///     Construct a ChatMessage object with predefined fields
        ///     Any null parameters will be initialized to empty strings or a default DateTime() object
        /// </summary>
        /// <param name="time">MessageTimeStamp of the ChatMessage</param>
        /// <param name="speaker">Speaker of the ChatMessage</param>
        /// <param name="text">Full text of the ChatMessage</param>
        /// <param name="messageType">MessageType of the ChatMessage</param>
        public ChatMessage(DateTime time, String speaker, String text, String messageType)
        {
            MessageTimeStamp = time;
            Speaker = (speaker ?? "");
            Text = (text ?? "");
            MessageType = (messageType ?? "all");
            PartitionKey = time.Day + "";
            RowKey = (DateTime.MaxValue.Ticks - time.Ticks).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Fetch a preformatted output version of this ChatMessage
        /// </summary>
        /// <returns>Formatted output string</returns>
        public override String ToString()
        {
            return "[" + MessageTimeStamp + "] [" + MessageType + "] " + Speaker + ": " + Text;
        }
    }
}