using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebFrontend.Models;
using core;

namespace WebFrontend.Controllers
{
    public class RolesController : ApiController
    {
        private readonly ICore core;

        public RolesController(ICore core)
        {
            this.core = core;
        }

        public HttpResponseMessage Get(AuthenticatedUser requestingUser)
        {
            if(requestingUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden,
                                              "You must be an administrator to view user roles.");
            }

            //TODO:  Fill in when PermissionsUtility has code to get all users.  
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public HttpResponseMessage Put(AuthenticatedUser requestingUser)
        {
            if (requestingUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden,
                                              "You must be an administrator to add or update.");
            }
            return Request.CreateResponse(HttpStatusCode.Created);
        }
    }
}
