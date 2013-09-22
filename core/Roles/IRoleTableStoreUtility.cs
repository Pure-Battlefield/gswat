using System;
using core.Roles.Models;

namespace core.Roles
{
    /// <summary>
    /// Interface for table store operations involving roles
    /// </summary>
    public interface IRoleTableStoreUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        PermissionSetEntity GetPermissionSetEntity(string nameSpace);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionSet"></param>
        void SetPermissionSetEntity(PermissionSetEntity permissionSet);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="googleId"></param>
        /// <returns></returns>
        UserEntity GetUserEntity(string nameSpace, string googleId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        void SetUserEntity(UserEntity user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        void DeleteUserEntity(UserEntity user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        UnboundPermissionSetEntity GetUnboundPermissionSetEntity(string nameSpace, Guid guid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissions"></param>
        void AddOrUpdateUnboundPermission(UnboundPermissionSetEntity permissions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permissionsToken"></param>
        /// <returns></returns>
        bool ConfirmEmailToken(IValidatableUser user, Guid permissionsToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissions"></param>
        void DeleteUnboundPermission(UnboundPermissionSetEntity permissions);
    }
}
