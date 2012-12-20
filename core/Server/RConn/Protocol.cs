using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace core.Server.RConn
{
    class Protocol
    {
        public Protocol()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress addr = new IPAddress(0x4a5b713a); //The hex number is the dev server's IP address
            int port = 47200;

            sock.Connect(addr, port);
        }
    }
}
