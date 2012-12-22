using Microsoft.VisualStudio.TestTools.UnitTesting;
using core.Server.RConn;
using core.Server.RConn.Commands;
using test.Properties;

namespace test.Server.RConn
{
    [TestClass]
    public class PlainTextLoginText
    {
        /// <summary>
        ///     An exception will be thrown if Connect() fails.
        /// </summary>
        [TestMethod]
        public void ProtocolLoginTest()
        {
            Packet first = new HashedLogin();
            var protocol = new Protocol(Settings.Default.ServerIP,
                                        Settings.Default.ServerPort,
                                        Settings.Default.ServerPassword);
            protocol.Connect();
        }
    }
}