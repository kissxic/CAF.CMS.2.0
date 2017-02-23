using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Security;
using CAF.WebSite.Mvc.Models.Common;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Topics;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Html;
using CAF.WebSite.Application.WebUI.MvcCaptcha;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Models.Topics;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Mvc.Models.Feedbacks;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Feedbacks;
using CAF.WebSite.Mvc.Models.Articles;
using CAF.WebSite.Mvc.Models.HomeFloors;
using CAF.WebSite.Application.Services.PageSettings;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.WebSite.Mvc.Models.Homes;
using System.Drawing;
using CAF.WebSite.Application.Services.Vendors;

namespace CAF.WebSite.Mvc.Controllers
{

    public class HomeController : PublicControllerBase
    {
        #region Fields
        private readonly ICommonServices _services;
        private readonly HomePageSettings _homePageSettings;
        private readonly ArticleCatalogHelper _helper;
        private readonly Lazy<IArticleCategoryService> _articleCategoryService;
        private readonly Lazy<IArticleService> _articleService;
        private readonly Lazy<IFeedbackService> _feedbackService;
        private readonly Lazy<IWebHelper> _webHelper;
        private readonly Lazy<ITopicService> _topicService;
        private readonly Lazy<IQueuedEmailService> _queuedEmailService;
        private readonly Lazy<IEmailAccountService> _emailAccountService;
        private readonly Lazy<ISitemapGenerator> _sitemapGenerator;
        private readonly Lazy<CaptchaSettings> _captchaSettings;
        private readonly Lazy<CommonSettings> _commonSettings;
        private readonly Lazy<ISlideAdInfoService> _slideAdInfoService;
        private readonly Lazy<IPictureService> _pictureService;
        private readonly Lazy<IHomeFloorInfoService> _homeFloorInfoService;
        private readonly Lazy<IBannerInfoService> _bannerInfoService;
        private readonly Lazy<IVendorService> _vendorService;
        #endregion

        #region Constructors
        public HomeController(
                ICommonServices services,
                HomePageSettings homePageSettings,
                ArticleCatalogHelper helper,
                Lazy<IArticleCategoryService> articleCategoryService,
                Lazy<IArticleService> articleService,
                Lazy<IFeedbackService> feedbackService,
                Lazy<IWebHelper> webHelper,
                Lazy<ITopicService> topicService,
                Lazy<IQueuedEmailService> queuedEmailService,
                Lazy<IEmailAccountService> emailAccountService,
                Lazy<ISitemapGenerator> sitemapGenerator,
                Lazy<CaptchaSettings> captchaSettings,
                Lazy<CommonSettings> commonSettings,
                Lazy<ISlideAdInfoService> slideAdInfoService,
                Lazy<IPictureService> pictureService,
                Lazy<IHomeFloorInfoService> homeFloorInfoService,
                Lazy<IBannerInfoService> bannerInfoService,
                Lazy<IVendorService> vendorService
            )
        {
            this._helper = helper;
            this._homePageSettings = homePageSettings;
            this._services = services;
            this._articleCategoryService = articleCategoryService;
            this._feedbackService = feedbackService;
            this._webHelper = webHelper;
            this._articleService = articleService;
            this._topicService = topicService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._sitemapGenerator = sitemapGenerator;
            this._captchaSettings = captchaSettings;
            this._commonSettings = commonSettings;
            this._slideAdInfoService = slideAdInfoService;
            this._pictureService = pictureService;
            this._homeFloorInfoService = homeFloorInfoService;
            this._bannerInfoService = bannerInfoService;
            this._vendorService = vendorService;

            T = NullLocalizer.Instance;
        }
        #endregion

