using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Ninject.Modules;
using WebFrontend.Handlers;
using core;
using core.Roles;

namespace WebFrontend.App_Start
{
    public class WebFrontendServiceModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ICore>().To<Core>().InSingletonScope();
            this.Bind<IMessagesHandler>().To<MessagesHandler>();
            this.Bind<IPermissionsUtility>().To<PermissionsUtility>();
            this.Bind<IRoleTableStoreUtility>().To<RoleTableStoreUtility>();
        }
    }
}