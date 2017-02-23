using Autofac;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.Message.Distributed.Extensions;
using CAF.Message.Distributed.Extensions.Core;
using CAF.WebSite.Im.Core;
using CAF.WebSite.Im.RabbitMQ;

namespace CAF.WebSite.Im
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {

            builder.Register<IBaseMessage>(c => new RabbitMQMessage())
                .PropertiesAutowired(PropertyWiringOptions.None)
                .InstancePerRequest();
            builder.RegisterType<LayIMCache>().As<ILayIMCache>().InstancePerRequest();
 
            builder.RegisterType<LayIMHub>()
                    .AsSelf()
                    .As<ILayIMHub>()
                    .ExternallyOwned()
                    .SingleInstance();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
