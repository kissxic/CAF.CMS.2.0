using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Validators.Articles;
using CAF.WebSite.Mvc.Models.ArticleCatalog;
using CAF.WebSite.Mvc.Models.Media;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core.Domain.Cms.Articles;


namespace CAF.WebSite.Mvc.Models.Articles
{
    [Validator(typeof(ArticlePostValidator))]
    public partial class ArticleOverviewModel : EntityModelBase
    {
        public ArticleOverviewModel()
        {
            Tags = new List<ArticleTagModel>();
            PagingFilteringContext = new ArticleCatalogPagingFilteringModel();
            ProductPrice = new ProductPriceModel();
            IsAvailable = true;
        }
        private ArticleDetailsPictureModel _detailsPictureModel;
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
        public PictureModel PictureModel { get; set; }
        public IList<ArticleTagModel> Tags { get; set; }

        public ArticleCatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public ProductPriceModel ProductPrice { get; set; }
        // codehint: sm-add
        public int AvatarPictureSize { get; set; }
        public bool AllowUsersToUploadAvatars { get; set; }
        public bool HasSampleDownload { get; set; }
        public PictureModel DefaultPictureModel { get; set; }



        #region Nested Classes
   

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
        #endregion
    }

   

}
