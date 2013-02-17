using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace core.Roles.CoreRoleManager
{
    public class CoreRoleManager
    {
        private IRoleTableStoreUtility RoleTableStoreUtility;

        public CoreRoleManager()
        {
            RoleTableStoreUtility = new RoleTableStoreUtility();
            PermissionSetEntity permissionSet = new PermissionSetEntity();
            //RoleTableStoreUtility.SetPermissionSetEntity();
        }
    }
}