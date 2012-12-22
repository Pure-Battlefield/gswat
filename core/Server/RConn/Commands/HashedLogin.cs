using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

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
    public class HashedLogin : Packet
    {
        public HashedLogin()
            : base("login.hashed")
        {
            OriginatesFromServer = false;
            IsRequest = true;
        }

        public HashedLogin(string password, byte[] salt)
            : base("login.hashed")
        {
            OriginatesFromServer = false;
            IsRequest = true;

            string hexPassword = HashedPassword(password, salt);

            Words.Add(new Word(hexPassword));
        }

        public static string HashedPassword(string password, byte[] salt)
        {
            byte[] combinedBytes = new byte[password.Length + salt.Length];
            Encoding.UTF8.GetBytes(password).CopyTo(combinedBytes, salt.Length);
            salt.CopyTo(combinedBytes, 0);

            MD5 hasher = MD5.Create();
            byte[] hash = hasher.ComputeHash(combinedBytes);

            //
            // generate the hexadecimal string
            //

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        public static string GeneratePasswordHash(byte[] a_bSalt, string strData)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] a_bCombined = new byte[a_bSalt.Length + strData.Length];
            a_bSalt.CopyTo(a_bCombined, 0);
            Encoding.Default.GetBytes(strData).CopyTo(a_bCombined, a_bSalt.Length);

            byte[] a_bHash = md5Hasher.ComputeHash(a_bCombined);

            StringBuilder sbStringifyHash = new StringBuilder();
            for (int i = 0; i < a_bHash.Length; i++)
            {
                sbStringifyHash.Append(a_bHash[i].ToString("X2"));
            }

            return sbStringifyHash.ToString();
        }
    }
}
