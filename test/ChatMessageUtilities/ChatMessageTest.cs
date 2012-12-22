using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using core.ChatMessageUtilities;

namespace test.ChatMessageUtilities
{
    [TestClass]
    public class ChatMessageTest
    {
        [TestMethod]
        public void ChatMessageTest_Constructor_Default()
        {
            ChatMessage msg = new ChatMessage();
            Assert.AreEqual(msg.Speaker, "");
            Assert.AreEqual(msg.Text, "");
            Assert.AreEqual<DateTime>(msg.MessageTimeStamp, new DateTime());
        }

        [TestMethod]
        public void ChatMessageTest_Constructor_NullParams()
        {
            ChatMessage msg = new ChatMessage(new DateTime(), null, null);
            Assert.AreEqual(msg.Speaker, "");
            Assert.AreEqual(msg.Text, "");
            Assert.AreEqual<DateTime>(msg.MessageTimeStamp, new DateTime());
        }

        [TestMethod]
        public void ChatMessageTest_ToString()
        {
            ChatMessage msg = new ChatMessage(new DateTime(2012, 12, 11, 10, 9, 8), "TestSpeaker", "This is a test string");
            Assert.AreEqual(msg.ToString(), "[12/11/2012 10:09:08 AM] TestSpeaker: This is a test string");
        }
    }
}
