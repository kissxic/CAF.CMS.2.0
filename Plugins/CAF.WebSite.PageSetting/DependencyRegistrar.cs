using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Reflection;
using Autofac.Core;
using Autofac;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.Infrastructure.Core;
using CAF.WebSite.PageSettings.Services;
using CAF.Infrastructure.Core.Data;
using CAF.WebSite.PageSettings.Data;
using CAF.Infrastructure.Data;
using CAF.WebSite.PageSettings.Domain;

namespace CAF.WebSite.PageSettings
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 1;
            }
        }
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {
            builder.RegisterType<BannerInfoService>().As<IBannerInfoService>().InstancePerRequest();

            //data layer
            //register named context
            builder.Register<IDbContext>(c => new PageSettingObjectContext(DataSettings.Current.DataConnectionString))
                .Named<IDbContext>(PageSettingObjectContext.ALIASKEY)
                .InstancePerRequest();

            builder.Register<PageSettingObjectContext>(c => new PageSettingObjectContext(DataSettings.Current.DataConnectionString))
                .InstancePerRequest();

            //override required repository with our custom context
            builder.RegisterType<EfRepository<BannerInfo>>()
                .As<IRepository<BannerInfo>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(PageSettingObjectContext.ALIASKEY))
                .InstancePerRequest();
        }
    }
}
