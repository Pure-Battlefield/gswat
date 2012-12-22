using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn.Commands
{
    /// <summary>
    /// Request:  login.plainText [password: string]
    /// Response:  OK    - Login successful, you are now logged in regardless of prior status
    /// Response:  InvalidPassword  - Login unsuccessful, logged-in status unchanged
    /// Response:  PasswordNotSet  - Login unsuccessful, logged-in status unchanged
    /// Response:  InvalidArguments
    /// Effect:   Attempt to login to game server with password [password]
    /// Comments:  If you are connecting to the admin interface over the internet, then use login.hashed instead to avoid 
    /// having evildoers sniff the admin password
    /// </summary>
    public class PlainTextLogin : Packet
    {
        public PlainTextLogin(string password) : base("login.plainText {0}", password)
        {
            OriginatesFromServer = false;
            IsRequest = true;
        }
    }
}
