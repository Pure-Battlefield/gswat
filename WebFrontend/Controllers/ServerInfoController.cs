using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebFrontend.Models;
using core;

namespace WebFrontend.Controllers
{
    public class ServerInfoController : ApiController
    {
        private readonly ICore core;

        public ServerInfoController(ICore core)
        {
            this.core = core;
        }
        // GET api/serverinfo
        public Dictionary<string, string> Get()
        {
            // Query Azure Storage ** Right now were using Last and Server because of the current StorageScheme
            var settings = core.LoadServerSettings("Last", "Server");

            if (settings != null)
            {
                return new Dictionary<string, string>
                                         {
                                             {"ServerIP", settings.Address},
                                             {"ServerPort", settings.Port.ToString()}

                                         };
            }
            return new Dictionary<string, string>
                                  {
                                      {"ServerIP", ""},
                                      {"ServerPort", ""}
                                  };
        }

        // PUT api/serverinfo
        public HttpResponseMessage Put(HttpRequestMessage request, [FromBody]ConnectionInfo connection)
        {
            try
            {
                var result = core.Connect(connection.ServerIP, connection.ServerPort,
                                                                 connection.Password, connection.OldPassword);

                return request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (ArgumentException e)
            {
                return request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
