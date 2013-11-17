using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFrontend.Exceptions
{
    public class UnauthorizedException : System.Exception
    {
        public UnauthorizedException()
        {
        }

        public UnauthorizedException(string msg) : base(msg)
        {
        }
    }
}