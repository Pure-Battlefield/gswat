using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using core.Logging;
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
                    permissionSetEntity.PermissionSet = permissionSet.PermissionSet;
                    var insertOrReplaceOperation = TableOperation.InsertOrReplace(permissionSetEntity);
                    permissionSetTable.Execute(insertOrReplaceOperation);
                }
                catch (Exception e)
                {
                    LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        public UserEntity GetUserEntity(string nameSpace, string googleIDNumber)
        {
            try
            {
                var retrieveOp = TableOperation.Retrieve<UserEntity>(nameSpace, googleIDNumber);
                var result = userTable.Execute(retrieveOp);

                if (result.Result != null)
                {
                    return (UserEntity) result.Result;
                }
            }
            catch (Exception e)
            {
                LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return null;
        }

        public void SetUserEntity(UserEntity user)
        {
            user.ETag = "*";
            var userEntity = GetUserEntity(user.Permissions.Namespace, user.GoogleIDNumber);
            if (userEntity == null)
            {
                var insertOp = TableOperation.Insert(user);
                userTable.Execute(insertOp);
            }
            else
            {
                try
                {
                    userEntity.BattlelogID = user.BattlelogID;
                    userEntity.GoogleIDNumber = user.GoogleIDNumber;
                    userEntity.AccountEnabled = user.AccountEnabled;
                    userEntity.Permissions = user.Permissions;
                    var insertOrReplaceOperation = TableOperation.InsertOrReplace(userEntity);
                    userTable.Execute(insertOrReplaceOperation);
                }
                catch (Exception e)
                {
                    LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        public UnboundPermissionSetEntity GetUnboundPermissionSetEntity(string nameSpace, string email)
        {
            try
            {
                var retrieveOp = TableOperation.Retrieve<UnboundPermissionSetEntity>(nameSpace, email);
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

        public void AddOrUpdateUnboundPermission(UnboundPermissionSetEntity user)
        {
            user.ETag = "*";
            var existing = GetUnboundPermissionSetEntity(user.Namespace, user.Email);

            if (existing == null)
            {
                var insertOp = TableOperation.Insert(user);
                userTable.Execute(insertOp);
            }
            else
            {
                try
                {
                    existing.Permissions = user.Permissions;
                    var insertOrReplaceOperation = TableOperation.InsertOrReplace(existing);
                    unboundPermissionTable.Execute(insertOrReplaceOperation);
                }
                catch (Exception e)
                {
                    LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }
    }
}
