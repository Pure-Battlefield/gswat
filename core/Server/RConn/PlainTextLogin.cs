using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn
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
    public class PlainTextLogin
    {
        public PlainTextLogin(string password)
        {
            Command = new Packet();
            Command.IsRequest = true;
            Command.OrigininatesFromClient = true;

            Command.Words.Add(new Word("login.plainText"));
            Command.Words.Add(new Word(password));
        }

        public Packet Command
        {
            get;
            set;
        }
    }
}
