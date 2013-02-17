using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;

namespace WebFrontend.Models
{
    [Route("/messages/","POST")]
    public class DateTimeInfo
    {
        public long DateTimeUnix  { get; set; }
        public String Action { get; set; }
    }

    [Route("/messages/","GET")]
    public class DateTimeInfoGetAll
    {
        
    }
}