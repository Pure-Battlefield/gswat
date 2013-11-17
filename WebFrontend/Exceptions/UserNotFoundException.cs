using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFrontend.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string msg) : base(msg)
        {
        }
    }
}