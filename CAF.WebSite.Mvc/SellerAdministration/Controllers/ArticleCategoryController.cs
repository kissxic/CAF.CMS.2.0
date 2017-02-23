using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Authentication;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.WebSite.Application.WebUI.MvcCaptcha;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Seller.Models.Users;
using CAF.WebSite.Application.Services.Media;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Mvc.Seller.Models.Articles;
using CAF.Mvc.JQuery.Datatables.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Channels;
using CAF.Infrastructure.Core.Domain;

namespace CAF.WebSite.Mvc.Seller.Controllers
{
    public class ArticleCategoryController : SellerAdminControllerBase
    {
        #region Fields
        private readonly IArticleCategoryService _categoryService;
        private readonly IModelTemplateService _modelTemplateService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly IUserActivityService _userActivityService;
        private readonly IAclService _aclService;
        private readonly ISiteService _siteService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;
        private readonly IChannelService _channelService;
        private readonly SiteInformationSettings _siteSettings;
        #endregion

        #region Ctor

        public ArticleCategoryController(
            IModelTemplateService modelTemplateService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IUserService userService,
            UserSettings userSettings,
            IArticleCategoryService categoryService,
            IUrlRecordService urlRecordService,
            ILanguageService languageService,
            IPictureService pictureService,
            IAclService aclService,
            IUserActivityService userActivityService,
            ISiteService siteService, ISiteMappingService siteMappingService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            ArticleCatalogSettings catalogSettings,
            IPermissionService permissionService,
            IChannelService channelService,
            IWorkContext workContext,
            ISiteContext siteContext,
            SiteInformationSettings siteSettings)
        {
            this._modelTemplateService = modelTemplateService;
            this._categoryService = categoryService;
            this._localizedEntityService = localizedEntityService;
            this._urlRecordService = urlRecordService;
            this._localizationService = localizationService;
            this._userService = userService;
            this._userSettings = userSettings;
            this._userActivityService = userActivityService;
            this._aclService = aclService;
            this._languageService = languageService;
            this._pictureService = pictureService;
            this._siteService = siteService;
            this._siteMappingService = siteMappingService;
            this._dateTimeHelper = dateTimeHelper;
            this._eventPublisher = eventPublisher;
            this._workContext = workContext;
            this._catalogSettings = catalogSettings;
            this._permissionService = permissionService;
            this._channelService = channelService;
            this._siteContext = siteContext;
            this._siteSettings = siteSettings;
        }
        #endregion


       
    }
}