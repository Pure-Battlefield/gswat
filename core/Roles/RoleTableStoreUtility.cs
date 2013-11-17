using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using core.Logging;
using core.Roles.Models;
using core.Utilities;

namespace core.Roles
{
    /// <summary>
    /// Handles all table store operations for roles
    /// </summary>
    public class RoleTableStoreUtility : IRoleTableStoreUtility
    {
        private readonly CloudTable permissionSetTable;
        private readonly CloudTable userTable;
        private readonly CloudTable unboundPermissionTable;

        public RoleTableStoreUtility(ICloudSettingsManager settingsManager)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(settingsManager.GetConfigurationSettingValue("StorageConnectionString"));
                var tableClient = storageAccount.CreateCloudTableClient();

                permissionSetTable = tableClient.GetTableReference("permissionSets");
                permissionSetTable.CreateIfNotExists();

                userTable = tableClient.GetTableReference("users");
                userTable.CreateIfNotExists();

                unboundPermissionTable = tableClient.GetTableReference("unboundpermissions");
                unboundPermissionTable.CreateIfNotExists();
            }
            catch (Exception e)
            {
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
            }
        }

        public PermissionSetEntity GetPermissionSetEntity(string nameSpace)
        {
            try
            {
                var retrieveOp = TableOperation.Retrieve<PermissionSetEntity>(nameSpace, nameSpace);
                var result = permissionSetTable.Execute(retrieveOp);

                if (result.Result != null)
                {
                    return (PermissionSetEntity) result.Result;
                }
            }
            catch (Exception e)
            {
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return null;
        }

        public void SetPermissionSetEntity(PermissionSetEntity permissionSet)
        {
            permissionSet.ETag = "*";
            var permissionSetEntity = GetPermissionSetEntity(permissionSet.Namespace);
            if (permissionSetEntity == null)
            {
                var insertOp = TableOperation.Insert(permissionSet);
                permissionSetTable.Execute(insertOp);
            }
            else
            {
                try
                {
                    permissionSetEntity.Namespace = permissionSet.Namespace;
                    permissionSetEntity.PermissionSet = (List<string>) permissionSet.GetPermissionSet();
                    var insertOrReplaceOperation = TableOperation.InsertOrReplace(permissionSetEntity);
                    permissionSetTable.Execute(insertOrReplaceOperation);
                }
                catch (Exception e)
                {
                    LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        public UserEntity GetUserEntity(string nameSpace, string googleId)
        {
            try
            {
                var retrieveOp = TableOperation.Retrieve<UserEntity>(nameSpace, googleId);
                var result = userTable.Execute(retrieveOp);

                if (result.Result != null)
                {
                    return (UserEntity) result.Result;
                }
            }
            catch (Exception e)
            {
                //TODO: Improve this pokemon exception handling.
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return null;
        }

        public IEnumerable<UserEntity> GetUserEntitiesInNamespace(string nameSpace)
        {
            try
            {
                var query =
                    new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                        QueryComparisons.Equal, nameSpace));
                var result = userTable.ExecuteQuery(query);

                return result;
            }
            catch (Exception e)
            {
                //TODO: Improve this pokemon exception handling.
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return null;
        }

        public void SetUserEntity(UserEntity user)
        {
            user.ETag = "*";
            try
            {
                var setOp = TableOperation.InsertOrReplace(user);
                userTable.Execute(setOp);
            }
            catch (Exception e)
            {
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                throw;
            }
        }

        public void DeleteUserEntity(UserEntity user)
        {
            try
            {
                var deleteOp = TableOperation.Delete(user);
                userTable.Execute(deleteOp);
            }
            catch (Exception e)
            {
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
            }
        }

        public UnboundPermissionSetEntity GetUnboundPermissionSetEntity(string nameSpace, Guid guid)
        {
            try
            {
                var retrieveOp = TableOperation.Retrieve<UnboundPermissionSetEntity>(nameSpace, guid.ToString());
                var result = unboundPermissionTable.Execute(retrieveOp);

                if (result.Result != null)
                {
                    return (UnboundPermissionSetEntity)result.Result;
                }
            }
            catch (Exception e)
            {
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return null;
        }

        public void AddOrUpdateUnboundPermission(UnboundPermissionSetEntity permissions)
        {
            permissions.ETag = "*";
            try
            {
                var insertOp = TableOperation.InsertOrReplace(permissions);
                unboundPermissionTable.Execute(insertOp);
            }
            catch (Exception e)
            {
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                throw;
            }
            
        }

        public void DeleteUnboundPermission(UnboundPermissionSetEntity permissions)
        {
            try
            {
                var deleteOp = TableOperation.Delete(permissions);
                unboundPermissionTable.Execute(deleteOp);
            }
            catch (Exception e)
            {
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
            }
        }

        public bool ConfirmEmailToken(IValidatableUser user, Guid permissionsToken)
        {
            if (user == null)
            {
                throw new ArgumentException("Cannot be null", "user");
            }

            var userId = user.GetGoogleId();
            if (userId == null)
            {
                throw new ArgumentException("User does not return valid token", "user");
            }

            var permissions = GetUnboundPermissionSetEntity("GSWAT", permissionsToken);
            if (permissions == null)
            {
                return false;
            }
            else
            {
                //Get the user object so we don't overwrite anything unless the user doesn't exist.
                var userEntity = GetUserEntity(permissions.Namespace, user.GetGoogleId());
                if (userEntity == null)
                {
                    userEntity = new UserEntity(user.GetGoogleId(), "", "", true, permissions.Permissions)
                                     {
                                         ETag = Guid.NewGuid().ToString()
                                     };
                    SetUserEntity(userEntity);
                }
                else
                {
                    userEntity.Permissions.AddPermissions(permissions.Permissions.GetPermissionSet());
                    SetUserEntity(userEntity);
                }
                return true;
            }
        }
    }
}
