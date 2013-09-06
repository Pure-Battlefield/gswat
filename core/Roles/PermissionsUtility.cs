using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Script.Serialization;
using core.Logging;
using core.Utilities;

namespace core.Roles
{
    public interface IPermissionsUtility
    {
        /// <summary>
        /// Adds or updates a user in the data store.  If a user is updated to have an empty PermissionSetEntity, 
        /// the user will be removed from the data store.  
        /// </summary>
        /// <param name="user">The user to be added to the data store or updated.</param>
        void AddorUpdateUser(UserEntity user);

        /// <summary>
        /// Attemps to validate a user given his OpenID token as well as a PermissionSet
        /// Every permission in the permissionSet parameter must be present in the user's permission set for the function to return true
        /// </summary>
        /// <param name="token">OpenID token of the user to be validated</param>
        /// <param name="permissionSet">PermissionSet containing all permissions for which the user is to be validated</param>
        /// <param name="debugID">Only needed for debugging; leave blank.</param>
        /// <returns></returns>
        bool ValidateUser(string token, PermissionSetEntity permissionSet, string debugID = "");

        /// <summary>
        /// Loads permissions for all plugins from the global config file.
        /// </summary>
        void LoadPermissionsFromConfig();
    }

    public class PermissionsUtility : IPermissionsUtility
    {
        public IRoleTableStoreUtility RoleUtility;
        private readonly ICloudSettingsManager settingsManager;

        public PermissionsUtility(IRoleTableStoreUtility roleUtility, ICloudSettingsManager settingsManager)
        {
            RoleUtility = roleUtility;
            this.settingsManager = settingsManager;
        }

        /// <summary>
        /// Adds or updates the user in Table Store using the Role Utility.  
        /// If the user has no Google ID, an UnboundPermissionsEntity will be created.  The permissions will be stored
        /// in the UnboundPermissionsEntity until the user logs in for the first time, at which time the proper UserEntity
        /// will be created.  
        /// </summary>
        /// <param name="user">The UserEntity to be created.</param>
        public void AddorUpdateUser(UserEntity user)
        {
            if (user.GoogleIDNumber != null)
            {
                RoleUtility.SetUserEntity(user);
            }
            //If the GoogleID is unknown, we need to make an UnboundPermissionSet to be bound upon verification.  
            else if (user.Email != null)
            {
                var ubps = new UnboundPermissionSetEntity();
                ubps.Email = user.Email;
                ubps.Permissions = user.Permissions;
                ubps.Namespace = user.Permissions.Namespace;
                RoleUtility.AddOrUpdateUnboundPermission(ubps);
            }
        }

        /// <summary>
        /// Attemps to validate a user given his OpenID token as well as a PermissionSet
        /// Every permission in the permissionSet parameter must be present in the user's permission set for the function to return true
        /// In addition, the user record is updated according to the following rules:
        ///     1. The token is used to retrieve the GoogleIDNumber of the user
        ///     2. The GoogleIDNumber is used to retrieve the user from the database
        ///     3. If the entry does not exist, return false
        ///     3. If the email in the record does not match the email provided by the user parameter, the table is updated with the new email
        /// </summary>
        /// <param name="token">Google auth token</param>
        /// <param name="permissionSet">PermissionSet containing all permissions for which the user is to be validated</param>
        /// <param name="debugID">Debug parameter if we want to use our own ID</param>
        /// <returns></returns>
        public bool ValidateUser(string token, PermissionSetEntity permissionSet, string debugID)
        {
            try
            {
                string userid = "";
                string email = "";
                if (debugID.Equals(""))
                {
                    var request = WebRequest.Create("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=" + token);
                    request.Method = "GET";
                    using (var response = (HttpWebResponse) request.GetResponse())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var js = new JavaScriptSerializer();
                            var obj = js.Deserialize<dynamic>(reader.ReadToEnd());
                            userid = obj["user_id"];
                            email = obj["email"];
                        }
                    }
                }
                else
                {
                    userid = debugID;
                }

                //First check to see if the user is a Service Administrator (All permissions).  If so, validate as true.  
                var serviceAdmins = settingsManager.GetConfigurationSettingValue("ServiceAdministrators").Split(',');
                if (serviceAdmins.Any(serviceAdmin => userid == serviceAdmin.Trim()))
                {
                    return true;
                }

                var existingUser = RoleUtility.GetUserEntity(permissionSet.Namespace, userid);

                //Resharper is amazing - this returns false if any permissions are not found or the user is null, otherwise returns true
                var validated = existingUser != null && permissionSet.PermissionSet.All(permission => existingUser.Permissions.PermissionSet.Contains(permission));

                if (!validated)
                {
                    
                }

                return validated;
            }
            catch (Exception e) //TODO: Make this specific to TableStore Exceptions only.  
            {
                if (!(e is WebException))
                {
                    LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// Loads permissions for all plugins from the global config file.
        /// </summary>
        public void LoadPermissionsFromConfig()
        {
            var config = Resources.Permissions;
            var lines = config.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var permissionSets = new List<PermissionSetEntity>();
            var permissionSet = new PermissionSetEntity();
            foreach (var line in lines)
            {
                var split = line.Split(':');

                //If we don't have at least two tokens, gtfo
                if (split.Length <= 1) continue;

                //Isolate the label (NAMESPACE or PERMISSIONS)
                var label = split[0];

                //This ugly-ass line splits a string like "anonymous, admin" into a list of comma-split, trimmed entries.
                var parameters = split[1].Split(',').Select(p => p.Trim()).ToList();

                //Let's not discriminate - nAmEsPaCe works too
                if (label.Equals("NAMESPACE", StringComparison.OrdinalIgnoreCase))
                {
                    permissionSet.Namespace = parameters[0];
                }
                else if(label.Equals("PERMISSIONS", StringComparison.OrdinalIgnoreCase))
                {
                    permissionSet.PermissionSet = parameters;
                    permissionSets.Add(permissionSet);
                    permissionSet = new PermissionSetEntity();
                }
            }

            //Upload each permissionSet to Table Store
            foreach (var pSet in permissionSets)
            {
                RoleUtility.SetPermissionSetEntity(pSet);
            }
        }
    }
}
