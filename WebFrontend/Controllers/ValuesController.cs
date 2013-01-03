using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Diagnostics;
using WebFrontend.Models;
using core.ChatMessageUtilities;

namespace WebFrontend.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        [ActionName("GetAllMessages")]
        public string GetAllMessages()
        {
            IEnumerable<ChatMessage> q = GlobalStaticVars.StaticCore.GetMessageQueue();
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(q);
        }

        [HttpGet]
        [ActionName("DownloadByDay")]
        public HttpResponseMessage DownloadByDay([FromUri] DateTimeInfo dateTime)
        {
            DateTime temp = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            IEnumerable<ChatMessage> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
            const string messageFmt = @"[{0}] [{1}] {2}:  {3}";
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            foreach (var message in q)
            {
                writer.Write(String.Format(messageFmt, message.MessageTimeStamp, message.MessageType, message.Speaker,
                                  message.Text) + "\n");
            }
            writer.Flush();
            stream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/octet-stream");

            return result;
        }

        [HttpGet]
        [ActionName("GetByDay")]
        public string GetByDay([FromUri]DateTimeInfo dateTime)
        {
                DateTime temp = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                IEnumerable<ChatMessage> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
                JavaScriptSerializer json = new JavaScriptSerializer();
                return json.Serialize(q);
        }

        [HttpPost]
        [ActionName("SetServerInfo")]
        public void SetServerInfo([FromBody]ConnectionInfo connection)
        {
            try
            {
                GlobalStaticVars.StaticCore.Connect(connection.ServerIP, connection.ServerPort, connection.Password, connection.OldPassword);
            }
            catch (ArgumentException)
            {
                
            }
        }
    }
}