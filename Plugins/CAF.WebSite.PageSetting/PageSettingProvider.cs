
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Logging;
using System;
using System.ServiceModel;
using System.Web.Routing;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;
using System.Data.Entity.Migrations;
using CAF.WebSite.PageSettings.Data.Migrations;
using CAF.WebSite.Application.Services.Cms;
using System.Collections.Generic;

namespace CAF.WebSite.PageSettings
{
    /// <summary>
    /// Represents the Clickatell SMS provider
    /// </summary>
    public class PageSettingProvider : BasePlugin, IConfigurable, IWidget, IProvider
    {
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;

        public PageSettingProvider(
            ILogger logger,
            ILocalizationService localizationService)
        {
            this._logger = logger;
            _localizationService = localizationService;
        }


        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PageSetting";
            routeValues = new RouteValueDictionary() { { "area", "CAF.WebSite.PageSettings" } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //locales
            _localizationService.ImportPluginResourcesFromXml(this.PluginDescriptor);

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //locales
            _localizationService.DeleteLocaleStringResources(this.PluginDescriptor.ResourceRootKey);
            //var migrator = new DbMigrator(new Configuration());
            //migrator.Update(DbMigrator.InitialDatabase);

            base.Uninstall();
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>
            {
                "home_content_before"
            };
        }
        public void GetDisplayWidgetRoute(string widgetZone, object model, int storeId, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "PageSetting";
            routeValues = new RouteValueDictionary
            {

                {
                    "Namespaces",
                    "CAF.WebSite.PageSettings.Controllers"
                },

                {
                    "area",
                    "CAF.WebSite.PageSettings"
                },

                {
                    "widgetZone",
                    widgetZone
                },

                {
                    "model",
                    model
                }
            };
        }
      
    }
}
