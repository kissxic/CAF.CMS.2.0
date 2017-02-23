using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AutoMapper;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Seller.Validators.Articles;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Seller.Models.Sites;
using CAF.WebSite.Mvc.Seller.Models.Users;
using CAF.Infrastructure.Core.Domain.Cms.Channels;

namespace CAF.WebSite.Mvc.Seller.Models.Articles
{
    [Validator(typeof(ArticleValidator))]
    public class ArticleModel : TabbableModel, ILocalizedModel<ArticleLocalizedModel>
    {
        public ArticleModel()
        {
            Locales = new List<ArticleLocalizedModel>();
            ArticlePictureModels = new List<ArticlePictureModel>();
            AddPictureModel = new ArticlePictureModel();
            AvailableModelTemplates = new List<SelectListItem>();
            AvailableArticleTags = new List<SelectListItem>();
            ArticleExtendedAttributes = new List<ArticleExtendedAttributeModel>();
            AvailableCategorys = new List<SelectListItem>();
            AvailableDeliveryTimes = new List<SelectListItem>();
            ProductVariantAttributeCombinationSku = new ProductVariantAttributeCombinationSkuModel();
            AddSpecificationAttributeModel = new AddProductSpecificationAttributeModel();
            ArticleSpecificationAttributeModels = new List<ArticleSpecificationAttributeModel>();

            AvailableStates = new List<SelectListItem>();
            AvailableCitys = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
        }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ID")]
        public override int Id { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.CategoryId")]
        [AllowHtml]
        public int? CategoryId { get; set; }
        public List<SelectListItem> AvailableCategorys { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.CategoryName")]
        public string CategoryBreadcrumb { get; set; }

        //picture thumbnail
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }
        public bool NoThumb { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Picture")]
        [AllowHtml]
        [UIHint("Picture")]
        public int PictureId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsPasswordProtected")]
        public bool IsPasswordProtected { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }
        /// <summary>
        /// 地址连接
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Url")]
        [AllowHtml]
        public string Url { get; set; }
        /// <summary>
        /// 外部链接
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.LinkUrl")]
        [AllowHtml]
        public string LinkUrl { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ImgUrl")]
        [AllowHtml]
        public string ImgUrl { get; set; }
        /// <summary>
        /// SEO标题
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }
        /// <summary>
        /// SEO关健字
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }
        /// <summary>
        /// 内容摘要
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ShortContent")]
        [AllowHtml]
        public string ShortContent { get; set; }
        /// <summary>
        /// 详细内容
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.FullContent")]
        [AllowHtml]
        public string FullContent { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Click")]
        public int Click { get; set; }
        /// <summary>
        /// 状态0正常1未审核2锁定
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Status")]
        public int StatusId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Status")]
        public string StatusName { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsTop")]
        public bool IsTop { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsRed")]
        public bool IsRed { get; set; }
        /// <summary>
        /// 是否热门
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsHot")]
        public bool IsHot { get; set; }
        /// <summary>
        /// 是否幻灯片
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsSlide")]
        public bool IsSlide { get; set; }
        /// <summary>
        /// 是否管理员发布0不是1是
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsSys")]
        public bool IsSys { get; set; }
        /// <summary>
        /// 是否允许评论
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.AllowComments")]
        public bool AllowComments { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.AllowUserReviews")]
        public bool AllowUserReviews { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Comments")]
        public int Comments { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.StartDate")]
        public DateTime? StartDate { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.EndDate")]
        public DateTime? EndDate { get; set; }

        [LangResourceDisplayName("Common.CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        [LangResourceDisplayName("Common.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Author")]
        [AllowHtml]
        public string Author { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
        //频道Id
        public int ChannelId { get; set; }

        #region 下载


        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsDownload")]
        public bool IsDownload { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Download")]
        [UIHint("Download")]
        public int DownloadId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.UnlimitedDownloads")]
        public bool UnlimitedDownloads { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MaxNumberOfDownloads")]
        public int MaxNumberOfDownloads { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DownloadExpirationDays")]
        public int? DownloadExpirationDays { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DownloadActivationType")]
        public int DownloadActivationTypeId { get; set; }
        #endregion



        #region 省市区

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.StateProvince", "省")]
        public int? StateProvinceId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.StateProvince")]
        [AllowHtml]
        public string StateProvinceName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.City", "市")]
        public int? CityId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.City")]
        [AllowHtml]
        public string CityeName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Fields.District", "区")]
        public int? DistrictId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Fields.District")]
        [AllowHtml]
        public string DistrictName { get; set; }

        public bool StateProvinceEnabled { get; set; }
        public bool CityEnabled { get; set; }
        public bool DistrictEnabled { get; set; }

        public IList<SelectListItem> AvailableStates { get; set; }
        public IList<SelectListItem> AvailableCitys { get; set; }
        public IList<SelectListItem> AvailableDistricts { get; set; }

        #endregion

        #region 标签显示属性
        /// <summary>
        /// 显示扩展信息
        /// </summary>
        public bool ShowExtendedAttribute { get; set; }
        /// <summary>
        /// 显示内容属性
        /// </summary>
        public bool ShowSpecificationAttributes { get; set; }
        /// <summary>
        /// 显示库存
        /// </summary>
        public bool ShowInventory { get; set; }
        /// <summary>
        /// 显示价格
        /// </summary>
        public bool ShowPrice { get; set; }
        /// <summary>
        /// 显示规格属性
        /// </summary>
        public bool ShowAttributes { get; set; }
        /// <summary>
        /// 显示推荐
        /// </summary>
        public bool ShowPromotion { get; set; }

        public int CategoryShowTypeId { get; set; }
        public CategoryShowType CategoryShowType
        {
            get
            {
                return (CategoryShowType)this.CategoryShowTypeId;
            }
            set
            {
                this.CategoryShowTypeId = (int)value;
            }
        }

        public int ProductCategoryShowTypeId { get; set; }
        public ProductCategoryShowType ProductCategoryShowType
        {
            get
            {
                return (ProductCategoryShowType)this.ProductCategoryShowTypeId;
            }
            set
            {
                this.ProductCategoryShowTypeId = (int)value;
            }
        }
        #endregion

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ArticleTags")]
        [AllowHtml]
        public string ArticleTags { get; set; }
        public IList<SelectListItem> AvailableArticleTags { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ModelTemplate")]
        [AllowHtml]
        public int ModelTemplateId { get; set; }
        public IList<SelectListItem> AvailableModelTemplates { get; set; }

        public IList<ArticleLocalizedModel> Locales { get; set; }

        //pictures
        public ArticlePictureModel AddPictureModel { get; set; }
        public IList<ArticlePictureModel> ArticlePictureModels { get; set; }
        //Site mapping
        [LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
        public bool LimitedToSites { get; set; }
        [LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
        public List<SiteModel> AvailableSites { get; set; }
        public int[] SelectedSiteIds { get; set; }
        //ACL (customer roles)
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.AclUserRoles")]
        public List<UserRoleModel> AvailableUserRoles { get; set; }
        public int[] SelectedUserRoleIds { get; set; }

        /// <summary>
        /// 显示内容图片
        /// </summary>
        public bool DisplayArticlePictures { get; set; }

        //扩展属性
        public string ExtendedAttributeInfo { get; set; }
        public IList<ArticleExtendedAttributeModel> ArticleExtendedAttributes { get; set; }

        public bool ShowSiteContentShare { get; set; }
        /// <summary>
        /// 内容属性
        /// </summary>
        public AddProductSpecificationAttributeModel AddSpecificationAttributeModel { get; set; }
        public IList<ArticleSpecificationAttributeModel> ArticleSpecificationAttributeModels { get; set; }
        /// <summary>
        /// 内容属性字符串
        /// </summary>
        public string SpaValues { get; set; }

        #region 商品属性
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ProductCategory", "商品分类")]
        public int? ProductCategoryId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ProductCategory", "商品分类")]
        public string ProductCategoryBreadcrumb { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Manufacturer", "供应商")]
        public int? ManufacturerId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ProductCategory", "供应商")]
        public string ManufacturerBreadcrumb { get; set; }

        /// <summary>
        /// 规格字符串
        /// </summary>
        public string ProductSpecifications { get; set; }

        public ProductVariantAttributeCombinationSkuModel ProductVariantAttributeCombinationSku { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Sku", "产品SKU")]
        [AllowHtml]
        public string Sku { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DeliveryTime", "发货时间")]
        public int? DeliveryTimeId { get; set; }
        public IList<SelectListItem> AvailableDeliveryTimes { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.IsFreeShipping", "免邮")]
        public bool IsFreeShipping { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.QuantityUnit", "单位")]
        public int? QuantityUnitId { get; set; }
        public IList<SelectListItem> AvailableQuantityUnits { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Weight", "重量")]
        public decimal Weight { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Length", "长度")]
        public decimal Length { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Width", "宽度")]
        public decimal Width { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Height", "产品类型")]
        public decimal Height { get; set; }
        #endregion

        #region 价格
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DisableBuyButton", "隐藏购买按钮")]
        public bool DisableBuyButton { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Price", "价格")]
        public decimal Price { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.OldPrice", "旧价格")]
        public decimal OldPrice { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ProductCost", "成本")]
        public decimal ProductCost { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.SpecialPrice", "特别价格")]
        public decimal? SpecialPrice { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.SpecialPriceStartDateTimeUtc", "开始时间")]
        public DateTime? SpecialPriceStartDateTimeUtc { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.SpecialPriceEndDateTimeUtc", "结束时间")]
        public DateTime? SpecialPriceEndDateTimeUtc { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.CustomerEntersPrice", "自定义价格")]
        public bool CustomerEntersPrice { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MinimumCustomerEnteredPrice", "最小输入价格")]
        public decimal MinimumCustomerEnteredPrice { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MaximumCustomerEnteredPrice", "最大输入价格")]
        public decimal MaximumCustomerEnteredPrice { get; set; }
        #endregion

        #region 库存信息

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.StockQuantity", "数量")]
        public int StockQuantity { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DisplayStockAvailability", "显示可用库存")]
        public bool DisplayStockAvailability { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.DisplayStockQuantity", "显示库存")]
        public bool DisplayStockQuantity { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MinStockQuantity", "最小库存数量")]
        public int MinStockQuantity { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.LowStockActivity", "最低库存操作")]
        public int LowStockActivityId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.NotifyAdminForQuantityBelow", "通知管理员的最低数量")]
        public int NotifyAdminForQuantityBelow { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.AllowBackInStockSubscriptions", "库存到货通知")]
        public bool AllowBackInStockSubscriptions { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.OrderMinimumQuantity", "订单最低库存")]
        public int OrderMinimumQuantity { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.OrderMaximumQuantity", "订单最大库存")]
        public int OrderMaximumQuantity { get; set; }
        #endregion

        #region Nested classes
        public class AddProductSpecificationAttributeModel : ModelBase
        {
            public AddProductSpecificationAttributeModel()
            {
                AvailableAttributes = new List<SelectListItem>();
                AvailableOptions = new List<SelectListItem>();
                AllowFiltering = true;
            }

            [LangResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttribute", "名称")]
            public int SpecificationAttributeId { get; set; }

            [LangResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttributeOption", "属性值")]
            public int SpecificationAttributeOptionId { get; set; }

            [LangResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.AllowFiltering", "允许过滤")]
            public bool AllowFiltering { get; set; }

            [LangResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.ShowOnProductPage", "在内容页面显示")]
            public bool ShowOnProductPage { get; set; }

            [LangResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.DisplayOrder", "排序")]
            public int DisplayOrder { get; set; }

            public IList<SelectListItem> AvailableAttributes { get; set; }
            public IList<SelectListItem> AvailableOptions { get; set; }
        }

        public class ArticlePictureModel : EntityModelBase
        {
            public int ArticleId { get; set; }
            [UIHint("Picture")]
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Pictures.Fields.Picture")]
            public int PictureId { get; set; }
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Pictures.Fields.DisplayOrder")]
            public string SeoFilename { get; set; }
        }

        public class ArticleCategoryModel : EntityModelBase
        {
            [LangResourceDisplayName("Admin.ContentManagement.Articles.Categories.Fields.Category")]
            [UIHint("ArticleCategory")]
            public string Category { get; set; }

            public int ArticleId { get; set; }

            public int CategoryId { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.Categories.Fields.IsFeaturedArticle")]
            public bool IsFeaturedArticle { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.Categories.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public class RelatedArticleModel : EntityModelBase
        {
            public int ArticleId2 { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.RelatedArticles.Fields.Article")]
            public string Article2Name { get; set; }


            [LangResourceDisplayName("Admin.ContentManagement.Articles.RelatedArticles.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }


            [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Published")]
            public bool Article2Published { get; set; }
        }

        public class AddRelatedArticleModel : ModelBase
        {
            public AddRelatedArticleModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableSites = new List<SelectListItem>();
            }
            public List<ArticleModel> Articles { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchArticleName")]
            [AllowHtml]
            public string SearchArticleName { get; set; }

            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchCategory")]
            public int SearchCategoryId { get; set; }


            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SearchSite")]
            public int SearchSiteId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableSites { get; set; }

            public int ArticleId { get; set; }

            public int[] SelectedArticleIds { get; set; }


        }

        public partial class ArticleExtendedAttributeModel : EntityModelBase
        {
            public ArticleExtendedAttributeModel()
            {
                Values = new List<ArticleExtendedAttributeValueModel>();
                AllowedFileExtensions = new List<string>();
            }

            public string Name { get; set; }

            public string DefaultValue { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }
            public AttributeControlType AttributeControlType { get; set; }
            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public IList<ArticleExtendedAttributeValueModel> Values { get; set; }
        }

        public partial class ArticleExtendedAttributeValueModel : EntityModelBase
        {
            public string Name { get; set; }

            //public string PriceAdjustment { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }

    public class ArticleLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortContent { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.FullDescription")]
        [AllowHtml]
        public string FullContent { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }



    }


}