        #region Utitilies
        //文章信息装配
        private List<ArticleOverviewModel> PrepareArticleModel(int[] articleIds, bool preparePictureModel = false, int width = 0, int height = 0)
        {
            var articles = _articleService.Value.GetArticlesByIds(articleIds);
            return _helper.PrepareArticlePostModels(
                       articles,
                       preparePictureModel: preparePictureModel, articleThumbPictureSize: new Size(width, height), prepareArticleExtendedAttributes: true).ToList();

        }
        /// <summary>
        /// 商家信息装配
        /// </summary>
        /// <param name="vendorIds"></param>
        /// <returns></returns>
        private List<HomeFloorModel.WebFloorTextLink> PrepareVendorModel(int[] vendorIds)
        {
            var vendorList = new List<HomeFloorModel.WebFloorTextLink>();
            //推荐商家

            var vendors = _vendorService.Value.GetVendorById(vendorIds.ToArray());
            foreach (var vendor in vendors)
            {
                var vendorModel = new HomeFloorModel.WebFloorTextLink()
                {
                    Name = vendor.Name,
                    Id = vendor.Id,
                    Url = Url.RouteUrl("Vendor", new { SystemName = vendor.GetSeName() }),
                };
                vendorList.Add(vendorModel);

            }
            return vendorList;
        }
        /// <summary>
        /// 楼层装配
        /// </summary>
        /// <returns></returns>
        private List<HomeFloorModel> PrepareHomeFloorModel()
        {
            return _services.Cache.Get(ModelCacheEventConsumer.HOMEFLOOR_PATTERN_KEY.FormatWith(_services.SiteContext.CurrentSite.Id), () =>
             {
                 var homeFloorModels = new List<HomeFloorModel>();
                 var homeFloors = _homeFloorInfoService.Value.GetAllHomeFloorInfos();
                 foreach (var homeFloorInfo in homeFloors)
                 {
                     var homeFloorModel = new HomeFloorModel();
                     //文本
                     var floorTopicInfos = homeFloorInfo.FloorTopicInfos.Where(v => v.TopicType == Position.Top).ToList();
                     //图片
                     var floorTopicImageInfos = homeFloorInfo.FloorTopicInfos.Where(v => v.TopicType != Position.Top).ToList();

                     homeFloorModel.Name = homeFloorInfo.FloorName;
                     homeFloorModel.SubName = homeFloorInfo.SubName;
                     homeFloorModel.StyleLevel = homeFloorInfo.StyleLevel;
                     homeFloorModel.DefaultTabName = homeFloorInfo.DefaultTabName;
                     homeFloorModel.URL = homeFloorInfo.Url;
                     foreach (var floorTopicInfo in floorTopicInfos)
                     {
                         var webFloorTextLink = new HomeFloorModel.WebFloorTextLink()
                         {
                             Id = floorTopicInfo.Id,
                             Name = floorTopicInfo.TopicName,
                             Url = floorTopicInfo.Url
                         };
                         homeFloorModel.TextLinks.Add(webFloorTextLink);
                     }
                     foreach (var floorTopicInfo in floorTopicImageInfos)
                     {

                         var webFloorProductLink = new HomeFloorModel.WebFloorArticleLinks()
                         {
                             Id = floorTopicInfo.Id,
                             ImageUrl = _pictureService.Value.GetPictureUrl(floorTopicInfo.PictureId ?? 0, showDefaultPicture: true),
                             PictureId = floorTopicInfo.PictureId ?? 0,
                             Url = floorTopicInfo.Url,
                             Type = floorTopicInfo.TopicType
                         };
                         homeFloorModel.ArticleLinks.Add(webFloorProductLink);
                     }
                     //楼层一
                     if (homeFloorModel.StyleLevel == 0)
                     {
                         if (!homeFloorInfo.RelateProductIds.IsEmpty())
                         {
                             var productIds = new List<int>();
                             homeFloorInfo.RelateProductIds.Split(',').ToList().Each(p =>
                             {
                                 productIds.Add(p.ToInt());
                             });
                             homeFloorModel.Articles = PrepareArticleModel(productIds.ToArray(), true, 200, 200);
                         }
                     }
                     //楼层二
                     else if (homeFloorModel.StyleLevel == 1)
                     {
                         var vendorIds = new List<int>();
                         if (!homeFloorInfo.RelateVendorIds.IsEmpty())
                         {
                             homeFloorInfo.RelateVendorIds.Split(',').Each(x =>
                             {
                                 vendorIds.Add(x.ToInt());
                             });
                             homeFloorModel.Vendors = PrepareVendorModel(vendorIds.ToArray());
                         }

                     }
                     //楼层三
                     else if (homeFloorModel.StyleLevel == 2)
                     {

                     }
                     homeFloorModels.Add(homeFloorModel);
                 }
                 return homeFloorModels;
             });
        }

