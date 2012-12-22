using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using core.Server.RConn;
using core.Server.RConn.Commands;

namespace test.Server.RConn
{
    [TestClass]
    public class PlainTextLoginText
    {
        /// <summary>
        /// An exception will be thrown if Connect() fails.
        /// </summary>
        [TestMethod]
        public void ProtocolLoginTest()
        {
            Packet first = new HashedLogin();
            Protocol protocol = new Protocol(Properties.Settings.Default.ServerIP, 
                                                Properties.Settings.Default.ServerPort, 
                                                Properties.Settings.Default.ServerPassword);
            protocol.Connect();
        }
    }
}
