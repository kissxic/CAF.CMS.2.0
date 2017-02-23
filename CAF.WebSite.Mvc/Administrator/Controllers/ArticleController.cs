using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Data;
using CAF.WebSite.Application.Services.Seo;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Sites;
using System.Text;
using CAF.WebSite.Application.Services.Common;
using System.Globalization;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Mvc.Admin.Models.Common;
using CAF.Infrastructure.Core.Domain;
using CAF.WebSite.Application.Services.Directory;
using Newtonsoft.Json;
using CAF.WebSite.Application.Services.Manufacturers;
using CAF.Infrastructure.Core.Domain.Cms.Channels;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class ArticleController : AdminControllerBase
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly IModelTemplateService _modelTemplateService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly IUserContentService _userContentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;
        private readonly IArticleCategoryService _categoryService;
        private readonly IProductCategoryService _productcategoryService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IUserActivityService _userActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ISiteService _siteService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IChannelService _channelService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IArticleExtendedAttributeService _articleAttributeService;
        private readonly IExtendedAttributeService _extendedAttributeService;
        private readonly IExtendedAttributeParser _extendedAttributeParser;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly IDownloadService _downloadService;
        private readonly IDeliveryTimeService _deliveryTimesService;
        private readonly IAclService _aclService;
        private readonly IDbContext _dbContext;
        private readonly SiteInformationSettings _siteSettings;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        #endregion

        #region Ctor

        public ArticleController(
            IWorkContext workContext,
            ISiteContext siteContext,
            IModelTemplateService modelTemplateService,
            IArticleCategoryService categoryService,
            IProductCategoryService productcategoryService,
            IProductAttributeService productAttributeService,
            IManufacturerService manufacturerService,
            ILanguageService languageService,
            IPictureService pictureService,
            AdminAreaSettings adminAreaSettings,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            UserSettings userSettings,
            IUserService userService,
            IUserContentService userContentService,
            IArticleService articleService,
            IArticleTagService articleTagService,
            IUserActivityService userActivityService,
            IEventPublisher eventPublisher,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            ISiteService siteService,
            IChannelService channelService,
            ISiteMappingService siteMappingService,
            IArticleExtendedAttributeService articleAttributeService,
            IExtendedAttributeService extendedAttributeService,
            IExtendedAttributeParser extendedAttributeParser,
            IDateTimeHelper dateTimeHelper,
            ArticleCatalogSettings catalogSettings,
            IDownloadService downloadService,
            IDeliveryTimeService deliveryTimesService,
            IAclService aclService,
            IDbContext dbContext,
            SiteInformationSettings siteSettings,
            IProductAttributeParser productAttributeParser,
            ISpecificationAttributeService specificationAttributeService)
        {
            this._pictureService = pictureService;
            this._modelTemplateService = modelTemplateService;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._userSettings = userSettings;
            this._userService = userService;
            this._userContentService = userContentService;
            this._articleService = articleService;
            this._articleTagService = articleTagService;
            this._categoryService = categoryService;
            this._productcategoryService = productcategoryService;
            this._productAttributeService = productAttributeService;
            this._userActivityService = userActivityService;
            this._adminAreaSettings = adminAreaSettings;
            this._eventPublisher = eventPublisher;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._urlRecordService = urlRecordService;
            this._siteService = siteService;
            this._siteMappingService = siteMappingService;
            this._articleAttributeService = articleAttributeService;
            this._extendedAttributeService = extendedAttributeService;
            this._extendedAttributeParser = extendedAttributeParser;
            this._aclService = aclService;
            this._dbContext = dbContext;
            this._downloadService = downloadService;
            this._deliveryTimesService = deliveryTimesService;
            this._catalogSettings = catalogSettings;
            this._channelService = channelService;
            this._siteSettings = siteSettings;
            this._productAttributeParser = productAttributeParser;
            this._specificationAttributeService = specificationAttributeService;
            this._manufacturerService = manufacturerService;
        }
        #endregion

        #region Update[...]
        /// <summary>
        /// 更新基础信息
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateArticleGeneralInfo(Article article, ArticleModel model)
        {
            var a = article;
            var m = model;

            if (m.CategoryId == 0) a.CategoryId = null;
            else a.CategoryId = m.CategoryId;

            a.StatusId = m.StatusId;
            a.ModelTemplateId = m.ModelTemplateId;
            if (m.PictureId != 0)
                a.PictureId = m.PictureId;
            a.Title = m.Title;
            a.ShortContent = m.ShortContent;
            a.FullContent = m.FullContent;
            a.DisplayOrder = m.DisplayOrder;
            //SEo
            a.MetaKeywords = m.MetaKeywords;
            a.MetaDescription = m.MetaDescription;
            a.MetaTitle = m.MetaTitle;
            a.ModifiedOnUtc = DateTime.UtcNow;


            //网站限制
            if (this._siteSettings.SiteContentShare)
            {
                a.LimitedToSites = m.LimitedToSites;
            }
            else
            {
                a.LimitedToSites = true;
                var siteIds = new List<int>();
                siteIds.Add(this._siteContext.CurrentSite.Id);
                m.SelectedSiteIds = siteIds.ToArray();
            }
            //权限
            a.SubjectToAcl = m.SubjectToAcl;

            if (m.ProductCategoryId == 0) a.ProductCategoryId = null;
            else a.ProductCategoryId = m.ProductCategoryId;

            //扩展信息
            if (model.ShowExtendedAttribute)
            {

                a.IsHot = m.IsHot;
                a.IsRed = m.IsRed;
                a.IsSlide = m.IsSlide;
                a.IsSys = m.IsSys;
                a.IsTop = m.IsTop;
                a.AllowComments = m.AllowComments;
                a.LinkUrl = m.LinkUrl;
                a.Author = m.Author;
                a.ImgUrl = m.ImgUrl;
                a.Click = m.Click;
                a.IsPasswordProtected = m.IsPasswordProtected;
                a.Password = m.Password;
                a.StartDateUtc = m.StartDate;
                a.EndDateUtc = m.EndDate;
                a.AllowUserReviews = m.AllowUserReviews;
                a.IsDownload = m.IsDownload;
                a.DownloadId = m.DownloadId;
                a.UnlimitedDownloads = m.UnlimitedDownloads;
                a.MaxNumberOfDownloads = m.MaxNumberOfDownloads;

                a.ManufacturerId = m.ManufacturerId;
                if (_catalogSettings.StateProvinceEnabled)
                    a.StateProvinceId = m.StateProvinceId;
                if (_catalogSettings.StateProvinceEnabled && _catalogSettings.CityEnabled)
                    a.CityId = m.CityId;
                if (_catalogSettings.StateProvinceEnabled && _catalogSettings.CityEnabled && _catalogSettings.DistrictEnabled)
                    a.DistrictId = m.DistrictId;
            }

        }
        /// <summary>
        /// 更新标签
        /// </summary>
        /// <param name="article"></param>
        /// <param name="rawArticleTags"></param>
        [NonAction]
        protected void UpdateArticleTags(Article article, string rawArticleTags)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var articleTags = new List<string>();

            foreach (string str in rawArticleTags.SplitSafe(","))
            {
                string tag = str.TrimSafe();
                if (tag.HasValue())
                    articleTags.Add(tag);
            }

            var existingArticleTags = article.ArticleTags.ToList();
            var articleTagsToRemove = new List<ArticleTag>();

            foreach (var existingArticleTag in existingArticleTags)
            {
                bool found = false;
                foreach (string newArticleTag in articleTags)
                {
                    if (existingArticleTag.Name.Equals(newArticleTag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    articleTagsToRemove.Add(existingArticleTag);
                }
            }

            foreach (var articleTag in articleTagsToRemove)
            {
                article.ArticleTags.Remove(articleTag);
                _articleService.UpdateArticle(article);
            }

            foreach (string articleTagName in articleTags)
            {
                ArticleTag articleTag = null;
                var articleTag2 = _articleTagService.GetArticleTagByName(articleTagName);

                if (articleTag2 == null)
                {
                    //add new article tag
                    articleTag = new ArticleTag()
                    {
                        Name = articleTagName
                    };
                    _articleTagService.InsertArticleTag(articleTag);
                }
                else
                {
                    articleTag = articleTag2;
                }

                if (!article.ArticleTagExists(articleTag.Id))
                {
                    article.ArticleTags.Add(articleTag);
                    _articleService.UpdateArticle(article);
                }
            }
        }
        /// <summary>
        /// 更新授权
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateArticleAcl(Article article, ArticleModel model)
        {


            var existingAclRecords = _aclService.GetAclRecords(article);
            var allUserRoles = _userService.GetAllUserRoles(true);
            foreach (var userRole in allUserRoles)
            {
                if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                {
                    //new role
                    if (existingAclRecords.Where(acl => acl.UserRoleId == userRole.Id).Count() == 0)
                        _aclService.InsertAclRecord(article, userRole.Id);
                }
                else
                {
                    //removed role
                    var aclRecordToDelete = existingAclRecords.Where(acl => acl.UserRoleId == userRole.Id).FirstOrDefault();
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }
        [NonAction]
        protected void UpdateSiteMappings(Article article, ArticleModel model)
        {
            _siteMappingService.SaveSiteMappings<Article>(article, model.SelectedSiteIds);
        }
        /// <summary>
        /// 更新SEO
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateArticleSeo(Article article, ArticleModel model)
        {
            var a = article;
            var m = model;
            // SEO
            var service = _localizedEntityService;
            foreach (var localized in model.Locales)
            {
                service.SaveLocalizedValue(a, x => x.MetaKeywords, localized.MetaKeywords, localized.LanguageId);
                service.SaveLocalizedValue(a, x => x.MetaDescription, localized.MetaDescription, localized.LanguageId);
                service.SaveLocalizedValue(a, x => x.MetaTitle, localized.MetaTitle, localized.LanguageId);
            }
        }
        /// <summary>
        /// 更新图片SEO
        /// </summary>
        /// <param name="article"></param>
        [NonAction]
        private void UpdatePictureSeoNames(Article article)
        {
            foreach (var pp in article.ArticleAlbum)
            {
                _pictureService.SetSeoFilename(pp.PictureId, _pictureService.GetPictureSeName(article.Title));
            }
        }
        /// <summary>
        /// 对已存在的Articel 更新相关数据
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        /// <param name="editMode"></param>
        [NonAction]
        private void UpdateDataOfExistingArticle(Article article, ArticleModel model, bool editMode)
        {
            var p = article;
            var m = model;

            var modifiedProperties = editMode ? _dbContext.GetModifiedProperties(p) : new Dictionary<string, object>();

            var nameChanged = modifiedProperties.ContainsKey("Name");
            var seoTabLoaded = m.LoadedTabs.Contains("SEO", StringComparer.OrdinalIgnoreCase);

            // SEO
            m.SeName = p.ValidateSeName(m.SeName, p.Title, true);
            _urlRecordService.SaveSlug(p, m.SeName, 0);

            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(article, x => x.Title, localized.Title, localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(article, x => x.ShortContent, localized.ShortContent, localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(article, x => x.FullContent, localized.FullContent, localized.LanguageId);

                // search engine name
                var localizedSeName = p.ValidateSeName(localized.SeName, localized.Title, false, localized.LanguageId);
                _urlRecordService.SaveSlug(p, localizedSeName, localized.LanguageId);
            }

            // picture seo names
            if (nameChanged)
            {
                UpdatePictureSeoNames(p);
            }


            UpdateArticleTags(p, m.ArticleTags);
            UpdateArticleSeo(article, model);
            UpdateSiteMappings(article, model);
            UpdateArticleAcl(article, model);
            UpdateProductAttribute(article, model);
            UpdateProductVariantAttributeCombinations(article, model);
            UpdateArticleSpecifications(article, model);
            UpdateProductPicture(article, model);

        }
        /// <summary>
        /// 更新本地语言信息
        /// </summary>
        /// <param name="articleTag"></param>
        /// <param name="model"></param>
        [NonAction]
        private void UpdateLocales(ArticleTag articleTag, ArticleTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(articleTag, x => x.Name, localized.Name, localized.LanguageId);
            }
        }
        /// <summary>
        /// 更新扩展属性
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        /// <param name="form"></param>
        [NonAction]
        protected void UpdateExtendedAttribute(Article article, ArticleModel model, FormCollection form)
        {
            if (!model.ShowExtendedAttribute) return;

            string selectedAttributes = "";

            var channel = _channelService.GetChannelById(model.ChannelId);
            if (channel == null)
                return;
            var extendedAttributes = channel.ExtendedAttributes;
            foreach (var attribute in extendedAttributes)
            {
                string controlId = string.Format("extended_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var rblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(rblAttributes))
                            {
                                int selectedAttributeId = int.Parse(rblAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                        {
                            var colorAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(colorAttributes))
                            {
                                string enteredText = colorAttributes.Trim();
                                selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;

                    case AttributeControlType.VideoUpload:
                    case AttributeControlType.FileUpload:
                        {
                            //var postedFile = this.Request.Files[controlId].ToPostedFileResult();
                            //if (postedFile != null && postedFile.FileName.HasValue())
                            //{
                            //    int fileMaxSize = _catalogSettings.FileUploadMaximumSizeBytes;
                            //    if (postedFile.Size > fileMaxSize)
                            //    {
                            //        //TODO display warning
                            //        //warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), (int)(fileMaxSize / 1024)));
                            //    }
                            //    else
                            //    {
                            //        //save an uploaded file
                            //        var download = new Download
                            //        {
                            //            DownloadGuid = Guid.NewGuid(),
                            //            UseDownloadUrl = false,
                            //            DownloadUrl = "",
                            //            DownloadBinary = postedFile.Buffer,
                            //            ContentType = postedFile.ContentType,
                            //            Filename = postedFile.FileTitle,
                            //            Extension = postedFile.FileExtension,
                            //            IsNew = true
                            //        };
                            //        _downloadService.InsertDownload(download);
                            //        //save attribute
                            //        selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes, attribute, download.DownloadGuid.ToString());
                            //    }
                            //}
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _extendedAttributeParser.AddExtendedAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //save extended attributes
            _articleAttributeService.SaveAttribute(article, ArticleAttributeNames.ExtendedAttributes, selectedAttributes);

        }
        /// <summary>
        /// 更新库存
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateArticleInventory(Article article, ArticleModel model)
        {
            if (!model.ShowInventory) return;

            var p = article;
            var m = model;

            var prevStockQuantity = article.StockQuantity;


            p.StockQuantity = m.StockQuantity;
            p.DisplayStockAvailability = m.DisplayStockAvailability;
            p.DisplayStockQuantity = m.DisplayStockQuantity;
            p.MinStockQuantity = m.MinStockQuantity;
            p.LowStockActivityId = m.LowStockActivityId;
            p.NotifyAdminForQuantityBelow = m.NotifyAdminForQuantityBelow;
            p.AllowBackInStockSubscriptions = m.AllowBackInStockSubscriptions;
            p.OrderMinimumQuantity = m.OrderMinimumQuantity;
            p.OrderMaximumQuantity = m.OrderMaximumQuantity;
            p.DeliveryTimeId = m.DeliveryTimeId == 0 ? (int?)null : m.DeliveryTimeId;
            p.QuantityUnitId = m.QuantityUnitId == 0 ? (int?)null : m.QuantityUnitId;
            p.IsFreeShipping = m.IsFreeShipping;
            // back in stock notifications
            if (p.AllowBackInStockSubscriptions &&
                p.StockQuantity > 0 &&
                prevStockQuantity <= 0 &&
                p.StatusId == 1 &&
                !p.Deleted)
            {
                //  _backInStockSubscriptionService.SendNotificationsToSubscribers(p);
            }

            if (p.StockQuantity != prevStockQuantity)
            {
                // _productService.AdjustInventory(p, true, 0, string.Empty);
            }
        }
        /// <summary>
        /// 更新价格
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateArticlePrice(Article article, ArticleModel model)
        {
            if (!model.ShowPrice) return;

            var p = article;
            var m = model;

            p.Price = m.Price;
            p.OldPrice = m.OldPrice;
            p.ProductCost = m.ProductCost;
            p.SpecialPrice = m.SpecialPrice;
            p.SpecialPriceStartDateTimeUtc = m.SpecialPriceStartDateTimeUtc;
            p.SpecialPriceEndDateTimeUtc = m.SpecialPriceEndDateTimeUtc;
            p.DisableBuyButton = m.DisableBuyButton;

            p.CustomerEntersPrice = m.CustomerEntersPrice;
            p.MinimumCustomerEnteredPrice = m.MinimumCustomerEnteredPrice;
            p.MaximumCustomerEnteredPrice = m.MaximumCustomerEnteredPrice;

        }
        /// <summary>
        /// 更新规格属性数据
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateProductAttribute(Article article, ArticleModel model)
        {
            if (!model.ShowAttributes) return;
            var p = article;
            var m = model;
            if (model.ProductSpecifications.IsEmpty())
                return;
            JObject data = JsonConvert.DeserializeObject<dynamic>(model.ProductSpecifications);
            //  ProductSpecifications productSpecifications = JsonConvert.DeserializeObject<ProductSpecifications>(model.ProductSpecifications);
            var productSpecifications = new ProductSpecifications();
            var type = data["type"];
            foreach (var item in type.ToList())
            {
                productSpecifications.Type.Add(item["name"].ToString());
                productSpecifications.TypeId.Add(item["attrId"].Convert<int>());
            }
            var is_pic = data["is_pic"];
            productSpecifications.IsPic = is_pic.ToString().Convert<int>();
            var spdata = data["data"];
            var sValue = new SpecificationsValue();
            //属性值矩阵数据
            var toCombine = new List<List<ProductVariantAttributeValue>>();
            var resultMatrix = new List<List<ProductVariantAttributeValue>>();
            var tmp = new List<ProductVariantAttributeValue>();
            //属性值列表
            var resultValueLists = new List<List<ProductVariantAttributeValue>>();


            CombineAll(spdata.ToList(), resultMatrix, productSpecifications.TypeId, 0, tmp);

            CombineAllValueList(spdata.ToList(), resultValueLists, productSpecifications.TypeId, 0);
            int index = 1;
            int typeCount = productSpecifications.TypeId.Count();
            var productVariantAttributes = new List<ProductVariantAttribute>();
            //属性
            foreach (var productAttributeId in productSpecifications.TypeId)
            {
                var pva = new ProductVariantAttribute
                {
                    ArticleId = article.Id,
                    ProductAttributeId = productAttributeId,
                    TextPrompt = "",
                    IsRequired = false,
                    AttributeControlTypeId = (int)AttributeControlType.RadioList,
                    DisplayOrder = index
                };
                //倒序获取
                var valueLists = resultValueLists[typeCount - index];
                //属性值
                foreach (var valueItem in valueLists)
                {
                    pva.ProductVariantAttributeValues.Add(valueItem);
                }
                productVariantAttributes.Add(pva);
                index++;
            }
            _productAttributeService.InsertProductVariantAttribute(article, productVariantAttributes);


        }
        /// <summary>
        /// 更新规格SKU数据
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateProductVariantAttributeCombinations(Article article, ArticleModel model)
        {
            if (!model.ShowAttributes) return;
            var p = article;
            var m = model;
            if (model.ProductSpecifications.IsEmpty())
                return;
            JObject data = JsonConvert.DeserializeObject<dynamic>(model.ProductSpecifications);

            var productSpecifications = new ProductSpecifications();
            var type = data["type"];
            foreach (var item in type.ToList())
            {
                productSpecifications.Type.Add(item["name"].ToString());
                productSpecifications.TypeId.Add(item["attrId"].Convert<int>());
            }
            var is_pic = data["is_pic"];
            productSpecifications.IsPic = is_pic.ToString().Convert<int>();
            var spdata = data["data"];
            var sValue = new SpecificationsValue();
            //SKU详细信息
            var skus = new List<ProductVariantAttributeCombination>();

            CombinValueeAll(spdata.ToList(), skus, 0, "", "");


            _productAttributeService.CreateAllProductVariantAttributeCombinations(article, skus);
        }
        /// <summary>
        /// 更新新图片上传
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateProductPicture(Article article, ArticleModel model)
        {
            var pictureIds = Request.Form["PictureIds"];
            if (pictureIds.IsEmpty()) return;
            var pictureIdArray = pictureIds.Split(',');
            var index = 1;
            foreach (var pictureId in pictureIdArray)
            {
                _articleService.InsertArticleAlbum(new ArticleAlbum()
                {
                    PictureId = pictureId.ToInt(),
                    ArticleId = article.Id,
                    DisplayOrder = index,
                });

                _pictureService.SetSeoFilename(pictureId.ToInt(), _pictureService.GetPictureSeName(article.Title));
                index++;
            }

        }

        /// <summary>
        /// 更新文档属性数据
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateArticleSpecifications(Article article, ArticleModel model)
        {
            if (!model.ShowSpecificationAttributes) return;
            var p = article;
            var m = model;
            if (model.SpaValues.IsEmpty())
                return;
            var articleSpecificationAttributes = JsonConvert.DeserializeObject<List<ArticleSpecificationAttribute>>(model.SpaValues);
            var list = new List<ArticleSpecificationAttribute>();
            foreach (var item in articleSpecificationAttributes)
            {
                var psa = new ArticleSpecificationAttribute()
                {
                    SpecificationAttributeOptionId = item.SpecificationAttributeOptionId,
                    ArticleId = article.Id,
                    AllowFiltering = item.AllowFiltering,
                    ShowOnArticlePage = item.ShowOnArticlePage,
                    DisplayOrder = item.DisplayOrder,
                };
                list.Add(psa);
            }
            _specificationAttributeService.InsertArticleSpecificationAttribute(article.Id, list);
        }
        #endregion

        #region Utitilies
        /// <summary>
        /// 装配基本信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        /// <param name="setPredefinedValues"></param>
        /// <param name="excludeProperties"></param>
        [NonAction]
        protected void PrepareArticleModel(ArticleModel model, Article article, bool setPredefinedValues, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (article != null)
            {
                model.StartDate = article.StartDateUtc;
                model.EndDate = article.EndDateUtc;
                //非步骤一进入情况下，需要赋值ProductCategoryId
                if (!model.ProductCategoryId.HasValue)
                    model.ProductCategoryId = article.ProductCategoryId;

                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(article.CreatedOnUtc, DateTimeKind.Utc);
                model.UpdatedOn = article.ModifiedOnUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(article.ModifiedOnUtc.Value, DateTimeKind.Utc) : DateTime.Now;

                model.Url = Url.RouteUrl("Article", new { SeName = article.GetSeName() }, Request.Url.Scheme);


                var parentCategory = _categoryService.GetArticleCategoryById(model.CategoryId ?? 0);
                if (parentCategory != null && !parentCategory.Deleted)
                {
                    model.CategoryBreadcrumb = parentCategory.GetCategoryBreadCrumb(_categoryService);
                    model.ChannelId = parentCategory.ChannelId;
                    var channel = _channelService.GetChannelById(parentCategory.ChannelId);
                    if (channel != null)
                    {
                        model.ShowExtendedAttribute = channel.ShowExtendedAttribute;
                        model.ShowSpecificationAttributes = channel.ShowSpecificationAttributes;
                        model.ShowInventory = channel.ShowInventory;
                        model.ShowPrice = channel.ShowPrice;
                        model.ShowAttributes = channel.ShowAttributes;
                        model.ShowPromotion = channel.ShowPromotion;
                        model.CategoryShowTypeId = channel.CategoryShowTypeId;
                        model.ProductCategoryShowTypeId = channel.ProductCategoryShowTypeId;
                        if (channel.CategoryShowType == CategoryShowType.CheckBox)
                        {
                            var categorys = _categoryService.GetAllArticleCategoriesByChannelId(model.ChannelId);
                            foreach (var category in categorys)
                            {
                                model.AvailableCategorys.Add(new SelectListItem()
                                {
                                    Text = category.Name,
                                    Value = category.Id.ToString(),
                                    Selected = model.CategoryId == category.Id
                                });
                            }
                        }
                    }
                }
                else
                {
                    var channel = _channelService.GetChannelById(model.ChannelId);
                    if (channel != null)
                    {
                        model.ShowExtendedAttribute = channel.ShowExtendedAttribute;
                        model.ShowSpecificationAttributes = channel.ShowSpecificationAttributes;
                        model.ShowInventory = channel.ShowInventory;
                        model.ShowPrice = channel.ShowPrice;
                        model.ShowAttributes = channel.ShowAttributes;
                        model.ShowPromotion = channel.ShowPromotion;
                        model.CategoryShowTypeId = channel.CategoryShowTypeId;
                        model.ProductCategoryShowTypeId = channel.ProductCategoryShowTypeId;
                        var categorys = _categoryService.GetAllArticleCategoriesByChannelId(model.ChannelId);
                        foreach (var category in categorys)
                        {
                            model.AvailableCategorys.Add(new SelectListItem()
                            {
                                Text = category.Name,
                                Value = category.Id.ToString()
                            });
                        }

                    }

                }
            }
            //频道分类显示模式一导航
            else if (model.CategoryId.HasValue && model.CategoryId != 0)
            {
                var categoryItem = _categoryService.GetArticleCategoryById(model.CategoryId ?? 0);
                model.ModelTemplateId = categoryItem.DetailModelTemplateId;
                if (categoryItem != null && !categoryItem.Deleted)
                {
                    model.CategoryBreadcrumb = categoryItem.GetCategoryBreadCrumb(_categoryService);
                    model.ChannelId = categoryItem.ChannelId;
                    var channel = _channelService.GetChannelById(categoryItem.ChannelId);
                    if (channel != null)
                    {
                        model.ShowExtendedAttribute = channel.ShowExtendedAttribute;
                        model.ShowSpecificationAttributes = channel.ShowSpecificationAttributes;
                        model.ShowInventory = channel.ShowInventory;
                        model.ShowPrice = channel.ShowPrice;
                        model.ShowAttributes = channel.ShowAttributes;
                        model.ShowPromotion = channel.ShowPromotion;
                    }
                }
                else
                    model.CategoryId = 0;



            }
            //频道分类显示模式一复选框
            else if (model.ChannelId != 0)
            {
                var channel = _channelService.GetChannelById(model.ChannelId);
                if (channel != null)
                {
                    model.ShowExtendedAttribute = channel.ShowExtendedAttribute;
                    model.ShowSpecificationAttributes = channel.ShowSpecificationAttributes;
                    model.ShowInventory = channel.ShowInventory;
                    model.ShowPrice = channel.ShowPrice;
                    model.ShowAttributes = channel.ShowAttributes;
                    model.ShowPromotion = channel.ShowPromotion;
                    model.CategoryShowTypeId = channel.CategoryShowTypeId;
                    model.ProductCategoryShowTypeId = channel.ProductCategoryShowTypeId;
                    model.ModelTemplateId = channel.DetailModelTemplateId;
                    if (channel.CategoryShowType == CategoryShowType.CheckBox)
                    {
                        var categorys = _categoryService.GetAllArticleCategoriesByChannelId(model.ChannelId);
                        foreach (var category in categorys)
                        {
                            model.AvailableCategorys.Add(new SelectListItem()
                            {
                                Text = category.Name,
                                Value = category.Id.ToString()
                            });
                        }
                    }
                }
            }
            if (model.ProductCategoryId.HasValue)
            {

                var productCategory = _productcategoryService.GetProductCategoryById(model.ProductCategoryId.Value);
                if (productCategory != null)
                    model.ProductCategoryBreadcrumb = productCategory.GetProductCategoryBreadCrumb(_productcategoryService);
                else
                    model.ProductCategoryId = 0;
            }
            if (model.ManufacturerId.HasValue)
            {

                var manufactureCategory = _manufacturerService.GetManufacturerById(model.ManufacturerId.Value);
                if (manufactureCategory != null)
                    model.ManufacturerBreadcrumb = manufactureCategory.Name;
                else
                    model.ManufacturerId = 0;
            }
            #region templates

            var templates = _modelTemplateService.GetAllModelTemplates((int)TemplateTypeFormat.Detail);
            foreach (var template in templates)
            {
                model.AvailableModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }

            #endregion

            #region Tag
            var allTags = _articleTagService.GetAllArticleTagNames();
            foreach (var tag in allTags)
            {
                model.AvailableArticleTags.Add(new SelectListItem() { Text = tag, Value = tag });
            }

            if (article != null)
            {
                var result = new StringBuilder();
                var tags = article.ArticleTags.ToList();
                for (int i = 0; i < article.ArticleTags.Count; i++)
                {
                    var pt = tags[i];
                    result.Append(pt.Name);
                    if (i != article.ArticleTags.Count - 1)
                        result.Append(", ");
                }
                model.ArticleTags = result.ToString();
            }
            #endregion

            #region Delivery Time

            // delivery times
            var deliveryTimes = _deliveryTimesService.GetAllDeliveryTimes();
            foreach (var dt in deliveryTimes)
            {
                model.AvailableDeliveryTimes.Add(new SelectListItem
                {
                    Text = dt.Name,
                    Value = dt.Id.ToString(),
                    Selected = article != null && !setPredefinedValues && dt.Id == article.DeliveryTimeId.GetValueOrDefault()
                });
            }

            #endregion

            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (article != null)
                {
                    model.SelectedUserRoleIds = _aclService.GetUserRoleIdsWithAccess(article);
                }
                else
                {
                    model.SelectedUserRoleIds = new int[0];
                }
            }


            if (setPredefinedValues)
            {
                model.MaximumCustomerEnteredPrice = 1000;
                model.MaxNumberOfDownloads = 10;
                model.StockQuantity = 10000;
                model.NotifyAdminForQuantityBelow = 1;
                model.OrderMinimumQuantity = 1;
                model.OrderMaximumQuantity = 10000;

                model.UnlimitedDownloads = true;
            }
            PrerpareArticleProductCategory(model);
        }
        /// <summary>
        /// 匹配商品分类
        /// </summary>
        /// <param name="model"></param>
        private void PrerpareArticleProductCategory(ArticleModel model)
        {
            var categoryNames = "";
            if (model.ProductCategoryId.HasValue)
            {
                var productCategory = _productcategoryService.GetProductCategoryById(model.ProductCategoryId.Value);
                string[] strArrays = productCategory.Path.Split(new char[] { '|' });
                for (int i = 0; i < strArrays.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(strArrays[i]))
                    {
                        var category = _productcategoryService.GetProductCategoryById(strArrays[i].ToInt());
                        categoryNames = string.Concat(categoryNames, string.Format("{0} {1} ", (category == null ? "" : category.Name), (i == strArrays.Length - 1 ? "" : " > ")));
                    }
                }
            }
            ViewBag.CategoryNames = categoryNames;
        }

        /// <summary>
        /// 装配扩展属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        [NonAction]
        private void PrerpareArticleExtendedAttributes(ArticleModel model, Article article)
        {
            if (!model.ShowExtendedAttribute) return;

            #region Extended attributes

            var channel = _channelService.GetChannelById(model.ChannelId);
            if (channel != null)
            {
                var extendedAttributes = channel.ExtendedAttributes;

                foreach (var attribute in extendedAttributes)
                {
                    var caModel = new ArticleModel.ArticleExtendedAttributeModel()
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
                            var pvaValueModel = new ArticleModel.ArticleExtendedAttributeValueModel()
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
                                                item.IsPreSelected = true;
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

            model.StateProvinceEnabled = _catalogSettings.StateProvinceEnabled;
            model.CityEnabled = _catalogSettings.CityEnabled;
            model.DistrictEnabled = _catalogSettings.DistrictEnabled;

            if (article != null)
            {
                model.StateProvinceId = article.StateProvinceId;
                model.CityId = article.CityId;
                model.DistrictId = article.DistrictId;
            }
        }
        /// <summary>
        /// 装配规格属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        [NonAction]
        private void PrerpareProductAttributes(ArticleModel model, Article article)
        {
            if (!model.ShowAttributes) return;

            IList<ProductVariantAttribute> productVariantAttributes = new List<ProductVariantAttribute>();
            var resultMatrix = new List<SpecificationsValue>();
            var tmp = new List<SpecificationsValue>();
            var IsPicture = false;
            if (article != null)
            {
                PrepareVariantCombinationAttributes(model.ProductVariantAttributeCombinationSku, article);

                var allCombinations = _productAttributeService.GetAllProductVariantAttributeCombinations(article.Id);
                var data = new List<SpecificationsValue>();

                //解析SKU信息
                foreach (var item in allCombinations)
                {
                    var pictureItem = new ProductVariantAttributeCombinationSkuModel.PictureSelectItemModel();
                    var spValue = new SpecificationsValue();
                    var tempId = "";
                    var productVariantAttributeValues = _productAttributeParser.ParseProductVariantAttributeValues(item.AttributesXml);
                    if (!productVariantAttributeValues.Any()) continue;
                    foreach (var vauleItem in productVariantAttributeValues)
                    {
                        tempId = (tempId.IsEmpty() ? "" : tempId + "-") + vauleItem.ProductAttributeOptionId;
                    }
                    item.AttributesXml = tempId;
                    if (!model.ProductVariantAttributeCombinationSku.AssignablePictures.Where(p => p.ProductAttributeOptionId == productVariantAttributeValues.FirstOrDefault().ProductAttributeOptionId).Any())
                    {
                        //获取图片信息
                        pictureItem.PicturId = item.AssignedPictureIds;
                        pictureItem.ProductAttributeOptionId = productVariantAttributeValues.FirstOrDefault().ProductAttributeOptionId;
                        pictureItem.ProductAttributeOptionName = productVariantAttributeValues.FirstOrDefault().Name;
                        model.ProductVariantAttributeCombinationSku.AssignablePictures.Add(pictureItem);
                    }
                }
                productVariantAttributes = _productAttributeService.GetProductVariantAttributesByArticleId(article.Id);

                CombineSku(productVariantAttributes, resultMatrix, allCombinations, null, 0, productVariantAttributes.Count, "", IsPicture);

            }

            var skus = new
            {
                type = productVariantAttributes.Select(x =>
                {
                    return x.ProductAttribute.Name;
                }),
                is_pic = IsPicture ? 1 : 0,
                data = resultMatrix,
            };
            model.ProductSpecifications = JsonConvert.SerializeObject(skus);
        }
        /// <summary>
        /// 装配规格属性值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        private void PrepareVariantCombinationAttributes(ProductVariantAttributeCombinationSkuModel model, Article article)
        {

            model.ProductAttributes = _productAttributeService.GetAllProductAttributes().Select(x => x.ToModel()).ToList();
            var productVariantAttributes = _productAttributeService.GetProductVariantAttributesByArticleId(article.Id);
            foreach (var attribute in productVariantAttributes)
            {
                var pvaModel = new ProductVariantAttributeCombinationSkuModel.ProductVariantAttributeModel()
                {
                    Id = attribute.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = attribute.ProductAttribute.Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var pvaValues = _productAttributeService.GetProductVariantAttributeValues(attribute.Id);
                    var pvaOptions = _productAttributeService.GetProductAttributeOptionsByProductAttribute(attribute.ProductAttributeId);
                    foreach (var pvaOption in pvaOptions)
                    {
                        var pvaValuel = pvaValues.Where(p => p.ProductAttributeOptionId == pvaOption.Id).ToList();
                        var pvaValueModel = new ProductVariantAttributeCombinationSkuModel.ProductVariantAttributeValueModel()
                        {

                            Name = pvaOption.Name,
                            Color = pvaOption.ColorSquaresRgb,
                            ProductAttributeOptionId = pvaOption.Id,
                            IsPreSelected = pvaValuel.Any()
                        };
                        pvaModel.Values.Add(pvaValueModel);
                    }

                }
                model.ProductVariantAttributes.Add(pvaModel);
            }
            var allCombinations = _productAttributeService.GetAllProductVariantAttributeCombinations(article.Id);
            var data = new List<SpecificationsValue>();


        }
        /// <summary>
        /// 装配文档属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        private void PrerpareArticleSpecifications(ArticleModel model, Article article)
        {
            if (!model.ShowSpecificationAttributes) return;

            #region specification attributes
            //specification attributes
            var specificationAttributes = _specificationAttributeService.GetSpecificationAttributes().ToList();
            for (int i = 0; i < specificationAttributes.Count; i++)
            {
                var sa = specificationAttributes[i];
                model.AddSpecificationAttributeModel.AvailableAttributes.Add(new SelectListItem { Text = sa.Name, Value = sa.Id.ToString() });
                if (i == 0)
                {
                    //attribute options
                    foreach (var sao in _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(sa.Id))
                    {
                        model.AddSpecificationAttributeModel.AvailableOptions.Add(new SelectListItem { Text = sao.Name, Value = sao.Id.ToString() });
                    }
                }
            }
            //default specs values
            model.AddSpecificationAttributeModel.ShowOnProductPage = true;
            #endregion

            var productrSpecsModel = new List<ArticleSpecificationAttributeModel>();
            if (article != null)
            {
                var productrSpecs = _specificationAttributeService.GetArticleSpecificationAttributesByArticleId(article.Id);
                productrSpecsModel = productrSpecs
                 .Select(x =>
                 {
                     var psaModel = new ArticleSpecificationAttributeModel
                     {
                         Id = x.Id,
                         SpecificationAttributeName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                         SpecificationAttributeOptionName = x.SpecificationAttributeOption.Name,
                         SpecificationAttributeOptionAttributeId = x.SpecificationAttributeOption.SpecificationAttributeId,
                         SpecificationAttributeOptionId = x.SpecificationAttributeOptionId,
                         AllowFiltering = x.AllowFiltering,
                         ShowOnArticlePage = x.ShowOnArticlePage,
                         DisplayOrder = x.DisplayOrder
                     };
                     return psaModel;
                 })
                 .ToList();
            }
            else if (model != null)
            {
                //栏目属性
                var productrSpecs1 = _specificationAttributeService.GetCategorySpecificationAttributesById(model.CategoryId ?? 0);
                //频道属性
                var productrSpecs2 = _specificationAttributeService.GetChannelSpecificationAttributesById(model.ChannelId);
                productrSpecsModel = productrSpecs1
                 .Select(x =>
                 {
                     var psaModel = new ArticleSpecificationAttributeModel
                     {
                         Id = x.Id,
                         SpecificationAttributeName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                         SpecificationAttributeOptionName = x.SpecificationAttributeOption.Name,
                         SpecificationAttributeOptionAttributeId = x.SpecificationAttributeOption.SpecificationAttributeId,
                         SpecificationAttributeOptionId = x.SpecificationAttributeOptionId,
                         AllowFiltering = x.AllowFiltering,
                         ShowOnArticlePage = x.ShowOnArticlePage,
                         DisplayOrder = x.DisplayOrder
                     };
                     return psaModel;
                 })
                 .ToList();
                foreach (var item in productrSpecs2)
                {
                    if (!productrSpecsModel.Where(x => x.Id == item.Id).Any())
                    {
                        var psaModel = new ArticleSpecificationAttributeModel
                        {
                            Id = item.Id,
                            SpecificationAttributeName = item.SpecificationAttributeOption.SpecificationAttribute.Name,
                            SpecificationAttributeOptionName = item.SpecificationAttributeOption.Name,
                            SpecificationAttributeOptionAttributeId = item.SpecificationAttributeOption.SpecificationAttributeId,
                            SpecificationAttributeOptionId = item.SpecificationAttributeOptionId,
                            AllowFiltering = item.AllowFiltering,
                            ShowOnArticlePage = item.ShowOnArticlePage,
                            DisplayOrder = item.DisplayOrder
                        };
                        productrSpecsModel.Add(psaModel);
                    }
                }
            }


            foreach (var attr in productrSpecsModel)
            {
                var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(attr.SpecificationAttributeOptionAttributeId);

                foreach (var option in options)
                {
                    attr.SpecificationAttributeOptions.Add(new ArticleSpecificationAttributeModel.SpecificationAttributeOption
                    {
                        id = option.Id,
                        name = option.Name,
                        text = option.Name,
                        select = attr.SpecificationAttributeOptionId == option.Id
                    });
                }

                attr.SpecificationAttributeOptionsJsonString = JsonConvert.SerializeObject(attr.SpecificationAttributeOptions);
            }
            model.ArticleSpecificationAttributeModels = productrSpecsModel;

        }

        /// <summary>
        /// 装配图片信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        [NonAction]
        private void PrepareArticlePictureThumbnailModel(ArticleModel model, Article article)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (article != null && _adminAreaSettings.DisplayArticlePictures)
            {
                var defaultArticlePicture = _pictureService.GetPicturesByArticleId(article.Id, 1).FirstOrDefault();
                model.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultArticlePicture, 75, 75, true);
                model.NoThumb = defaultArticlePicture == null;
            }
        }
        /// <summary>
        /// 装配网站授权
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        /// <param name="excludeProperties"></param>
        [NonAction]
        private void PrepareSitesMappingModel(ArticleModel model, Article article, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableSites = _siteService
                .GetAllSites()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (article != null)
                {
                    model.SelectedSiteIds = _siteMappingService.GetSitesIdsWithAccess(article);
                }
                else
                {
                    model.SelectedSiteIds = new int[0];
                }
            }
            model.ShowSiteContentShare = _siteSettings.SiteContentShare;
        }

        private void ConvertMenuItemNode(MenuModel node, List<ArticleCategory> allCategroySource, List<ArticleCategory> categroySource)
        {
            foreach (var childNode in categroySource)
            {
                var menuItemModel = new MenuModel();
                menuItemModel.Text = childNode.Name;
                menuItemModel.Id = childNode.Id.ToString();
                menuItemModel.Href = Url.Action("Center", new { SearchCategoryId = childNode.Id });
                node.Childitems.Add(menuItemModel);
                var childCategory = allCategroySource.Where(p => p.ParentCategoryId == childNode.Id).ToList();
                if (childCategory.Count > 0)
                    ConvertMenuItemNode(menuItemModel, allCategroySource, childCategory);
            }
        }

        /// <summary>
        /// 显示SKU信息
        /// </summary>
        /// <param name="toCombine"></param>
        /// <param name="result"></param>
        /// <param name="combinations"></param>
        /// <param name="y"></param>
        /// <param name="count"></param>
        /// <param name="tmpId"></param>
        private void CombineSku(IList<ProductVariantAttribute> toCombine, IList<SpecificationsValue> result, IList<ProductVariantAttributeCombination> combinations,
           SpecificationsValue root, int y, int count, string tmpId, bool isPicture)
        {
            if (!toCombine.Any()) return;
            var combine = toCombine[y];
            var pvAttributeValues = _productAttributeService.GetProductVariantAttributeValues(combine.Id);
            foreach (var item in pvAttributeValues)
            {
                var sp = new SpecificationsValue();
                sp.typeid = item.ProductAttributeOptionId;
                sp.name = item.Name;
                sp.color = item.ColorSquaresRgb;
                root = root ?? sp;
                var tempId = (tmpId.IsEmpty() ? "" : tmpId + "-") + item.ProductAttributeOptionId.ToString();
                if (y != count - 1)
                    CombineSku(toCombine, sp.ch, combinations, root, y + 1, count, tempId, isPicture);
                else
                {
                    var combinationList = combinations.Where(p => p.AttributesXml == tempId).ToList();
                    if (combinationList.Any())
                    {
                        var combination = combinationList.FirstOrDefault();
                        sp.sku_code = combination.Sku;
                        sp.sku_price = combination.Price.Value;
                        sp.sku_stock = combination.StockQuantity;
                        root.pic = combination.AssignedPictureIds;
                        isPicture = !combination.AssignedPictureIds.IsEmpty();
                    }
                }
                result.Add(sp);


            }


        }


        /// <summary>
        /// 获取规格属性信息，树结构
        /// </summary>
        /// <param name="toCombine"></param>
        /// <param name="result"></param>
        /// <param name="productVariantAttributeIds"></param>
        /// <param name="y"></param>
        /// <param name="tmp"></param>
        private void CombineAll(List<JToken> toCombine, List<List<ProductVariantAttributeValue>> result, List<int> productVariantAttributeIds, int y, List<ProductVariantAttributeValue> tmp)
        {

            for (int i = 0; i < toCombine.Count; ++i)
            {
                var combine = toCombine[i];

                var lst = new List<ProductVariantAttributeValue>(tmp);
                var pvav = new ProductVariantAttributeValue
                {
                    ProductVariantAttributeId = productVariantAttributeIds[y],
                    Name = combine["name"].Convert<string>(),
                    DisplayOrder = i + 1,
                };
                lst.Add(pvav);

                if (combine["ch"] == null)
                    result.Add(lst);
                else
                    CombineAll(combine["ch"].ToList(), result, productVariantAttributeIds, y + 1, lst);
            }
        }

        /// <summary>
        /// 获取规格属性信息列表
        /// </summary>
        /// <param name="toCombine"></param>
        /// <param name="result"></param>
        /// <param name="productVariantAttributeIds"></param>
        /// <param name="y"></param>
        /// <param name="tmp"></param>
        private void CombineAllValueList(List<JToken> toCombine, List<List<ProductVariantAttributeValue>> result, List<int> productVariantAttributeIds, int y)
        {
            var lst = new List<ProductVariantAttributeValue>();
            for (int i = 0; i < toCombine.Count; ++i)
            {
                var combine = toCombine[i];
                var pvav = new ProductVariantAttributeValue
                {
                    ProductAttributeOptionId = combine["typeid"].Convert<int>(),
                    Name = combine["name"].Convert<string>(),
                    ColorSquaresRgb = combine["color"].Convert<string>(),
                    DisplayOrder = i + 1,
                };
                lst.Add(pvav);

                if (i == toCombine.Count - 1)
                    result.Add(lst);
                if (i == 0 && combine["ch"] != null)
                    CombineAllValueList(combine["ch"].ToList(), result, productVariantAttributeIds, y + 1);
            }
        }
        /// <summary>
        /// 获取SKU信息
        /// </summary>
        /// <param name="toCombine"></param>
        /// <param name="result"></param>
        /// <param name="y"></param>
        /// <param name="tmp"></param>
        private void CombinValueeAll(List<JToken> toCombine, List<ProductVariantAttributeCombination> result, int y, string tmp, string picTmp)
        {

            for (int i = 0; i < toCombine.Count; ++i)
            {
                var combine = toCombine[i];

                var lst = tmp.IsEmpty() ? combine["typeid"].Convert<string>() : tmp + "-" + combine["typeid"].Convert<string>();
                var picId = (picTmp.IsEmpty() ? "" : picTmp) + (combine["pic"] == null || combine["pic"].ToString().IsEmpty() ? "" : combine["pic"].Convert<string>() + ",");
                if (combine["ch"] == null)
                {
                    var pvav = new ProductVariantAttributeCombination
                    {
                        AttributesXml = lst,
                        StockQuantity = combine["sku_stock"] == null || combine["sku_stock"].ToString().IsEmpty() ? 0 : combine["sku_stock"].Convert<int>(),
                        Price = combine["sku_price"] == null || combine["sku_price"].ToString().IsEmpty() ? 0 : combine["sku_price"].Convert<decimal>(),
                        Sku = combine["sku_code"] == null ? "" : combine["sku_code"].Convert<string>(),
                        AssignedPictureIds = picId.IsEmpty() ? "" : picId.TrimEnd(','),
                        AllowOutOfStockOrders = true,
                        IsActive = true
                    };
                    result.Add(pvav);
                }
                else
                    CombinValueeAll(combine["ch"].ToList(), result, y + 1, lst, picId);
            }
        }



        #endregion

        #region Methods
        #region Article list / create / edit / delete
        // GET: Edit
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        /// <summary>
        /// 发布步骤一
        /// ProductId空代表新建步骤，非空代表修改步骤
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult PublicStepOne(PublicStepOneModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var amodel = new ArticleListModel();
            amodel.SearchChannelId = model.ChannelId ?? 0;
            amodel.SearchProductCategoryId = model.PcategoryId ?? 0;
            amodel.ArticleId = model.ProductId ?? 0;
            return View(amodel);

        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var allChannels = _channelService.GetAllChannels();
            #region categories
            //categories
            var allCategories = _categoryService.GetAllCategories(showHidden: true);

            var mappedCategories = allCategories.ToDictionary(x => x.Id);
            //foreach (var c in allCategories)
            //{
            //    // c.Name = c.GetCategoryNameWithPrefix(_categoryService, mappedCategories);
            //    c.Name = c.GetCategoryBreadCrumb(_categoryService, mappedCategories);
            //}

            var menuModels = new List<MenuModel>();
            foreach (var item in allChannels)
            {
                var menuModel = new MenuModel();
                menuModel.Text = item.Title;
                if (item.CategoryShowType != CategoryShowType.LeftNavigationt)
                {
                    menuModel.Id = item.Id.ToString();
                    menuModel.Href = Url.Action("Center", new { SearchChannelId = item.Id });
                }
                else
                {
                    var channelCategory = allCategories.Where(p => p.ChannelId == item.Id && p.ParentCategoryId == 0).ToList();
                    ConvertMenuItemNode(menuModel, allCategories.ToList(), channelCategory);
                }
                menuModels.Add(menuModel);
            }

            #endregion



            return View(menuModels);
        }

        public ActionResult Center(ArticleListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();


            #region templates
            var templates = _modelTemplateService.GetAllModelTemplates();
            foreach (var template in templates)
            {
                model.AvailableModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }

            #endregion

            #region categories
            //categories
            var allCategories = _categoryService.GetAllCategories(showHidden: true, channelId: model.SearchChannelId);

            var mappedCategories = allCategories.ToDictionary(x => x.Id);
            foreach (var c in allCategories)
            {
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetCategoryNameWithPrefix(_categoryService, mappedCategories), Value = c.Id.ToString() });
            }
            allCategories.Insert(0, new ArticleCategory() { Id = 0, Name = "所有" });
            #endregion

            if (model.SearchChannelId > 0) return PartialView("CenterChannel", model);

            return View(model);
        }
        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam, ArticleListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();


            var searchContext = new ArticleSearchContext
            {
                SiteId = model.SearchSiteId,
                Keywords = model.SearchArticleName,
                LanguageId = _workContext.WorkingLanguage.Id,
                OrderBy = ArticleSortingEnum.Position,
                PageIndex = dataTableParam.PageIndex,
                PageSize = dataTableParam.PageSize,
                ShowHidden = true,
                WithoutCategories = model.SearchWithoutCategories
            };

            if (model.SearchCategoryId > 0)
                searchContext.CategoryIds.Add(model.SearchCategoryId);

            if (model.SearchChannelId > 0)
                searchContext.ChannelIds.Add(model.SearchChannelId);

            if (model.SearchCategoryId > 0)
            {
                var categories = _categoryService.GetAllProductCategoriesByParentCategoryId(model.SearchCategoryId);
                var articlesID = categories.CategoriesForTree();
                searchContext.CategoryIds.AddRange(articlesID);
            }
            var articles = _articleService.SearchArticles(searchContext);

            var total = articles.TotalCount;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = articles.Select(x =>
                {
                    var m = x.ToModel();
                    //获取图片
                    //  PrepareArticlePictureThumbnailModel(m, x);
                    m.StatusName = x.GetArticleStatusLabel(_localizationService);
                    if (x.ModifiedOnUtc.HasValue)
                        m.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.ModifiedOnUtc.Value, DateTimeKind.Utc);
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    //m.LanguageName = x.Language.Name;
                    m.Comments = x.ApprovedCommentCount + x.NotApprovedCommentCount;
                    m.CategoryBreadcrumb = _categoryService.GetArticleCategoryById(x.CategoryId ?? 0).GetCategoryBreadCrumb(_categoryService);
                    return m;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };

        }

        public ActionResult Create(PublicStepOneModel pmodel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var model = new ArticleModel();
            if (pmodel.CategoryId.HasValue)
            {
                model.CategoryId = pmodel.CategoryId.Value;

            }
            else if (pmodel.ChannelId.HasValue)
            {
                model.ChannelId = pmodel.ChannelId.Value;
            }
            model.ProductCategoryId = pmodel.PcategoryId;
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            PrepareArticleModel(model, null, false, false);
            PrerpareArticleExtendedAttributes(model, null);
            PrepareSitesMappingModel(model, null, false);
            PrerpareProductAttributes(model, null);
            PrerpareArticleSpecifications(model, null);
            //article
            AddLocales(_languageService, model.Locales);
            model.ShowSiteContentShare = _siteSettings.SiteContentShare;
            model.DisplayOrder = 1;

            if (!model.ProductCategoryId.HasValue && model.ProductCategoryShowType == ProductCategoryShowType.Step)
            {
                return RedirectToAction("PublicStepOne", new { categoryId = pmodel.CategoryId, channelId = pmodel.ChannelId });
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing", "save-create", "continueCreate")]
        [ValidateInput(false)]
        public ActionResult Create(string btnId, string formId, ArticleModel model, bool continueEditing, bool continueCreate, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                var article = model.ToEntity();
                article.AddEntitySysParam();
                MapModelToArticle(model, article, form);
                _articleService.InsertArticle(article);
                UpdateExtendedAttribute(article, model, form);
                UpdateDataOfExistingArticle(article, model, false);

                //搜索引擎添加索引
                // this._eventPublisher.Publish(new ArticleEvent(article, Url.RouteUrl("Article", new { SeName = article.GetSeName() }, Request.Url.Scheme)));

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Articles.Added"));
                if (continueCreate)
                {
                    return (model.CategoryShowType == CategoryShowType.LeftNavigationt ? RedirectToAction("Create", new { categoryId = model.CategoryId }) : RedirectToAction("Create", new { channelId = model.ChannelId }));

                }

                // ensure that the same tab gets selected in edit view
                var selectedTab = TempData["SelectedTab.article-edit"] as CAF.WebSite.Application.WebUI.SelectedTabInfo;
                if (selectedTab != null)
                {
                    selectedTab.Path = Url.Action("Edit", new System.Web.Routing.RouteValueDictionary { { "id", article.Id } });
                }

                return continueEditing ? RedirectToAction("Edit", new { id = article.Id }) :
                    (model.CategoryShowType == CategoryShowType.LeftNavigationt ? RedirectToAction("Center", new { SearchCategoryId = model.CategoryId }) : RedirectToAction("Center", new { SearchChannelId = model.ChannelId }));


            }

            PrepareArticleModel(model, null, false, false);
            PrerpareArticleExtendedAttributes(model, null);
            PrepareSitesMappingModel(model, null, false);
            PrerpareProductAttributes(model, null);
            PrerpareArticleSpecifications(model, null);
            return View(model);
        }

        //edit article
        public ActionResult Edit(int id, string btnId, string formId, int? pcategoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var article = _articleService.GetArticleById(id);

            if (article == null || article.Deleted)
                return RedirectToAction("Center");

            var model = article.ToModel();

            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Title = article.GetLocalized(x => x.Title, languageId, false, false);
                locale.ShortContent = article.GetLocalized(x => x.ShortContent, languageId, false, false);
                locale.FullContent = article.GetLocalized(x => x.FullContent, languageId, false, false);
                locale.MetaKeywords = article.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = article.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = article.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = article.GetSeName(languageId, false, false);
            });
            //商品分类ID不为空代表当前操作是从步骤一进入并修改商品分类
            model.ProductCategoryId = pcategoryId;

            PrepareArticleModel(model, article, false, false);
            PrerpareArticleExtendedAttributes(model, article);
            PrepareArticlePictureThumbnailModel(model, article);
            PrepareSitesMappingModel(model, article, false);
            PrerpareProductAttributes(model, article);
            PrerpareArticleSpecifications(model, article);
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing", "save-create", "continueCreate")]
        [ValidateInput(false)]
        public ActionResult Edit(string btnId, string formId, ArticleModel model, bool continueEditing, bool continueCreate, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var article = _articleService.GetArticleById(model.Id);
            if (article == null || article.Deleted)
            {
                return RedirectToAction("Center", new { categoryId = model.CategoryId });
            }

            if (ModelState.IsValid)
            {
                MapModelToArticle(model, article, form);
                article.AddEntitySysParam(false, true);
                _articleService.UpdateArticle(article);
                UpdateExtendedAttribute(article, model, form);
                UpdateDataOfExistingArticle(article, model, false);

                // activity log
                _userActivityService.InsertActivity("EditArticle", _localizationService.GetResource("ActivityLog.EditArticle"), article.Title);
                //搜索引擎添加索引
                // this._eventPublisher.Publish(new ArticleEvent(article, Url.RouteUrl("Article", new { SeName = article.GetSeName() }, Request.Url.Scheme)));

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Articles.Updated"));
                if (continueCreate)
                {
                    return (model.CategoryShowType == CategoryShowType.LeftNavigationt ? RedirectToAction("Create", new { categoryId = model.CategoryId }) : RedirectToAction("Create", new { channelId = model.ChannelId }));
                }
                return continueEditing ? RedirectToAction("Edit", new { id = article.Id }) :
                (model.CategoryShowType == CategoryShowType.LeftNavigationt ? RedirectToAction("Center", new { SearchCategoryId = model.CategoryId }) : RedirectToAction("Center", new { SearchChannelId = model.ChannelId }));

            }

            PrepareArticleModel(model, article, false, true);
            PrerpareArticleExtendedAttributes(model, article);
            PrepareArticlePictureThumbnailModel(model, article);
            PrepareSitesMappingModel(model, article, false);
            PrerpareProductAttributes(model, article);
            PrerpareArticleSpecifications(model, article);
            return View(model);
        }

        [NonAction]
        protected void MapModelToArticle(ArticleModel model, Article article, FormCollection form)
        {
            if (model.LoadedTabs == null || model.LoadedTabs.Length == 0)
            {
                model.LoadedTabs = new string[] { "Base" };
            }

            foreach (var tab in model.LoadedTabs)
            {
                switch (tab.ToLower())
                {
                    case "base":
                    case "info":
                        UpdateArticleGeneralInfo(article, model);
                        break;
                    case "inventory":
                        UpdateArticleInventory(article, model);
                        break;
                    case "price":
                        UpdateArticlePrice(article, model);
                        break;
                    case "attributes":
                        break;
                    case "seo":
                        break;
                    case "acl":
                        break;
                    case "sites":
                        break;
                    case "exts":
                        break;
                }
            }

            _eventPublisher.Publish(new ModelBoundEvent(model, article, form));
        }

        public ActionResult LoadEditTab(int id, string tabName, string viewPath = null)
        {

            try
            {
                if (id == 0)
                {
                    // is Create mode
                    return PartialView("_Create.SaveFirst");
                }

                if (tabName.IsEmpty())
                {
                    return Content("A unique tab name has to specified (route parameter: tabName)");
                }

                var article = _articleService.GetArticleById(id);

                var model = article.ToModel();


                AddLocales(_languageService, model.Locales, (locale, languageId) =>
                {
                    locale.Title = article.GetLocalized(x => x.Title, languageId, false, false);
                    locale.ShortContent = article.GetLocalized(x => x.ShortContent, languageId, false, false);
                    locale.FullContent = article.GetLocalized(x => x.FullContent, languageId, false, false);
                    locale.MetaKeywords = article.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = article.GetLocalized(x => x.MetaDescription, languageId, false, false);
                    locale.MetaTitle = article.GetLocalized(x => x.MetaTitle, languageId, false, false);
                    locale.SeName = article.GetSeName(languageId, false, false);
                    // locale.BundleTitleText = article.GetLocalized(x => x.BundleTitleText, languageId, false, false);
                });

                PrepareArticleModel(model, article, false, false);
                PrepareSitesMappingModel(model, article, false);
                PrepareArticlePictureThumbnailModel(model, article);

                return PartialView(viewPath.NullEmpty() ?? "_CreateOrUpdate." + tabName, model);
            }
            catch (Exception ex)
            {
                return Content("Error while loading template: " + ex.Message);
            }
        }

        public ActionResult ArticleExtendedPartialView(int id, int categoryid, string tabName, string viewPath = null)
        {

            try
            {
                if (tabName.IsEmpty())
                {
                    return Content("A unique tab name has to specified (route parameter: tabName)");
                }

                var article = _articleService.GetArticleById(id);
                var model = article == null ? new ArticleModel() : article.ToModel();
                model.CategoryId = categoryid;
                PrerpareArticleExtendedAttributes(model, article);

                return PartialView(viewPath.NullEmpty() ?? "_CreateOrUpdate." + tabName, model);
            }
            catch (Exception ex)
            {
                return Content("Error while loading template: " + ex.Message);
            }
        }

        //delete article
        [HttpPost]
        public ActionResult Delete(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var article = _articleService.GetArticleById(id);
            _articleService.DeleteArticle(article);

            //activity log
            _userActivityService.InsertActivity("DeleteArticle", _localizationService.GetResource("ActivityLog.DeleteArticle"), article.Title);

            NotifySuccess(_localizationService.GetResource("Admin.Catalog.Articles.Deleted"));

            var channel = _channelService.GetChannelById(article.ChannelId);
            if (channel != null)
            {
                return channel.CategoryShowType == CategoryShowType.LeftNavigationt ? RedirectToAction("Center", new { SearchCategoryId = article.CategoryId }) : RedirectToAction("Center", new { SearchChannelId = article.ChannelId });
            }
            return RedirectToAction("Center", new { SearchCategoryId = article.CategoryId });
        }

        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var articles = new List<Article>();
            if (selectedIds != null)
            {
                articles.AddRange(_articleService.GetArticlesByIds(selectedIds.ToArray()));

                for (int i = 0; i < articles.Count; i++)
                {
                    var article = articles[i];
                    _articleService.DeleteArticle(article);
                }
            }
            return Json(new { Result = true });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BulkEditSave(ArticleListModel.BatchCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();


            if (model.OpenCategorieCheckBox && model.CategoryId.HasValue)
            {
                foreach (var id in model.SelectedIds)
                {
                    //update
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        article.CategoryId = model.CategoryId.Value;
                        _articleService.UpdateArticle(article);
                    }
                }
            }
            if (model.OpenTemplateCheckBox && model.TemplateId.HasValue)
            {
                foreach (var id in model.SelectedIds)
                {
                    //update
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        article.ModelTemplateId = model.TemplateId.Value;
                        _articleService.UpdateArticle(article);
                    }
                }
            }
            if (model.OpenCheckBox)
            {
                foreach (var id in model.SelectedIds)
                {
                    //update
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        article.AllowComments = model.AllowComments;
                        article.IsTop = model.IsTop;
                        article.IsRed = model.IsRed;
                        article.IsHot = model.IsHot;
                        article.IsSlide = model.IsSlide;
                        _articleService.UpdateArticle(article);
                    }
                }
            }
            return Json(new { state = 1 }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult BulkEditOrderSave(List<String> updatedArticles)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();


            if (updatedArticles != null)
            {
                foreach (var aModel in updatedArticles)
                {
                    //update
                    var values = aModel.Split('|');
                    var article = _articleService.GetArticleById(values[0].ToInt());
                    if (article != null)
                    {
                        article.DisplayOrder = values[1].ToInt();
                        _articleService.UpdateArticle(article);
                    }
                }
            }

            return Json(new { state = 1 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Article pictures

        public ActionResult ArticlePictureAdd(int pictureId, int displayOrder, int articleId)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            if (pictureId == 0)
                throw new ArgumentException();

            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            _articleService.InsertArticleAlbum(new ArticleAlbum()
            {
                PictureId = pictureId,
                ArticleId = articleId,
                DisplayOrder = displayOrder,
            });

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(article.Title));

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ArticlePictureList(DataTablesParam dataTableParam, int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var articlePictures = _articleService.GetArticleAlbumsByArticleId(Id);
            var articlePicturesModel = articlePictures
                .Select(x =>
                {
                    return new ArticleModel.ArticlePictureModel()
                    {
                        Id = x.Id,
                        ArticleId = x.ArticleId,
                        PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                        PictureId = x.PictureId,
                        DisplayOrder = x.DisplayOrder
                    };
                })
                .ToList().AsQueryable();
            var total = articlePicturesModel.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = articlePicturesModel.Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };

        }


        public ActionResult ArticlePictureUpdate(int id, string name, string value)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var articlePicture = _articleService.GetArticleAlbumById(id);
            if (articlePicture == null)
                throw new ArgumentException("No article picture found with the specified id");

            articlePicture.DisplayOrder = value.ToInt();
            _articleService.UpdateArticleAlbum(articlePicture);
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            // return ArticlePictureList(articlePicture.ArticleId);
        }


        public ActionResult ArticlePictureDelete(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var articlePicture = _articleService.GetArticleAlbumById(id);
            if (articlePicture == null)
                throw new ArgumentException("No article picture found with the specified id");

            var articleId = articlePicture.ArticleId;
            _articleService.DeleteArticleAlbum(articlePicture);

            var picture = _pictureService.GetPictureById(articlePicture.PictureId);
            _pictureService.DeletePicture(picture);
            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Article.ArticlePictureDelete") }, JsonRequestBehavior.AllowGet);
            // return ArticlePictureList(articleId);
        }

        #region 多图片上传
        /// <summary>
        /// 多图片上传
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="displayOrder"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public ActionResult BatchArticlePictureAdd(int articleId)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            if (articleId == 0)
                throw new ArgumentException("No article found with the specified id");

            var postedFile = Request.ToPostedFileResult();

            var picture = _pictureService.InsertPicture(postedFile.Buffer, postedFile.ContentType, null, true, false);
            var article = _articleService.GetArticleById(articleId);
            var articleAlbum = new ArticleAlbum()
            {
                PictureId = picture.Id,
                ArticleId = articleId,
                DisplayOrder = 1,
            };
            _articleService.InsertArticleAlbum(articleAlbum);
            _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(article.Title));
            return Json(new { result = true, data = articleAlbum.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BatchArticlePictureList(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var articlePictures = _articleService.GetArticleAlbumsByArticleId(Id);
            var articlePicturesModel = articlePictures
                .Select(x =>
                {
                    return new ArticleModel.ArticlePictureModel()
                    {
                        Id = x.Id,
                        ArticleId = x.ArticleId,
                        PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                        PictureId = x.PictureId,
                        DisplayOrder = x.DisplayOrder
                    };
                })
                .ToList().AsQueryable();

            return new JsonResult
            {
                Data = articlePicturesModel
            };

        }
        #endregion

        #endregion

        #region Article tags

        public ActionResult ArticleTags()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult ArticleTags(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var tags = _articleTagService.GetAllArticleTags()
                //order by Article count
                .OrderByDescending(x => _articleTagService.GetArticleCount(x.Id, 0))
                .Select(x =>
                {
                    return new ArticleTagModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ArticleCount = _articleTagService.GetArticleCount(x.Id, 0)
                    };
                });

            var total = tags.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = tags.PagedForCommand(dataTableParam.PageIndex, dataTableParam.PageSize).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }


        public ActionResult ArticleTagDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var tag = _articleTagService.GetArticleTagById(id);
            if (tag == null)
                throw new ArgumentException("No Article tag found with the specified id");
            _articleTagService.DeleteArticleTag(tag);

            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Article.ArticleTagDelete") }, JsonRequestBehavior.AllowGet);
        }
        //create
        public ActionResult CreateArticleTag()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var model = new ArticleTagModel();

            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateArticleTag(string btnId, string formId, ArticleTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var articleTag = new ArticleTag();
                articleTag.Name = model.Name;
                _articleTagService.InsertArticleTag(articleTag);
                //locales
                UpdateLocales(articleTag, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }
            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //edit
        public ActionResult EditArticleTag(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var ArticleTag = _articleTagService.GetArticleTagById(id);
            if (ArticleTag == null)
                //No Article tag found with the specified id
                return RedirectToAction("List");

            var model = new ArticleTagModel()
            {
                Id = ArticleTag.Id,
                Name = ArticleTag.Name,
                ArticleCount = _articleTagService.GetArticleCount(ArticleTag.Id, 0)
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = ArticleTag.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult EditArticleTag(string btnId, string formId, ArticleTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var ArticleTag = _articleTagService.GetArticleTagById(model.Id);
            if (ArticleTag == null)
                //No Article tag found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                ArticleTag.Name = model.Name;
                _articleTagService.UpdateArticleTag(ArticleTag);
                //locales
                UpdateLocales(ArticleTag, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Related articles

        [HttpPost]
        public ActionResult RelatedArticleList(DataTablesParam dataTableParam, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var relatedArticles = _articleService.GetRelatedArticlesByArticleId1(articleId, true);
            var relatedArticlesModel = relatedArticles
                .Select(x =>
                {
                    var article2 = _articleService.GetArticleById(x.ArticleId2);

                    return new ArticleModel.RelatedArticleModel()
                    {
                        Id = x.Id,
                        ArticleId2 = x.ArticleId2,
                        Article2Name = article2.Title,
                        DisplayOrder = x.DisplayOrder,
                        Article2Published = article2.StatusFormat == StatusFormat.Norma
                    };
                })
                .ToList();
            var total = relatedArticlesModel.Count;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = relatedArticlesModel.Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }


        public ActionResult RelatedArticleUpdate(ArticleModel.RelatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var relatedArticle = _articleService.GetRelatedArticleById(model.Id);
            if (relatedArticle == null)
                throw new ArgumentException("No related article found with the specified id");

            relatedArticle.DisplayOrder = model.DisplayOrder;
            _articleService.UpdateRelatedArticle(relatedArticle);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RelatedArticleDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var relatedArticle = _articleService.GetRelatedArticleById(id);
            if (relatedArticle == null)
                throw new ArgumentException("No related article found with the specified id");

            var articleId = relatedArticle.ArticleId1;
            _articleService.DeleteRelatedArticle(relatedArticle);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RelatedArticleAddPopup(int articleId, string btnId, string formId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var ctx = new ArticleSearchContext();
            ctx.LanguageId = _workContext.WorkingLanguage.Id;
            ctx.OrderBy = ArticleSortingEnum.Position;
            ctx.PageSize = _adminAreaSettings.GridPageSize;
            ctx.ShowHidden = true;

            var articles = _articleService.SearchArticles(ctx);

            var model = new ArticleModel.AddRelatedArticleModel();
            model.ArticleId = articleId;
            //categories
            var allCategories = _categoryService.GetAllCategories(showHidden: true);
            var mappedCategories = allCategories.ToDictionary(x => x.Id);
            foreach (var c in allCategories)
            {
                model.AvailableCategories.Add(new SelectListItem() { Text = c.GetCategoryNameWithPrefix(_categoryService, mappedCategories), Value = c.Id.ToString() });
            }

            //sites
            model.AvailableSites.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _siteService.GetAllSites())
                model.AvailableSites.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        [HttpPost]
        public ActionResult RelatedArticleAddPopupList(DataTablesParam dataTableParam, ArticleModel.AddRelatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var ctx = new ArticleSearchContext();

            if (model.SearchCategoryId > 0)
                ctx.CategoryIds.Add(model.SearchCategoryId);

            ctx.SiteId = model.SearchSiteId;
            ctx.Keywords = model.SearchArticleName;
            ctx.LanguageId = _workContext.WorkingLanguage.Id;
            ctx.OrderBy = ArticleSortingEnum.Position;
            ctx.PageIndex = dataTableParam.PageIndex;
            ctx.PageSize = dataTableParam.PageSize;
            ctx.ShowHidden = true;

            var articles = _articleService.SearchArticles(ctx);

            var total = articles.Count;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = articles.Select(x =>
                {
                    var articleModel = x.ToModel();
                    return articleModel;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        [HttpPost]
        public ActionResult RelatedArticleAddPopup(string btnId, string formId, ArticleModel.AddRelatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (model.SelectedArticleIds != null)
            {
                foreach (int id in model.SelectedArticleIds)
                {
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        var existingRelatedArticles = _articleService.GetRelatedArticlesByArticleId1(model.ArticleId);
                        if (existingRelatedArticles.FindRelatedArticle(model.ArticleId, id) == null)
                        {
                            _articleService.InsertRelatedArticle(
                                new RelatedArticle()
                                {
                                    ArticleId1 = model.ArticleId,
                                    ArticleId2 = id,
                                    DisplayOrder = 1
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateAllMutuallyRelatedArticles(int articleId)
        {
            string message = null;

            if (_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null)
                {
                    int count = _articleService.EnsureMutuallyRelatedArticles(articleId);
                    message = T("Admin.Common.CreateMutuallyAssociationsResult", count);
                }
                else
                {
                    message = "No article found with the specified id";
                }
            }
            else
            {
                message = T("Admin.AccessDenied.Title");
            }

            return new JsonResult
            {
                Data = new { Message = message }
            };
        }

        #endregion

        #region Comments

        public ActionResult Comments(int? filterByArticleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            ViewBag.FilterByArticleId = filterByArticleId;

            return View();
        }

        [HttpPost]
        public ActionResult Comments(int? filterByArticleId, DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            IList<ArticleComment> comments;
            if (filterByArticleId.HasValue)
            {
                //filter comments by article
                var articlePost = _articleService.GetArticleById(filterByArticleId.Value);
                comments = articlePost.ArticleComments.OrderBy(bc => bc.CreatedOnUtc).ToList();
            }
            else
            {
                //load all article comments
                comments = _userContentService.GetAllUserContent<ArticleComment>(0,0, null);
            }

            var total = comments.Count;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = comments.PagedForCommand(dataTableParam.PageIndex, dataTableParam.PageSize).Select(articleComment =>
                {
                    var commentModel = new ArticleCommentModel();
                    var user = _userService.GetUserById(articleComment.UserId ?? 0);

                    commentModel.Id = articleComment.Id;
                    commentModel.ArticleId = articleComment.ArticleId;
                    commentModel.ArticleTitle = articleComment.Article.Title;
                    commentModel.UserId = articleComment.UserId ?? 0;
                    commentModel.IpAddress = articleComment.IpAddress;
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(articleComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.Comment = CAF.Infrastructure.Core.Html.HtmlUtils.FormatText(articleComment.CommentText, false, true, false, false, false, false);

                    if (user == null)
                        commentModel.UserName = "".NaIfEmpty();
                    else
                        commentModel.UserName = user.GetFullName();

                    return commentModel;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public ActionResult CommentDelete(int? filterByArticleId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var comment = _userContentService.GetUserContentById(id) as ArticleComment;
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var articlePost = comment.Article;
            _userContentService.DeleteUserContent(comment);
            //update totals
            _articleService.UpdateCommentTotals(articlePost);

            return Json(new { Result = true });
        }


        #endregion

        #region 商品规格属性
        [HttpGet]
        public ActionResult AllProductVariantAttributes(string label, int selectedId)
        {
            var attributes = _productAttributeService.GetAllProductAttributes();

            if (label.HasValue())
            {
                attributes.Insert(0, new ProductAttribute { Name = label, Id = 0 });
            }

            var query =
                from attr in attributes
                select new
                {
                    id = attr.Id.ToString(),
                    text = attr.Name,
                    selected = attr.Id == selectedId
                };

            return new JsonResult { Data = query.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ProductAttributeValueList(int productAttributeId)
        {
            var productAttributeOptions = _productAttributeService.GetProductAttributeOptionsByProductAttribute(productAttributeId);
            var data = productAttributeOptions.Select(x =>
              {
                  return new
                  {
                      id = x.Id,
                      text = x.Name,
                      color = x.ColorSquaresRgb
                  };
              });
            return new JsonResult { Data = data.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region 文档属性
        [HttpGet]
        public ActionResult GetSpecificationAttributes(int categoryId)
        {
            var productrSpecs = _specificationAttributeService.GetCategorySpecificationAttributesById(categoryId);
            var productrSpecsModel = new List<ArticleSpecificationAttributeModel>();
            foreach (var item in productrSpecs)
            {

                var psaModel = new ArticleSpecificationAttributeModel()
                {
                    Id = item.Id,
                    SpecificationAttributeName = item.SpecificationAttributeOption.SpecificationAttribute.Name,
                    SpecificationAttributeOptionName = item.SpecificationAttributeOption.Name,
                    SpecificationAttributeOptionAttributeId = item.SpecificationAttributeOption.SpecificationAttributeId,
                    SpecificationAttributeOptionId = item.SpecificationAttributeOptionId,
                    AllowFiltering = item.AllowFiltering,
                    ShowOnArticlePage = item.ShowOnArticlePage,
                    DisplayOrder = item.DisplayOrder
                };
                productrSpecsModel.Add(psaModel);
            }
            foreach (var attr in productrSpecsModel)
            {
                var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(attr.SpecificationAttributeOptionAttributeId);

                foreach (var option in options)
                {
                    attr.SpecificationAttributeOptions.Add(new ArticleSpecificationAttributeModel.SpecificationAttributeOption
                    {
                        id = option.Id,
                        name = option.Name,
                        text = option.Name,
                        select = attr.SpecificationAttributeOptionId == option.Id
                    });
                }

                attr.SpecificationAttributeOptionsJsonString = JsonConvert.SerializeObject(attr.SpecificationAttributeOptions);
            }

            return PartialView("_SpecificationAttributes", productrSpecsModel);
        }
        #endregion

        #endregion



    }
}