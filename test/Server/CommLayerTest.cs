using core.ChatMessageUtilities;
using core.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace test.CommMock
{
    [TestClass]
    public class CommLayerTest
    {
        [TestMethod]
        public void CommLayerTest_Constructor_ValidParams()
        {
            // Create objects
            var mockServer = new Mock<IServerMock>();
            var commLayer = new CommLayer(mockServer.Object);

            // Subscribe to CommHandler
            bool raised = false;
            commLayer.CommHandler += delegate { raised = true; };

            // Raise MessageSent event in Server
            ChatMessage msg = new ChatMessage(new DateTime(2012, 12, 18), "Llamautomatic", "This is a test message");
            mockServer.Raise(m => m.MessageSent += null, new ChatEventArgs(msg));

            // Check if CommHandler was successfully raised in response
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void CommLayerTest_Constructor_NullParams()
        {
            // Create objects
            var commLayer = new CommLayer(null);

            // Check for empty CommHandler reference in Core
            Assert.AreEqual(commLayer.Server, null);
        }
    }
}
