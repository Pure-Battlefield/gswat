using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.ChatMessageUtilities
{
    public class ChatMessage
    {
        DateTime _timestamp;
        String _speaker;
        String _text;

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
        
        public String Speaker
        {
            get { return _speaker; }
            set { _speaker = value; }
        }
        
        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public String ToString()
        {
            return "[" + Timestamp + "] " + Speaker + ": " + Text;
        }
    }
}
