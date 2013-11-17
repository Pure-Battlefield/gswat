using System.Diagnostics;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using WebFrontend.Exceptions;

namespace WebFrontend.Utilities
{
    public class HandleFrontendExceptionsAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is UnauthorizedException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase =
                        context.Exception.Message
                };
            }
            else if (context.Exception is UserNotFoundException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase =
                        context.Exception.Message
                };
            }
        }
    }
}