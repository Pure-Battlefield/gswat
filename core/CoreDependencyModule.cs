using Ninject.Modules;
using core.Roles;

namespace core
{
    class CoreDependencyModule : NinjectModule  
    {
        public override void Load()
        {
            this.Bind<IPermissionsUtility>().To<PermissionsUtility>();
        }
    }
}
