namespace core.Roles
{
    /// <summary>
    /// Interface for table store operations involving roles
    /// </summary>
    interface IRoleTableStoreUtility
    {
        PermissionSetEntity GetPermissionSetEntity(string nameSpace);
        void SetPermissionSetEntity(PermissionSetEntity permissionSet);
        UserEntity GetUserEntity(string nameSpace, string googleUserName);
        void SetUserEntity(UserEntity user);
    }
}