        private void PrepareHomePageSettings()
        {

            if (!_homePageSettings.RelateProductIds.IsEmpty())
            {
                var productIds = new List<int>();
                _homePageSettings.RelateProductIds.Split(',').ToList().Each(p =>
                {
                    productIds.Add(p.ToInt());
                });
                ViewBag.RelateArticles = PrepareArticleModel(productIds.ToArray(), false, 200, 200);
            }

            var vendorIds = new List<int>();
            if (!_homePageSettings.RelateVendorIds.IsEmpty())
            {
                _homePageSettings.RelateVendorIds.Split(',').Each(x =>
                {
                    vendorIds.Add(x.ToInt());
                });
                ViewBag.RelateVendors = PrepareVendorModel(vendorIds.ToArray());
            }
        }
        #endregion

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult Index()
        {
            PrepareHomePageSettings();
            //首页幻灯片
            var slideList = _services.Cache.Get(ModelCacheEventConsumer.SLIDEADINFOS_PATTERN_KEY.FormatWith(_services.SiteContext.CurrentSite.Id), () =>
            {
                var slideAdInfos = this._slideAdInfoService.Value.GetAllSlideAdInfos().Select(x => new SlideAdInfoModel()
                {
                    Id = x.Id,
                    PictureId = x.PictureId,
                    // PictureUrl = _pictureService.Value.GetPictureUrl(x.PictureId, showDefaultPicture: true),
                    Url = x.Url,
                    SlideAdTypeId = x.SlideAdTypeId
                }).ToList();
                slideAdInfos.Each(x =>
                {
                    x.PictureUrl = _pictureService.Value.GetPictureUrl(x.PictureId, showDefaultPicture: true);
                });
                return slideAdInfos;
            });
            ViewBag.slideImage = slideList.Where(p => p.SlideAdTypeId == (int)SlideAdType.PlatformHome);
            ViewBag.slideMiniImage = slideList.Where(p => p.SlideAdTypeId == (int)SlideAdType.PlatformMiniHome);
            ViewBag.slideAreaImage = slideList.Where(p => p.SlideAdTypeId == (int)SlideAdType.PlatformAreaHome);
            ViewBag.slideLimitTimeImage = slideList.Where(p => p.SlideAdTypeId == (int)SlideAdType.PlatformLimitTime);
            //区域小图
            var imageAdInfoList = _services.Cache.Get(ModelCacheEventConsumer.IMAGEADINFOS_PATTERN_KEY.FormatWith(_services.SiteContext.CurrentSite.Id), () =>
            {
                var imageAdInfos = this._slideAdInfoService.Value.GetAllImageAdInfos().Select(x => new ImageAdInfoModel()
                {
                    Id = x.Id,
                    PictureId = x.PictureId,
                    //PictureUrl = _pictureService.Value.GetPictureUrl(x.PictureId, showDefaultPicture: true),
                    Url = x.Url
                }).ToList();
                imageAdInfos.Each(x =>
                {
                    x.PictureUrl = _pictureService.Value.GetPictureUrl(x.PictureId, showDefaultPicture: true);
                });
                return imageAdInfos;
            });
            ViewBag.imageAdsTop = imageAdInfoList.ToList();
            //楼层
            var homeFloorModelList = PrepareHomeFloorModel();
            return View(homeFloorModelList);
        }

