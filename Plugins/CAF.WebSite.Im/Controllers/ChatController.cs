using System;
using System.Web.Mvc;
using CAF.WebSite.Im.Models;
using CAF.WebSite.Im;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Localization;
using System.Web;
using CAF.WebSite.Im.Core;
using CAF.Message.Distributed.Extensions;
using CAF.Message.Distributed.Extensions.Core;
using CAF.Message.Distributed.Extensions.Models;

namespace CAF.WebSite.Im.Controllers
{


    public class ChatController : PluginControllerBase
    {
        private readonly IMSettings _imSettings;
        private readonly ISettingService _settingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;
        private readonly ILayIMCache _layIMCache;
        public ChatController(IMSettings imSettings,
            ISettingService settingService, IPluginFinder pluginFinder,
            ILocalizationService localizationService,
            ILayIMCache layIMCache)
        {
            this._imSettings = imSettings;
            this._settingService = settingService;
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
            this._layIMCache = layIMCache;
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object model)
        {
            return base.View(model);
        }

    }
}