using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;

namespace WebFrontend.Models
{
    [Route("/messages/","GET")]
    public class DateTimeInfo
    {
        public long DateTimeUnix  { get; set; }
        public String Action { get; set; }
    }
}