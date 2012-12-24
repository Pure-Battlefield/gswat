using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Diagnostics;
using WebFrontend.Models;
using core.ChatMessageUtilities;

namespace WebFrontend.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        public string Get()
        {
            IEnumerable<ChatMessage> q = GlobalStaticVars.StaticCore.GetMessageQueue();
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(q);
        }

        // GET api/values/5
        [HttpGet]
        [ActionName("GetByDay")]
        public string GetByDay([FromBody]DateTimeInfo dateTime)
        {
                DateTime temp = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                IEnumerable<ChatMessage> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
                JavaScriptSerializer json = new JavaScriptSerializer();
                return json.Serialize(q);
        }

        // POST api/values
        public void Post([FromBody]ConnectionInfo connection)
        {
            try
            {
                GlobalStaticVars.StaticCommLayer.Connect(connection.ServerIP, connection.ServerPort, connection.Password);
            }
            catch (ArgumentException e)
            {
                
            }
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