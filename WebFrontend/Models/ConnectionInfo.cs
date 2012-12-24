using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFrontend.Models
{
    public class ConnectionInfo
    {
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string Password { get; set; }
    }
}