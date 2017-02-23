using Autofac;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.WebSite.WeiXinAuth.Core;


namespace CAF.WebSite.WeiXinAuth
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {
             builder.RegisterType<WXProviderAuthorizer>().As<IOAuthProviderWXAuthorizer>().InstancePerRequest();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
