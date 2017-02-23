using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Localization;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Security;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.WebSite.Mvc.Models.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Application.WebUI.MvcCaptcha;
using System.ServiceModel.Syndication;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Vendors;
using CAF.Infrastructure.Core.Domain.Cms.Agents;
using CAF.Infrastructure.Core.Utilities;
using CAF.Infrastructure.Core.Exceptions;

namespace CAF.WebSite.Mvc.Controllers
{
    [WhitespaceFilterAttribute]
    public class ArticleController : PublicControllerBase
    {
        #region Fields


        private readonly IPictureService _pictureService;
        private readonly ICommonServices _services;
        private readonly IArticleService _articleService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IUserContentService _userContentService;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IAclService _aclService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ArticleCatalogHelper _helper;
        private readonly IArticleTagService _articleTagService;
        private readonly IRecentlyViewedArticlesService _recentlyViewedArticlesService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IDownloadService _downloadService;

        #endregion

        #region Constructors

        public ArticleController(
            IPictureService pictureService,
            ICommonServices services,
            IArticleService articleService,
             IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            IUserContentService userContentService,
            IUserService userService,
            IUserActivityService userActivityService,
            IWorkflowMessageService workflowMessageService,
            IAclService aclService,
            ISiteMappingService siteMappingService,
            ArticleCatalogSettings catalogSettings,
            LocalizationSettings localizationSettings,
            ILocalizationService localizationService,
            ArticleCatalogHelper helper, IArticleTagService articleTagService,
            IRecentlyViewedArticlesService recentlyViewedArticlesService,
            IDownloadService downloadService,
            CaptchaSettings captchaSettings,
            IWebHelper webHelper,
            IWorkContext workContext
            )
        {
            this._pictureService = pictureService;
            this._services = services;
            this._articleService = articleService;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._userContentService = userContentService;
            this._userService = userService;
            this._userActivityService = userActivityService;
            this._workflowMessageService = workflowMessageService;
            this._aclService = aclService;
            this._siteMappingService = siteMappingService;
            this._catalogSettings = catalogSettings;
            this._localizationSettings = localizationSettings;
            this._localizationService = localizationService;
            this._helper = helper;
            this._articleTagService = articleTagService;
            this._recentlyViewedArticlesService = recentlyViewedArticlesService;
            this._workContext = workContext;
            this._captchaSettings = captchaSettings;
            this._webHelper = webHelper;
            this._downloadService = downloadService;

            T = NullLocalizer.Instance;
        }

        #endregion

        #region Articles
        /// <summary>
        /// 获取内容明细
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult ArticleDetails(int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null || article.Deleted)
                return HttpNotFound();

            //Is published?
            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a article before publishing
            if (article.StatusFormat != StatusFormat.Norma)
                return HttpNotFound();
            //判断内容访问权限，实际项目中需求及去掉注释启用
            ////ACL (access control list)
            //if (!_aclService.Authorize(article))
            //    return HttpNotFound();

            //Site mapping
            if (!_siteMappingService.Authorize(article))
                return HttpNotFound();

            var model = _helper.PrepareArticleDetailsPageModel(article, true, prepareArticleExtendedAttributes: true);

            //save as recently viewed
            _recentlyViewedArticlesService.AddArticleToRecentlyViewedList(article.Id);

            //activity log
            _services.UserActivity.InsertActivity("PublicSite.ViewArticle", T("ActivityLog.PublicSite.ViewArticle"), article.Title);

            _services.EventPublisher.Publish(new ArticleViewEvent(model.Id, model.Click));

