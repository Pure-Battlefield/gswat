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
        [ActionName("GetAllMessages")]
        public string GetAllMessages()
        {
            IEnumerable<ChatMessage> q = GlobalStaticVars.StaticCore.GetMessageQueue();
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(q);
        }

        // GET api/values/5
        [HttpGet]
        [ActionName("GetByDay")]
        public string GetByDay([FromUri]DateTimeInfo dateTime)
        {
                DateTime temp = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                IEnumerable<ChatMessage> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
                JavaScriptSerializer json = new JavaScriptSerializer();
                return json.Serialize(q);
        }

        // POST api/values
        [HttpPost]
        [ActionName("SetServerInfo")]
        public void SetServerInfo([FromBody]ConnectionInfo connection)
        {
            try
            {
                GlobalStaticVars.StaticCommLayer.Connect(connection.ServerIP, connection.ServerPort, connection.Password);
            }
            catch (ArgumentException)
            {
                
            }
        }
    }
}