        /// <summary>
        /// 首页幻灯片
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ContentSlider()
        {
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var settings = _services.Settings.LoadSetting<ContentSliderSettings>();

            settings.BackgroundPictureUrl = pictureService.GetPictureUrl(settings.BackgroundPictureId, 0, 0, false);

            var slides = settings.Slides
                .Where(s =>
                    s.LanguageCulture == _services.WorkContext.WorkingLanguage.LanguageCulture &&
                    (!s.LimitedToSites || (s.SelectedSiteIds != null && s.SelectedSiteIds.Contains(_services.SiteContext.CurrentSite.Id)))
                )
                .OrderBy(s => s.DisplayOrder);

            foreach (var slide in slides)
            {
                slide.PictureUrl = pictureService.GetPictureUrl(slide.PictureId, 0, 0, false);
                slide.BackgroundPictureUrl = pictureService.GetPictureUrl(slide.BackgroundPictureId, 0, 0, false);
                slide.Button1.Url = CheckButtonUrl(slide.Button1.Url);
                slide.Button2.Url = CheckButtonUrl(slide.Button2.Url);
                slide.Button3.Url = CheckButtonUrl(slide.Button3.Url);
            }

            settings.Slides = slides.ToList();

            return PartialView(settings);
        }

        #region 联系我们

        /// <summary>
        /// 联系我们
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult ContactUs()
        {
            var model = new ContactUsModel()
            {
                Email = _services.WorkContext.CurrentUser.Email,
                FullName = _services.WorkContext.CurrentUser.GetFullName(),
                DisplayCaptcha = _captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage
            };

            return View(model);
        }
        /// <summary>
        /// 联系我们留言
        /// </summary>
        /// <param name="model"></param>
        /// <param name="captchaValid"></param>
        /// <returns></returns>
        [HttpPost, ActionName("ContactUs")]
        [ValidateMvcCaptcha]
        public ActionResult ContactUsSend(ContactUsModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", T("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {
                string email = model.Email.Trim();
                string fullName = model.FullName;
                string subject = T("ContactUs.EmailSubject", _services.SiteContext.CurrentSite.Name);

                var emailAccount = _emailAccountService.Value.GetEmailAccountById(EngineContext.Current.Resolve<EmailAccountSettings>().DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.Value.GetAllEmailAccounts().FirstOrDefault();

                string from = null;
                string fromName = null;
                string body = HtmlUtils.FormatText(model.Enquiry, false, true, false, false, false, false);
                //required for some SMTP servers
                if (_commonSettings.Value.UseSystemEmailForContactUsForm)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                        Server.HtmlEncode(fullName),
                        Server.HtmlEncode(email), body);
                }
                else
                {
                    from = email;
                    fromName = fullName;
                }
                _queuedEmailService.Value.InsertQueuedEmail(new QueuedEmail
                {
                    From = from,
                    FromName = fromName,
                    To = emailAccount.Email,
                    ToName = emailAccount.DisplayName,
                    Priority = 5,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    ReplyTo = email,
                    ReplyToName = fullName
                });

                model.SuccessfullySent = true;
                model.Result = T("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                _services.UserActivity.InsertActivity("PublicSite.ContactUs", T("ActivityLog.PublicSite.ContactUs"));

                return View(model);
            }

            model.DisplayCaptcha = _captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage;
            return View(model);
        }

        #endregion

        #region 留言
        /// <留言>
        /// 联系我们
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult Feedback()
        {
            var model = new FeedbackModel()
            {
                DisplayCaptcha = _captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage
            };

            return View(model);
        }
        /// <summary>
        /// 留言
        /// </summary>
        /// <param name="model"></param>
        /// <param name="captchaValid"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Feedback")]
        [ValidateMvcCaptcha]
        public ActionResult FeedbackSend(FeedbackModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", T("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {

                var feedback = new Feedback()
                {
                    UserEmail = model.UserEmail.Trim(),
                    Title = model.Title,
                    Content = model.Content,
                    UserName = model.UserName.Trim(),
                    AddTime = DateTime.UtcNow,
                    ReplyTime = DateTime.UtcNow,
                    UserQQ = model.UserQQ,
                    UserTel = model.UserTel,
                    IPAddress = _webHelper.Value.GetCurrentIpAddress()
                };
                this._feedbackService.Value.InsertFeedback(feedback);
                //activity log
                _services.UserActivity.InsertActivity("PublicSite.Feedback", T("ActivityLog.PublicSite.Feedback"));

                return View(model);
            }

            model.DisplayCaptcha = _captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage;
            return View(model);
        }
        #endregion

