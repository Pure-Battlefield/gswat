using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WebFrontend.Handlers;
using WebFrontend.Models;
using WebFrontend.Utilities;
using core;
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
    }
}
