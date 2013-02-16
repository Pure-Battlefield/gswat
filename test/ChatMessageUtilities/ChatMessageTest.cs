using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using core.TableStoreEntities;

namespace test.ChatMessageUtilities
{
    [TestClass]
    public class ChatMessageTest
    {
        [TestMethod]
        public void ChatMessageTest_Constructor_Default()
        {
            var msg = new ChatMessage();
            Assert.AreEqual(msg.Speaker, "");
            Assert.AreEqual(msg.Text, "");
            Assert.AreEqual(msg.MessageTimeStamp, new DateTime());
        }

        [TestMethod]
        public void ChatMessageTest_Constructor_NullParams()
        {
            var msg = new ChatMessage(new DateTime(), null, null, null);
            Assert.AreEqual(msg.Speaker, "");
            Assert.AreEqual(msg.Text, "");
            Assert.AreEqual(msg.MessageTimeStamp, new DateTime());
        }

        [TestMethod]
        public void ChatMessageTest_ToString()
        {
            var msg = new ChatMessage(new DateTime(2012, 12, 11, 10, 9, 8), "TestSpeaker", "This is a test string", "all");
            Assert.AreEqual(msg.ToString(), "[12/11/2012 10:09:08 AM] [all] TestSpeaker: This is a test string");
        }
    }
}