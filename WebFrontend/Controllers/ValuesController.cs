using core.TableStoreEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Script.Serialization;
using WebFrontend.Models;

namespace WebFrontend.Controllers
{
    public class ValuesController : ApiController
    {
        private static readonly Dictionary<string, string> Squads;
        private static readonly Dictionary<string, string> Teams; 

        static ValuesController()
        {
            Squads = new Dictionary<string, string>();
            Teams = new Dictionary<string, string>();

            Teams["TEAM1"] = "US";
            Teams["TEAM2"] = "RU";

            Squads.Add("SQUAD0", " UNSQUADDED");
            Squads.Add("SQUAD1", " ALPHA");
            Squads.Add("SQUAD2", " BRAVO");
            Squads.Add("SQUAD3", " CHARLIE");
            Squads.Add("SQUAD4", " DELTA");
            Squads.Add("SQUAD5", " ECHO");
            Squads.Add("SQUAD6", " FOXTROT");
            Squads.Add("SQUAD7", " GOLF");
            Squads.Add("SQUAD8", " HOTEL");
            Squads.Add("SQUAD9", " INDIA");
            Squads.Add("SQUAD10", " JULIET");
            Squads.Add("SQUAD11", " KILO");
            Squads.Add("SQUAD12", " LIMA");
            Squads.Add("SQUAD13", " MIKE");
            Squads.Add("SQUAD14", " NOVEMBER");
            Squads.Add("SQUAD15", " OSCAR");
            Squads.Add("SQUAD16", " PAPA");
            Squads.Add("SQUAD17", " QUEBEC");
            Squads.Add("SQUAD18", " ROMEO");
            Squads.Add("SQUAD19", " SIERRA");
            Squads.Add("SQUAD20", " TANGO");
            Squads.Add("SQUAD21", " UNIFORM");
            Squads.Add("SQUAD22", " VICTOR");
            Squads.Add("SQUAD23", " WHISKEY");
            Squads.Add("SQUAD24", " XRAY");
            Squads.Add("SQUAD25", " YANKEE");
            Squads.Add("SQUAD26", " ZULU");
        }

        [HttpGet]
        [ActionName("GetAllMessages")]
        public string GetAllMessages()
        {
            IEnumerable<ChatMessageEntity> q = GlobalStaticVars.StaticCore.GetMessageQueue();
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(q);
        }

        [HttpGet]
        [ActionName("GetAllMessagesFromTime")]
        public string GetAllMessagesFromTime([FromUri] double timestamp)
        {
            /*timestamp is a unix timestamp of milliseconds since the epoch.*/
            /*TODO: Note that this is still unsafe; while it is *highly unlikely* that two messages could be received in under a ms,
             * there still exists a race condition here, and a message may not be sent.  This should be fixed with sessions.  
            */
            var q = GlobalStaticVars.StaticCore.GetMessageQueue();
            var constructedDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            constructedDateTime = constructedDateTime.AddMilliseconds(timestamp);

            var output = q.Where(chatMessage => (chatMessage.MessageTimeStamp - constructedDateTime).TotalMilliseconds >= 1).ToList();

            var json = new JavaScriptSerializer();
            return json.Serialize(output);
        }

        [HttpGet]
        [ActionName("DownloadByDay")]
        public HttpResponseMessage DownloadByDay([FromUri] DateTimeInfo dateTime)
        {
            try
            {
                DateTime temp = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                IEnumerable<ChatMessageEntity> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
                const string messageFmt = @"[{0}] [{1}] {2}:  {3}";
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);

                foreach (var message in q)
                {
                    message.MessageType = message.MessageType.ToUpperInvariant();
                    foreach (var teamPair in Teams)
                    {
                        message.MessageType = message.MessageType.Replace(teamPair.Key, teamPair.Value);
                    }

                    foreach (var squadPair in Squads)
                    {
                        message.MessageType = message.MessageType.Replace(squadPair.Key, squadPair.Value);
                    }

                    writer.Write(String.Format(messageFmt, message.MessageTimeStamp, message.MessageType,
                                               message.Speaker,
                                               message.Text) + "\n");
                }
                writer.Flush();
                stream.Position = 0;

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                var disposition = new ContentDispositionHeaderValue("attachment");
                disposition.FileName = string.Format("{0}.txt", temp.ToString("yyyyMMdd"));

                result.Content.Headers.ContentDisposition = disposition;

                return result;
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        [HttpGet]
        [ActionName("GetByDay")]
        public string GetByDay([FromUri]DateTimeInfo dateTime)
        {
            try
            {
                DateTime temp = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                IEnumerable<ChatMessageEntity> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
                JavaScriptSerializer json = new JavaScriptSerializer();
                return json.Serialize(q);
            }
            catch (Exception e)
            {
                JavaScriptSerializer json = new JavaScriptSerializer();
                return json.Serialize(new List<ChatMessageEntity>());
            }
        }

        [HttpPost]
        [ActionName("SetServerInfo")]
        public String SetServerInfo([FromBody]ConnectionInfo connection)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            try
            {
                return json.Serialize(GlobalStaticVars.StaticCore.Connect(connection.ServerIP, connection.ServerPort, connection.Password, connection.OldPassword));
            }
            catch (Exception e)
            {
                return json.Serialize(e.Message);
            }
        }

        /** Queries the Azure storage for the setting saved for the given Server
         *  using the Core.LoadServerSettings() method. <Auth> */

       [HttpGet]
       [ActionName("GetServerSettings")]
       public String GetServerSettings()
       {
           // Query Azure Storage ** Right now were using Last and Server because of the current StorageScheme
           var settings = GlobalStaticVars.StaticCore.LoadServerSettings("Last", "Server");
           JavaScriptSerializer json = new JavaScriptSerializer();
           return json.Serialize(new string[]{settings.Address,settings.Port.ToString()});
       }
    }
}