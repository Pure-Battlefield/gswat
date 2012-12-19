using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.ChatMessageUtilities
{
    public class ChatMessage
    {
        // Timestamp for the message
        public DateTime Timestamp { get; set; }

        // Speaker name for the message
        public String Speaker { get; set; }

        // Full message text
        public String Text { get; set; }

        /// <summary>
        /// Construct an empty ChatMessage object
        /// Timestamp will be initiated to a default DateTime()
        /// Speaker and text will be empty strings
        /// </summary>
        public ChatMessage()
        {
            Timestamp = new DateTime();
            Speaker = "";
            Text = "";
        }

        /// <summary>
        /// Construct a ChatMessage object with predefined fields
        /// Any null parameters will be initialized to empty strings or a default DateTime() object
        /// </summary>
        /// <param name="time">Timestamp of the ChatMessage</param>
        /// <param name="speaker">Speaker of the ChatMessage</param>
        /// <param name="text">Full text of the ChatMessage</param>
        public ChatMessage(DateTime time, String speaker, String text)
        {
            Timestamp = (time == null ? new DateTime() : time);
            Speaker = (speaker == null ? "" : speaker);
            Text = (text == null ? "" : text);
        }

        /// <summary>
        /// Fetch a preformatted output version of this ChatMessage
        /// </summary>
        /// <returns>Formatted output string</returns>
        public override String ToString()
        {
            return "[" + Timestamp + "] " + Speaker + ": " + Text;
        }
    }
}
