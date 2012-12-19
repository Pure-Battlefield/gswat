using core;
using core.ChatMessageUtilities;
using core.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using core.ServerInterface;

namespace test
{
    [TestClass]
    public class FullCoreTest
    {
        [TestMethod]
        public void FullCoreTest_SendMessage()
        {
            var mockServer = new Mock<IServerMock>();
            var commLayer = new CommLayer(mockServer.Object);
            var commHandler = new CommHandler(commLayer);
            Core c = new Core(commHandler);
            ChatMessage msg = new ChatMessage(new DateTime(2012, 12, 18), "Llamautomatic", "This is a test message");
            mockServer.Raise(m => m.MessageSent += null, new ChatEventArgs(msg));
            Queue<ChatMessage> msgList = c.GetMessageQueue();
            ChatMessage msgResult = msgList.Dequeue();
            Assert.AreEqual(msgResult.Speaker, msg.Speaker);
            Assert.AreEqual(msgResult.Timestamp, msg.Timestamp);
            Assert.AreEqual(msgResult.Text, msg.Text);
        }
    }
}
