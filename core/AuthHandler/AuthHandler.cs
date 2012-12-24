using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.AuthHandler
{
    public class AuthHandler : IAuthHandler
    {
        // Implements IAuthHandler
        public String Address { get; set; }
        public int Port { get; set; }
        public String Password { get; set; }
    }
}
