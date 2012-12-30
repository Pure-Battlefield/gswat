using System;
using core.ChatMessageUtilities;
using core.Server.RConn;
using core.Server.RConn.Commands;

namespace core.Server
{
    public class CommLayer : ICommLayer
    {
        private Protocol RconProtocol { get; set; }
        public event ChatEventHandler CommHandler;

        public void Connect(string address, int port, string password)
        {
            RconProtocol = new Protocol(address, port, password);
            RconProtocol.PacketEvent += RConnPacketHandler;
            RconProtocol.Connect();
        }


        public void NotifyCommHandler(object sender, ChatEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            if (RconProtocol != null)
            {
                RconProtocol.Disconnect();
                RconProtocol.PacketEvent -= RConnPacketHandler;
            }
        }

        private void RConnPacketHandler(Packet args)
        {
            //Version .1 we only want player.onChat, admin.say, and admin.yell
            if (OnChat.IsOnChat(args))
            {
                var chat = new OnChat(args);
                var message = new ChatMessage
                    {
                        MessageTimeStamp = DateTime.Now,
                        Text = chat.Text,
                        Speaker = chat.SoldierName,
                        MessageType = chat.TargetPlayers
                    };

                var chatArgs = new ChatEventArgs(message);
                CommHandler(this, chatArgs);
            }
        }
    }
}