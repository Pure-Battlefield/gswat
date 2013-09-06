namespace core.Roles
{
    /// <summary>
    /// Interface for table store operations involving roles
    /// </summary>
    public interface IRoleTableStoreUtility
    {
        PermissionSetEntity GetPermissionSetEntity(string nameSpace);
        void SetPermissionSetEntity(PermissionSetEntity permissionSet);
        UserEntity GetUserEntity(string nameSpace, string googleIDNumber);
        void SetUserEntity(UserEntity user);
        UnboundPermissionSetEntity GetUnboundPermissionSetEntity(string nameSpace, string email);
        void AddOrUpdateUnboundPermission(UnboundPermissionSetEntity user);
    }
}
