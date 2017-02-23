using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Collections;
using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Models.ArticleCatalog;
using CAF.WebSite.Mvc.Models.Articles;
using CAF.WebSite.Mvc.Models.Catalog;
using CAF.WebSite.Mvc.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Collections.Specialized;
using CAF.WebSite.Application.Services.Manufacturers;
using CAF.Infrastructure.Core.Domain.Cms.Manufacturers;
using System.Drawing;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Application.Services.Vendors;

namespace CAF.WebSite.Mvc.Controllers
{
    public class ArticleCatalogHelper
    {
        private static object s_lock = new object();

        private readonly IUserService _userService;
        private readonly ICommonServices _services;
        private readonly IManufacturerService _manufacturerService;
        private readonly IChannelService _channelService;
        private readonly IArticleCategoryService _articleCategoryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IModelTemplateService _modelTemplateService;
        private readonly IArticleService _articleService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IArticleTagService _articleTagService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IVendorService _vendorService;
        private readonly MediaSettings _mediaSettings;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly UserSettings _userSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IMeasureService _measureService;
        private readonly IDeliveryTimeService _deliveryTimeService;
        private readonly ISettingService _settingService;
        private readonly Lazy<IMenuPublisher> _menuPublisher;
        private readonly IArticleExtendedAttributeService _articleAttributeService;
        private readonly IExtendedAttributeService _extendedAttributeService;
        private readonly IExtendedAttributeParser _extendedAttributeParser;
        private readonly HttpRequestBase _httpRequest;
        private readonly UrlHelper _urlHelper;

        public ArticleCatalogHelper(
            ICommonServices services,
            IUserService userService,
            IManufacturerService manufacturerService,
            IChannelService channelService,
            IArticleCategoryService articleCategoryService,
            IProductCategoryService productCategoryService,
            IModelTemplateService modelTemplateService,
            IArticleService articleService,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            ISpecificationAttributeService specificationAttributeService,
            IArticleTagService articleTagService,
            IPictureService pictureService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IPriceCalculationService priceCalculationService,
            IVendorService vendorService,
            MediaSettings mediaSettings,
            ArticleCatalogSettings catalogSettings,
            UserSettings userSettings,
            CaptchaSettings captchaSettings,
            IMeasureService measureService,
            IDeliveryTimeService deliveryTimeService,
            IArticleExtendedAttributeService articleAttributeService,
            IExtendedAttributeService extendedAttributeService,
            IExtendedAttributeParser extendedAttributeParser,
            ISettingService settingService,
            Lazy<IMenuPublisher> _menuPublisher,
            HttpRequestBase httpRequest,
            UrlHelper urlHelper)
        {
            this._services = services;
            this._userService = userService;
            this._manufacturerService = manufacturerService;
            this._channelService = channelService;
            this._articleCategoryService = articleCategoryService;
            this._productCategoryService = productCategoryService;
            this._modelTemplateService = modelTemplateService;
            this._articleService = articleService;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._specificationAttributeService = specificationAttributeService;
            this._articleTagService = articleTagService;
            this._pictureService = pictureService;
            this._localizationService = _services.Localization;
            this._dateTimeHelper = dateTimeHelper;
            this._downloadService = downloadService;
            this._priceCalculationService = priceCalculationService;
            this._vendorService = vendorService;
            this._measureService = measureService;
            this._deliveryTimeService = deliveryTimeService;
            this._settingService = settingService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._userSettings = userSettings;
            this._captchaSettings = captchaSettings;
            this._articleAttributeService = articleAttributeService;
            this._extendedAttributeService = extendedAttributeService;
            this._extendedAttributeParser = extendedAttributeParser;
            this._menuPublisher = _menuPublisher;
            this._httpRequest = httpRequest;
            this._urlHelper = urlHelper;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }
        /// <summary>
        /// 获取内容明细，并显示对应的模板页
        /// </summary>
        /// <param name="article"></param>
        /// <param name="prepareComments"></param>
        /// <param name="selectedAttributes"></param>
        /// <returns></returns>
        public ArticlePostModel PrepareArticleDetailsPageModel(Article article,
            bool prepareComments = false,
            bool preparePictureModel = true,
            bool preparePrevAndNext = false,
            bool preparePriceModel = false,
            bool prepareArticleExtendedAttributes = false,
            FormCollection selectedAttributes = null)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var model = new ArticlePostModel();

            //template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_TEMPLATE_MODEL_KEY, article.ModelTemplateId);
            model.ModelTemplateViewPath = _services.Cache.Get(templateCacheKey, () =>
            {
                var template = _modelTemplateService.GetModelTemplateById(article.ModelTemplateId);
                if (template == null)
                    template = _modelTemplateService.GetAllModelTemplates().FirstOrDefault();
                return template.ViewPath;
            });

            model = PrepareArticleDetailModel(model, article, prepareComments,
                preparePrevAndNext: preparePrevAndNext,
                preparePictureModel: preparePictureModel,
                prepareArticleExtendedAttributes: prepareArticleExtendedAttributes);
            return model;
        }

        /// <summary>
        /// 组装内容全部信息(明细)，方便其他内容调用,主要用于明细页面
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        /// <param name="prepareComments"></param>
        /// <returns></returns>
        public ArticlePostModel PrepareArticleDetailModel(
        ArticlePostModel model,
        Article article,
        bool prepareComments = false,
       NameValueCollection selectedAttributes = null,
        int selectedQuantity = 1,
        bool preparePictureModel = true,
        bool preparePrevAndNext = false,
        bool preparePriceModel = false,
        bool prepareArticleExtendedAttributes = false)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (model == null)
                throw new ArgumentNullException("model");

            if (selectedAttributes == null)
                selectedAttributes = new NameValueCollection();


            #region Properties

            model.Id = article.Id;
            model.VendorId = article.VendorId;
            model.Title = article.GetLocalized(x => x.Title);
            model.ShortContent = article.GetLocalized(x => x.ShortContent);
            model.FullContent = article.GetLocalized(x => x.FullContent);
            model.MetaKeywords = article.GetLocalized(x => x.MetaKeywords);
            model.MetaDescription = article.GetLocalized(x => x.MetaDescription);
            model.MetaTitle = article.GetLocalized(x => x.MetaTitle);
            model.SeName = article.GetSeName();
            model.Author = article.Author;
            model.IsHot = article.IsHot;
            model.IsRed = article.IsRed;
            model.IsSlide = article.IsSlide;
            model.IsTop = article.IsTop;
            model.LinkUrl = article.LinkUrl;
            model.IsLogin = _services.WorkContext.CurrentUser.IsRegistered();
            model.AllowComments = article.AllowComments;
            model.AvatarPictureSize = _mediaSettings.AvatarPictureSize; // codehint: sm-add
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(article.CreatedOnUtc, DateTimeKind.Utc);
            model.DisplayArticleReviews = _catalogSettings.ShowArticleReviewsInArticleDetail;
            model.NumberOfComments = article.ApprovedCommentCount;
            model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnArticleCommentPage;
            model.AllowUsersToUploadAvatars = _userSettings.AllowUsersToUploadAvatars;
            model.HasSampleDownload = article.IsDownload;
            model.ChannelId = article.ChannelId;
            model.CategroyId = article.CategoryId ?? 0;
            model.ProductCategroyId = article.ProductCategoryId ?? 0;
            if (_catalogSettings.RelativeDateTimeFormattingEnabled)
                model.PostCreatedOnStr = article.CreatedOnUtc.RelativeFormat(true, "f");
            else
                model.PostCreatedOnStr = _dateTimeHelper.ConvertToUserTime(article.CreatedOnUtc, DateTimeKind.Utc).ToString("f");

