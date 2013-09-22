using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebFrontend.Models;
using core;
using core.Roles;
using core.Roles.Models;
using core.Utilities;

namespace WebFrontend.Controllers
{
    public class RolesController : ApiController
    {
        private readonly ICore _core;
        private readonly IRoleTableStoreUtility _roleUtility;
        private readonly IMailer _mailer;

        public RolesController(ICore core, IRoleTableStoreUtility roleUtility, IMailer mailer)
        {
            _core = core;
            _roleUtility = roleUtility;
            _mailer = mailer;
        }

        public HttpResponseMessage Get(AuthenticatedUser requestingUser)
        {
            if(requestingUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden,
                                              "You must be an administrator to view user roles.");
            }

            //TODO:  Fill in when PermissionsUtility has code to get all users.  
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public HttpResponseMessage Put(HttpRequestMessage request, AuthenticatedUser requestingUser, [FromBody] User userToAdd)
        {
            if (requestingUser == null || !_core.PermissionsUtil.ValidateUser(requestingUser,
                                                   new PermissionSetEntity("gswat", new List<string> {"operations"})))
            {
                return request.CreateResponse(HttpStatusCode.Forbidden,
                                              "You must be an administrator to add or update.");
            }
            var response = ValidateUserToAdd(userToAdd, request);
            if (response != null)
            {
                return response;
            }

            var unboundPermissionsGuid = Guid.NewGuid();
            var permissions = new PermissionSetEntity(userToAdd.Namespace, userToAdd.Permissions);
            var unboundPermissionSet = new UnboundPermissionSetEntity
                                           {
                                               Namespace = userToAdd.Namespace,
                                               Permissions = permissions,
                                               Guid = unboundPermissionsGuid.ToString()
                                           };
            try
            {
                _roleUtility.AddOrUpdateUnboundPermission(unboundPermissionSet);
            }
            catch (Exception)
            {
                return request.CreateResponse(HttpStatusCode.InternalServerError,
                                              "User could not be added.");
            }

            //TODO: Find out what the link needs to be from frontend devs!
            _mailer.SendMail(userToAdd.Email, "You have been granted permissions on GSWAT", "<a>Click me!</a> (If you can't click this link, enter this special code at the given place on the site: " + unboundPermissionsGuid);

            return request.CreateResponse(HttpStatusCode.Created);
        }

        public HttpResponseMessage Post(HttpRequestMessage request, AuthenticatedUser requestingUser, string id, string subresource, [FromBody]string confirmToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest,
                                              "Updaing a user requires an id.  Specify 'me' for the current user.");
            }
            if (requestingUser == null)
            {
                request.CreateResponse(HttpStatusCode.BadRequest, "RequestingUser (with Token) must be specified.");
            }

            id = requestingUser.GetGoogleId();

            if (string.IsNullOrEmpty(id))
            {
                request.CreateResponse(HttpStatusCode.BadRequest,
                                       "Invalid Google user token.");
            }

            switch (subresource)
            {
                case "":
                    throw new NotImplementedException();
                case "emailconfirmation":

                    if (string.IsNullOrEmpty(confirmToken))
                    {
                        return request.CreateResponse(HttpStatusCode.BadRequest, "No confirmation token provided.");
                    }
                    var confirmationToken = Guid.Parse(confirmToken);

                    var success = _roleUtility.ConfirmEmailToken(requestingUser, confirmationToken);

                    if (!success)
                    {
                        return request.CreateResponse(HttpStatusCode.BadRequest, "Invalid confirmation token.");
                    }

                    return request.CreateResponse(HttpStatusCode.OK);
                default:
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Invalid subresource specified");
            }
        }

        private HttpResponseMessage ValidateUserToAdd(User userToAdd, HttpRequestMessage request)
        {
            if (userToAdd == null)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest,
                                              "userToAdd is null.");
            }
            if (string.IsNullOrEmpty(userToAdd.Email))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest,
                                              "User e-mail address is empty or null.");
            }
            if (string.IsNullOrEmpty(userToAdd.Email.Trim()))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest,
                                              "User e-mail address was just a bunch of spaces.  You monster.");
            }
            if (string.IsNullOrEmpty(userToAdd.Namespace))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest,
                                              "User Namespace is empty or null.");
            }
            if (string.IsNullOrEmpty(userToAdd.Namespace.Trim()))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest,
                                              "User Namespace was just a bunch of spaces.  You monster.");
            }
            if (userToAdd.Permissions == null)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest,
                                              "The Permissions list was null.");
            }
            return null;
        }
    }
}
