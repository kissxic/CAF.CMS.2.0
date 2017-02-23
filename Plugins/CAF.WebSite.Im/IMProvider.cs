
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Logging;
using System;
using System.ServiceModel;
using System.Web.Routing;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.Services.Cms;
using System.Collections.Generic;

namespace CAF.WebSite.Im
{
    /// <summary>
    /// Represents the Clickatell SMS provider
    /// </summary>
    public class IMProvider : BasePlugin, IConfigurable, IWidget
    {
        private readonly ILogger _logger;
        private readonly IMSettings _clickatellSettings;
        private readonly ILocalizationService _localizationService;

        public IMProvider(IMSettings clickatellSettings,
            ILogger logger,
            ILocalizationService localizationService)
        {
            this._clickatellSettings = clickatellSettings;
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
            controllerName = "IM";
            routeValues = new RouteValueDictionary() { { "area", "CAF.WebSite.Im" } };
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
            _localizationService.DeleteLocaleStringResources("Plugins.FriendlyName.Mobile.IM", false);

            base.Uninstall();
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>
            {
                "Footer_before",
                "Chat_widget"
            };
        }
        public void GetDisplayWidgetRoute(string widgetZone, object model, int storeId, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {

            if (widgetZone == "Chat_widget")
            {
                actionName = "PublicInfo";
                controllerName = "Chat";
                routeValues = new RouteValueDictionary
                    {
                        {"Namespaces","CAF.WebSite.Im.Controllers"},
                        {"area","CAF.WebSite.Im"},
                        {"widgetZone", widgetZone },
                        {"model", model }
                    };
            }
            else
            {
                actionName = "PublicInfo";
                controllerName = "IM";
                routeValues = new RouteValueDictionary
                    {
                        {"Namespaces","CAF.WebSite.Im.Controllers"},
                        {"area","CAF.WebSite.Im"},
                        {"widgetZone", widgetZone },
                        {   "model", model }
                    };
            }
        }

    }
}
