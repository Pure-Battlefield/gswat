using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using WebFrontend.Handlers;
using WebFrontend.Models;
using core;
using core.Roles;
using core.TableStoreEntities;

namespace WebFrontend.Controllers
{
    public class TestsController : ApiController
    {
        private readonly ICore core;

        public TestsController(ICore core)
        {
            this.core = core;
        }
    }
}
