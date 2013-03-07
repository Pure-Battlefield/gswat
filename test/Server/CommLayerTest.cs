using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using core.Server;
using core.Server.RConn;
using core.TableStoreEntities;

namespace test.Server
{
    [TestClass]
    public class CommLayerTest
    {
        [TestMethod]
        public void CommLayerTest_RecognizedPacket()
        {
            RecognizedPacket.LoadScrapedData();
        }
    }
}