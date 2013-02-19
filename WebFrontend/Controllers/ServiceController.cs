using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using WebFrontend.Models;
using core.TableStoreEntities;

namespace WebFrontend.Controllers
{
    public class ServerInfoService : Service
    {
        public object Put(ConnectionInfo connection)
        {
            var json = new JavaScriptSerializer();

            try
            {
                Response.StatusCode = 200; // HttpStatusCode.OK
                return
                    json.Serialize(GlobalStaticVars.StaticCore.Connect(connection.ServerIP, connection.ServerPort,
                                                                       connection.Password,
                                                                       connection.OldPassword));
            }
            catch (ArgumentException e)
            {
                Response.StatusCode = 500; // HttpStatusCode.Error
                return json.Serialize(e.Message);
            }
        }

        /** Queries the Azure storage for the setting saved for the given Server
         *  using the Core.LoadServerSettings() method. */

        public object Get(GetServerInfoModel getServerInfo)
        {
            // Query Azure Storage ** Right now were using Last and Server because of the current StorageScheme
            ServerSettingsEntity settings = GlobalStaticVars.StaticCore.LoadServerSettings("Last", "Server");

            var json = new JavaScriptSerializer();

            if (settings != null)
            {
                return
                    json.Serialize(new Dictionary<String, String>
                        {
                            {"ServerIP", settings.Address},
                            {"ServerPort", settings.Port.ToString()}
                        });
            }
            else
            {
                return
                    json.Serialize(new Dictionary<String, String>
                        {
                            {"ServerIP", ""},
                            {"ServerPort", ""}
                        });
            }
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

        public object Get(DateTimeInfo timestamp)
        {
            timestamp.Action = timestamp.Action.ToLower();
            switch (timestamp.Action)
            {
                case ("getfromtime"):
                    return GetAllMessagesFromTime(timestamp);
                case ("downloadbyday"):
                    return DownloadByDay(timestamp);
                case ("getbyday"):
                    return GetByDay(timestamp);
                default:
                    return GetAllMessages();

            }
        }

        # region MessageFunctions

        public string GetAllMessages()
        {
            IEnumerable<ChatMessageEntity> q = GlobalStaticVars.StaticCore.GetMessageQueue();
            var json = new JavaScriptSerializer();
            return json.Serialize(q);
        }

        public string GetAllMessagesFromTime(DateTimeInfo dateTime)
        {
            /*timestamp is a unix timestamp of milliseconds since the epoch.*/
            /*TODO: Note that this is still unsafe; while it is *highly unlikely* that two messages could be received in under a ms,
             * there still exists a race condition here, and a message may not be sent.  This should be fixed with sessions.  
            */
            IEnumerable<ChatMessageEntity> q = GlobalStaticVars.StaticCore.GetMessageQueue();
            var constructedDateTime = UnixTimeStampToDateTime(dateTime.DateTimeUnix);

            List<ChatMessageEntity> output =
                q.Where(chatMessage => (chatMessage.MessageTimeStamp - constructedDateTime).TotalMilliseconds >= 1)
                 .ToList();

            var json = new JavaScriptSerializer();
            return json.Serialize(output);
        }

        public IHttpResult DownloadByDay(DateTimeInfo dateTime)
        {
            try
            {
                var timestamp = UnixTimeStampToDateTime(dateTime.DateTimeUnix);
                IEnumerable<ChatMessageEntity> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(timestamp);
                const string messageFmt = @"[{0}] [{1}] {2}:  {3}";
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);

                foreach (ChatMessageEntity message in q)
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

                
                var response = new HttpResult(stream, "text/plain");
                response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}.txt", 
                    timestamp.ToString("yyyyMMdd")));

                return response;
            }
            catch (Exception e)
            {
                return new HttpResult(
                    HttpStatusCode.NoContent, "Date not valid");
            }
        }

        public string GetByDay(DateTimeInfo dateTime)
        {
            try
            {
                var temp = UnixTimeStampToDateTime(dateTime.DateTimeUnix);
                IEnumerable<ChatMessageEntity> q = GlobalStaticVars.StaticCore.GetMessagesFromDate(temp);
                var json = new JavaScriptSerializer();
                Response.StatusCode = 200; // HttpStatusCode.OK
                return json.Serialize(q);
            }
            catch (Exception e)
            {
                var json = new JavaScriptSerializer();
                Response.StatusCode = 500; // HttpStatusCode.Error
                return json.Serialize(new List<ChatMessageEntity>());
            }
        }

        #endregion

        #region Service Functions

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970,1,1,0,0,0,0);
            return dtDateTime.AddMilliseconds( unixTimeStamp );
        }

        #endregion
    }
}
