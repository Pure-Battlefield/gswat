using System;
using System.Reflection;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using core.Logging;

namespace core.Roles
{
    /// <summary>
    /// Handles all table store operations for roles
    /// </summary>
    public class RoleTableStoreUtility : IRoleTableStoreUtility
    {
        public CloudTable PermissionSetTable;
        public CloudTable UserTable;

        public RoleTableStoreUtility()
        {
            try
            {
                var storageAccount =
                    CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
                var tableClient = storageAccount.CreateCloudTableClient();

                PermissionSetTable = tableClient.GetTableReference("permissionSets");
                PermissionSetTable.CreateIfNotExists();

                UserTable = tableClient.GetTableReference("users");
                UserTable.CreateIfNotExists();
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
                var result = PermissionSetTable.Execute(retrieveOp);

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
            var permissionSetEntity = GetPermissionSetEntity(permissionSet.Namespace);
            if (permissionSetEntity == null)
            {
                var insertOp = TableOperation.Insert(permissionSet);
                PermissionSetTable.Execute(insertOp);
            }
            else
            {
                try
                {
                    permissionSetEntity.Namespace = permissionSet.Namespace;
                    permissionSetEntity.PermissionSet = permissionSet.PermissionSet;
                    var insertOrReplaceOperation = TableOperation.InsertOrReplace(permissionSetEntity);
                    PermissionSetTable.Execute(insertOrReplaceOperation);
                }
                catch (Exception e)
                {
                    LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        public UserEntity GetUserEntity(string nameSpace, string googleUserName)
        {
            try
            {
                var retrieveOp = TableOperation.Retrieve<UserEntity>(nameSpace, googleUserName);
                var result = PermissionSetTable.Execute(retrieveOp);

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
            var userEntity = GetUserEntity(user.Permissions.Namespace, user.GoogleUsername);
            if (userEntity == null)
            {
                var insertOp = TableOperation.Insert(user);
                UserTable.Execute(insertOp);
            }
            else
            {
                try
                {
                    userEntity.BattlelogID = user.BattlelogID;
                    userEntity.GoogleUsername = user.GoogleUsername;
                    userEntity.AccountEnabled = user.AccountEnabled;
                    userEntity.Permissions = user.Permissions;
                    var insertOrReplaceOperation = TableOperation.InsertOrReplace(userEntity);
                    UserTable.Execute(insertOrReplaceOperation);
                }
                catch (Exception e)
                {
                    LogUtility.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }
    }
}
