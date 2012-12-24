using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.AuthHandler
{
    public interface IAuthHandler
    {
        // IP address of the server
        String Address { get; set; }

        // Port number of the server
        int Port { get; set; }

        // RCon password for the server
        String Password { get; set; }
    }
}
