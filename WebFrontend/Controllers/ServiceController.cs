using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Script.Serialization;
using ServiceStack.ServiceInterface;
using WebFrontend.Models;
using core.ChatMessageUtilities;
using core.TableStore;

namespace WebFrontend.Controllers
{
    public class ServerInfoService : Service
    {
        public object Post(ConnectionInfo connection)
        {
            var json = new JavaScriptSerializer();

            try
            {
                return json.Serialize(GlobalStaticVars.StaticCore.Connect(connection.ServerIP, connection.ServerPort, connection.Password,
                                                    connection.OldPassword));
            }
            catch (ArgumentException e)
            {
                return json.Serialize(e.Message);
            }
        }

        /** Queries the Azure storage for the setting saved for the given Server
         *  using the Core.LoadServerSettings() method. */

        public object Get(GetServerInfoModel getServerInfo)
        {
            // Query Azure Storage ** Right now were using Last and Server because of the current StorageScheme
            ServerConfig settings = GlobalStaticVars.StaticCore.LoadServerSettings("Last", "Server");

            JavaScriptSerializer json = new JavaScriptSerializer();

            return json.Serialize(new string[] { settings.Address, settings.Port.ToString() });
        }
    }

    public class MessagesService : Service
    {
        private static readonly Dictionary<string, string> Squads;
        private static readonly Dictionary<string, string> Teams; 

        static MessagesService()
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
        public object Post(DateTimeInfo timestamp)
        {
            switch (timestamp.Action)
            {
                case ("GetFromTime"):
                    return GetAllMessagesFromTime(timestamp);
                    break;
                case ("Download"):
                    return DownloadByDay(timestamp);
                    break;
                case ("GetByDay"):
                    return GetByDay(timestamp);
                    break;
            }

            return null;
        }

        # region MessageFunctions
        public string GetAllMessagesFromTime(DateTimeInfo timestamp)
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

        public HttpResponseMessage DownloadByDay(DateTimeInfo dateTime)
        {
            try
            {
                DateTime temp = new DateTime(dateTime.DateTimeUnix);
                IEnumerable<ChatMessage> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
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

        public string GetByDay(DateTimeInfo dateTime)
        {
            try
            {
                DateTime temp = new DateTime(dateTime.DateTimeUnix);
                IEnumerable<ChatMessage> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
                JavaScriptSerializer json = new JavaScriptSerializer();
                return json.Serialize(q);
            }
            catch (Exception e)
            {
                JavaScriptSerializer json = new JavaScriptSerializer();
                return json.Serialize(new List<ChatMessage>());
            }
        }

        #endregion
    }
}
