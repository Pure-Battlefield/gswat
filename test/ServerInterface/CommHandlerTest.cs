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
        public void CommHandlerTest_Constructor_NullParams()
        {
            // Create objects
            var commHandler = new CommHandler();

            // Check for empty CommHandler reference in Core
            Assert.AreEqual(commHandler.CommLayer, null);
        }
    }
}