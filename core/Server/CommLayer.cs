using System;
using core.Server.RConn;
using core.Server.RConn.Commands;
using System.Collections.Generic;
using core.TableStoreEntities;

namespace core.Server
{
    public class CommLayer : ICommLayer
    {
        private Protocol RconProtocol { get; set; }
        private Dictionary<uint, MessageEventHandler> RequestCallbacks;
        private Dictionary<uint, Packet> RequestPackets;
        private string Address, Password;
        private int Port;

        public event ChatEventHandler CommHandler;

        public CommLayer()
        {
            MessageEvents = new Dictionary<string, MessageEventHandler>();
            RequestCallbacks = new Dictionary<uint, MessageEventHandler>();
            RequestPackets = new Dictionary<uint, Packet>();
            RecognizedPacket.LoadScrapedData();
        }
        public override void Connect(string address, int port, string password)
        {
            RconProtocol = new Protocol(address, port, password);
            RconProtocol.PacketEvent += RConnPacketHandler;
            RconProtocol.Connect();

            RequestCallbacks = new Dictionary<uint, MessageEventHandler>();
            RequestPackets = new Dictionary<uint, Packet>();

            Address = address;
            Port = port;
            Password = password;

            RecognizedPacket.LoadScrapedData();
            if (CommHandler != null)
            {
                CommHandler(this, e);
            }
        }

        public override void Disconnect()
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

        public void RConnPacketHandler(Packet args)
        {
            //Detect a socket exception and initiate a reconnect.
            if (args.FirstWord == "SocketException")
            {
                Disconnect();
                Connect(Address, Port, Password);
                return;
            }

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
        }
    }
}