            if (IsAjaxRequest())
                return Json(model);
            else
                return View(model.ModelTemplateViewPath, model);
        }
        /// <summary>
        /// 获取关联内容部分页面
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="articleThumbPictureSize"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RelatedArticles(int articleId, int? articleThumbPictureSize)
        {
            var articles = new List<Article>();
            var relatedArticles = _articleService.GetRelatedArticlesByArticleId1(articleId);
            foreach (var article in _articleService.GetArticlesByIds(relatedArticles.Select(x => x.ArticleId2).ToArray()))
            {
                //ensure has ACL permission and appropriate store mapping
                if (_aclService.Authorize(article) && _siteMappingService.Authorize(article))
                    articles.Add(article);
            }

            if (articles.Count == 0)
                return Content("");

            var model = _helper.PrepareArticlePostModels(articles, true, false, new System.Drawing.Size(articleThumbPictureSize ?? 0, articleThumbPictureSize ?? 0)).ToList();

            return PartialView(model);
        }
        /// <summary>
        /// 分享按钮部分页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ShareButton()
        {
            if (_catalogSettings.ShowShareButton && !String.IsNullOrEmpty(_catalogSettings.PageShareCode))
            {
                var shareCode = _catalogSettings.PageShareCode;
                if (_services.WebHelper.IsCurrentConnectionSecured())
                {
                    //need to change the addthis link to be https linked when the page is, so that the page doesnt ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }

                return PartialView("ShareButton", shareCode);
            }

            return Content("");
        }
        [ChildActionOnly]
        public ActionResult ArticleSpecifications(int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null || article.Deleted)
                throw new ArgumentException(T("Products.NotFound", articleId));

            var model = _helper.PrepareProductSpecificationModel(article);

            if (model.Count == 0)
                return Content("");

            return PartialView(model);
        }

        #region Article reviews
        /// <summary>
        /// 内容点赞部分页面
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ArticleDetailReviews(int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null || !article.AllowUserReviews)
                return Content("");

            var model = new ArticleReviewsModel();
            _helper.PrepareArticleReviewsModel(model, article);

