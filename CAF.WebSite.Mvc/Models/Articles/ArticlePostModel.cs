using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Validators.Articles;
using CAF.WebSite.Mvc.Models.ArticleCatalog;
using CAF.WebSite.Mvc.Models.Media;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Models.ShopProfile;

namespace CAF.WebSite.Mvc.Models.Articles
{
    [Validator(typeof(ArticlePostValidator))]
    public partial class ArticlePostModel : EntityModelBase
    {
        private ArticleDetailsPictureModel _detailsPictureModel;
        public ArticlePostModel()
        {
            Tags = new List<ArticleTagModel>();
            Comments = new List<ArticleCommentModel>();
            AddNewComment = new AddArticleCommentModel();
            PagingFilteringContext = new ArticleCatalogPagingFilteringModel();
            ArticleExtendedAttributes = new List<ArticleExtendedAttributeModel>();
            ProductVariantAttributes = new List<ProductVariantAttributeModel>();
            Manufacturers = new List<ManufacturerOverviewModel>();
            SpecificationAttributeModels = new List<ProductSpecificationModel>();
            ProductPrice = new ProductPriceModel();
            VendorModel = new VendorModel();
            IsAvailable = true;
             
        }
        //picture(s)
        public ArticleDetailsPictureModel DetailsPictureModel
        {
            get
            {
                if (_detailsPictureModel == null)
                    _detailsPictureModel = new ArticleDetailsPictureModel();
                return _detailsPictureModel;
            }
        }
        public string ShortContent { get; set; }
        public string FullContent { get; set; }
        public string SeName { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string ModelTemplateViewPath { get; set; }
        public string Title { get; set; }
        public string LinkUrl { get; set; }
        public int VendorId { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImgUrl { get; set; }
        public int Click { get; set; }
        public int Status { get; set; }
        public bool IsTop { get; set; }
        public bool IsRed { get; set; }
        public bool IsHot { get; set; }
        public bool IsSlide { get; set; }
        public bool IsNew { get; set; }
        public bool AllowComments { get; set; }
        public bool DisplayArticleReviews { get; set; }
        public int NumberOfComments { get; set; }
        public int ApprovedCommentCount { get; set; }
        public int NotApprovedCommentCount { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsLogin { get; set; }
        public int ProductCategroyId { get; set; }
        public int CategroyId { get; set; }
        public int ChannelId { get; set; }

        public string Sku { get; set; }
        public decimal WeightValue { get; set; }
        public string Weight { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public bool DisplayDeliveryTime { get; set; }
        public string DeliveryTimeName { get; set; }
        public string DeliveryTimeHexValue { get; set; }
        public int StockQuantity { get; set; }
        public string StockAvailability { get; set; }
        public bool IsAvailable { get; set; }

        public string Body { get; set; }
        public string Author { get; set; }
        public string PostCreatedOnStr { get; set; }
        public int? PreId { get; set; }
        public int? NextId { get; set; }

        public VendorModel VendorModel { get; set; }
        public PictureModel PictureModel { get; set; }
        public IList<ArticleTagModel> Tags { get; set; }

        public IList<ArticleCommentModel> Comments { get; set; }
        public AddArticleCommentModel AddNewComment { get; set; }
        public ArticleCatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public ProductPriceModel ProductPrice { get; set; }
        public ProductVariantAttributeCombination SelectedCombination { get; set; }
        public IList<ArticleExtendedAttributeModel> ArticleExtendedAttributes { get; set; }
        public IList<ProductVariantAttributeModel> ProductVariantAttributes { get; set; }
        public IList<ManufacturerOverviewModel> Manufacturers { get; set; }
        public IList<ProductSpecificationModel> SpecificationAttributeModels { get; set; }
        // codehint: sm-add
        public int AvatarPictureSize { get; set; }
        public bool AllowUsersToUploadAvatars { get; set; }
        public bool HasSampleDownload { get; set; }
        public PictureModel DefaultPictureModel { get; set; }

        public ArticlePrevAndNextModel ArticlePNModel { get; set; }

        #region Nested Classes
        public partial class ArticlePrevAndNextModel
        {
            public int? PrevId { get; set; }
            public string PrevSeName { get; set; }
            public string PrevTitle { get; set; }
            public int? NextId { get; set; }
            public string NextSeName { get; set; }
            public string NextTitle { get; set; }
        }
        public partial class ArticleBreadcrumbModel : ModelBase
        {
            public ArticleBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<MenuItem>();
            }

            public int ArticleId { get; set; }
            public string ArticleName { get; set; }
            public string ArticleSeName { get; set; }
            public bool OnlyCurrentCategory { get; set; }
            public IList<MenuItem> CategoryBreadcrumb { get; set; }
        }

        public partial class ArticleExtendedAttributeModel : EntityModelBase
        {
            public ArticleExtendedAttributeModel()
            {
                Values = new List<ArticleExtendedAttributeValueModel>();
                AllowedFileExtensions = new List<string>();
            }
            public int ArticleId { get; set; }

            public int ProductAttributeId { get; set; }

            public string Name { get; set; }

            public string Alias { get; set; }
            
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
            /// <summary>
            /// Begin year for datepicker
            /// </summary>
            public int? BeginYear { get; set; }
            /// <summary>
            /// End year for datepicker
            /// </summary>
            public int? EndYear { get; set; }
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
            public string SeName { get; set; }
            public string Alias { get; set; }
            public string ColorSquaresRgb { get; set; }
            public string PriceAdjustment { get; set; }
            public decimal PriceAdjustmentValue { get; set; }
            public int QuantityInfo { get; set; }
            public bool IsPreSelected { get; set; }
            public string ImageUrl { get; set; }
        }


        public partial class ProductVariantAttributeModel : EntityModelBase
        {
            public ProductVariantAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<ProductVariantAttributeValueModel>();
            }

            public int ArticleId { get; set; }
           

            public int ProductAttributeId { get; set; }

            public string Alias { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public bool IsDisabled { get; set; }

            /// <summary>
            /// Selected value for textboxes
            /// </summary>
            public string TextValue { get; set; }
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
            /// <summary>
            /// Begin year for datepicker
            /// </summary>
            public int? BeginYear { get; set; }
            /// <summary>
            /// End year for datepicker
            /// </summary>
            public int? EndYear { get; set; }
            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<ProductVariantAttributeValueModel> Values { get; set; }

        }

        public partial class ProductVariantAttributeValueModel : EntityModelBase
        {
            public string Name { get; set; }
            public string SeName { get; set; }
            public string Alias { get; set; }
            public string ColorSquaresRgb { get; set; }
            public string PriceAdjustment { get; set; }
            public decimal PriceAdjustmentValue { get; set; }
            public int QuantityInfo { get; set; }
            public bool IsPreSelected { get; set; }
            public string ImageUrl { get; set; }
        }

        public partial class ProductPriceModel : ModelBase
        {
            public string OldPrice { get; set; }

            public string Price { get; set; }
            public string PriceWithDiscount { get; set; }

            public decimal PriceValue { get; set; }
            public decimal PriceWithDiscountValue { get; set; }
            public bool HasDiscount { get; set; }
            public bool CustomerEntersPrice { get; set; }

            public int ProductId { get; set; }

            public bool HidePrices { get; set; }

            public string NoteWithDiscount { get; set; }
            public string NoteWithoutDiscount { get; set; }
            public bool DisableBuyButton { get; set; }

        }
        #endregion
    }

    public partial class ArticleDetailsPictureModel : ModelBase
    {
        public ArticleDetailsPictureModel()
        {
            PictureModels = new List<PictureModel>();
        }

        public string Name { get; set; }
        public string AlternateText { get; set; }
        public bool DefaultPictureZoomEnabled { get; set; }
        public string PictureZoomType { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public IList<PictureModel> PictureModels { get; set; }
        public int GalleryStartIndex { get; set; }
    }


}
