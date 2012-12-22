using core.ChatMessageUtilities;
using core.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

using core.Server.RConn;
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
            var commLayer = new CommLayerMock(mockServer.Object);

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
        public void CommLayerTest_Live_Server()
        {
            // Create objects
            var commLayer = new CommLayer();

            // Subscribe to CommHandler
            bool raised = false;
            commLayer.CommHandler += delegate { raised = true; };

            //This test succeeds if it logs in, any actual chat messages are only for
            //ad-hoc debugging.
            commLayer.Connect(Properties.Settings.Default.ServerIP, 
                                Properties.Settings.Default.ServerPort,
                                Properties.Settings.Default.ServerPassword);
            

            // Check if CommHandler was successfully raised in response
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void CommLayerTest_Constructor_NullParams()
        {
            // Create objects
            var commLayer = new CommLayerMock(null);

            // Check for empty CommHandler reference in Core
            Assert.AreEqual(commLayer.Server, null);
        }
    }
}
