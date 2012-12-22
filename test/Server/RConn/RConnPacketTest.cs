using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using core.Server.RConn;
using core.Server.RConn.Commands;

using System.Net.Sockets;
using System.Net;

namespace test.Server.RConn
{
    [TestClass]
    public class RConnPacketTest
    {
        [TestMethod]
        public void PacketFlagTest()
        {
            Packet packet = new Packet();

            Assert.IsTrue(packet.IsResponse != packet.IsRequest, "Packet is a request and a response, after initialization!");
            Assert.IsTrue(packet.OriginatesFromServer != packet.OrigininatesFromClient, "Packet is from server and client, after initialization!");

            packet.IsResponse = !packet.IsResponse;
            packet.OrigininatesFromClient = !packet.OrigininatesFromClient;

            Assert.IsTrue(packet.IsResponse != packet.IsRequest, "Packet is a request and a response, after flipping!");
            Assert.IsTrue(packet.OriginatesFromServer != packet.OrigininatesFromClient, "Packet is from server and client, after flipping!");

            packet.IsResponse = packet.IsResponse;
            packet.OrigininatesFromClient = packet.OrigininatesFromClient;

            Assert.IsTrue(packet.IsResponse != packet.IsRequest, "Packet is a request and a response!");
            Assert.IsTrue(packet.OriginatesFromServer != packet.OrigininatesFromClient, "Packet is from server and client!");
        }

        [TestMethod]
        public void PacketWordCountTest()
        {
            Packet packet = new Packet();

            Word word1 = new Word();
            Word word2 = new Word();
            Word word3 = new Word();

            word1.Content = new char[] { 'h', 'e', 'l', 'l', 'o' };
            word2.Content = new char[] { ' ', ',', ' ' };
            word3.Content = new char[] { 'w', 'o', 'r', 'l', 'd' };

            Assert.IsTrue(packet.WordCount == 0);
            Assert.IsTrue(packet.Size == 12, "Packet size, expected: {0}, actual: {1}", 12, packet.Size);

            packet.Words.Add(word1);

            Assert.IsTrue(packet.WordCount == 1);
            Assert.IsTrue(packet.Size == 22, "Packet size, expected: {0}, actual: {1}", 22, packet.Size);

            packet.Words.Add(word2);

            Assert.IsTrue(packet.WordCount == 2);
            Assert.IsTrue(packet.Size == 30, "Packet size, expected: {0}, actual: {1}", 30, packet.Size);

            packet.Words.Add(word3);

            Assert.IsTrue(packet.WordCount == 3);
            Assert.IsTrue(packet.Size == 40, "Packet size, expected: {0}, actual: {1}", 40, packet.Size);
        }

        [TestMethod]
        public void PacketEmissionTest()
        {
            Packet packet = new Packet();

            Word word1 = new Word();
            Word word2 = new Word();
            Word word3 = new Word();

            word1.Content = new char[] { 'h', 'e', 'l', 'l', 'o' };
            word2.Content = new char[] { ' ', ',', ' ' };
            word3.Content = new char[] { 'w', 'o', 'r', 'l', 'd' };

            packet.Words.Add(word1);
            packet.Words.Add(word2);
            packet.Words.Add(word3);

            byte[] bytes = packet.Emit();

            Assert.IsTrue(bytes.Length == packet.Size, "Packet binary size, expected: {0}, actual: {1}", packet.Size, bytes.Length);
        }
        
        [TestMethod]
        public void ByteToUInt32Test()
        {
            byte[] bytes = new byte[] { 0xff, 0x00, 0x00, 0x00 };
            UInt32 result = bytes.BytesToUInt();

            Assert.IsTrue(result == 0xff, "Byte to UInt32, expected: {0}, actual: {1}", 0xff, result);
        }

        [TestMethod]
        public void ByteToWordTest()
        {
            //                           size: 4,              'h',   'e', 'y',    'o', '\0'
            byte[] bytes = new byte[] { 0x04, 0x00, 0x00, 0x00, 0x48, 0x45, 0x59, 0x4f, 0x00 };
            Word word = bytes.BytesToWord(0);

            Assert.IsTrue(word.Size == 4, "Byte to Word, expected size: {0}, actual: {1}", 4, word.Size);
            char[] expected = new char[] { 'H', 'E', 'Y', 'O' };

            Assert.IsTrue(CompareCharArrays(expected, word.Content),
                        "Byte to Word, expected content: {0}, actual: {1}",
                        expected, word.Content);

        }

        [TestMethod]
        public void ByteToPacketTest()
        {
            PlainTextLogin login = new PlainTextLogin("DEADBEEF");

            byte[] bytes = login.Emit();

            Packet actual = bytes.BytesToPacket();

            Assert.IsTrue(login.OrigininatesFromClient == actual.OrigininatesFromClient);
            Assert.IsTrue(login.IsResponse == actual.IsResponse);
            Assert.IsTrue(login.SequenceNumber == actual.SequenceNumber);
            Assert.IsTrue(login.WordCount == actual.WordCount);

            Assert.IsTrue(CompareCharArrays(login.Words[0].Content, actual.Words[0].Content));
        }

        private bool CompareCharArrays(char[] first, char[] second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