            return PartialView(model);
        }
        /// <summary>
        /// 获取点赞结果
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("Reviews")]
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult Reviews(int id)
        {
            var article = _articleService.GetArticleById(id);
            if (article == null || article.Deleted || article.StatusFormat != StatusFormat.Norma || !article.AllowUserReviews)
                return HttpNotFound();

            var model = new ArticleReviewsModel();
            _helper.PrepareArticleReviewsModel(model, article);
            //only registered users can leave reviews
            if (_services.WorkContext.CurrentUser.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewArticle)
                ModelState.AddModelError("", T("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            //default value
            model.AddArticleReview.Rating = _catalogSettings.DefaultArticleRatingValue;
            return View(model);
        }
        /// <summary>
        /// 点赞事件提交
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="captchaValid"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Reviews")]
        [FormValueRequired("add-review")]
        // [CaptchaValidator]
        public ActionResult ReviewsAdd(int id, ArticleReviewsModel model, bool captchaValid)
        {
            var article = _articleService.GetArticleById(id);
            if (article == null || article.Deleted || article.StatusFormat != StatusFormat.Norma || !article.AllowUserReviews)
                return HttpNotFound();

            if (_services.WorkContext.CurrentUser.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewArticle)
            {
                ModelState.AddModelError("", T("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }

            if (ModelState.IsValid)
            {
                //save review
                int rating = model.AddArticleReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultArticleRatingValue;

                bool isApproved = !_catalogSettings.ArticleReviewsMustBeApproved;
                var user = _services.WorkContext.CurrentUser;

                var articleReview = new ArticleReview()
                {
                    ArticleId = article.Id,
                    UserId = user.Id,
                    IpAddress = _services.WebHelper.GetCurrentIpAddress(),

                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    ModifiedOnUtc = DateTime.UtcNow,
                };
                _userContentService.InsertUserContent(articleReview);

                //update article totals
                _articleService.UpdateArticleReviewTotals(article);

                //activity log
                _services.UserActivity.InsertActivity("PublicStore.AddArticleReview", T("ActivityLog.PublicStore.AddArticleReview"), article.Title);

                //if (isApproved)
                //    _userService.RewardPointsForArticleReview(user, article, true);

                _helper.PrepareArticleReviewsModel(model, article);

                model.AddArticleReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddArticleReview.Result = T("Reviews.SeeAfterApproving");
                else
                    model.AddArticleReview.Result = T("Reviews.SuccessfullyAdded");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            _helper.PrepareArticleReviewsModel(model, article);
            return View(model);
        }
        /// <summary>
        /// Ajax点赞评论提交
        /// </summary>
        /// <param name="articleReviewId"></param>
        /// <param name="washelpful"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetReviewHelpfulness(int articleReviewId, bool washelpful)
        {
            var article = _articleService.GetArticleById(articleReviewId);
            if (article == null || article.Deleted || article.StatusFormat != StatusFormat.Norma || !article.AllowUserReviews)
                return HttpNotFound();

            var articleReview = article.ArticleReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc).FirstOrDefault() as ArticleReview;
            if (articleReview == null)
            {
                bool isApproved = !_catalogSettings.ArticleReviewsMustBeApproved;
                var user = _services.WorkContext.CurrentUser;
                //添加新的评论赞
                articleReview = new ArticleReview()
                {
                    ArticleId = article.Id,
                    UserId = user.Id,
                    IpAddress = _services.WebHelper.GetCurrentIpAddress(),
                    Rating = 0,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    ModifiedOnUtc = DateTime.UtcNow,
                };
                _userContentService.InsertUserContent(articleReview);
            }

            if (_services.WorkContext.CurrentUser.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewArticle)
            {
                return Json(new
                {
                    Success = false,
                    Result = T("Reviews.Helpfulness.OnlyRegistered").Text,
                    TotalYes = articleReview.HelpfulYesTotal,
                    TotalNo = articleReview.HelpfulNoTotal
                });
            }

            //users aren't allowed to vote for their own reviews
            if (articleReview.UserId == _services.WorkContext.CurrentUser.Id)
            {
                return Json(new
                {
                    Success = false,
                    Result = T("Reviews.Helpfulness.YourOwnReview").Text,
                    TotalYes = articleReview.HelpfulYesTotal,
                    TotalNo = articleReview.HelpfulNoTotal
                });
            }

            //delete previous helpfulness
            var oldPrh = (from prh in articleReview.ArticleReviewHelpfulnessEntries
                          where prh.UserId == _services.WorkContext.CurrentUser.Id
                          select prh).FirstOrDefault();
            if (oldPrh != null)
                _userContentService.DeleteUserContent(oldPrh);

            //insert new helpfulness
            var newPrh = new ArticleReviewHelpfulness()
            {
                ArticleReviewId = articleReview.Id,
                UserId = _services.WorkContext.CurrentUser.Id,
                IpAddress = _services.WebHelper.GetCurrentIpAddress(),
                WasHelpful = washelpful,
                IsApproved = true, //always approved
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow,
            };
            _userContentService.InsertUserContent(newPrh);

            //new totals
            int helpfulYesTotal = (from prh in articleReview.ArticleReviewHelpfulnessEntries
                                   where prh.WasHelpful
                                   select prh).Count();
            int helpfulNoTotal = (from prh in articleReview.ArticleReviewHelpfulnessEntries
                                  where !prh.WasHelpful
                                  select prh).Count();

            articleReview.HelpfulYesTotal = helpfulYesTotal;
            articleReview.HelpfulNoTotal = helpfulNoTotal;
            _userContentService.UpdateUserContent(articleReview);

            return Json(new
            {
                Success = true,
                Result = T("Reviews.Helpfulness.SuccessfullyVoted").Text,
                TotalYes = articleReview.HelpfulYesTotal,
                TotalNo = articleReview.HelpfulNoTotal
            });
        }

        #endregion

        [HttpPost]
        public ActionResult UpdateArticleDetails(int productId, string itemType, FormCollection form)
        {
            int quantity = 1;
            int galleryStartIndex = -1;
            string galleryHtml = null;
            string dynamicThumbUrl = null;
            bool isAssociated = itemType.IsCaseInsensitiveEqual("associateditem");
            var pictureModel = new ArticleDetailsPictureModel();
            var m = new ArticlePostModel();
            var article = _articleService.GetArticleById(productId);



            var warnings = new List<string>();
            var attributes = _productAttributeService.GetProductVariantAttributesByArticleId(productId);

            string attributeXml = form.CreateSelectedAttributesXml(productId, attributes, _productAttributeParser,
                _localizationService, _downloadService, _catalogSettings, this.Request, warnings, true);


            // quantity required for tier prices
            string quantityKey = form.AllKeys.FirstOrDefault(k => k.EndsWith("EnteredQuantity"));
            if (quantityKey.HasValue())
                int.TryParse(form[quantityKey], out quantity);



            // get merged model data
            _helper.PrepareArticleDetailModel(m, article, isAssociated, form, quantity);

            var pictures = _pictureService.GetPicturesByArticleId(productId);

            if (pictures.Count <= _catalogSettings.DisplayAllImagesNumber)  // all pictures rendered... only index is required
            {
                var picture = m.GetAssignedPicture(_pictureService, pictures);
                galleryStartIndex = (picture == null ? 0 : pictures.IndexOf(picture));
            }
            else
            {
                var allCombinationPictureIds = _productAttributeService.GetAllProductVariantAttributeCombinationPictureIds(article.Id);

                _helper.PrepareArticleDetailsPictureModel(
                    pictureModel,
                    pictures,
                    article.GetLocalized(x => x.Title),
                    m.SelectedCombination);

                galleryStartIndex = pictureModel.GalleryStartIndex;
                galleryHtml = this.RenderPartialViewToString("_PictureGallery", pictureModel);
            }


            #region data object

            object data = new
            {
                Delivery = new
                {
                    Id = 0,
                    Name = m.DeliveryTimeName,
                    Color = m.DeliveryTimeHexValue,
                },
                Measure = new
                {
                    Weight = new { Value = m.WeightValue, Text = m.Weight },
                    Height = new { Value = article.Height, Text = m.Height },
                    Width = new { Value = article.Width, Text = m.Width },
                    Length = new { Value = article.Length, Text = m.Length }
                },
                Number = new
                {
                    Sku = new { Value = m.Sku, Show = true }
                },
                Price = new
                {
                    Base = new
                    {
                        Enabled = true,
                        Info = m.ProductPrice.Price
                    },
                    Old = new
                    {
                        Value = decimal.Zero,
                        Text = m.ProductPrice.OldPrice
                    },
                    WithoutDiscount = new
                    {
                        Value = m.ProductPrice.PriceValue,
                        Text = m.ProductPrice.Price
                    },
                    WithDiscount = new
                    {
                        Value = m.ProductPrice.PriceWithDiscountValue,
                        Text = m.ProductPrice.PriceWithDiscount
                    }
                },
                Stock = new
                {
                    Quantity = new
                    {
                        Value = article.StockQuantity,
                        Show = article.DisplayStockQuantity
                    },
                    Availability = new
                    {
                        Text = m.StockAvailability,
                        Show = article.DisplayStockAvailability,
                        Available = m.IsAvailable
                    }
                },

                DynamicThumblUrl = dynamicThumbUrl,
                GalleryStartIndex = galleryStartIndex,
                GalleryHtml = galleryHtml
            };

            #endregion

            return new JsonResult { Data = data };
        }
        /// <summary>
        /// 添加新评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="model"></param>
        /// <param name="captchaValid"></param>
        /// <returns></returns>
        [HttpPost, ActionName("ArticleDetails")]
        [FormValueRequired("add-comment")]
        public ActionResult ArticleCommentAdd(int articleId, ArticlePostModel model, bool captchaValid)
        {

            var article = _articleService.GetArticleById(articleId);
            if (article == null || !article.AllowComments)
                return HttpNotFound();

            if (_workContext.CurrentUser.IsGuest() && !_catalogSettings.AllowNotRegisteredUsersToLeaveComments)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Article.Comments.OnlyRegisteredUsersLeaveComments"));
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnArticleCommentPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {
                var comment = new ArticleComment()
                {
                    ArticleId = article.Id,
                    AddTime = DateTime.Now,
                    UserId = _workContext.CurrentUser.Id,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    CommentText = model.AddNewComment.CommentText,
                    CommentTitle = model.AddNewComment.CommentTitle,
                    IsApproved = true
                };
                comment.AddEntitySysParam(true, true);
                _userContentService.InsertUserContent(comment);

                //update totals
                _articleService.UpdateCommentTotals(article);

                //notify a site owner
                //if (_articleSettings.NotifyAboutNewArticleComments)
                //    _workflowMessageService.SendArticleCommentNotificationMessage(comment, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _userActivityService.InsertActivity("PublicSite.AddArticleComment", _localizationService.GetResource("ActivityLog.PublicSite.AddArticleComment"));

                //The text boxes should be cleared after a comment has been posted
                //That' why we reload the page
                TempData["sm.article.addcomment.result"] = _localizationService.GetResource("Article.Comments.SuccessfullyAdded");


                return RedirectToRoute("Article", new { SeName = article.GetSeName() });
                // codehint: sm-delete
                //return RedirectToRoute("ArticlePost", new { SeName = article.GetSeName(article.LanguageId, ensureTwoPublishedLanguages: false) });
            }

            //If we got this far, something failed, redisplay form
            _helper.PrepareArticleDetailModel(model, article, true);
            return View(model);
        }

        /// <summary>
        /// 添加新评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="model"></param>
        /// <param name="captchaValid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ArticleAgencyAdd(ArticleAgnetModel model)
        {
            bool flag = true;
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage)
            {
                string item = base.Session[CacheKeyCollection.SessionVerifyCode("Agent")] as string;
                flag = item == model.CheckCode.ToLower().ToMd5();
                if (!flag)
                    //数据验证错误
                    return Json(new { Result = false, Code = 99, Message = "验证码错误，请重新填写!" });
            }
            var comment = new AgencyContent()
            {
                VendorId = model.VendorId ?? 0,
                ArticleId = model.ArticleId ?? 0,
                UserId = _workContext.CurrentUser.Id,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                TrueName = model.TrueName,
                TelePhone = model.TelePhone,
                Mobile = model.Mobile,
                QQ = model.QQ,
                Email = model.Email,
                ProvinceName = model.ProvinceName,
                CityName = string.Join(",", model.CityName),
                DistributionChannelId = string.Join(",", model.DistributionChannelId),
                AgentPropertyId = string.Join(",", model.AgentPropertyId),
                Message = model.Message,
                ShowAuthId = model.ShowAuthId,
                OtherChannel = model.OtherChannel,
                IsApproved = true,
                
            };
            comment.AddEntitySysParam(true, true);
            _userContentService.InsertUserContent(comment);

            //activity log
            _userActivityService.InsertActivity("PublicSite.AddArticleAgent", "新增代理咨询");

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 内容Rss
        /// </summary>
        /// <returns></returns>
        [ActionName("rss")]
        public ActionResult ListRss()
        {
            var feed = new SyndicationFeed(
                                string.Format("{0}: {1}", _services.SiteContext.CurrentSite.Name, T("RSS.RecentlyAddedArticles")),
                                T("RSS.InformationAboutArticles"),
                                new Uri(_services.WebHelper.GetSiteLocation(false)),
                                "RecentlyAddedArticlesRSS",
                                DateTime.UtcNow);

            if (!_catalogSettings.RecentlyAddedArticlesEnabled)
                return new RssActionResult() { Feed = feed };

            var items = new List<SyndicationItem>();

            var ctx = new ArticleSearchContext();
            ctx.LanguageId = _services.WorkContext.WorkingLanguage.Id;
            ctx.OrderBy = ArticleSortingEnum.CreatedOn;
            ctx.PageSize = _catalogSettings.RecentlyAddedArticlesNumber;
            ctx.SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode;
            ctx.VisibleIndividuallyOnly = true;

            var articles = _articleService.SearchArticles(ctx);

            foreach (var article in articles)
            {
                string articleUrl = Url.RouteUrl("Article", new { SeName = article.GetSeName() }, "http");
                if (!String.IsNullOrEmpty(articleUrl))
                {
                    items.Add(new SyndicationItem(article.GetLocalized(x => x.Title), article.GetLocalized(x => x.ShortContent), new Uri(articleUrl), String.Format("RecentlyAddedArticle:{0}", article.Id), article.CreatedOnUtc));
                }
            }
            feed.Items = items;
            return new RssActionResult() { Feed = feed };
        }


        #endregion

        #region Article tags
        /// <summary>
        /// 获取内容标签
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ArticleTags(int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            var cacheKey = string.Format(ModelCacheEventConsumer.ARTICLETAG_BY_ARTICLE_MODEL_KEY, article.Id, _services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id);
            var cacheModel = _services.Cache.Get(cacheKey, () =>
            {
                var model = article.ArticleTags
                    //filter by site
                    .Where(x => _articleTagService.GetArticleCount(x.Id, _services.SiteContext.CurrentSite.Id) > 0)
                    .Select(x =>
                    {
                        var ptModel = new ArticleTagModel()
                        {
                            Id = x.Id,
                            Name = x.GetLocalized(y => y.Name),
                            SeName = x.GetSeName(),
                            ArticleCount = _articleTagService.GetArticleCount(x.Id, _services.SiteContext.CurrentSite.Id)
                        };
                        return ptModel;
                    })
                    .ToList();
                return model;
            });

            return PartialView(cacheModel);
        }
        #endregion



    }
}