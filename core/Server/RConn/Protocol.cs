using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using System.Threading;

using core.Server.RConn.Commands;

namespace core.Server.RConn
{
    public class Protocol
    {
        private UInt32 _sequenceCounter { get; set; }
        private Socket _sock { get; set; }
        private string _password { get; set; }

        public delegate void PacketEventHandler(Packet args);
        public event PacketEventHandler PacketEvent;

        public Protocol(string address, int port, string password)
        {
            IPAddress addr;
            if (!IPAddress.TryParse(address, out addr) || port > 65536)
            {
                throw new ArgumentException("Invalid address specified");
            }

            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _sock.Connect(addr, port);
            _password = password;
        }

        
        public void Connect() 
        {
            try
            {
                Packet login = new PlainTextLogin(_password);
                SendRequest(login);

                if (!ResponseOK.IsPacketResponseOK(ReceivePacket()))
                {
                    throw new Exception("Password incorrect.");
                }

                Packet enableEvents = new EnableEvents();
                SendRequest(enableEvents);

                if (!ResponseOK.IsPacketResponseOK(ReceivePacket()))
                {
                    throw new Exception("Events not enabled.");
                }

                Thread messagePump = new Thread(this.ReceivePackets);
                messagePump.Start();
            }
            catch (SocketException)
            {
                throw new Exception("Logging in failed.");
            }
        }


        public void ReceivePackets()
        {
            while (_sock.Connected)
            {
                Packet packet = ReceivePacket();

                if (packet != null)
                {
                    if (packet.IsRequest)
                    {
                        HandleRequest(packet);
                    }
                    else
                    {
                        HandleResponse(packet);
                    }
                }
            }
        }

        private void HandleResponse(Packet packet)
        {
            PacketEvent(packet);
        }

        public void HandleRequest(Packet packet)
        {
            //All requests we'll be getting for version .1 will be events.
            SendOkResponse(packet);
            PacketEvent(packet);
        }

        private Packet ReceivePacket()
        {
            byte[] buffer = new byte[Packet.MAX_SIZE];
            int received = _sock.Receive(buffer, Packet.HEADER_SIZE, SocketFlags.None);
            UInt32 size = buffer.BytesToUInt(4);
            _sock.Receive(buffer, Packet.HEADER_SIZE, (int)size-Packet.HEADER_SIZE, SocketFlags.None);

            return buffer.BytesToPacket();
        }

        private void SendRequest(Packet packet)
        {
            packet.OrigininatesFromClient = true;
            packet.IsRequest = true;
            packet.SequenceNumber = _sequenceCounter++;
            _sock.Send(packet.Emit());
        }

        private void SendOkResponse(Packet packet)
        {
            packet.OrigininatesFromClient = packet.OrigininatesFromClient;
            packet.IsRequest = false;
            packet.SequenceNumber = packet.SequenceNumber;
            _sock.Send(packet.Emit());
        }
    }
}
