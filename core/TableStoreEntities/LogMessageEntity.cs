using System;
using System.Globalization;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.TableStoreEntities
{
    public class LogMessageEntity : TableEntity
    {
        private DateTime _messageTimeStamp;
        private String _text;
        
        // Function Name of the logMessage
        public String FuncName { get; set; }

        // Class name of the logMessage
        public String ClassName { get; set; }

        // TimeStamp for the message
        public DateTime MessageTimeStamp
        {
            get { return _messageTimeStamp; }
            set
            {
                PartitionKey = value.Date.ToString("yyyyMMdd");
                RowKey = (DateTime.MaxValue.Ticks - value.Ticks).ToString(CultureInfo.InvariantCulture);
                _messageTimeStamp = value;
            }
        }

        // Full message text
        public String Text
        {
            get { return _text; }
            set { _text = value.Trim(); }
        }

        /// <summary>
        ///     Construct an empty LogMessage object
        ///     MessageTimeStamp will be initiated to a default DateTime()
        ///     Speaker and text will be empty strings
        /// </summary>
        public LogMessageEntity()
        {
            MessageTimeStamp = new DateTime();
            Text = "";
            FuncName = "";
            ClassName = "";
        }

        /// <summary>
        ///     Construct a ChatMessage object with predefined fields
        ///     Any null parameters will be initialized to empty strings or a default DateTime() object
        /// </summary>
        /// <param name="time">Timestamp of the LogMessage</param>
        /// <param name="className">Class name to include in the log</param>
        /// <param name="funcName">Function name to be logged</param>
        /// <param name="text">Full text of the LogMessage</param>
        public LogMessageEntity(DateTime time, String className, String funcName, String text)
        {
            MessageTimeStamp = time;
            Text = (text.Trim() ?? "");
            FuncName = funcName;
            ClassName = className;
        }

        /// <summary>
        ///     Fetch a preformatted output version of this ChatMessage
        /// </summary>
        /// <returns>Formatted output string</returns>
        public override String ToString()
        {
            return "[" + MessageTimeStamp + "] at {" + FuncName + "} in {" + ClassName + "}: " + Text;
        }
    }
}