            if (prepareComments)
            {
                var articleComments = article.ArticleComments.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
                foreach (var ac in articleComments)
                {
                    var commentModel = new ArticleCommentModel()
                    {
                        Id = ac.Id,
                        UserId = ac.UserId.Value,
                        UserName = ac.User.FormatUserName(),
                        CommentText = ac.CommentText,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(ac.CreatedOnUtc, DateTimeKind.Utc),
                        AllowViewingProfiles = _userSettings.AllowViewingProfiles && ac.User != null && !ac.User.IsGuest()
                    };
                    if (_userSettings.AllowUsersToUploadAvatars)
                    {
                        var user = ac.User;
                        string avatarUrl = _pictureService.GetPictureUrl(user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId), _mediaSettings.AvatarPictureSize, _mediaSettings.AvatarPictureSize, false);
                        if (String.IsNullOrEmpty(avatarUrl) && _userSettings.DefaultAvatarEnabled)
                            avatarUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.AvatarPictureSize, _mediaSettings.AvatarPictureSize, PictureType.Avatar);
                        commentModel.UserAvatarUrl = avatarUrl;
                    }
                    model.Comments.Add(commentModel);

                }
            }


            #endregion



            var customer = _services.WorkContext.CurrentUser;
            decimal preSelectedPriceAdjustmentBase = decimal.Zero;
            decimal preSelectedWeightAdjustment = decimal.Zero;
            bool hasSelectedAttributesValues = false;
            bool hasSelectedAttributes = (selectedAttributes.Count > 0);
            List<ProductVariantAttributeValue> selectedAttributeValues = null;

            var displayPrices = _services.Permissions.Authorize(StandardPermissionProvider.DisplayPrices);
            var variantAttributes = _productAttributeService.GetProductVariantAttributesByArticleId(article.Id);

            #region Product attributes
            foreach (var attribute in variantAttributes)
            {
                var pvaModel = new ArticlePostModel.ProductVariantAttributeModel
                {
                    Id = attribute.Id,
                    ArticleId = attribute.ArticleId,

                    ProductAttributeId = attribute.ProductAttributeId,
                    Alias = attribute.ProductAttribute.Alias,
                    Name = attribute.ProductAttribute.GetLocalized(x => x.Name),
                    Description = attribute.ProductAttribute.GetLocalized(x => x.Description),
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    AllowedFileExtensions = _catalogSettings.FileUploadAllowedExtensions
                };

                if (attribute.AttributeControlType == AttributeControlType.Datepicker)
                {
                    if (pvaModel.Alias.HasValue() && RegularExpressions.IsYearRange.IsMatch(pvaModel.Alias))
                    {
                        var match = RegularExpressions.IsYearRange.Match(pvaModel.Alias);
                        pvaModel.BeginYear = match.Groups[1].Value.ToInt();
                        pvaModel.EndYear = match.Groups[2].Value.ToInt();
                    }
                }

                int preSelectedValueId = 0;
                var pvaValues = (attribute.ShouldHaveValues() ? _productAttributeService.GetProductVariantAttributeValues(attribute.Id) : new List<ProductVariantAttributeValue>());

                foreach (var pvaValue in pvaValues)
                {

                    var linkedProduct = _articleService.GetArticleById(pvaValue.LinkedProductId);

                    var pvaValueModel = new ArticlePostModel.ProductVariantAttributeValueModel();
                    pvaValueModel.Id = pvaValue.Id;
                    pvaValueModel.Name = pvaValue.GetLocalized(x => x.Name);
                    pvaValueModel.Alias = pvaValue.Alias;
                    pvaValueModel.ColorSquaresRgb = pvaValue.ColorSquaresRgb; //used with "Color squares" attribute type
                    pvaValueModel.IsPreSelected = pvaValue.IsPreSelected;

                    if (linkedProduct != null)
                        pvaValueModel.SeName = linkedProduct.GetSeName();

                    //价格调整
                    // display price if allowed
                    if (displayPrices)
                    {

                        decimal attributeValuePriceAdjustment = _priceCalculationService.GetProductVariantAttributeValuePriceAdjustment(pvaValue);

                        if (attributeValuePriceAdjustment > decimal.Zero)
                            pvaValueModel.PriceAdjustment = "+" + attributeValuePriceAdjustment;
                        else if (attributeValuePriceAdjustment < decimal.Zero)
                            pvaValueModel.PriceAdjustment = "-" + -attributeValuePriceAdjustment;

                        if (pvaValueModel.IsPreSelected)
                        {
                            preSelectedPriceAdjustmentBase = decimal.Add(preSelectedPriceAdjustmentBase, attributeValuePriceAdjustment);
                            preSelectedWeightAdjustment = decimal.Add(preSelectedWeightAdjustment, pvaValue.WeightAdjustment);
                        }
                        pvaValueModel.PriceAdjustmentValue = attributeValuePriceAdjustment;
                    }

                    pvaModel.Values.Add(pvaValueModel);
                }

                // we need selected attributes to get initially displayed combination images
                if (!hasSelectedAttributes)
                {
                    ArticlePostModel.ProductVariantAttributeValueModel defaultValue = null;

                    if (preSelectedValueId != 0)    // value pre-selected by a bundle item filter discards the default pre-selection
                    {
                        pvaModel.Values.Each(x => x.IsPreSelected = false);

                        if ((defaultValue = pvaModel.Values.FirstOrDefault(v => v.Id == preSelectedValueId)) != null)
                        {
                            defaultValue.IsPreSelected = true;
                            selectedAttributes.AddProductAttribute(attribute.ProductAttributeId, attribute.Id, defaultValue.Id, article.Id);
                        }
                    }

                    if (defaultValue == null)
                    {
                        foreach (var value in pvaModel.Values.Where(x => x.IsPreSelected))
                        {
                            selectedAttributes.AddProductAttribute(attribute.ProductAttributeId, attribute.Id, value.Id, article.Id);
                        }
                    }
                }

                model.ProductVariantAttributes.Add(pvaModel);
            }
            #endregion

            #region Attribute combinations


            if (selectedAttributes.Count > 0)
            {
                // merge with combination data if there's a match
                var warnings = new List<string>();
                string attributeXml = selectedAttributes.CreateSelectedAttributesXml(article.Id, variantAttributes, _productAttributeParser, _localizationService,
                    _downloadService, _catalogSettings, _httpRequest, warnings, true);

                selectedAttributeValues = _productAttributeParser.ParseProductVariantAttributeValues(attributeXml).ToList();
                hasSelectedAttributesValues = (selectedAttributeValues.Count > 0);


                model.SelectedCombination = _productAttributeParser.FindProductVariantAttributeCombination(article.Id, attributeXml);

                if (model.SelectedCombination != null && model.SelectedCombination.IsActive == false)
                {
                    model.IsAvailable = false;
                    model.StockAvailability = T("Products.Availability.IsNotActive");
                }

                article.MergeWithCombination(model.SelectedCombination);

                // mark explicitly selected as pre-selected
                foreach (var attribute in model.ProductVariantAttributes)
                {
                    foreach (var value in attribute.Values)
                    {
                        if (selectedAttributeValues.FirstOrDefault(v => v.Id == value.Id) != null)
                            value.IsPreSelected = true;

                        if (!_catalogSettings.ShowVariantCombinationPriceAdjustment)
                            value.PriceAdjustment = "";
                    }
                }
            }


            #endregion

            #region 产品属性

            model.WeightValue = article.Weight;

            if (selectedAttributeValues != null)
            {
                foreach (var attributeValue in selectedAttributeValues)
                    model.WeightValue = decimal.Add(model.WeightValue, attributeValue.WeightAdjustment);
            }
            else
            {
                model.WeightValue = decimal.Add(model.WeightValue, preSelectedWeightAdjustment);
            }

            model.Weight = (article.Weight > 0) ? "{0} {1}".FormatCurrent(article.Weight.ToString("F2")) : "";
            model.Height = (article.Height > 0) ? "{0} {1}".FormatCurrent(article.Height.ToString("F2")) : "";
            model.Length = (article.Length > 0) ? "{0} {1}".FormatCurrent(article.Length.ToString("F2")) : "";
            model.Width = (article.Width > 0) ? "{0} {1}".FormatCurrent(article.Width.ToString("F2")) : "";

            model.DisplayDeliveryTime = _catalogSettings.ShowDeliveryTimesInProductDetail;
            if (model.DisplayDeliveryTime)
            {
                var deliveryTime = _deliveryTimeService.GetDeliveryTime(article);
                if (deliveryTime != null)
                {
                    model.DeliveryTimeName = deliveryTime.GetLocalized(x => x.Name);
                    model.DeliveryTimeHexValue = deliveryTime.ColorHexValue;
                }
            }

            //back in stock subscriptions)
            model.StockQuantity = article.StockQuantity;
            #endregion




            if (preparePriceModel)
            {
                #region 商品价格

                model.ProductPrice.ProductId = article.Id;

                if (displayPrices)
                {
                    model.ProductPrice.HidePrices = false;

                    if (article.CustomerEntersPrice)
                    {
                        model.ProductPrice.CustomerEntersPrice = true;
                    }
                    else
                    {

                        decimal oldPrice = decimal.Zero;
                        decimal finalPriceWithoutDiscountBase = decimal.Zero;
                        decimal attributesTotalPriceBase = decimal.Zero;
                        //没有折扣的价格
                        decimal finalPriceWithoutDiscount = decimal.Zero;
                        //折扣价
                        decimal finalPriceWithDiscount = decimal.Zero;

                        decimal oldPriceBase = article.OldPrice;


                        if (selectedAttributeValues != null)
                        {
                            selectedAttributeValues.Each(x => attributesTotalPriceBase += _priceCalculationService.GetProductVariantAttributeValuePriceAdjustment(x));
                        }
                        else
                        {
                            attributesTotalPriceBase = preSelectedPriceAdjustmentBase;
                        }


                        //基本价格
                        finalPriceWithoutDiscountBase = _priceCalculationService.GetFinalPrice(article,
                                            customer, attributesTotalPriceBase, false, selectedQuantity);
                        //可后续加入汇率
                        finalPriceWithoutDiscount = finalPriceWithoutDiscountBase;
                        //可后续加入汇率
                        oldPrice = oldPriceBase;

                        model.ProductPrice.PriceValue = finalPriceWithoutDiscount;
                        model.ProductPrice.PriceWithDiscountValue = finalPriceWithDiscount;

                    }

                }
                else
                {
                    model.ProductPrice.HidePrices = true;
                    model.ProductPrice.OldPrice = null;
                    model.ProductPrice.Price = null;
                }
                #endregion
            }


            #region 上一页下一页
            if (preparePrevAndNext)
            {
                var prevArticle = _articleService.GetPreArticleById(article.Id, article.CategoryId ?? 0);
                var nextArticle = _articleService.GetNextArticleById(article.Id, article.CategoryId ?? 0);
                var articlePreAndNextModel = new ArticlePostModel.ArticlePrevAndNextModel();
                if (prevArticle != null)
                {
                    articlePreAndNextModel.PrevId = prevArticle.Id;
                    articlePreAndNextModel.PrevSeName = prevArticle.GetSeName();
                    articlePreAndNextModel.PrevTitle = prevArticle.Title;
                }
                else
                {
                    articlePreAndNextModel.PrevId = null;
                    articlePreAndNextModel.PrevTitle = "没有了";
                }
                if (nextArticle != null)
                {
                    articlePreAndNextModel.NextId = nextArticle.Id;
                    articlePreAndNextModel.NextSeName = nextArticle.GetSeName();
                    articlePreAndNextModel.NextTitle = nextArticle.Title;
                }
                else
                {
                    articlePreAndNextModel.NextId = null;
                    articlePreAndNextModel.NextTitle = "没有了";
                }
                model.ArticlePNModel = articlePreAndNextModel;
            }
            #endregion

            if (preparePictureModel)
            {
                // 默认缩略图
                if (article.PictureId.HasValue)
                {
                    model.DefaultPictureModel = new PictureModel()
                    {
                        PictureId = article.PictureId.GetValueOrDefault(),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(article.PictureId.GetValueOrDefault()),
                        ImageUrl = _pictureService.GetPictureUrl(article.PictureId.GetValueOrDefault(), _mediaSettings.DetailsPictureSize),
                        Title = string.Format(T("Media.Article.ImageLinkTitleFormat"), article.Title),
                        AlternateText = string.Format(T("Media.Article.ImageAlternateTextFormat"), article.Title)
                    };
                }
                // 获取图片
                var pictures = _pictureService.GetPicturesByArticleId(article.Id);
                PrepareArticleDetailsPictureModel(model.DetailsPictureModel, pictures, model.Title);
                //如果缩略图空，则默认图片集
                if (model.DefaultPictureModel == null)
                    model.DefaultPictureModel = model.DetailsPictureModel.DefaultPictureModel;
            }
            //文档扩展属性
            if (prepareArticleExtendedAttributes)
                PrerpareArticleExtendedAttributes(model, article);

            PrerpareArticleVendor(model);
            return model;
        }
        /// <summary>
        /// 店铺信息
        /// </summary>
        /// <param name="model"></param>
        public void PrerpareArticleVendor(ArticlePostModel model)
        {
            #region 店铺信息
            var vendor = _vendorService.GetVendorById(model.VendorId);
            if (vendor != null)
            {
                var vendorModel = vendor.ToModel();
                vendorModel.LogoPictureUrl = _pictureService.GetPictureUrl(vendor.PictureId.GetValueOrDefault());
                model.VendorModel = vendorModel;
                model.VendorModel.UserId = this._userService.GetUserByVendor(model.VendorId).Id;
            }
            else
            {
                model.VendorModel.LogoPictureUrl = _pictureService.GetDefaultPictureUrl(198);
            }
            #endregion
        }

        /// <summary>
        /// 内容点赞评论组装
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        public void PrepareArticleReviewsModel(ArticleReviewsModel model, Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (model == null)
                throw new ArgumentNullException("model");

            model.ArticleId = article.Id;
            //model.ArticleName = article.GetLocalized(x => x.Name);
            //model.ArticleSeName = article.GetSeName();

            var articleReviews = article.ArticleReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc).FirstOrDefault();
            if (articleReviews != null)
            {
                model.Item = new ArticleReviewModel()
                {
                    Id = articleReviews.Id,
                    UserId = articleReviews.UserId.Value,
                    UserName = articleReviews.User.FormatUserName(),
                    AllowViewingProfiles = _userSettings.AllowViewingProfiles && articleReviews.User != null && !articleReviews.User.IsGuest(),
                    Rating = articleReviews.Rating,
                    Helpfulness = new ArticleReviewHelpfulnessModel()
                    {
                        ArticleReviewId = articleReviews.Id,
                        HelpfulYesTotal = articleReviews.HelpfulYesTotal,
                        HelpfulNoTotal = articleReviews.HelpfulNoTotal,
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(articleReviews.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                };

            }

            model.AddArticleReview.CanCurrentUserLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewArticle || !_services.WorkContext.CurrentUser.IsGuest();
            //验证码
            // model.AddArticleReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnArticleReviewPage;
        }

        /// <summary>
        /// 获取扩展信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        private void PrerpareArticleExtendedAttributes(ArticlePostModel model, Article article)
        {
            if (model != null)
            {
                #region Extended attributes
                var channel = _channelService.GetChannelById(article.ChannelId);
                if (channel != null)
                {

                    var extendedAttributes = channel.ExtendedAttributes;

                    foreach (var attribute in extendedAttributes)
                    {
                        var caModel = new ArticlePostModel.ArticleExtendedAttributeModel()
                        {
                            Id = attribute.Id,
                            Name = attribute.GetLocalized(x => x.Title),
                            TextPrompt = attribute.GetLocalized(x => x.TextPrompt),
                            IsRequired = attribute.IsRequired,
                            AttributeControlType = attribute.AttributeControlType,
                            AllowedFileExtensions = _catalogSettings.FileUploadAllowedExtensions
                        };

                        if (attribute.ShouldHaveValues())
                        {
                            //values
                            var caValues = _extendedAttributeService.GetExtendedAttributeValues(attribute.Id);
                            foreach (var caValue in caValues)
                            {
                                var pvaValueModel = new ArticlePostModel.ArticleExtendedAttributeValueModel()
                                {
                                    Id = caValue.Id,
                                    Name = caValue.GetLocalized(x => x.Name),
                                    IsPreSelected = caValue.IsPreSelected
                                };
                                caModel.Values.Add(pvaValueModel);

                            }
                        }

                        //set already selected attributes
                        string selectedExtendedAttributes = article == null ? "" : article.GetArticleAttribute<string>(ArticleAttributeNames.ExtendedAttributes, _articleAttributeService);
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlType.DropdownList:
                            case AttributeControlType.RadioList:
                            case AttributeControlType.Checkboxes:
                                {
                                    if (!String.IsNullOrEmpty(selectedExtendedAttributes))
                                    {
                                        //clear default selection
                                        foreach (var item in caModel.Values)
                                            item.IsPreSelected = false;

                                        //select new values
                                        var selectedCaValues = _extendedAttributeParser.ParseExtendedAttributeValues(selectedExtendedAttributes);
                                        foreach (var caValue in selectedCaValues)
                                            foreach (var item in caModel.Values)
                                                if (caValue.Id == item.Id)
                                                {
                                                    item.IsPreSelected = true;
                                                    caModel.DefaultValue = item.Name;
                                                }
                                    }
                                }
                                break;
                            case AttributeControlType.ColorSquares:
                            case AttributeControlType.TextBox:
                            case AttributeControlType.MultilineTextbox:
                            case AttributeControlType.VideoUpload:
                            case AttributeControlType.FileUpload:
                                {
                                    if (!String.IsNullOrEmpty(selectedExtendedAttributes))
                                    {
                                        var enteredText = _extendedAttributeParser.ParseValues(selectedExtendedAttributes, attribute.Id);
                                        if (enteredText.Count > 0)
                                            caModel.DefaultValue = enteredText[0];
                                    }
                                }
                                break;
                            case AttributeControlType.Datepicker:
                                {
                                    //keep in mind my that the code below works only in the current culture
                                    var selectedDateStr = _extendedAttributeParser.ParseValues(selectedExtendedAttributes, attribute.Id);
                                    if (selectedDateStr.Count > 0)
                                    {
                                        DateTime selectedDate;
                                        if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                               DateTimeStyles.None, out selectedDate))
                                        {
                                            //successfully parsed
                                            caModel.SelectedDay = selectedDate.Day;
                                            caModel.SelectedMonth = selectedDate.Month;
                                            caModel.SelectedYear = selectedDate.Year;
                                        }
                                    }

                                }
                                break;
                            default:
                                break;
                        }

                        model.ArticleExtendedAttributes.Add(caModel);
                    }
                }

                #endregion
            }
        }
        /// <summary>
        /// 新建图片模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="picture"></param>
        /// <param name="pictureSize"></param>
        /// <returns></returns>
        private PictureModel CreatePictureModel(ArticleDetailsPictureModel model, Picture picture, int pictureSize)
        {
            var result = new PictureModel()
            {
                PictureId = picture.Id,
                ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ThumbPictureSizeOnDetailsPage),
                ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                Title = model.Name,
                AlternateText = model.AlternateText
            };

            return result;
        }
        /// <summary>
        /// 内容图片相册
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dpModel"></param>
        /// <param name="pictures"></param>
        /// <param name="name"></param>
        public void PrepareArticleDetailsPictureModel(
            ArticleDetailsPictureModel dpModel,
            IList<Picture> pictures,
            string name,
            ProductVariantAttributeCombination combination = null)
        {
            dpModel.Name = name;
            dpModel.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            dpModel.PictureZoomType = _mediaSettings.PictureZoomType;
            dpModel.AlternateText = T("Media.Article.ImageAlternateTextFormat", dpModel.Name);

            Picture defaultPicture = null;
            int defaultPictureSize;
            var combiAssignedImages = (combination == null ? null : combination.GetAssignedPictureIds());

            defaultPictureSize = _mediaSettings.DetailsPictureSize;

            if (pictures.Count > 0)
            {
                if (pictures.Count <= _catalogSettings.DisplayAllImagesNumber)
                {
                    // show all images
                    foreach (var picture in pictures)
                    {
                        dpModel.PictureModels.Add(CreatePictureModel(dpModel, picture, _mediaSettings.DetailsPictureSize));

                        if (defaultPicture == null && combiAssignedImages != null && combiAssignedImages.Contains(picture.Id))
                        {
                            dpModel.GalleryStartIndex = dpModel.PictureModels.Count - 1;
                            defaultPicture = picture;
                        }
                    }
                }
                else
                {
                    // images not belonging to any combination...
                    foreach (var picture in pictures)
                    {
                        dpModel.PictureModels.Add(CreatePictureModel(dpModel, picture, _mediaSettings.DetailsPictureSize));
                    }
                    // plus images belonging to selected combination
                    if (combiAssignedImages != null)
                    {
                        foreach (var picture in pictures.Where(p => combiAssignedImages.Contains(p.Id)))
                        {
                            dpModel.PictureModels.Add(CreatePictureModel(dpModel, picture, _mediaSettings.DetailsPictureSize));

                            if (defaultPicture == null)
                            {
                                dpModel.GalleryStartIndex = dpModel.PictureModels.Count - 1;
                                defaultPicture = picture;
                            }
                        }
                    }
                }

                if (defaultPicture == null)
                {
                    dpModel.GalleryStartIndex = 0;
                    defaultPicture = pictures.First();
                }
            }

            // default picture
            if (dpModel.DefaultPictureModel == null)
            {
                if (defaultPicture == null)
                {
                    dpModel.DefaultPictureModel = new PictureModel()
                    {
                        ThumbImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.ThumbPictureSizeOnDetailsPage),
                        ImageUrl = _pictureService.GetDefaultPictureUrl(defaultPictureSize),
                        FullSizeImageUrl = _pictureService.GetDefaultPictureUrl(),
                        Title = T("Media.Article.ImageLinkTitleFormat", dpModel.Name),
                        AlternateText = dpModel.AlternateText
                    };
                }
                else
                {
                    dpModel.DefaultPictureModel = CreatePictureModel(dpModel, defaultPicture, defaultPictureSize);
                }
            }
        }



        #region 分类

        /// <summary>
        /// 内容模型组装(分页)，主要用于列表页
        /// </summary>
        /// <param name="articles"></param>
        /// <param name="preparePictureModel"></param>
        /// <param name="prepareTagModel"></param>
        /// <param name="articleThumbPictureSize"></param>
        /// <returns></returns>
        public IEnumerable<ArticleOverviewModel> PrepareArticlePostModels(
        IEnumerable<Article> articles,
        bool preparePictureModel = true,
        bool prepareTagModel = false,
        Size? articleThumbPictureSize = null,
        bool preparePriceModel = true,
        bool prepareFullDescription = false,
        bool prepareColorAttributes = false,
        bool prepareManufacturers = false,
        bool prepareSpecificationAttributes = false,
        bool prepareArticleExtendedAttributes = false)
        {
            if (articles == null)
                throw new ArgumentNullException("article");

            // PERF!!
            var displayPrices = _services.Permissions.Authorize(StandardPermissionProvider.DisplayPrices);
            var currentUser = _services.WorkContext.CurrentUser;
            var cachedManufacturerModels = new Dictionary<int, ManufacturerOverviewModel>();

            //  var taxDisplayType = _services.WorkContext.GetTaxDisplayTypeFor(currentUser, _services.SiteContext.CurrentSite.Id);
            var res = new Dictionary<string, LocalizedString>(StringComparer.OrdinalIgnoreCase)
                {

                    { "Media.Article.ImageLinkTitleFormat", T("Media.Article.ImageLinkTitleFormat") },
                    { "Media.Article.ImageAlternateTextFormat", T("Media.Article.ImageAlternateTextFormat") }

                };

            var models = new List<ArticleOverviewModel>();

            foreach (var article in articles)
            {


                var model = new ArticleOverviewModel
                {
                    Id = article.Id,
                    Title = article.GetLocalized(x => x.Title).EmptyNull(),
                    ShortContent = article.GetLocalized(x => x.ShortContent),
                    SeName = article.GetSeName(),
                    IsHot = article.IsHot,
                    IsRed = article.IsRed,
                    IsSlide = article.IsSlide,
                    IsTop = article.IsTop,
                    Author = article.Author,
                    Sku = article.Sku,
                    ChannelId = article.ChannelId,
                    CategroyId = article.CategoryId ?? 0,
                    ProductCategroyId = article.ProductCategoryId ?? 0,
                };
                if (article.Weight > 0)
                {
                    model.Weight = "{0}".FormatCurrent(article.Weight.ToString("F2"));
                }
                if (prepareFullDescription)
                {
                    model.FullContent = article.GetLocalized(x => x.FullContent);
                }
                // 显示价格
                if (preparePriceModel)
                {
                    #region Prepare article price

                    var priceModel = new ArticleOverviewModel.ProductPriceModel();

                    #region Simple article

                    //add to cart button
                    priceModel.DisableBuyButton = article.DisableBuyButton || !displayPrices;


                    //价格
                    if (displayPrices && !article.CustomerEntersPrice)
                    {

                        //calculate prices
                        decimal displayPrice = decimal.Zero;
                        displayPrice = _priceCalculationService.GetFinalPrice(article, currentUser, decimal.Zero, false, 1);

                        //可扩展加入税收及汇率
                        decimal oldPriceBase = article.OldPrice;
                        decimal finalPriceBase = displayPrice;


                        priceModel.HasDiscount = (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero);


                        if (priceModel.HasDiscount)
                        {
                            priceModel.OldPrice = oldPriceBase.ToString();
                            priceModel.Price = finalPriceBase.ToString();
                        }
                        else
                        {
                            priceModel.OldPrice = null;
                            priceModel.Price = finalPriceBase.ToString();
                        }

                    }

                    #endregion

                    model.ProductPrice = priceModel;

                    #endregion
                }

                // 图片
                if (preparePictureModel)
                {
                    #region Prepare article picture

                    //If a size has been set in the view, we use it in priority
                    int pictureSizeWidth = articleThumbPictureSize.HasValue ? articleThumbPictureSize.Value.Width : _mediaSettings.ArticleThumbPictureSize;
                    int pictureSizeHeight = articleThumbPictureSize.HasValue ? articleThumbPictureSize.Value.Height : _mediaSettings.ArticleThumbPictureSize;

                    //prepare picture model
                    var defaultArticlePictureCacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_DEFAULTPICTURE_MODEL_KEY, article.Id, pictureSizeWidth, true,
                        _services.WorkContext.WorkingLanguage.Id, _services.WebHelper.IsCurrentConnectionSecured(), _services.SiteContext.CurrentSite.Id);

                    model.DefaultPictureModel = _services.Cache.Get(defaultArticlePictureCacheKey, () =>
                    {
                        if (article.PictureId.HasValue)
                        {
                            var pictureModel = new PictureModel()
                            {
                                PictureId = article.PictureId.GetValueOrDefault(),
                                FullSizeImageUrl = _pictureService.GetPictureUrl(article.PictureId.GetValueOrDefault()),
                                ImageUrl = _pictureService.GetPictureUrl(article.PictureId.GetValueOrDefault(), _mediaSettings.DetailsPictureSize),
                                Title = string.Format(T("Media.Article.ImageLinkTitleFormat"), article.Title),
                                AlternateText = string.Format(T("Media.Article.ImageAlternateTextFormat"), article.Title)
                            };
                            return pictureModel;
                        }
                        else
                        {
                            var picture = article.GetDefaultArticlePicture(_pictureService);
                            var pictureModel = new PictureModel
                            {
                                ImageUrl = _pictureService.GetPictureUrl(picture, pictureSizeWidth, pictureSizeHeight),
                                FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                                Title = string.Format(res["Media.Article.ImageLinkTitleFormat"], model.Title),
                                AlternateText = string.Format(res["Media.Article.ImageAlternateTextFormat"], model.Title),
                                IsDefault = picture == null

                            };
                            return pictureModel;
                        }

                    });
                    // pictures
                    var pictures = _pictureService.GetPicturesByArticleId(article.Id);
                    PrepareArticleDetailsPictureModel(model.DetailsPictureModel, pictures, model.Title);
                    #endregion
                }

                //标签
                if (prepareTagModel)
                {
                    var cacheKey = string.Format(ModelCacheEventConsumer.ARTICLETAG_BY_ARTICLE_MODEL_KEY, article.Id, _services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id);
                    model.Tags = _services.Cache.Get(cacheKey, () =>
                    {
                        var modelTags = article.ArticleTags
                            //filter by store
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
                        return modelTags;
                    });

                }
                // IsNew
                if (_catalogSettings.LabelAsNewForMaxDays.HasValue)
                {
                    model.IsNew = (DateTime.UtcNow - article.CreatedOnUtc).Days <= _catalogSettings.LabelAsNewForMaxDays.Value;
                }
                if (_catalogSettings.RelativeDateTimeFormattingEnabled)
                    model.PostCreatedOnStr = article.CreatedOnUtc.RelativeFormat(true, "f");
                else
                    model.PostCreatedOnStr = _dateTimeHelper.ConvertToUserTime(article.CreatedOnUtc, DateTimeKind.Utc).ToString("f");

                model.DisplayDeliveryTime = _catalogSettings.ShowDeliveryTimesInProductDetail;
                if (model.DisplayDeliveryTime)
                {
                    var deliveryTime = _deliveryTimeService.GetDeliveryTime(article);
                    if (deliveryTime != null)
                    {
                        model.DeliveryTimeName = deliveryTime.GetLocalized(x => x.Name);
                        model.DeliveryTimeHexValue = deliveryTime.ColorHexValue;
                    }
                }


                models.Add(model);
            }
            return models;
        }


        public List<ManufacturerOverviewModel> PrepareManufacturersOverviewModel(
          ICollection<Manufacturer> manufacturers,
          IDictionary<int, ManufacturerOverviewModel> cachedModels = null)
        {
            var model = new List<ManufacturerOverviewModel>();

            if (cachedModels == null)
            {
                cachedModels = new Dictionary<int, ManufacturerOverviewModel>();
            }

            foreach (var pm in manufacturers)
            {
                var manufacturer = pm;
                ManufacturerOverviewModel item;

                if (!cachedModels.TryGetValue(manufacturer.Id, out item))
                {
                    item = new ManufacturerOverviewModel
                    {
                        Id = manufacturer.Id,
                        Name = manufacturer.Name,
                        Description = manufacturer.Description,
                        SeName = manufacturer.GetSeName()

                    };

                    item.PictureModel = PrepareManufacturerPictureModel(manufacturer, manufacturer.GetLocalized(x => x.Name));

                    cachedModels.Add(item.Id, item);
                }

                model.Add(item);
            }

            return model;
        }

        public IList<ProductSpecificationModel> PrepareProductSpecificationModel(Article product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string cacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_SPECS_MODEL_KEY, product.Id, _services.WorkContext.WorkingLanguage.Id);
            return _services.Cache.Get(cacheKey, () =>
            {
                var model = _specificationAttributeService.GetArticleSpecificationAttributesByArticleId(product.Id, null, true)
                   .Select(psa =>
                   {
                       return new ProductSpecificationModel
                       {
                           SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                           SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(x => x.Name),
                           SpecificationAttributeOption = psa.SpecificationAttributeOption.GetLocalized(x => x.Name)
                       };
                   }).ToList();
                return model;
            });
        }

        public PictureModel PrepareManufacturerPictureModel(Manufacturer manufacturer, string localizedName)
        {
            var model = new PictureModel();

            int pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
            var manufacturerPictureCacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_PICTURE_MODEL_KEY,
                manufacturer.Id,
                pictureSize,
                true,
                _services.WorkContext.WorkingLanguage.Id,
                _services.WebHelper.IsCurrentConnectionSecured(),
                _services.SiteContext.CurrentSite.Id);

            model = _services.Cache.Get(manufacturerPictureCacheKey, () =>
            {
                var pictureModel = new PictureModel
                {
                    PictureId = manufacturer.PictureId.GetValueOrDefault(),
                    //FullSizeImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId.GetValueOrDefault()),
                    ImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId.GetValueOrDefault(), pictureSize),
                    Title = string.Format(T("Media.Manufacturer.ImageLinkTitleFormat"), localizedName),
                    AlternateText = string.Format(T("Media.Manufacturer.ImageAlternateTextFormat"), localizedName)
                };
                return pictureModel;
            });

            return model;
        }

        /// <summary>
        /// 获取每个分类的文章数
        /// </summary>
        /// <param name="curNode">分类列表</param>
        protected void ResolveCategoryArticlesCount(TreeNode<MenuItem> curNode)
        {
            try
            {
                // Perf: only resolve counts for categories in the current path.
                while (curNode != null)
                {
                    if (curNode.Children.Any(x => !x.Value.ElementsCount.HasValue))
                    {
                        lock (s_lock)
                        {
                            if (curNode.Children.Any(x => !x.Value.ElementsCount.HasValue))
                            {
                                foreach (var node in curNode.Children)
                                {
                                    var categoryIds = new List<int>();

                                    if (_catalogSettings.ShowCategoryArticleNumberIncludingSubcategories)
                                    {
                                        // include subcategories
                                        node.TraverseTree(x => categoryIds.Add(x.Value.EntityId));
                                    }
                                    else
                                    {
                                        categoryIds.Add(node.Value.EntityId);
                                    }

                                    var ctx = new ArticleSearchContext();
                                    ctx.CategoryIds = categoryIds;
                                    ctx.SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode;
                                    node.Value.ElementsCount = _articleService.CountArticles(ctx);
                                }
                            }
                        }
                    }

                    curNode = curNode.Parent;
                }
            }
            catch (Exception exc)
            {
                Logger.Error(exc.Message, exc);
            }
        }

        /// <summary>
        /// 配置筛选条件
        /// </summary>
        /// <param name="model">当前分页条件</param>
        /// <param name="command">分页查询条件</param>
        /// <param name="pageSizeContext">分页内容</param>
        public void PreparePagingFilteringModel(PagingFilteringModel model, PagingFilteringModel command, PageSizeContext pageSizeContext)
        {
            //sorting
            model.AllowSorting = _catalogSettings.AllowArticleSorting;
            if (model.AllowSorting)
            {
                model.OrderBy = command.OrderBy;

                foreach (ArticleSortingEnum enumValue in Enum.GetValues(typeof(ArticleSortingEnum)))
                {

                    var currentPageUrl = _services.WebHelper.GetThisPageUrl(true);
                    var sortUrl = _services.WebHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);

                    var sortValue = enumValue.GetLocalizedEnum(_localizationService, _services.WorkContext);
                    model.AvailableSortOptions.Add(new ListOptionItem()
                    {
                        Text = sortValue,
                        Url = sortUrl,
                        Selected = enumValue == (ArticleSortingEnum)command.OrderBy,
                        Id = (int)enumValue
                    });
                }
            }

            //view mode
            model.AllowViewModeChanging = _catalogSettings.AllowArticleViewModeChanging;
            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                            ? command.ViewMode
                            : _catalogSettings.DefaultViewMode;

            model.ViewMode = viewMode;

            if (model.AllowViewModeChanging)
            {
                var currentPageUrl = _services.WebHelper.GetThisPageUrl(true);
                //grid
                model.AvailableViewModes.Add(new ListOptionItem()
                {
                    Text = T("Categories.ViewMode.Grid"),
                    Url = _services.WebHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid",
                    ExtraData = "grid"
                });
                //list
                model.AvailableViewModes.Add(new ListOptionItem()
                {
                    Text = T("Categories.ViewMode.List"),
                    Url = _services.WebHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list",
                    ExtraData = "list"
                });

            }

            //page size
            model.AllowUsersToSelectPageSize = false;
            if (pageSizeContext.AllowUsersToSelectPageSize && pageSizeContext.PageSizeOptions.IsEmpty())
            {
                pageSizeContext.PageSizeOptions = _catalogSettings.DefaultPageSizeOptions; // "12, 18, 36, 72, 150";

            }
            if (pageSizeContext.AllowUsersToSelectPageSize && pageSizeContext.PageSizeOptions.HasValue())
            {
                var pageSizes = pageSizeContext.PageSizeOptions.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if user enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp = 0;

                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _services.WebHelper.GetThisPageUrl(true);
                    var sortUrl = _services.WebHelper.ModifyQueryString(currentPageUrl, "pagesize=__pagesize__", null);
                    sortUrl = _services.WebHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp = 0;
                        if (!int.TryParse(pageSize, out temp) || temp <= 0)
                        {
                            continue;
                        }

                        model.PageSizeOptions.Add(new ListOptionItem()
                        {
                            Text = pageSize,
                            Url = sortUrl.Replace("__pagesize__", pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (model.PageSizeOptions.Any())
                    {
                        model.PageSizeOptions = model.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        model.AllowUsersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(model.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }
            else
            {
                //user is not allowed to select a page size
                command.PageSize = pageSizeContext.PageSize;
            }

            if (command.PageSize <= 0)
                command.PageSize = pageSizeContext.PageSize;
        }
        #region 栏目分类


        /// <summary>
        /// 获取分类导航
        /// </summary>
        /// <param name="currentCategoryId">当前分类ID</param>
        /// <param name="currentArticleId">当前内容ID</param>
        /// <param name="onlyShowChildren">只显示当前分类，不显示全分类导航</param>
        /// <param name="showParent">显示父级分类</param>
        /// <returns></returns>
        public NavigationModel PrepareCategoryNavigationModel(int currentCategoryId, int currentArticleId, bool onlyShowChildren = false, bool showParent = false)
        {
            var root = GetCategoryMenu();

            IList<MenuItem> breadcrumb = GetCategoryBreadCrumb(currentCategoryId, currentArticleId);
            var curItem = breadcrumb.LastOrDefault();

            if (onlyShowChildren && curItem != null)
            {
                root = root.SelectNode(x => x.Value.EntityId == curItem.EntityId);

            }
            if (showParent && curItem != null)
            {
                if (root.Depth > 0)
                {
                    root.TraverseParentTree(x =>
                    {
                        if (x.Depth == 0)
                            root = x;
                    });
                    breadcrumb = GetProductCategoryBreadCrumb(curItem.EntityId, currentArticleId);
                }

            }


            // resolve number of articles
            if (_catalogSettings.ShowCategoryArticleNumber)
            {

                var curNode = curItem == null ? root.Root : root.SelectNode(x => x.Value.EntityId == curItem.EntityId);

                this.ResolveCategoryArticlesCount(curNode);
            }

            var model = new NavigationModel
            {
                Root = root,
                Path = breadcrumb,

            };

            return model;
        }

        /// <summary>
        /// 获取分类导航
        /// </summary>
        /// <param name="channelId">当前频道ID</param>
        /// <returns></returns>
        public NavigationModel PrepareCategoryModels(int channelId,int? currentCategoryId, int? currentArticleId)
        {
            var root = GetCategoryMenu();
            IList<MenuItem> breadcrumb = GetCategoryBreadCrumb(currentCategoryId??0, currentArticleId ?? 0);
            var curItem = breadcrumb.LastOrDefault();
            var  childrens = root.SelectNodes(x => x.Value.ChannelId == channelId).ToList();

           
            var model = new NavigationModel
            {
                FildMenu = childrens,
                Path = breadcrumb,
                Root= root,
            };

            return model;
        }



        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        public TreeNode<MenuItem> GetCategoryMenu()
        {
            var userRolesIds = _services.WorkContext.CurrentUser.UserRoles.Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NAVIGATION_MODEL_KEY,
                _services.WorkContext.WorkingLanguage.Id,
                string.Join(",", userRolesIds),
                _services.SiteContext.CurrentSite.Id);

            var model = _services.Cache.Get(cacheKey, () =>
            {
                var curParent = new TreeNode<MenuItem>(new MenuItem
                {
                    EntityId = 0,
                    Text = "Home",
                    RouteName = "HomePage"
                });

                ArticleCategory prevCat = null;

                var categories = _articleCategoryService.GetAllCategories();
                foreach (var category in categories)
                {
                    var menuItem = new MenuItem
                    {
                        EntityId = category.Id,
                        ChannelId = category.ChannelId,
                        Text = category.GetLocalized(x => x.Name),
                        RouteName = "ArticleCategory"
                    };
                    menuItem.RouteValues.Add("SeName", category.GetSeName());

                    // determine parent
                    if (prevCat != null)
                    {
                        if (category.ParentCategoryId != curParent.Value.EntityId)
                        {
                            if (category.ParentCategoryId == prevCat.Id)
                            {
                                // level +1
                                curParent = curParent.LastChild;
                            }
                            else
                            {
                                // level -x
                                while (!curParent.IsRoot)
                                {
                                    if (curParent.Value.EntityId == category.ParentCategoryId)
                                    {
                                        break;
                                    }
                                    curParent = curParent.Parent;
                                }
                            }
                        }
                    }

                    // add to parent
                    curParent.Append(menuItem);

                    prevCat = category;
                }

                var root = curParent.Root;

                // menu publisher
                _menuPublisher.Value.RegisterMenus(root, "articlecategory");

                // event
                _services.EventPublisher.Publish(new NavigationModelBuiltEvent(root));

                return root;
            });

            return model;
        }

        /// <summary>
        /// 获取分类导航列表
        /// </summary>
        /// <param name="currentCategoryId">当前分类ID</param>
        /// <param name="currentArticleId">当前内容ID</param>
        /// <param name="OnlyCurrentCategory">只显示当前分类，不显示全分类导航</param>
        /// <returns></returns>
        public IList<MenuItem> GetCategoryBreadCrumb(int currentCategoryId, int currentArticleId, bool OnlyCurrentCategory = false)
        {
            var requestCache = CAF.Infrastructure.Core.EngineContext.Current.Resolve<ICacheManager>();
            string cacheKey = "caf.temp.category.path.{0}-{1}".FormatInvariant(currentCategoryId, currentArticleId);

            var breadcrumb = requestCache.Get(cacheKey, () =>
            {
                var root = GetCategoryMenu();
                TreeNode<MenuItem> node = null;

                if (currentCategoryId > 0)
                {
                    node = root.SelectNode(x => x.Value.EntityId == currentCategoryId);
                }

                if (node == null && currentArticleId > 0)
                {
                    var articleCategories = _articleCategoryService.GetArticleCategoriesByArticleId(currentArticleId);
                    if (articleCategories != null)
                    {
                        currentCategoryId = articleCategories.Id;
                        node = root.SelectNode(x => x.Value.EntityId == currentCategoryId);
                    }
                }

                if (node != null)
                {
                    IList<MenuItem> path;
                    if (OnlyCurrentCategory)
                        path = node.GetCurrentBreadcrumb();
                    else
                        path = node.GetBreadcrumb();
                    return path;
                }

                return new List<MenuItem>();
            });

            return breadcrumb;

        }
        /// <summary>
        /// 根据父级ID获取所有下级分类ID
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <returns></returns>
        public List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var userRolesIds = _services.WorkContext.CurrentUser.UserRoles.Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                false,
                string.Join(",", userRolesIds),
                _services.SiteContext.CurrentSite.Id);

            return _services.Cache.Get(cacheKey, () =>
            {
                var root = GetCategoryMenu();
                var node = root.SelectNode(x => x.Value.EntityId == parentCategoryId);
                if (node != null)
                {
                    var ids = node.Flatten(false).Select(x => x.EntityId).ToList();
                    return ids;
                }
                return new List<int>();
            });
        }
        /// <summary>
        /// 根据父级ID获取所有下级分类
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <returns></returns>
        public IList<MenuItem> GetChildCategorys(int parentCategoryId)
        {
            var userRolesIds = _services.WorkContext.CurrentUser.UserRoles.Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_ITEMS_MODEL_KEY,
                parentCategoryId,
                false,
                string.Join(",", userRolesIds),
                _services.SiteContext.CurrentSite.Id);

            return _services.Cache.Get(cacheKey, () =>
            {
                var root = GetCategoryMenu();
                var node = root.SelectNode(x => x.Value.EntityId == parentCategoryId);
                if (node != null)
                {
                    var categorys = node.Flatten(false).ToList();
                    return categorys;
                }
                return new List<MenuItem>();
            });
        }

        #endregion

        #region 商品分类
        /// <summary>
        /// 获取分类导航
        /// </summary>
        /// <param name="currentCategoryId">当前分类ID</param>
        /// <param name="currentArticleId">当前内容ID</param>
        /// <param name="onlyShowChildren">只显示当前分类，不显示全分类导航</param>
        /// <returns></returns>
        public NavigationModel PrepareProductCategoryNavigationModel(int currentCategoryId, int currentArticleId, bool onlyShowChildren = false, bool showParent = false)
        {
            var root = GetProductCategoryMenu();
            IList<MenuItem> breadcrumb = GetProductCategoryBreadCrumb(currentCategoryId, currentArticleId);
            var curItem = breadcrumb.LastOrDefault();

            if (onlyShowChildren && curItem != null)
            {
                root = root.SelectNode(x => x.Value.EntityId == curItem.EntityId);

            }
            if (showParent && curItem != null)
            {
                if (root.Depth > 0)
                {
                    root.TraverseParentTree(x =>
                    {
                        if (x.Depth == 0)
                            root = x;
                    });
                    breadcrumb = GetProductCategoryBreadCrumb(curItem.EntityId, currentArticleId);
                }

            }
            var model = new NavigationModel
            {
                Root = root,
                Path = breadcrumb,

            };

            return model;
        }



        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        public TreeNode<MenuItem> GetProductCategoryMenu()
        {
            var userRolesIds = _services.WorkContext.CurrentUser.UserRoles.Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_CATEGORY_NAVIGATION_MODEL_KEY,
                _services.WorkContext.WorkingLanguage.Id,
                string.Join(",", userRolesIds),
                _services.SiteContext.CurrentSite.Id);

            var model = _services.Cache.Get(cacheKey, () =>
            {
                var curParent = new TreeNode<MenuItem>(new MenuItem
                {
                    EntityId = 0,
                    Text = "Home",
                    RouteName = "HomePage"
                });

                ProductCategory prevCat = null;

                var categories = _productCategoryService.GetAllCategories();
                foreach (var category in categories)
                {
                    var menuItem = new MenuItem
                    {
                        EntityId = category.Id,
                        Text = category.GetLocalized(x => x.Name),
                        RouteName = "ProductCategory"
                    };
                    menuItem.RouteValues.Add("SeName", category.GetSeName());

                    // determine parent
                    if (prevCat != null)
                    {
                        if (category.ParentCategoryId != curParent.Value.EntityId)
                        {
                            if (category.ParentCategoryId == prevCat.Id)
                            {
                                // level +1
                                curParent = curParent.LastChild;
                            }
                            else
                            {
                                // level -x
                                while (!curParent.IsRoot)
                                {
                                    if (curParent.Value.EntityId == category.ParentCategoryId)
                                    {
                                        break;
                                    }
                                    curParent = curParent.Parent;
                                }
                            }
                        }
                    }

                    // add to parent
                    curParent.Append(menuItem);

                    prevCat = category;
                }

                var root = curParent.Root;

                // menu publisher
                _menuPublisher.Value.RegisterMenus(root, "productcategory");

                // event
                _services.EventPublisher.Publish(new NavigationModelBuiltEvent(root));

                return root;
            });

            return model;
        }

        /// <summary>
        /// 获取分类导航列表
        /// </summary>
        /// <param name="currentCategoryId">当前分类ID</param>
        /// <param name="currentArticleId">当前内容ID</param>
        /// <param name="OnlyCurrentCategory">只显示当前分类，不显示全分类导航</param>
        /// <returns></returns>
        public IList<MenuItem> GetProductCategoryBreadCrumb(int currentCategoryId, int currentArticleId, bool OnlyCurrentCategory = false)
        {
            var requestCache = CAF.Infrastructure.Core.EngineContext.Current.Resolve<ICacheManager>();
            string cacheKey = "caf.temp.productcategory.path.{0}-{1}".FormatInvariant(currentCategoryId, currentArticleId);

            var breadcrumb = requestCache.Get(cacheKey, () =>
            {
                var root = GetProductCategoryMenu();
                TreeNode<MenuItem> node = null;

                if (currentCategoryId > 0)
                {
                    node = root.SelectNode(x => x.Value.EntityId == currentCategoryId);
                }

                if (node == null && currentArticleId > 0)
                {

                    var articleCategories = _productCategoryService.GetProductCategoriesByArticleId(currentArticleId);
                    if (articleCategories != null)
                    {
                        currentCategoryId = articleCategories.Id;
                        node = root.SelectNode(x => x.Value.EntityId == currentCategoryId);
                    }
                }

                if (node != null)
                {
                    IList<MenuItem> path;
                    if (OnlyCurrentCategory)
                        path = node.GetCurrentBreadcrumb();
                    else
                        path = node.GetBreadcrumb();
                    return path;
                }

                return new List<MenuItem>();
            });

            return breadcrumb;

        }
        /// <summary>
        /// 根据父级ID获取所有下级分类ID
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <returns></returns>
        public List<int> GetChildProductCategoryIds(int parentCategoryId)
        {
            var userRolesIds = _services.WorkContext.CurrentUser.UserRoles.Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                false,
                string.Join(",", userRolesIds),
                _services.SiteContext.CurrentSite.Id);

            return _services.Cache.Get(cacheKey, () =>
            {
                var root = GetProductCategoryMenu();
                var node = root.SelectNode(x => x.Value.EntityId == parentCategoryId);
                if (node != null)
                {
                    var ids = node.Flatten(false).Select(x => x.EntityId).ToList();
                    return ids;
                }
                return new List<int>();
            });
        }
        /// <summary>
        /// 根据父级ID获取所有下级分类
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <returns></returns>
        public IList<MenuItem> GetChildProductCategorys(int parentCategoryId)
        {
            var userRolesIds = _services.WorkContext.CurrentUser.UserRoles.Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_CATEGORY_CHILD_ITEMS_MODEL_KEY,
                parentCategoryId,
                false,
                string.Join(",", userRolesIds),
                _services.SiteContext.CurrentSite.Id);

            return _services.Cache.Get(cacheKey, () =>
            {
                var root = GetProductCategoryMenu();
                var node = root.SelectNode(x => x.Value.EntityId == parentCategoryId);
                if (node != null)
                {
                    var categorys = node.Flatten(false).ToList();
                    return categorys;
                }
                return new List<MenuItem>();
            });
        }

        #endregion

        #endregion
    }

    #region Nested Classes
    /// <summary>
    /// 分页内容信息
    /// </summary>
    public class PageSizeContext
    {
        public bool AllowUsersToSelectPageSize { get; set; }
        public string PageSizeOptions { get; set; }
        public int PageSize { get; set; }
    }

    #endregion
}