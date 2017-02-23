using CAF.WebSite.Application.WebUI.Themes;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Collections;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.WebSite.Application.WebUI.UI;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.WebSite.Application.Services.Forums;
using CAF.Infrastructure.Core.Themes;
using CAF.WebSite.Application.Services.Topics;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System.Drawing;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.Services.Security;
using System.Text;
using CAF.Infrastructure.Core.Domain.Seo;
using System.Globalization;
using CAF.WebSite.Application.Services.Links;
using CAF.WebSite.Mvc.Models.Media;
using CAF.WebSite.Application.Services.RegionalContents;
using CAF.WebSite.Application.WebUI.Theming;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Manufacturers;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Vendors;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Application.Services.PageSettings;
using CAF.WebSite.Mvc.Models.HomeFloors;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Utilities;

namespace CAF.WebSite.Mvc.Controllers
{
    public class CommonController : PublicControllerBase
    {
        #region Fields
       
        private readonly ITopicService _topicService;
        private readonly Lazy<ILanguageService> _languageService;
        private readonly Lazy<IVisitRecordService> _visitRecordService;
        private readonly Lazy<IThemeRegistry> _themeRegistry;
        private readonly Lazy<IForumService> _forumservice;
        private readonly Lazy<IGenericAttributeService> _genericAttributeService;
        private readonly Lazy<IMobileDeviceHelper> _mobileDeviceHelper;
        private readonly Lazy<ILinkService> _linkService;
        private readonly Lazy<IRegionalContentService> _regionalContentService;
        private readonly static string[] s_hints = new string[] { "CafCms" };
        private readonly IPageAssetsBuilder _pageAssetsBuilder;
        private readonly Lazy<IPictureService> _pictureService;
        private readonly ICommonServices _services;
        private readonly Lazy<IManufacturerService> _manufacturerService;
        private readonly Lazy<IArticleCategoryService> _categoryService;
        private readonly Lazy<IProductCategoryService> _prioductcategoryService;
        private readonly Lazy<IArticleService> _productService;
        private readonly Lazy<IVendorService> _vendorService;
        private readonly Lazy<IChannelService> _channelService;
        private readonly Lazy<IHomeCategoryService> _homeCategoryService;
        private readonly Lazy<IBannerInfoService> _bannerInfoService;

        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly UserSettings _userSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CommonSettings _commonSettings;
        private readonly ThemeSettings _themeSettings;
        private readonly ForumSettings _forumSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly Lazy<SecuritySettings> _securitySettings;
        private readonly Lazy<MediaSettings> _mediaSettings;
        private readonly static object _lock = new object();	// codehint: sm-add
        #endregion

        #region Constructors

        public CommonController(
            ITopicService topicService,
            Lazy<ILanguageService> languageService,
            Lazy<IVisitRecordService> visitRecordService,
            Lazy<IThemeRegistry> themeRegistry,
            Lazy<IForumService> forumService,
            Lazy<IGenericAttributeService> genericAttributeService,
            Lazy<IMobileDeviceHelper> mobileDeviceHelper,
            Lazy<ILinkService> linkService,
            Lazy<IRegionalContentService> regionalContentService,
            Lazy<IPictureService> pictureService,
            Lazy<MediaSettings> mediaSettings,
            Lazy<SecuritySettings> securitySettings,
            Lazy<IManufacturerService> manufacturerService,
            Lazy<IArticleCategoryService> categoryService,
            Lazy<IProductCategoryService> prioductcategoryService,
            Lazy<IArticleService> productService,
            Lazy<IVendorService> vendorService,
            Lazy<IChannelService> channelService,
            Lazy<IHomeCategoryService> homeCategoryService,
            Lazy<IBannerInfoService> bannerInfoService,

            LocalizationSettings localizationSettings,
            ThemeSettings themeSettings,
            IPageAssetsBuilder pageAssetsBuilder,
            ICommonServices services,
            IArticleCategoryService articlecategoryService,
            ArticleCatalogSettings catalogSettings,
            UserSettings userSettings,
            TaxSettings taxSettings,
            EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings,
            ForumSettings forumSettings)
        {
            this._topicService = topicService;
            this._languageService = languageService;
            this._visitRecordService = visitRecordService;
            this._themeRegistry = themeRegistry;
            this._forumservice = forumService;
            this._genericAttributeService = genericAttributeService;
            this._mobileDeviceHelper = mobileDeviceHelper;
            this._linkService = linkService;
            this._regionalContentService = regionalContentService;
            this._manufacturerService = manufacturerService;
            this._categoryService = categoryService;
            this._prioductcategoryService = prioductcategoryService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._channelService = channelService;
            this._homeCategoryService = homeCategoryService;
            this._bannerInfoService = bannerInfoService;

            this._catalogSettings = catalogSettings;
            this._userSettings = userSettings;
            this._taxSettings = taxSettings;
            this._commonSettings = commonSettings;
            this._forumSettings = forumSettings;
            this._localizationSettings = localizationSettings;
            this._securitySettings = securitySettings;
            this._themeSettings = themeSettings;
            this._pageAssetsBuilder = pageAssetsBuilder;
            this._pictureService = pictureService;
            this._services = services;
            this._mediaSettings = mediaSettings;
            T = NullLocalizer.Instance;
        }

