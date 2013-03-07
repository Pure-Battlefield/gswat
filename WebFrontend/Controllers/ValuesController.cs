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
        public string GetAllMessagesFromTime([FromUri] DateTimeInfo timestamp)
        {
            /*timestamp is a unix timestamp of milliseconds since the epoch.*/
            /*TODO: Note that this is still unsafe; while it is *highly unlikely* that two messages could be received in under a ms,
             * there still exists a race condition here, and a message may not be sent.  This should be fixed with sessions.  
            */
            var q = GlobalStaticVars.StaticCore.GetMessageQueue();
            var constructedDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            constructedDateTime = constructedDateTime.AddMilliseconds(timestamp.DateTimeUnix);

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
                var temp = new DateTime(dateTime.DateTimeUnix);
                var q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
                const string messageFmt = @"[{0}] [{1}] {2}:  {3}";
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);

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

                var result = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StreamContent(stream)};
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                var disposition = new ContentDispositionHeaderValue("attachment")
                                      {
                                          FileName =
                                              string.Format("{0}.txt",
                                                            temp.ToString(
                                                                "yyyyMMdd"))
                                      };

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
        public IEnumerable<ChatMessageEntity> GetByDay([FromUri]DateTimeInfo dateTime)
        {
            try
            {
                var temp = new DateTime(dateTime.DateTimeUnix);
                var q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
                return q;
            }
            catch (Exception e)
            {
                return new List<ChatMessageEntity>();
            }
        }
    }
}