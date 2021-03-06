﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Script.Serialization;
using core.Logging;
using core.Roles.Models;
using core.Utilities;

namespace core.Roles
{
    public interface IPermissionsUtility
    {
        /// <summary>
        /// Attemps to validate a user given his OpenID token as well as a PermissionSet
        /// Every permission in the permissionSet parameter must be present in the user's permission set for the function to return true
        /// </summary>
        /// <param name="user">AuthenticatedUser object which contains the user token to validate</param>
        /// <param name="permissionSet">PermissionSet containing all permissions for which the user is to be validated</param>
        /// <returns></returns>
        bool ValidateUser(IValidatableUser user, PermissionSetEntity permissionSet);

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
        /// <returns></returns>
        public bool ValidateUser(IValidatableUser user, PermissionSetEntity permissionSet)
        {
            try
            {
                var userid = user.GetGoogleId();

                //First check to see if the user is a Service Administrator (All permissions).  If so, validate as true.  
                var serviceAdmins = settingsManager.GetConfigurationSettingValue("ServiceAdministrators").Split(',');
                if (serviceAdmins.Any(serviceAdmin => userid == serviceAdmin.Trim()))
                {
                    return true;
                }

                var existingUser = RoleUtility.GetUserEntity(permissionSet.Namespace, userid);

                //Resharper is amazing - this returns false if any permissions are not found or the user is null, otherwise returns true
                var validated = existingUser != null && permissionSet.GetPermissionSet().All(permission => existingUser.Permissions.GetPermissionSet().Contains(permission));

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
