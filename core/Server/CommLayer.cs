using System;
using System.Reflection;
using System.Threading;
using core.Logging;
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
                //Continue to attempt to reconnect unless credentials fail.  
                while (true)
                {
                    try
                    {
                        Connect(Address, Port, Password);
                        break;
                    }
                    catch (ArgumentException e)
                    {
                        //Bad Password -- we should stop trying to reconnect and let the user handle it.  
                        LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                        throw;
                    }
                    catch (Exception e)
                    {
                        //Random connection exceptions; sleep for 2 seconds and try again.  
                        LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                        Thread.Sleep(2000);
                    }
                }
                
                return;
            }

            if (args.IsRequest)
            {
                try
                {
                    var formattedPacket = RecognizedPacket.FormatRequestPacket(args);

                    if (MessageEvents.ContainsKey(args.FirstWord) && MessageEvents[args.FirstWord] != null)
                    {
                        MessageEvents[args.FirstWord](this, formattedPacket);
                    }
                }
                catch (ArgumentException e)
                {
                    LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                    //TODO: Investigate why FormatRequestPacket is throwing an exception upon server reconnects
                }
            }
            else if (args.IsResponse && RequestCallbacks.ContainsKey(args.SequenceNumber))
            {
                Packet request = RequestPackets[args.SequenceNumber];
                RequestCallbacks[args.SequenceNumber](this, RecognizedPacket.FormatResponsePacket(request, args));
            }
        }
    }
}