using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn.Commands
{
    /// <summary>
    /// Responses must contain at least one word. The first word can be one of the following:
    /// OK    - request completed successfully
    /// UnknownCommand  - unknown command
    /// InvalidArguments   - Arguments not appropriate for command
    /// [other]    - command-specific error
    /// OK is the only response which signifies success.
    /// Subsequent arguments (if any) are command-specific.
    /// </summary>
    class ResponseUnknown : Packet
    {
        public ResponseUnknown(Packet request) 
            : base("UnknownCommand")
        {
            OriginatesFromServer = request.OriginatesFromServer;
            IsResponse = true;
            SequenceNumber = request.SequenceNumber;
        }
    }
}
