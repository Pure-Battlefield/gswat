﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using core.Server.RConn.Commands;

namespace core.Server.RConn
{
    public class Protocol
    {
        public delegate void PacketEventHandler(Packet args);

        public Protocol(string address, int port, string password)
        {
            IPAddress addr;
            if (!IPAddress.TryParse(address, out addr) || port > 65536)
            {
                throw new ArgumentException("Invalid address specified");
            }

            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Sock.Connect(addr, port);
            Password = password;
        }

        private UInt32 SequenceCounter { get; set; }
        private Socket Sock { get; set; }
        private string Password { get; set; }

        public event PacketEventHandler PacketEvent;


        public void Connect()
        {
            try
            {
                Packet login = new PlainTextLogin(Password);
                SendRequest(login);

                if (!ResponseOk.IsPacketResponseOk(ReceivePacket()))
                {
                    throw new Exception("Password incorrect.");
                }

                Packet enableEvents = new EnableEvents();
                SendRequest(enableEvents);

                if (!ResponseOk.IsPacketResponseOk(ReceivePacket()))
                {
                    throw new Exception("Events not enabled.");
                }

                var messagePump = new Thread(ReceivePackets);
                messagePump.Start();
            }
            catch (SocketException)
            {
                throw new Exception("Logging in failed.");
            }
        }


        private void ReceivePackets()
        {
            while (Sock.Connected)
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

        private void HandleRequest(Packet packet)
        {
            //All requests we'll be getting for version .1 will be events.
            SendOkResponse(packet);
            PacketEvent(packet);
        }

        private Packet ReceivePacket()
        {
            var buffer = new byte[Packet.MaxSize];
            Sock.Receive(buffer, Packet.HeaderSize, SocketFlags.None);
            UInt32 size = buffer.BytesToUInt(4);
            Sock.Receive(buffer, Packet.HeaderSize, (int) size - Packet.HeaderSize, SocketFlags.None);

            return buffer.BytesToPacket();
        }

        private void SendRequest(Packet packet)
        {
            packet.OrigininatesFromClient = true;
            packet.IsRequest = true;
            packet.SequenceNumber = SequenceCounter++;
            Sock.Send(packet.Emit());
        }

        private void SendOkResponse(Packet packet)
        {
            packet.OrigininatesFromClient = packet.OrigininatesFromClient;
            packet.IsRequest = false;
            packet.SequenceNumber = packet.SequenceNumber;
            Sock.Send(packet.Emit());
        }
    }
}