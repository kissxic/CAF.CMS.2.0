#region usings
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using WebGrease.Extensions;
using Microsoft.AspNet.SignalR;
using CAF.Infrastructure.SearchModule;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.WebSite.Application.WebUI;
using FluentValidation.Mvc;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Theming;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using CAF.WebSite.Application.WebUI.Mvc.Bundles;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Tasks;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Plugins;
using System.Web.WebPages;
using CAF.WebSite.Application.Searchs;
using CAF.WebSite.Application.Searchs.Owin;

#endregion

//[assembly: OwinStartup(typeof(CAF.WebSite.Mvc.Startup))]
namespace CAF.WebSite.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableJavaScriptProxies = true;

            //hubConfiguration.EnableDetailedErrors = true;
            //hubConfiguration.EnableJavaScriptProxies = true;
            // app.MapSignalR("/signalr", hubConfiguration);


            //app.Map("/layim", map =>
            //{
            //    var imHubConfiguration = new HubConfiguration()
            //    {
            //        EnableJSONP = true
            //    };
            //    map.RunSignalR(imHubConfiguration);
            //});
            // we use our own mobile devices support (".Mobile" is reserved). that's why we disable it.
            var mobileDisplayMode = DisplayModeProvider.Instance.Modes.FirstOrDefault(x => x.DisplayModeId == DisplayModeProvider.MobileDisplayModeId);
            if (mobileDisplayMode != null)
                DisplayModeProvider.Instance.Modes.Remove(mobileDisplayMode);

            // DisplayModeProvider.Instance.Modes.Add(new DefaultDisplayMode("iPhone") { ContextCondition = context => context.Request.UserAgent.Contains("iPhone") });

            bool installed = DataSettings.DatabaseIsInstalled();

            if (installed)
            {
                // remove all view engines
                ViewEngines.Engines.Clear();
            }

            var hangfireOptions = new HangfireOptions
            {
                StartServer = ConfigurationManager.AppSettings.GetValue("caf:Jobs.Enabled", true),
                JobStorageType = ConfigurationManager.AppSettings.GetValue("caf:Jobs.StorageType", "Memory"),
                DatabaseConnectionStringName = DataSettings.Current.DataConnectionString,
            };
            var hangfireLauncher = new HangfireLauncher(hangfireOptions);

            hangfireLauncher.ConfigureOwin(app);

            // initialize engine context
            EngineContext.Initialize(false);

        

            // model binders
            ModelBinders.Binders.DefaultBinder = new DefinedModelBinder();

            // Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new DefinedMetadataProvider();

            // Register MVC areas
            AreaRegistration.RegisterAllAreas();

            // fluent validation
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new ValidatorFactory()));

            // Routes
            RegisterRoutes(RouteTable.Routes, installed);

            // localize MVC resources
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "MvcLocalization";
            DefaultModelBinder.ResourceClassKey = "MvcLocalization";
            ErrorMessageProvider.SetResourceClassKey("MvcLocalization");

            if (installed)
            {
                // register our themeable razor view engine we use
                ViewEngines.Engines.Add(new ThemeableRazorViewEngine());

                // Global filters
                RegisterGlobalFilters(GlobalFilters.Filters);

                // Bundles
                RegisterBundles(BundleTable.Bundles);

                // register virtual path provider for theming (file inheritance & variables handling)
                HostingEnvironment.RegisterVirtualPathProvider(new ThemingVirtualPathProvider(HostingEnvironment.VirtualPathProvider));
                BundleTable.VirtualPathProvider = HostingEnvironment.VirtualPathProvider;

                // register plugin debug view virtual path provider
                if (HttpContext.Current.IsDebuggingEnabled)
                {
                    HostingEnvironment.RegisterVirtualPathProvider(new PluginDebugViewVirtualPathProvider());
                }



                if (!ModelBinders.Binders.ContainsKey(typeof(DataTablesParam)))
                    ModelBinders.Binders.Add(typeof(DataTablesParam), new DataTablesModelBinder());

                // Install filter
                GlobalFilters.Filters.Add(new InitializeSchedulerFilter());

                //ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
                //ValueProviderFactories.Factories.Add(new JsonNetValueProviderFactory());
                app.Use<WorkContextOwinMiddleware>();
            }
            else
            {
                // app not installed

                // Install filter
                GlobalFilters.Filters.Add(new HandleInstallFilter());
            }
           
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
            eventPublisher.Publish(new AppRegisterGlobalFiltersEvent
            {
                Filters = filters
            });
        }
        public static void RegisterRoutes(RouteCollection routes, bool databaseInstalled = true)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute(".db/{*virtualpath}");


            // register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

        }
        public static void RegisterBundles(BundleCollection bundles)
        {
            // register custom bundles
            var bundlePublisher = EngineContext.Current.Resolve<IBundlePublisher>();
            bundlePublisher.RegisterBundles(bundles);
        }
    }

}