        /// <summary>
        /// 网站关闭提示
        /// </summary>
        /// <returns></returns>
        public ActionResult SiteClosed()
        {
            return View();
        }
        /// <summary>
        /// 网站地图Seo
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult SitemapSeo()
        {
            if (!_commonSettings.Value.SitemapEnabled)
                return HttpNotFound();

            var roleIds = _services.WorkContext.CurrentUser.UserRoles.Where(x => x.Active).Select(x => x.Id).ToList();
            string cacheKey = ModelCacheEventConsumer.SITEMAP_XML_MODEL_KEY.FormatInvariant(_services.WorkContext.WorkingLanguage.Id, string.Join(",", roleIds), _services.SiteContext.CurrentSite.Id);
            var sitemap = _services.Cache.Get(cacheKey, () =>
            {
                return _sitemapGenerator.Value.Generate(this.Url);
            } , TimeSpan.FromMinutes(120));

            return Content(sitemap, "text/xml");
        }
        /// <summary>
        /// 网站地图
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult Sitemap()
        {
            if (!_commonSettings.Value.SitemapEnabled)
                return HttpNotFound();

            var roleIds = _services.WorkContext.CurrentUser.UserRoles.Where(x => x.Active).Select(x => x.Id).ToList();
            string cacheKey = ModelCacheEventConsumer.SITEMAP_PAGE_MODEL_KEY.FormatInvariant(_services.WorkContext.WorkingLanguage.Id, string.Join(",", roleIds), _services.SiteContext.CurrentSite.Id);

            var result = _services.Cache.Get(cacheKey, () =>
            {
                var model = new SitemapModel();
                if (_commonSettings.Value.SitemapIncludeCategories)
                {
                    var categories = _articleCategoryService.Value.GetAllCategories();
                    model.ArticleCategories = categories.Select(x => x.ToModel()).ToList();
                }
                if (_commonSettings.Value.SitemapIncludeProducts)
                {
                    //limit articel to 200 until paging is supported on this page
                    var articleSearchContext = new ArticleSearchContext();

                    articleSearchContext.OrderBy = ArticleSortingEnum.Position;
                    articleSearchContext.PageSize = 200;
                    articleSearchContext.SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode;
                    articleSearchContext.VisibleIndividuallyOnly = true;

                    var articels = _articleService.Value.SearchArticles(articleSearchContext);

                    model.Articles = articels.Select(articel => new ArticlePostModel()
                    {
                        Id = articel.Id,
                        Title = articel.GetLocalized(x => x.Title).EmptyNull(),
                        ShortContent = articel.GetLocalized(x => x.ShortContent),
                        FullContent = articel.GetLocalized(x => x.FullContent),
                        SeName = articel.GetSeName(),
                    }).ToList();
                }
                if (_commonSettings.Value.SitemapIncludeTopics)
                {
                    var topics = _topicService.Value.GetAllTopics(_services.SiteContext.CurrentSite.Id)
                         .ToList()
                         .FindAll(t => t.IncludeInSitemap);

                    model.Topics = topics.Select(topic => new TopicModel()
                    {
                        Id = topic.Id,
                        SystemName = topic.SystemName,
                        IncludeInSitemap = topic.IncludeInSitemap,
                        IsPasswordProtected = topic.IsPasswordProtected,
                        Title = topic.GetLocalized(x => x.Title),
                    })
                    .ToList();
                }
                return model;
            });

            return View(result);
        }

        #region helper functions
        /// <summary>
        /// 判断按钮是否URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string CheckButtonUrl(string url)
        {
            if (!String.IsNullOrEmpty(url))
            {
                if (url.StartsWith("//") || url.StartsWith("/") || url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    //  //www.domain.de/dir
                    //  http://www.domain.de/dir
                    // nothing needs to be done
                    return url;
                }
                else if (url.StartsWith("~/"))
                {
                    //  ~/directory
                    return Url.Content(url);
                }
                else
                {
                    //  directory
                    return Url.Content("~/" + url);
                }
            }

            return url.EmptyNull();
        }

        private void GetHomeFloor()
        {
            var homeFloorModels = new List<HomeFloorModel>();
        }

        #endregion helper functions
    }
}