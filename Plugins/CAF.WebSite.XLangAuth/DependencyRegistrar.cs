using Autofac;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.WebSite.XLAuth.Core;


namespace CAF.WebSite.XLAuth
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {
             builder.RegisterType<XLProviderAuthorizer>().As<IOAuthProviderXLAuthorizer>().InstancePerRequest();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
