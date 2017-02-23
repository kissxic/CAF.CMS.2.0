
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Logging;
using System;
using System.ServiceModel;
using System.Web.Routing;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;

namespace CAF.WebSite.Sms
{
    /// <summary>
    /// Represents the Clickatell SMS provider
    /// </summary>
    public class SmsProvider : BasePlugin, IConfigurable
    {
        private readonly ILogger _logger;
        private readonly SmsSettings _clickatellSettings;
        private readonly ILocalizationService _localizationService;

        public SmsProvider(SmsSettings clickatellSettings,
            ILogger logger,
            ILocalizationService localizationService)
        {
            this._clickatellSettings = clickatellSettings;
            this._logger = logger;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        public bool SendSms(string text, string phone)
        {
            var content = System.Web.HttpUtility.UrlEncode(text);
            try
            {
                HttpHelper http = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = "http://sms.bamikeji.com:8890/mtPort/mt2?" + "uid={0}&pwd={1}&phonelist={2}&content={3}".FormatCurrent(_clickatellSettings.Username, _clickatellSettings.Password.ToLower().ToMd5(), phone, content),//URL这里都是测试URl   必需项
                    Method = "get",//URL     可选项 默认为Get
                };
                //得到新的HTML代码
                HttpResult result = http.GetHtml(item);
                item = new HttpItem()
                {
                    URL = "http://sms.bamikeji.com:8890/mtPort/mt2",//URL这里都是测试URl   必需项
                    Encoding = null,//编码格式（utf-8,gb2312,gbk）     可选项 默认类会自动识别
                                    //Encoding = Encoding.Default,
                    Method = "get",//URL     可选项 默认为Get
                    Postdata = "uid={0}&pwd={1}&phonelist={2}&content={3}".FormatCurrent(_clickatellSettings.Username, _clickatellSettings.Password.ToLower().ToMd5(), phone, content)
                };
                //得到新的HTML代码
                result = http.GetHtml(item);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return false;
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
            controllerName = "Sms";
            routeValues = new RouteValueDictionary() { { "area", "CAF.WebSite.Sms" } };
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
            _localizationService.DeleteLocaleStringResources("Plugins.FriendlyName.Mobile.SMS", false);

            base.Uninstall();
        }
    }
}
