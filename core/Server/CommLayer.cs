using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using core.Server.RConn.Commands;
using System.Net.Sockets;
using System.Net;

using core.Server.RConn;

namespace core.Server
{
    public class CommLayer : ICommLayer
    {
        public event ChatEventHandler CommHandler;
        private Protocol _rconProtocol { get; set; }

        public void Connect(string address, int port, string password)
        {
            _rconProtocol = new Protocol(address, port, password);

            _rconProtocol.PacketEvent += RConnPacketHandler;

            _rconProtocol.Connect();
        }

        private void RConnPacketHandler(Packet args)
        {
            //Version .1 we only want player.onChat, admin.say, and admin.yell
            if (OnChat.IsOnChat(args))
            {
                OnChat chat = new OnChat(args);
                ChatMessageUtilities.ChatMessage message = new ChatMessageUtilities.ChatMessage();
                message.Timestamp = DateTime.Now;
                message.Text = chat.Text;
                message.Speaker = chat.SoldierName;

                ChatEventArgs chatArgs = new ChatEventArgs(message);
                CommHandler(this, chatArgs);
            }
        }

        

        public void NotifyCommHandler(object sender, ChatEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void close()
        {
            throw new NotImplementedException();
        }
    }
}
