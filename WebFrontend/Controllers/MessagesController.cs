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
using WebFrontend.Utilities;
using core.TableStoreEntities;

namespace WebFrontend.Controllers
{
    public class MessagesController : ApiController
    {
        public IEnumerable<ChatMessageEntity> RetrieveAllMessages()
        {
            var q = GlobalStaticVars.StaticCore.GetMessageQueue();
            
            return q;
        }
        // GET api/messages
        public HttpResponseMessage Get(HttpRequestMessage request, long DateTimeUnix, string Action = "")
        {
            var timestamp = new DateTimeInfo {Action = Action, DateTimeUnix = DateTimeUnix};
            timestamp.Action = timestamp.Action.ToLower();
            switch (timestamp.Action)
            {
                case ("getfromtime"):
                    try
                    {
                        return request.CreateResponse(HttpStatusCode.OK, RetrieveAllMessagesFromTime(timestamp));
                    }
                    catch (Exception)
                    {
                        return request.CreateResponse(HttpStatusCode.InternalServerError, new List<ChatMessageEntity>());
                    }
                case ("downloadbyday"):
                    return DownloadByDay(timestamp);
                case ("getbyday"):
                    try
                    {
                        return request.CreateResponse(HttpStatusCode.OK, RetrieveByDay(timestamp));
                    }
                    catch (Exception)
                    {
                        return request.CreateResponse(HttpStatusCode.InternalServerError, new List<ChatMessageEntity>());
                    }
                default:
                    var messages = RetrieveAllMessages();
                    return request.CreateResponse(HttpStatusCode.OK, messages);
            }
        }


        public IEnumerable<ChatMessageEntity> RetrieveAllMessagesFromTime(DateTimeInfo dateTime)
        {
            /*timestamp is a unix timestamp of milliseconds since the epoch.*/
            /*TODO: Note that this is still unsafe; while it is *highly unlikely* that two messages could be received in under a ms,
             * there still exists a race condition here, and a message may not be sent.  This should be fixed with sessions.  
            */
            var q = GlobalStaticVars.StaticCore.GetMessageQueue();
            var constructedDateTime = UnixTimeStampToDateTime(dateTime.DateTimeUnix);

            var output =
                q.Where(chatMessage => (chatMessage.MessageTimeStamp - constructedDateTime).TotalMilliseconds >= 1)
                 .ToList();

            return output;
        }

        public IEnumerable<ChatMessageEntity> RetrieveByDay(DateTimeInfo dateTime)
        {
                var temp = UnixTimeStampToDateTime(dateTime.DateTimeUnix);
                return GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
        }

        public HttpResponseMessage DownloadByDay(DateTimeInfo dateTime)
        {
            var Teams = TeamNameConverter.Teams;
            var Squads = SquadNameConverter.Squads;
            try
            {
                var temp = UnixTimeStampToDateTime(dateTime.DateTimeUnix);
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

                var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
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
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dtDateTime.AddMilliseconds(unixTimeStamp);
        }
    }
}
