using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Web.Script.Serialization;
using core.Logging;

namespace core.Roles
{
    public class PermissionsUtility
    {
        public IRoleTableStoreUtility RoleUtility;

        public PermissionsUtility()
        {
            RoleUtility = new RoleTableStoreUtility();
        }

        public void SetUserPermissions(UserEntity user)
        {
            RoleUtility.SetUserEntity(user);
        }

        /// <summary>
        /// Attemps to validate a user given his OpenID token as well as a PermissionSet
        /// Every permission in the permissionSet parameter must be present in the user's permission set for the function to return true
        /// </summary>
        /// <param name="token">OpenID token of the user to be validated</param>
        /// <param name="permissionSet">PermissionSet containing all permissions for which the user is to be validated</param>
        /// <returns></returns>
        public bool ValidateToken(string token, PermissionSetEntity permissionSet)
        {
            var request = WebRequest.Create("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=" + token);
            request.Method = "GET";
            //request.ContentLength = 0;
            //request.ContentType = "application/x-www-form-urlencoded";
            string userid = "";
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var js = new JavaScriptSerializer();
                        var obj = js.Deserialize<dynamic>(reader.ReadToEnd());
                        userid = obj["userid"];
                    }
                }

                var user = RoleUtility.GetUserEntity(permissionSet.Namespace, userid);

                //Resharper is amazing - this returns false if any permissions are not found, otherwise returns true
                return permissionSet.PermissionSet.All(permission => user.Permissions.PermissionSet.Contains(permission));
            }
            catch (Exception e)
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
