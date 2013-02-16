using System;
using core.ChatMessageUtilities;
using core.Server.RConn;
using core.Server.RConn.Commands;
using System.Collections.Generic;

namespace core.Server
{
    public class CommLayer : ICommLayer
    {
        private Protocol RconProtocol { get; set; }
        private Dictionary<uint, MessageEventHandler> RequestCallbacks;
        private Dictionary<uint, Packet> RequestPackets;

        public event ChatEventHandler CommHandler;

        // Event handler for all message types.
        public delegate void MessageEventHandler(object sender, Dictionary<string, string> message);
        // Directory of handlers for various message types.
        public Dictionary<string, MessageEventHandler> MessageEvents;


        public void Connect(string address, int port, string password)
        {
            RconProtocol = new Protocol(address, port, password);
            RconProtocol.PacketEvent += RConnPacketHandler;
            RconProtocol.Connect();

            RequestCallbacks = new Dictionary<uint, MessageEventHandler>();
            RequestPackets = new Dictionary<uint, Packet>();

            RecognizedPacket.LoadScrapedData();
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

        public void IssueRequest(string requestName, Dictionary<string, string> parameters, MessageEventHandler callback)
        {
            Packet request = RecognizedPacket.CreatePacketFromFormattedData(requestName, parameters);
            request = RconProtocol.SendRequest(request);
            RequestCallbacks[request.SequenceNumber] = callback;
            RequestPackets[request.SequenceNumber] = request;
        }

        private void RConnPacketHandler(Packet args)
        {
            if (args.IsRequest)
            {
                var formattedPacket = RecognizedPacket.FormatRequestPacket(args);

                if (MessageEvents.ContainsKey(args.FirstWord) && MessageEvents[args.FirstWord] != null)
                {
                    MessageEvents[args.FirstWord](this, formattedPacket);
                }
            }
            else if(args.IsResponse && RequestCallbacks.ContainsKey(args.SequenceNumber)) 
            {
                Packet request = RequestPackets[args.SequenceNumber];
                RequestCallbacks[args.SequenceNumber](this, RecognizedPacket.FormatResponsePacket(request, args));
            }

            //Version .1 we only want player.onChat, admin.say, and admin.yell
            if (OnChat.IsOnChat(args))
            {
                var chat = new OnChat(args);
                var message = new ChatMessage
                    {
                        MessageTimeStamp = DateTime.UtcNow,
                        Text = chat.Text,
                        Speaker = chat.SoldierName,
                        MessageType = chat.TargetPlayers.ToString()
                    };

                var chatArgs = new ChatEventArgs(message);
                CommHandler(this, chatArgs);
            }
        }
    }
}