        #endregion

        #region Utilities
        [NonAction]
        protected LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _services.Cache.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, _services.SiteContext.CurrentSite.Id), () =>
            {
                var result = _languageService.Value
                    .GetAllLanguages(siteId: _services.SiteContext.CurrentSite.Id)
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NativeName = LocalizationHelper.GetLanguageNativeName(x.LanguageCulture) ?? x.Name,
                        ISOCode = x.LanguageCulture,
                        SeoCode = x.UniqueSeoCode,
                        FlagImageFileName = x.FlagImageFileName
                    })
                    .ToList();
                return result;
            });

            var workingLanguage = _services.WorkContext.WorkingLanguage;

            var model = new LanguageSelectorModel()
            {
                CurrentLanguageId = workingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            string defaultSeoCode = _languageService.Value.GetDefaultLanguageSeoCode();

            foreach (var lang in model.AvailableLanguages)
            {
                var helper = new LocalizedUrlHelper(HttpContext.Request, true);

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    if (lang.SeoCode == defaultSeoCode && (int)(_localizationSettings.DefaultLanguageRedirectBehaviour) > 0)
                    {
                        helper.StripSeoCode();
                    }
                    else
                    {
                        helper.PrependSeoCode(lang.SeoCode, true);
                    }
                }

                model.ReturnUrls[lang.SeoCode] = helper.GetAbsolutePath();
            }

            return model;
        }
        // TODO: (MC) zentral auslagern
        private string GetLanguageNativeName(string locale)
        {
            try
            {
                if (!string.IsNullOrEmpty(locale))
                {
                    var info = CultureInfo.GetCultureInfoByIetfLanguageTag(locale);
                    if (info == null)
                    {
                        return null;
                    }
                    return info.NativeName;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 未读的私人信息
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected int GetUnreadPrivateMessages()
        {
            var result = 0;
            var user = _services.WorkContext.CurrentUser;
            if (_forumSettings.AllowPrivateMessages && !user.IsGuest())
            {
                var privateMessages = _forumservice.Value.GetAllPrivateMessages(_services.SiteContext.CurrentSite.Id, 0, user.Id, false, null, false, string.Empty, 0, 1);

                if (privateMessages.TotalCount > 0)
                {
                    result = privateMessages.TotalCount;
                }
            }

            return result;
        }
        protected string GetVisitReferrerType(string url)
        {
            url = url.Trim();
            if ("" == url)
            {
                return "0"; //没有来源
            }
            else if (url.IndexOf("fromsource=") > -1)
            {
                return "1"; //推广链接
            }
            else if (url.IndexOf("baidu.com") > -1)
            {
                return "2"; // 百度搜索引擎
            }
            else if (url.IndexOf("google.com") > -1)
            {
                return "3"; // Google搜索引擎
            }
            else if (url.IndexOf("sogou.com") > -1)
            {
                return "4"; // 搜狗搜索引擎
            }
            else if (url.IndexOf("soso.com") > -1)
            {
                return "5"; // 搜搜搜索引擎
            }
            else
            {
                return "6"; // 其他浏览
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// 头部信息部分页面
        /// <remarks>包括Logo图片 图片大小</remarks>
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Header()
        {
            var model = _services.Cache.Get(ModelCacheEventConsumer.SITEHEADER_MODEL_KEY.FormatWith(_services.SiteContext.CurrentSite.Id), () =>
            {
                var pictureService = _pictureService.Value;
                int logoPictureId = _services.SiteContext.CurrentSite.LogoPictureId;

                Picture picture = null;
                if (logoPictureId > 0)
                {
                    picture = pictureService.GetPictureById(logoPictureId);
                }

                string logoUrl = null;
                var logoSize = new Size();
                if (picture != null)
                {
                    logoUrl = pictureService.GetPictureUrl(picture);
                    logoSize = pictureService.GetPictureSize(picture);
                }

                return new SiteHeaderModel()
                {
                    LogoUploaded = picture != null,
                    LogoUrl = logoUrl,
                    LogoWidth = logoSize.Width,
                    LogoHeight = logoSize.Height,
                    LogoTitle = _services.SiteContext.CurrentSite.Name
                };
            });


            return PartialView(model);
        }

        /// <summary>
        /// 首页导航
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Megamenu()
        {
            //首页分类导航
            var model = _services.Cache.Get(ModelCacheEventConsumer.CATEGORY_HOMENAVIGATION_PATTERN_KEY, () =>
            {

                var homeCategorys = _homeCategoryService.Value.GetAllHomeCategorys();
                var homeCategorySets = new List<HomeCategorySet>();
                int rowNumber = 1;
                foreach (var item in homeCategorys)
                {
                    var homeCategorySet = new HomeCategorySet()
                    {
                        RowNumber = rowNumber,
                        Name = item.Name,
                        Url = item.Url
                    };
                    var homeCategoryItems = item.HomeCategoryItems.ToList();
                    foreach (var homeItem in homeCategoryItems)
                    {
                   
                        var homeCategoryInfoModel = new HomeCategoryInfoModel()
                        {
                            Depth = homeItem.Depth,
                            CategoryId = homeItem.CategoryId,
                            Name = homeItem.Name,
                            ParentId = homeItem.ParentCategoryId,
                            SeName = homeItem.SeName,
                         
                        };
                        homeCategorySet.HomeCategories.Add(homeCategoryInfoModel);
                    }
                    rowNumber++;

                    //推荐商家
                    var vendorIds = new List<int>();
                    if (!item.RequiredVendorIds.IsEmpty())
                    {
                        item.RequiredVendorIds.Split(',').Each(x =>
                         {
                             vendorIds.Add(x.ToInt());
                         });
                    }
                    var vendors = _vendorService.Value.GetVendorById(vendorIds.ToArray());
                    foreach (var vendor in vendors)
                    {
                        var vendorModel = new HomeCategorySet.HomeCategoryNew()
                        {
                            Name = vendor.Name,
                            Id = vendor.Id,
                            Url = Url.RouteUrl("Vendor", new { SystemName = vendor.GetSeName() }),
                        };
                        var picture = vendor.Picture;
                        if (picture != null)
                        {
                            vendorModel.ImageUrl = _pictureService.Value.GetPictureUrl(picture, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage,
                                !_catalogSettings.HideArticleDefaultPictures);
                        }
                        homeCategorySet.Vendor.Add(vendorModel);
                    }

                    //推荐品牌
                    var brandIds = new List<int>();
                    if (!item.RequiredManufacturerIds.IsEmpty())
                    {
                        item.RequiredManufacturerIds.Split(',').Each(x =>
                    {
                        brandIds.Add(x.ToInt());
                    });
                    }
                    var manufacturers = _manufacturerService.Value.GetAllManufacturersById(brandIds.ToArray());
                    foreach (var manufacturer in manufacturers)
                    {
                        var manufacturerModel = new HomeCategorySet.HomeCategoryNew()
                        {
                            Name = manufacturer.Name,
                            Id = manufacturer.Id,
                        };
                        var picture = manufacturer.Picture;
                        if (picture != null)
                        {
                            manufacturerModel.ImageUrl = _pictureService.Value.GetPictureUrl(picture, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage,
                                !_catalogSettings.HideArticleDefaultPictures);
                        }
                        homeCategorySet.Brands.Add(manufacturerModel);
                    }
                    homeCategorySets.Add(homeCategorySet);

                }




                return homeCategorySets;
            });
            //菜单导航
            var navigratorsModel = _services.Cache.Get(ModelCacheEventConsumer.CATEGORY_HOMETOPNAVIGATION_PATTERN_KEY, () =>
            {
                var banners = _bannerInfoService.Value.GetAllBannerInfos();
                return banners;
            });
            ViewBag.Categories = model;
            ViewBag.Navigators = navigratorsModel;
            return PartialView(model);
        }


        /// <summary>
        /// 多语言选择部分页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = PrepareLanguageSelectorModel();

            if (model.AvailableLanguages.Count < 2)
                return Content("");

            // register all available languages as <link hreflang="..." ... />
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var host = _services.WebHelper.GetSiteLocation();
                foreach (var lang in model.AvailableLanguages)
                {
                    _pageAssetsBuilder.AddLinkPart("alternate", host + model.ReturnUrls[lang.SeoCode].TrimStart('/'), hreflang: lang.SeoCode);
                }
            }

            return PartialView(model);
        }
        /// <summary>
        /// 选择语言事件
        /// </summary>
        /// <param name="langid"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.Value.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _services.WorkContext.WorkingLanguage = language;
            }

            // url referrer
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = _services.WebHelper.GetUrlReferrer();
            }

            // home page
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.RouteUrl("HomePage");
            }

            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var helper = new LocalizedUrlHelper(HttpContext.Request.ApplicationPath, returnUrl, true);
                helper.PrependSeoCode(_services.WorkContext.WorkingLanguage.UniqueSeoCode, true);
                returnUrl = helper.GetAbsolutePath();
            }

            return Redirect(returnUrl);
        }
        /// <summary>
        /// 网站Barnner部分也面
        /// <remarks></remarks>
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult SiteBar()
        {
            var user = _services.WorkContext.CurrentUser;
            var pictureService = _pictureService.Value;
            var unreadMessageCount = GetUnreadPrivateMessages();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = T("PrivateMessages.TotalUnread");

                //notifications here
                if (_forumSettings.ShowAlertForPM &&
                    !user.GetAttribute<bool>(SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, _services.SiteContext.CurrentSite.Id))
                {
                    _genericAttributeService.Value.SaveAttribute(user, SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, true, _services.SiteContext.CurrentSite.Id);
                    alertMessage = T("PrivateMessages.YouHaveUnreadPM", unreadMessageCount);
                }
            }

            var model = new SiteBarModel
            {
                IsAuthenticated = user.IsRegistered(),
                UserEmailUserName = user.IsRegistered() ? (_userSettings.UserNamesEnabled ? user.UserName : user.Email) : "",
                IsUserImpersonated = _services.WorkContext.OriginalUserIfImpersonated != null,
                DisplayAdminLink = _services.Permissions.Authorize(StandardPermissionProvider.AccessAdminPanel),
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
                UserAvatar = pictureService.GetPictureUrl(
               user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
               _mediaSettings.Value.AvatarPictureSize, _mediaSettings.Value.AvatarPictureSize, false)

            };

            return PartialView(model);
        }
        /// <summary>
        /// 会员中心下拉部分也面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult AccountDropdown()
        {
            var user = _services.WorkContext.CurrentUser;

            var unreadMessageCount = GetUnreadPrivateMessages();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = unreadMessageCount.ToString();

                //notifications here
                if (_forumSettings.ShowAlertForPM &&
                    !user.GetAttribute<bool>(SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, _services.SiteContext.CurrentSite.Id))
                {
                    _genericAttributeService.Value.SaveAttribute(user, SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, true, _services.SiteContext.CurrentSite.Id);
                    alertMessage = T("PrivateMessages.YouHaveUnreadPM", unreadMessageCount);
                }
            }

            var model = new AccountDropdownModel
            {
                IsAuthenticated = user.IsRegistered(),
                DisplayAdminLink = _services.Permissions.Authorize(StandardPermissionProvider.AccessAdminPanel),
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage
            };

            return PartialView(model);
        }
        /// <summary>
        /// 信息块
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult InfoBlock()
        {
            var model = new InfoBlockModel
            {
                SitemapEnabled = _commonSettings.SitemapEnabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
            };

            return PartialView(model);
        }
        /// <summary>
        /// 友情链接
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult LinkBlock()
        {
            var links = _linkService.Value.GetAllLinks(_services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id, 0, 20);
            string cacheKey = ModelCacheEventConsumer.LINK_MODEL_KEY.FormatInvariant(_services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id);

            var result = _services.Cache.Get(cacheKey, () =>
            {
                var model = new LinkModel();

                foreach (var item in links)
                {
                    var linkBlck = new LinkBlockModel();
                    linkBlck.Name = item.Name;
                    linkBlck.Intro = item.Intro;
                    linkBlck.IsHome = item.IsHome;
                    linkBlck.SortId = item.SortId;
                    linkBlck.LogoUrl = item.LogoUrl;
                    linkBlck.LinkUrl = item.LinkUrl;
                    if (item.PictureId.HasValue)
                    {
                        linkBlck.DefaultPictureModel = new PictureModel()
                        {
                            PictureId = item.PictureId.GetValueOrDefault(),
                            FullSizeImageUrl = _pictureService.Value.GetPictureUrl(item.PictureId.GetValueOrDefault()),
                            ImageUrl = _pictureService.Value.GetPictureUrl(item.PictureId.GetValueOrDefault(), _mediaSettings.Value.DetailsPictureSize),
                        };
                    }
                    model.LinkBlocks.Add(linkBlck);
                }
                return model;
            });

            return PartialView(result);

        }
        /// <summary>
        /// 图标图标
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByCustom = "Theme_Site")]
        public ActionResult Favicon()
        {
            var icons = new string[]
            {
                "favicon-{0}.ico".FormatInvariant(_services.SiteContext.CurrentSite.Id),
                "favicon.ico"
            };

            string virtualPath = null;

            foreach (var icon in icons)
            {
                virtualPath = Url.ThemeAwareContent(icon);
                if (virtualPath.HasValue())
                {
                    break;
                }
            }

            if (virtualPath.IsEmpty())
            {
                return Content("");
            }

            var model = new FaviconModel()
            {
                Uploaded = true,
                FaviconUrl = virtualPath
            };

            return PartialView(model);
        }
        /// <summary>
        /// 底部部分页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Footer()
        {
            string taxInfo = (_services.WorkContext.GetTaxDisplayTypeFor(_services.WorkContext.CurrentUser, _services.SiteContext.CurrentSite.Id) == TaxDisplayType.IncludingTax)
                ? T("Tax.InclVAT")
                : T("Tax.ExclVAT");

            string shippingInfoLink = Url.RouteUrl("Topic", new { SystemName = "shippinginfo" });
            var site = _services.SiteContext.CurrentSite;

            var availableSiteThemes = !_themeSettings.AllowUserToSelectTheme ? new List<SiteThemeModel>() : _themeRegistry.Value.GetThemeManifests()
                .Where(x => !x.MobileTheme)
                .Select(x =>
                {
                    return new SiteThemeModel()
                    {
                        Name = x.ThemeName,
                        Title = x.ThemeTitle
                    };
                })
                .ToList();

            var model = new FooterModel
            {
                SiteName = site.Name,
                LegalInfo = T("Tax.LegalInfoFooter", taxInfo, shippingInfoLink),
                ShowLegalInfo = _taxSettings.ShowLegalHintsInFooter,
                ShowThemeSelector = availableSiteThemes.Count > 1,
                ForumEnabled = _forumSettings.ForumsEnabled,
                HideNewsletterBlock = _userSettings.HideNewsletterBlock,
            };

            var hint = _services.Settings.GetSettingByKey<string>("Rnd_CafCopyrightHint", string.Empty, site.Id);
            if (hint.IsEmpty())
            {
                hint = s_hints[new Random().Next(s_hints.Length)];
                _services.Settings.SetSetting<string>("Rnd_CafCopyrightHint", hint, site.Id);
            }

            var topics = new string[] { "paymentinfo", "imprint", "disclaimer" };
            foreach (var t in topics)
            {
                //load by site
                var topic = _topicService.GetTopicBySystemName(t, site.Id);
                if (topic == null)
                    //not found. let's find topic assigned to all sites
                    topic = _topicService.GetTopicBySystemName(t, 0);

                if (topic != null)
                {
                    model.Topics.Add(t, topic.Title);
                }
            }

            var socialSettings = EngineContext.Current.Resolve<SocialSettings>();

            model.ShowSocialLinks = socialSettings.ShowSocialLinksInFooter;
            model.FacebookLink = socialSettings.FacebookLink;
            model.GooglePlusLink = socialSettings.GooglePlusLink;
            model.TwitterLink = socialSettings.TwitterLink;
            model.PinterestLink = socialSettings.PinterestLink;
            model.YoutubeLink = socialSettings.YoutubeLink;
            model.CafSiteHint = "<a href='http://www.fengkuangmayi.net/' class='sm-hint' target='_blank'><strong>{0}</strong></a> by CafSite AG &copy; {1}".FormatCurrent(hint, DateTime.Now.Year);

            return PartialView(model);
        }
        /// <summary>
        /// (桌面或移动版)部分页面
        /// </summary>
        /// <param name="dontUseMobileVersion">True - 使用桌面版; false - 使用版本为移动设备</param>
        /// <returns>Action result</returns>
        public ActionResult ChangeDevice(bool dontUseMobileVersion)
        {
            _genericAttributeService.Value.SaveAttribute(_services.WorkContext.CurrentUser,
                SystemUserAttributeNames.DontUseMobileVersion, dontUseMobileVersion, _services.SiteContext.CurrentSite.Id);

            string returnurl = _services.WebHelper.GetUrlReferrer();
            if (String.IsNullOrEmpty(returnurl))
                returnurl = Url.RouteUrl("HomePage");
            return Redirect(returnurl);
        }
        /// <summary>
        /// 页面脚本块
        /// </summary>
        /// <param name="systemName"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RegionalContentBlock(string systemName)
        {
            string cacheKey = ModelCacheEventConsumer.REGIONALCONTENT_MODEL_KEY.FormatInvariant(systemName, _services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id);

            var result = _services.Cache.Get(cacheKey, () =>
            {
                var regionalContent = _regionalContentService.Value.GetRegionalContentBySystemName(systemName, _services.SiteContext.CurrentSite.Id, _services.WorkContext.WorkingLanguage.Id);
                return regionalContent;
            });

            if (result == null)
                return Content("");

            return Content(result.Body);
        }

        /// <summary>
        /// (桌面或移动版)切换事件
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ChangeDeviceBlock()
        {
            if (!_mobileDeviceHelper.Value.MobileDevicesSupported())
                //mobile devices support is disabled
                return Content("");

            if (!_mobileDeviceHelper.Value.IsMobileDevice())
                //request is made by a desktop computer
                return Content("");

            return View();
        }

        public ActionResult RobotsTextFile()
        {
            var disallowPaths = new List<string>()
            {
                "/bin/",
                "/Content/files/",
                "/Content/files/ExportImport/",
                "/Exchange/",
                "/Country/GetStatesByCountryId",
                "/Install",
                "/Article/SetReviewHelpfulness",
            };
            var localizableDisallowPaths = new List<string>()
            {

                "/Member/Avatar",
                "/Member/Activation",
                "/Member/Addresses",
                "/Member/BackInStockSubscriptions",
                "/Member/ChangePassword",
                "/Member/CheckUsernameAvailability",
                "/Member/DownloadableArticles",
                "/Member/ForumSubscriptions",
                "/Member/DeleteForumSubscriptions",
                "/Member/Info",
                "/Member/Orders",
                "/Member/ReturnRequests",
                "/Member/RewardPoints",
                "/PrivateMessages",
                "/PasswordRecovery",
                "/Poll/Vote",
                "/Topic/Authenticate",
                "/Article/AskQuestion",
                "/Article/EmailAFriend",
                "/Search",
                "/Config",
                "/Settings"
            };


            const string newLine = "\r\n"; //Environment.NewLine
            var sb = new StringBuilder();
            sb.Append("User-agent: *");
            sb.Append(newLine);
            sb.AppendFormat("Sitemap: {0}", Url.RouteUrl("SitemapSEO", (object)null, _securitySettings.Value.ForceSslForAllPages ? "https" : "http"));
            sb.AppendLine();

            var disallows = disallowPaths.Concat(localizableDisallowPaths);

            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                // URLs are localizable. Append SEO code
                foreach (var language in _languageService.Value.GetAllLanguages(siteId: _services.SiteContext.CurrentSite.Id))
                {
                    disallows = disallows.Concat(localizableDisallowPaths.Select(x => "/{0}{1}".FormatInvariant(language.UniqueSeoCode, x)));
                }
            }

            var seoSettings = EngineContext.Current.Resolve<SeoSettings>();

            // append extra disallows
            disallows = disallows.Concat(seoSettings.ExtraRobotsDisallows.Select(x => x.Trim()));

            // Append all lowercase variants (at least Google is case sensitive)
            disallows = disallows.Concat(GetLowerCaseVariants(disallows));

            foreach (var disallow in disallows)
            {
                sb.AppendFormat("Disallow: {0}", disallow);
                sb.Append(newLine);
            }

            Response.ContentType = "text/plain";
            Response.Write(sb.ToString());
            return null;
        }

        private IEnumerable<string> GetLowerCaseVariants(IEnumerable<string> disallows)
        {
            var other = new List<string>();
            foreach (var item in disallows)
            {
                var lower = item.ToLower();
                if (lower != item)
                {
                    other.Add(lower);
                }
            }

            return other;
        }

        /// <summary>
        /// JavaScript脚本开启警告
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult JavaScriptDisabledWarning()
        {
            if (!_commonSettings.DisplayJavaScriptDisabledWarning)
                return Content("");

            return PartialView();
        }
        /// <summary>
        /// 错误页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GenericUrl()
        {
            // seems that no entity was found
            return HttpNotFound();
        }
        /// <summary>
        /// 网站访问统计
        /// </summary>
        /// <returns></returns>
        public ActionResult WebVisit()
        {
            VisitRecord visitModel = new VisitRecord();
            visitModel.VisitReffer = Request.QueryString["oldlink"].ToString();
            visitModel.VisitRefferType = int.Parse(GetVisitReferrerType(Request.QueryString["oldlink"].ToString()));
            visitModel.VisitResolution = Request.QueryString["s"].ToString();
            visitModel.VisitURL = Request.QueryString["id"].ToString();
            visitModel.VisitTimeIn = System.DateTime.Now;
            visitModel.VisitIP = _services.WebHelper.GetCurrentIpAddress();
            visitModel.VisitOS = Request.QueryString["sys"].ToString();
            visitModel.VisitTitle = Request.QueryString["title"].ToString();
            visitModel.VisitBrowerType = Request.QueryString["b"].ToString();
            visitModel.VisitRefferKeyWork = "";
            visitModel.VisitProvince = Request.QueryString["p"].ToString();
            visitModel.VisitCity = Request.QueryString["c"].ToString();


            // 获取source后面的内容]
            string url = Request.QueryString["id"].ToString();
            if (url.IndexOf("fromsource=") > 0)
            {
                int startindex = url.IndexOf("fromsource=");
                int endindex = url.Length - startindex - 11;
                string id = url.Substring(startindex + 11, endindex);
                visitModel.FromSource = id;
            }
            // 这里执行添加到数据库的操作并返回添加以后ID信息
            this._visitRecordService.Value.InsertVisitRecord(visitModel);
            int newID = visitModel.Id;

            return Json(newID.ToString(), JsonRequestBehavior.AllowGet);

        }

        #region 省市区

        #endregion


        #region 普通验证码


        /// <summary>
        /// 验证码验证
        /// </summary>
        /// <param name="checkCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckCode(string checkCode)
        {
            JsonResult jsonResult;
            try
            {
                string item = base.Session[CacheKeyCollection.SessionVerifyCode("Agent")] as string;
                bool lower = item == checkCode.ToLower().ToMd5();
                jsonResult = Json(new { Result = lower });
            }
            catch (WorkException tpmallException)
            {
                jsonResult = Json(new { Result = false, Message = tpmallException.Message });
            }
            return jsonResult;
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCheckCode()
        {
            string captchaCode = "";
            //验证码一
            var memoryStream = VerifyCode.GetVerifyCode(out captchaCode);
            base.Session[CacheKeyCollection.SessionVerifyCode("Agent")] = captchaCode.ToLower().ToMd5();

            return base.File(memoryStream, @"image/Gif");
        }
        #endregion


        #region Entity Picker

        public ActionResult EntityPicker(EntityPickerModel model)
        {
            model.PageSize = _commonSettings.EntityPickerPageSize;
            model.AllString = T("Admin.Common.All");

            if (model.Entity.ToLower().Contains("article"))
            {
                var allCategories = _categoryService.Value.GetAllCategories(showHidden: true);
                var mappedCategories = allCategories.ToDictionary(x => x.Id);

                model.AvailableCategories = allCategories
                    .Select(x => new SelectListItem { Text = x.GetCategoryNameWithPrefix(_categoryService.Value, mappedCategories), Value = x.Id.ToString() })
                    .ToList();

                model.AvailableManufacturers = _manufacturerService.Value.GetAllManufacturers(true)
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                    .ToList();

                model.AvailableSites = _services.SiteService.GetAllSites()
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                    .ToList();

                model.AvailableChannels = _channelService.Value.GetAllChannels()
                .Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() })
                .ToList();


                model.AvailableArticleTypes = ArticleType.Simple.ToSelectList(false).ToList();
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult EntityPicker(EntityPickerModel model, FormCollection form)
        {
            model.PageSize = _commonSettings.EntityPickerPageSize;
            model.PublishedString = T("Common.Published");
            model.UnpublishedString = T("Common.Unpublished");

            var disableIf = model.DisableIf.SplitSafe(",").Select(x => x.ToLower().Trim()).ToList();

            try
            {
                using (var scope = new DbContextScope(_services.DbContext, autoDetectChanges: false, proxyCreation: true, validateOnSave: false, forceNoTracking: true))
                {
                    if (model.Entity.ToLower().Contains("article"))
                    {
                        #region Article

                        model.SearchTerm = model.ArticleName.TrimSafe();

                        var hasPermission = _services.Permissions.Authorize(StandardPermissionProvider.ManageCatalog);
                        var storeLocation = _services.WebHelper.GetSiteLocation(false);
                        var disableIfNotSimpleArticle = disableIf.Contains("notsimpleproduct");
                        var labelTextGrouped = T("Admin.Catalog.Articles.ArticleType.GroupedArticle.Label").Text;
                        var labelTextBundled = T("Admin.Catalog.Articles.ArticleType.BundledArticle.Label").Text;
                        var sku = T("Articles.Sku").Text;

                        var searchContext = new ArticleSearchContext
                        {
                            CategoryIds = (model.CategoryId == 0 ? null : new List<int> { model.CategoryId }),
                            ManufacturerId = model.ManufacturerId,
                            ChannelIds = (model.ChannelId == 0 ? null : new List<int> { model.ChannelId }),
                            VendorId = _services.WorkContext.CurrentVendor != null ? _services.WorkContext.CurrentVendor.Id : model.SiteId,
                            Keywords = model.SearchTerm,
                            ShowHidden = hasPermission
                        };


                        var query = _productService.Value.PrepareArticleSearchQuery(searchContext);

                        query = from x in query
                                group x by x.Id into grp
                                orderby grp.Key
                                select grp.FirstOrDefault();

                        var products = query
                            .OrderBy(x => x.Title)
                            .Skip(model.PageIndex * model.PageSize)
                            .Take(model.PageSize)
                            .ToList();

                        var productIds = products.Select(x => x.Id).ToArray();
                        var pictures = _productService.Value.GetArticlePicturesByArticleIds(productIds, true);

                        model.SearchResult = products
                            .Select(x =>
                            {
                                var item = new EntityPickerModel.SearchResultModel
                                {
                                    Id = x.Id,
                                    ReturnValue = (model.ReturnField.IsCaseInsensitiveEqual("url") ? Url.RouteUrl("Article", new { SeName = x.GetSeName() }, Request.Url.Scheme) : x.Id.ToString()),
                                    Title = x.Title,
                                    Summary = x.Sku,
                                    SummaryTitle = "{0}: {1}".FormatInvariant(sku, x.Sku.NaIfEmpty()),
                                    Published = (hasPermission ? x.StatusId == (int)ArticleStatus.Normal : (bool?)null)
                                };

                                if (disableIfNotSimpleArticle)
                                {
                                    item.Disable = (x.ArticleTypeId != (int)ArticleType.Simple);
                                }



                                var productPicture = pictures.FirstOrDefault(y => y.Key == x.Id);
                                if (productPicture.Value != null)
                                {
                                    var picture = productPicture.Value.FirstOrDefault();
                                    if (picture != null)
                                    {
                                        item.ImageUrl = _pictureService.Value.GetPictureUrl(picture.Picture, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage,
                                            !_catalogSettings.HideArticleDefaultPictures, storeLocation);
                                    }
                                }

                                return item;
                            })
                            .ToList();

                        #endregion
                    }
                    else if (model.Entity.ToLower().Contains("manufacturer"))
                    {
                        var storeLocation = _services.WebHelper.GetSiteLocation(false);
                        var manufacturers = _manufacturerService.Value.GetAllManufacturers(model.ArticleName.TrimSafe(), model.PageIndex, model.PageSize);
                        model.SearchResult = manufacturers
                         .Select(x =>
                         {
                             var item = new EntityPickerModel.SearchResultModel
                             {
                                 Id = x.Id,
                                 ReturnValue = x.Id.ToString(),
                                 Title = x.Name,
                                 Published = true
                             };
                             var picture = x.Picture;
                             if (picture != null)
                             {
                                 item.ImageUrl = _pictureService.Value.GetPictureUrl(picture, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage,
                                     !_catalogSettings.HideArticleDefaultPictures, storeLocation);
                             }

                             return item;
                         })
                         .ToList();
                    }
                    else if (model.Entity.ToLower().Contains("vendor"))
                    {
                        var storeLocation = _services.WebHelper.GetSiteLocation(false);
                        var vendors = _vendorService.Value.GetAllVendors(model.ArticleName.TrimSafe(), model.PageIndex, model.PageSize);
                        model.SearchResult = vendors
                         .Select(x =>
                         {
                             var item = new EntityPickerModel.SearchResultModel
                             {
                                 Id = x.Id,
                                 ReturnValue = (model.ReturnField.IsCaseInsensitiveEqual("url") ? Url.RouteUrl("Vendor", new { SystemName = x.GetSeName() }, Request.Url.Scheme) : x.Id.ToString()),
                                 Title = x.Name,
                                 Published = true
                             };
                             var picture = x.Picture;
                             if (picture != null)
                             {
                                 item.ImageUrl = _pictureService.Value.GetPictureUrl(picture, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage, _mediaSettings.Value.ProductThumbPictureSizeOnProductDetailsPage,
                                     !_catalogSettings.HideArticleDefaultPictures, storeLocation);
                             }

                             return item;
                         })
                         .ToList();
                    }
                }
            }
            catch (Exception exception)
            {
                NotifyError(exception.ToAllMessages());
            }

            return PartialView("EntityPickerList", model);
        }

        #endregion

        #endregion
    }
}