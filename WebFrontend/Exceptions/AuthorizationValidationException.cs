using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFrontend.Exceptions
{
    public class AuthorizationValidationException : System.Exception
    {
        public AuthorizationValidationException(){}

        public AuthorizationValidationException(string msg): base(msg){}
    }
}