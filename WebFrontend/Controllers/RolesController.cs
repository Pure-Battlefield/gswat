using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebFrontend.Exceptions;
using WebFrontend.Models;
using core;
using core.Roles;
using core.Roles.Models;
using core.Utilities;
using WebFrontend.Utilities;

namespace WebFrontend.Controllers
{
    [HandleFrontendExceptions]
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

        public JsonResult Get(AuthenticatedUser requestingUser, string id = null)
        {
            
            if(requestingUser == null || !_core.PermissionsUtil.ValidateUser(requestingUser,
                                                new PermissionSetEntity("gswat", new List<string> {"operations"})))
            {
                throw new UnauthorizedException("You must be a member of operations to view users.");
            }

            if (id != null)
            {
                var userEntity = _roleUtility.GetUserEntity("gswat", id);
                if (userEntity == null)
                {
                    throw new UserNotFoundException(String.Format("User with id {0} not found", id));
                }
                //TODO: namespace change when multi-server/plugin support added
                var user = new User
                           {
                               Email = userEntity.Email,
                               GoogleId = Int64.Parse(userEntity.GoogleIDNumber),
                               Namespace = "gswat",
                               Permissions = new List<string>(userEntity.Permissions.GetPermissionSet())
                           };

                var result = new JsonResult {Data = user};
                return result;
            }
            //TODO:  Fill in when PermissionsUtility has code to get all users.  
            throw new NotImplementedException();
            
        }

        public HttpStatusCodeResult Put(HttpRequestMessage request, AuthenticatedUser requestingUser, [FromBody] User userToAdd)
        {
            if (requestingUser == null || !_core.PermissionsUtil.ValidateUser(requestingUser,
                                                   new PermissionSetEntity("gswat", new List<string> {"operations"})))
            {
                throw new UnauthorizedException("You must be an administrator to add or update.");
            }
            var response = ValidateUserToAdd(userToAdd);
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
                
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,
                                              "User could not be added.");
            }

            //TODO: Find out what the link needs to be from frontend devs!
            _mailer.SendMail(userToAdd.Email, "You have been granted permissions on GSWAT", "<a>Click me!</a> (If you can't click this link, enter this special code at the given place on the site: " + unboundPermissionsGuid);

            return new HttpStatusCodeResult(HttpStatusCode.Created);
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

        private HttpStatusCodeResult ValidateUserToAdd(User userToAdd)
        {
            if (userToAdd == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                                              "userToAdd is null.");
            }
            if (string.IsNullOrEmpty(userToAdd.Email))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                                              "User e-mail address is empty or null.");
            }
            if (string.IsNullOrEmpty(userToAdd.Email.Trim()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                                              "User e-mail address was just a bunch of spaces.  You monster.");
            }
            if (string.IsNullOrEmpty(userToAdd.Namespace))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                                              "User Namespace is empty or null.");
            }
            if (string.IsNullOrEmpty(userToAdd.Namespace.Trim()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                                              "User Namespace was just a bunch of spaces.  You monster.");
            }
            if (userToAdd.Permissions == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                                              "The Permissions list was null.");
            }
            return null;
        }
    }
}
