using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFrontend.Models
{
    public class AuthenticatedUser
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}