using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using core;
using core.ChatMessageUtilities;
using core.Server;
using System.Collections.Generic;
using Moq;
using core.ServerInterface;

namespace test
{
    [TestClass]
    public class CoreTest
    {
        [TestMethod]
        public void CoreTest_Constructor_ValidParams()
        {
            // Create objects
            var mockHandler = new Mock<ICommHandler>();
            Core c = new Core(mockHandler.Object);

            // Raise MessageSent event in CommHandler
            ChatMessage msg = new ChatMessage(new DateTime(2012, 12, 18), "Llamautomatic", "This is a test message");
            mockHandler.Raise(m => m.CoreListener += null, new ChatEventArgs(msg));

            // Retrieve message queue and check for message
            IList<ChatMessage> msgList = c.GetMessageQueue();
            ChatMessage msgResult = msgList[0];
            Assert.AreEqual(msgResult.Speaker, msg.Speaker);
            Assert.AreEqual(msgResult.MessageTimeStamp, msg.MessageTimeStamp);
            Assert.AreEqual(msgResult.Text, msg.Text);
        }

        [TestMethod]
        public void CoreTest_Constructor_NullParams()
        {
            // Create objects
            var mockHandler = new Mock<ICommHandler>();
            Core c = new Core(null);

            // Raise MessageSent event in CommHandler
            ChatMessage msg = new ChatMessage(new DateTime(2012, 12, 18), "Llamautomatic", "This is a test message");
            mockHandler.Raise(m => m.CoreListener += null, new ChatEventArgs(msg));

            // Retrieve message queue and check for empty queue
            IList<ChatMessage> msgList = c.GetMessageQueue();
            Assert.AreEqual(msgList.Count, 0);

            // Check for empty CommHandler reference in Core
            Assert.AreEqual(c.CommHandler, null);
        }

        
    }
}
