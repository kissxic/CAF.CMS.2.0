using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services;
using System;


namespace CAF.WebSite.AliPayment
{

    public partial class Plugin : BasePlugin
    {
        private readonly ICommonServices _services;

        public Plugin(ICommonServices services)
        {
            this._services = services;
        }

        public override void Install()
        {
            var settings = _services.Settings;
            var loc = _services.Localization;


            // add resources
            loc.ImportPluginResourcesFromXml(this.PluginDescriptor);

            base.Install();
        }

        public override void Uninstall()
        {
            var settings = _services.Settings;
            var loc = _services.Localization;

            loc.DeleteLocaleStringResources(this.PluginDescriptor.ResourceRootKey);


            base.Uninstall();
        }
    }
}