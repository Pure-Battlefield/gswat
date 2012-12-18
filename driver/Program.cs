using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using core;
using core.ChatMessageUtilities;

namespace driver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Core c = new Core();
            c.Test();
            Queue<ChatMessage> msgList = c.GetMessageQueue();
            foreach (ChatMessage m in msgList)
            {
                System.Console.WriteLine(m.ToString());
            }
        }
    }
}
