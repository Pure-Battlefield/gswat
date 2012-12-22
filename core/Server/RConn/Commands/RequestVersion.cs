namespace core.Server.RConn.Commands
{
    /// <summary>
    ///     Request:  player.onChat [source soldier name: string] [text: string] [target players: player subset]
    ///     Response:  OK
    ///     Effect:   Player with name [source soldier name] (or the server, or the server admin) has sent chat
    ///     message [text] to [target players]
    ///     Comment:  If [source soldier name] is “Server”, then the message was sent from  the server rather than from an
    ///     actual player
    /// </summary>
    internal class RequestVersion : Packet
    {
        public RequestVersion()
            : base("version")
        {
        }
    }
}