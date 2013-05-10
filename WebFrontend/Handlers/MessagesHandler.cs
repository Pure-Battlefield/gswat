using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using WebFrontend.Exceptions;
using WebFrontend.Models;
using WebFrontend.Utilities;
using core;
using core.Roles;
using core.TableStoreEntities;

namespace WebFrontend.Handlers
{
    public interface IMessagesHandler
    {
        IEnumerable<ChatMessageEntity> RetrieveAllMessages();
        IEnumerable<ChatMessageEntity> RetrieveAllMessagesFromTime(DateTimeInfo dateTime);
        IEnumerable<ChatMessageEntity> RetrieveByDay(DateTimeInfo dateTime);
        HttpResponseMessage DownloadByDay(DateTimeInfo dateTime);
        void ImportMessages(IList<ChatMessageEntity> messages);
        void AdminSay(string message, string admin, AuthenticatedUser userInfo, IList<string> playerNames = null, string teamId = null, string squadId = null);
    }
    
    public class MessagesHandler : IMessagesHandler
    {
        private readonly ICore core;

        public MessagesHandler(ICore core)
        {
            this.core = core;
        }

        public void ImportMessages(IList<ChatMessageEntity> messages)
        {
            core.StoreMessagesIntoTableStore(messages);
        }

        public IEnumerable<ChatMessageEntity> RetrieveAllMessages()
        {
            var q = core.GetMessageQueue();

            return q;
        }

        public IEnumerable<ChatMessageEntity> RetrieveAllMessagesFromTime(DateTimeInfo dateTime)
        {
            /*timestamp is a unix timestamp of milliseconds since the epoch.*/
            /*TODO: Note that this is still unsafe; while it is *highly unlikely* that two messages could be received in under a ms,
             * there still exists a race condition here, and a message may not be sent.  This should be fixed with sessions.  
            */
            var q = core.GetMessageQueue();
            var constructedDateTime = UnixTimeStampToDateTime(dateTime.DateTimeUnix);

            var output =
                q.Where(chatMessage => (chatMessage.MessageTimeStamp - constructedDateTime).TotalMilliseconds >= 1)
                 .ToList();

            return output;
        }

        public IEnumerable<ChatMessageEntity> RetrieveByDay(DateTimeInfo dateTime)
        {
            var temp = UnixTimeStampToDateTime(dateTime.DateTimeUnix);
            return core.GetMessagesFromDate(temp);
        }

        public HttpResponseMessage DownloadByDay(DateTimeInfo dateTime)
        {
            var Teams = TeamNameConverter.Teams;
            var Squads = SquadNameConverter.Squads;
            try
            {
                var temp = UnixTimeStampToDateTime(dateTime.DateTimeUnix);
                var q = core.GetMessagesFromDate(temp);
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

        public void AdminSay(string message, string admin, AuthenticatedUser userInfo, IList<string> playerNames = null, string teamId = null, string squadId = null)
        {
            /*if (userInfo == null || !core.PermissionsUtil.ValidateUser(userInfo.Token, userInfo.Email,
                                                   new PermissionSetEntity("gswat", new List<string> {"admin"})))
            {
                throw new AuthorizationValidationException("You must be an administrator to send chat.");
            }*/


            var newMsg = String.Format("[{0}]:  {1}", admin, message);
            if (playerNames != null)
            {
                foreach (var player in playerNames)
                {
                    core.SendAdminSay(newMsg, player);
                }
            }
            else
            {
                core.SendAdminSay(newMsg, teamId: teamId, squadId: squadId);
            }
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dtDateTime.AddMilliseconds(unixTimeStamp);
        }
    }
}