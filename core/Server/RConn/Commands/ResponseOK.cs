namespace core.Server.RConn.Commands
{
    /// <summary>
    ///     Responses must contain at least one word. The first word can be one of the following:
    ///     OK    - request completed successfully
    ///     UnknownCommand  - unknown command
    ///     InvalidArguments   - Arguments not appropriate for command
    ///     [other]    - command-specific error
    ///     OK is the only response which signifies success.
    ///     Subsequent arguments (if any) are command-specific.
    /// </summary>
    internal class ResponseOk : Packet
    {
        public ResponseOk(Packet request)
            : base("OK")
        {
            OriginatesFromServer = request.OriginatesFromServer;
            IsResponse = true;
            SequenceNumber = request.SequenceNumber;
        }

        public static bool IsPacketResponseOk(Packet response)
        {
            if (response == null || !response.FirstWord.Equals("OK"))
            {
                return false;
            }

            return true;
        }
    }
}