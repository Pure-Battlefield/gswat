using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server
{
    public interface IServerMock
    {
        // Event for mocking chat messages sent from the server
        event ChatEventHandler MessageSent;
    }
}
