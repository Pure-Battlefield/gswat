using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using core.Server;
using core.ServerInterface;
using core.TableStoreEntities;

namespace test.ServerInterface
{
    [TestClass]
    public class CommHandlerTest
    {
        [TestMethod]
        public void CommHandlerTest_Constructor_ValidParams()
        {
            // Create objects
            var mockCommLayer = new Mock<ICommLayer>();
            var commHandler = new CommHandler();
            commHandler.CommLayer = mockCommLayer.Object;

            // Subscribe to CommHandler
            bool raised = false;
            commHandler.CoreListener += delegate { raised = true; };

            // Raise MessageSent event in Server
            var msg = new ChatMessage(new DateTime(2012, 12, 18), "Llamautomatic", "This is a test message", "all");
            mockCommLayer.Raise(m => m.CommHandler += null, new ChatEventArgs(msg));

            // Check if CommHandler was successfully raised in response
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void CommHandlerTest_Constructor_NullParams()
        {
            // Create objects
            var commHandler = new CommHandler();

            // Check for empty CommHandler reference in Core
            Assert.AreEqual(commHandler.CommLayer, null);
        }
    }
}