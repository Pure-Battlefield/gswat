namespace core.Server.RConn.Commands
{
    /// <summary>
    ///     Request:  login.plainText [password: string]
    ///     Response:  OK    - Login successful, you are now logged in regardless of prior status
    ///     Response:  InvalidPassword  - Login unsuccessful, logged-in status unchanged
    ///     Response:  PasswordNotSet  - Login unsuccessful, logged-in status unchanged
    ///     Response:  InvalidArguments
    ///     Effect:   Attempt to login to game server with password [password]
    ///     Comments:  If you are connecting to the admin interface over the internet, then use login.hashed instead to avoid
    ///     having evildoers sniff the admin password
    /// </summary>
    public class EnableEvents : Packet
    {
        public EnableEvents(bool enable = true) : base("admin.eventsEnabled {0}", enable.ToString())
        {
            OriginatesFromServer = false;
            IsRequest = true;
        }
    }
}