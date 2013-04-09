using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using WebFrontend.Exceptions;
using WebFrontend.Handlers;
using WebFrontend.Models;
using core.TableStoreEntities;

namespace WebFrontend.Controllers
{
    public class MessagesController : ApiController
    {
        private readonly IMessagesHandler messagesHandler;

        public MessagesController(IMessagesHandler messagesHandler)
        {
            this.messagesHandler = messagesHandler;
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
                        return request.CreateResponse(HttpStatusCode.OK, messagesHandler.RetrieveAllMessagesFromTime(timestamp));
                    }
                    catch (Exception)
                    {
                        return request.CreateResponse(HttpStatusCode.InternalServerError, new List<ChatMessageEntity>());
                    }
                case ("downloadbyday"):
                    return messagesHandler.DownloadByDay(timestamp);
                case ("getbyday"):
                    try
                    {
                        return request.CreateResponse(HttpStatusCode.OK, messagesHandler.RetrieveByDay(timestamp));
                    }
                    catch (Exception)
                    {
                        return request.CreateResponse(HttpStatusCode.InternalServerError, new List<ChatMessageEntity>());
                    }
                default:
                    var messages = messagesHandler.RetrieveAllMessages();
                    return request.CreateResponse(HttpStatusCode.OK, messages);
            }
        }
        
        [HttpPost]
        public HttpResponseMessage Post([FromBody]InboundMessageWrapper messages)
        {
            if (messages != null)
            {
                messagesHandler.ImportMessages(messages.Data);
                return Request.CreateResponse(HttpStatusCode.Accepted);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        public HttpResponseMessage Put([FromUri]AuthenticatedUser user, [FromBody]AdminChatWrapper chatMessage)
        {
            if (chatMessage == null || chatMessage.Message == null || chatMessage.AdminName == null || chatMessage.Type == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "chatMessage object, chatMessage message, or chatMessage admin name not specified.");
            }
            try
            {
                switch (chatMessage.Type.ToLower())
                {
                    case "say":
                        if (chatMessage.PlayerTargets != null)
                        {
                            messagesHandler.AdminSay(chatMessage.Message, chatMessage.AdminName,
                                                     user, chatMessage.PlayerTargets);
                        }
                        else
                        {
                            messagesHandler.AdminSay(chatMessage.Message, chatMessage.AdminName, user,
                                                     teamId: chatMessage.TeamId, squadId: chatMessage.SquadId);
                        }
                        break;
                    case "yell":
                        break;
                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "type must be either say or yell.");
                }

            }
            catch (ArgumentException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (AuthorizationValidationException e)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
    public class InboundMessageWrapper{
        public List<ChatMessageEntity> Data { get; set; }
    }

    public class AdminChatWrapper
    {
        public string Message { get; set; }
        public string AdminName { get; set; }
        public string Type { get; set; }
        public IList<string> PlayerTargets { get; set; }
        public string TeamId { get; set; }
        public string SquadId { get; set; }
    }

}
