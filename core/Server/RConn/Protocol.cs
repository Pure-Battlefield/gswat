﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
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

        public Thread MessagePump;

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

                MessagePump = new Thread(ReceivePackets);
                MessagePump.Start();
            }
            catch (SocketException)
            {
                throw new Exception("Logging in failed.");
            }
        }


        private void ReceivePackets()
        {
            try
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
                        else if (packet.IsResponse)
                        {
                            HandleResponse(packet);
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                Packet serverDied = new Packet();
                List<Word> words = new List<Word>();
                words.Add(new Word("SocketException"));
                words.Add(new Word(ex.Message));
                serverDied.Words = words;

                if (PacketEvent != null)
                {
                    PacketEvent(serverDied);
                }
                return;
            }
        }

        private void HandleResponse(Packet packet)
        {
            if (PacketEvent != null) 
            {
                PacketEvent(packet);
            }
        }

        private void HandleRequest(Packet packet)
        {
            //All requests we'll be getting for version .1 will be events.
            SendOkResponse(packet);
            if (PacketEvent != null)
            {
                PacketEvent(packet);
            }
        }

        private Packet ReceivePacket()
        {
            var buffer = new byte[Packet.MaxSize];

            int bytesReceived = 0;
            while (bytesReceived < (int) Packet.HeaderSize)
            {
                bytesReceived += Sock.Receive(buffer, bytesReceived, Packet.HeaderSize - bytesReceived, SocketFlags.None);
            }

            UInt32 size = buffer.BytesToUInt(4);

            while (bytesReceived < (int)size)
            {
                bytesReceived += Sock.Receive(buffer,
                        bytesReceived,
                        ((int)size) - bytesReceived,
                        SocketFlags.None);
            }

            return buffer.BytesToPacket();
        }

        public Packet SendRequest(Packet packet)
        {
            packet.OrigininatesFromClient = true;
            packet.IsRequest = true;
            packet.SequenceNumber = ++SequenceCounter;
            Sock.Send(packet.Emit());

            return packet;
        }

        private void SendOkResponse(Packet packet)
        {
            Sock.Send(new ResponseOk(packet).Emit());
        }

        public void Disconnect()
        {
            Sock.Close();
            if (MessagePump != null)
            {
                MessagePump.Abort();
            }
        }
    }
}