using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using core;
using core.ChatMessageUtilities;

namespace WebFrontend.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public string Get()
        {
            IList<ChatMessage> q = GlobalStaticVars.StaticRole.GetMessageQueue();
            string output = "";
            foreach (ChatMessage msg in q) {
                output += msg.ToString() + "<br>";
            }
            return output;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}