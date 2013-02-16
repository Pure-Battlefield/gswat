using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;

namespace WebFrontend.Models
{
    [Route("/serverInfo/","POST")]
    public class ConnectionInfo
    {
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
    }
}