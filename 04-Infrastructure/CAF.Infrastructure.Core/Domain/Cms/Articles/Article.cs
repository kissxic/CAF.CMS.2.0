using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Manufacturers;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 文章内容实体类
    /// </summary>
    [Serializable]
    public partial class Article : AuditedBaseEntity, ILocalizedEntity, ISlugSupported, ISoftDeletable, IAclSupported, ISiteMappingSupported, IMergedData, IVendor
    {
        private int _stockQuantity;
        private decimal _price;
        private decimal _length;
        private decimal _width;
        private decimal _height;
        private string _sku;
        private int? _deliveryTimeId;
        private int? _quantityUnitId;
        public bool MergedDataIgnore { get; set; }
        public Dictionary<string, object> MergedDataValues { get; set; }

        #region 基础属性

        /// <summary>
        ///文章主体ID
        /// </summary>
        [DataMember]
        public Guid ArticleGuid { get; set; }

        /// <summary>
        /// 频道ID
        /// </summary>
        [DataMember]
        public int ChannelId { get; set; }
        /// <summary>
        /// 类别ID
        /// </summary>
        [DataMember]
        public int? CategoryId { get; set; }
        public ArticleCategory ArticleCategory { get; set; }
      
        public int ArticleTypeId { get; set; }
      
        /// <summary>
        /// 标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        /// <summary>
        /// 获取或设置值指示是否这个话题是密码保护
        /// </summary>
        public bool IsPasswordProtected { get; set; }
        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 外部链接
        /// </summary>
        [DataMember]
        public string LinkUrl { get; set; }
        /// <summary>
        /// 关联图片地址
        /// </summary>
        [DataMember]
        public string ImgUrl { get; set; }
        /// <summary>
        ///获取或设置图像标识符
        /// </summary>
        [DataMember]
        public int? PictureId { get; set; }
        /// <summary>
        /// Gets or sets the picture
        /// </summary>
        [DataMember]
        public virtual Picture Picture { get; set; }
        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }
        /// <summary>
        /// 内容摘要
        /// </summary>
        [DataMember]
        public string ShortContent { get; set; }
        /// <summary>
        /// 详细内容
        /// </summary>
        [DataMember]
        public string FullContent { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        [DataMember]
        public int Click { get; set; }
        /// <summary>
        /// 状态 0正常1未审核2锁定
        public int StatusId { get; set; }
        /// <summary>
        /// Gets or sets the product type
        /// </summary>
        [DataMember]
        public ArticleStatus ArticleStatus
        {
            get
            {
                return (ArticleStatus)this.StatusId;
            }
            set
            {
                this.StatusId = (int)value;
            }
        }
        /// <summary>
        /// Gets or sets the password format
        /// </summary>
        public StatusFormat StatusFormat
        {
            get { return (StatusFormat)StatusId; }
            set { this.StatusId = (int)value; }
        }
        /// <summary>
        /// 阅读权限
        /// </summary>
        public string GroupidsView { get; set; }
        /// <summary>
        /// 关联投票ID
        /// </summary>
        [DataMember]
        public int VoteId { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        [DataMember]
        public bool IsTop { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        [DataMember]
        public bool IsRed { get; set; }
        /// <summary>
        /// 是否热门
        /// </summary>
        [DataMember]
        public bool IsHot { get; set; }
        /// <summary>
        /// 是否幻灯片
        /// </summary>
        [DataMember]
        public bool IsSlide { get; set; }
        /// <summary>
        /// 是否管理员发布0不是1是
        /// </summary>
        [DataMember]
        public bool IsSys { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string Author  { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article is download
        /// </summary>
        [DataMember]
        public bool IsDownload { get; set; }

        /// <summary>
        /// Gets or sets the download identifier
        /// </summary>
        [DataMember]
        public int DownloadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this downloadable article can be downloaded unlimited number of times
        /// </summary>
        [DataMember]
        public bool UnlimitedDownloads { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of downloads
        /// </summary>
        [DataMember]
        public int MaxNumberOfDownloads { get; set; }
        /// <summary>
        /// Gets or sets the download count
        /// </summary>
        [DataMember]
        public int DownloadCount { get; set; }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool AllowComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article allows customer reviews
        /// </summary>
        [DataMember]
        public bool AllowUserReviews { get; set; }

        /// <summary>
        /// Gets or sets the rating sum (approved reviews)
        /// </summary>
        [DataMember]
        public int ApprovedRatingSum { get; set; }

        /// <summary>
        /// Gets or sets the rating sum (not approved reviews)
        /// </summary>
        [DataMember]
        public int NotApprovedRatingSum { get; set; }

        /// <summary>
        /// Gets or sets the total rating votes (approved reviews)
        /// </summary>
        [DataMember]
        public int ApprovedTotalReviews { get; set; }

        /// <summary>
        /// Gets or sets the total rating votes (not approved reviews)
        /// </summary>
        [DataMember]
        public int NotApprovedTotalReviews { get; set; }

        /// <summary>
        /// Gets or sets the total number of approved comments
        /// <remarks>The same as if we run newsItem.NewsComments.Where(n => n.IsApproved).Count()
        /// We use this property for performance optimization (no SQL command executed)
        /// </remarks>
        /// </summary>
        public int ApprovedCommentCount { get; set; }
        /// <summary>
        /// Gets or sets the total number of not approved comments
        /// <remarks>The same as if we run newsItem.NewsComments.Where(n => !n.IsApproved).Count()
        /// We use this property for performance optimization (no SQL command executed)
        /// </remarks>
        /// </summary>
        public int NotApprovedCommentCount { get; set; }

        /// <summary>
        /// Gets or sets a value of used article template identifier
        /// </summary>
        [DataMember]
        public int ModelTemplateId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToSites { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        [DataMember]
        public bool SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets the blog post start date and time
        /// </summary>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the blog post end date and time
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Review rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Review helpful votes total
        /// </summary>
        public int HelpfulYesTotal { get; set; }

        /// <summary>
        /// Review not helpful votes total
        /// </summary>
        public int HelpfulNoTotal { get; set; }
        /// <summary>
        /// Gets or sets the product type
        /// </summary>
        [DataMember]
        public ArticleType ArticleType
        {
            get
            {
                return (ArticleType)this.ArticleTypeId;
            }
            set
            {
                this.ArticleTypeId = (int)value;
            }
        }

        public string ArticleTypeLabelHint
        {
            get
            {
                switch (ArticleType)
                {
                    case ArticleType.Simple:
                        return "smnet-hide";
                    case ArticleType.Product:
                        return "success";
                   
                    default:
                        return "";
                }
            }
        }


        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
        [DataMember]
        public int? StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the City identifier
        /// </summary>
        [DataMember]
        public int? CityId { get; set; }
        /// <summary>
        /// Gets or sets the District identifier
        /// </summary>
        [DataMember]
        public int? DistrictId { get; set; }
        /// <summary>
        /// Gets or sets the state/province
        /// </summary>
        [DataMember]
        public virtual StateProvince StateProvince { get; set; }
        /// Gets or sets the City
        /// </summary>
        [DataMember]
        public virtual City City { get; set; }
        /// Gets or sets the District
        /// </summary>
        [DataMember]
        public virtual District District { get; set; }

        [DataMember]
        public int VendorId { get; set; }
        #endregion

        #region 商品信息
        /// <summary>
        /// 商品类别ID
        /// </summary>
        [DataMember]
        public int? ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }

        /// <summary>
        /// 商品品牌
        /// </summary>
        [DataMember]
        public int? ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the SKU
        /// </summary>
        [DataMember]
        public string Sku
        {
            [DebuggerStepThrough]
            get
            {
                return this.GetMergedDataValue<string>("Sku", _sku);
            }
            set
            {
                _sku = value;
            }
        }
        /// <summary>
        /// 发货时间
        /// </summary>
        [DataMember]
        public int? DeliveryTimeId
        {
            [DebuggerStepThrough]
            get
            {
                return this.GetMergedDataValue<int?>("DeliveryTimeId", _deliveryTimeId);
            }
            set
            {
                _deliveryTimeId = value;
            }
        }
        [DataMember]
        public virtual DeliveryTime DeliveryTime { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity is ship enabled
        /// </summary>
        [DataMember]
        public bool IsShipEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity is free shipping
        /// </summary>
        [DataMember]
        public bool IsFreeShipping { get; set; }

        /// <summary>
        /// 数量单位
        /// </summary>
        [DataMember]
        public int? QuantityUnitId
        {
            [DebuggerStepThrough]
            get
            {
                return this.GetMergedDataValue<int?>("QuantityUnitId", _quantityUnitId);
            }
            set
            {
                _quantityUnitId = value;
            }
        }

        [DataMember]
        public virtual QuantityUnit QuantityUnit { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        [DataMember]
        public decimal Weight { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        [DataMember]
        public decimal Length
        {
            [DebuggerStepThrough]
            get
            {
                return this.GetMergedDataValue<decimal>("Length", _length);
            }
            set
            {
                _length = value;
            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        [DataMember]
        public decimal Width
        {
            [DebuggerStepThrough]
            get
            {
                return this.GetMergedDataValue<decimal>("Width", _width);
            }
            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// 高度
        /// </summary>
        [DataMember]
        public decimal Height
        {
            [DebuggerStepThrough]
            get
            {
                return this.GetMergedDataValue<decimal>("Height", _height);
            }
            set
            {
                _height = value;
            }
        }
        #endregion

        #region 商品库存信息
        /// <summary>
        /// 库存数量
        /// </summary>
        [DataMember]
        public int StockQuantity
        {
            [DebuggerStepThrough]
            get
            {
                return this.GetMergedDataValue("StockQuantity", _stockQuantity);
            }
            set
            {
                _stockQuantity = value;
            }
        }

        /// <summary>
        /// 显示可用库存
        /// </summary>
        [DataMember]
        public bool DisplayStockAvailability { get; set; }

        /// <summary>
        /// 显示库存数量
        /// </summary>
        [DataMember]
        public bool DisplayStockQuantity { get; set; }

        /// <summary>
        /// 最小库存数量
        /// </summary>
        [DataMember]
        public int MinStockQuantity { get; set; }

        /// <summary>
        /// 低库存操作，如不显示购买按钮、隐藏发布信息、无操作
        /// </summary>
        [DataMember]
        public int LowStockActivityId { get; set; }

        /// <summary>
        /// 低于以下的数量通知管理员
        /// </summary>
        [DataMember]
        public int NotifyAdminForQuantityBelow { get; set; }
        /// <summary>
        /// 库存到货通知
        /// </summary>
        [DataMember]
        public bool AllowBackInStockSubscriptions { get; set; }

        /// <summary>
		/// 订单最小数量
		/// </summary>
		[DataMember]
        public int OrderMinimumQuantity { get; set; }

        /// <summary>
        /// 订单最大数量
        /// </summary>
        [DataMember]
        public int OrderMaximumQuantity { get; set; }
        #endregion

        #region 商品价格
        /// <summary>
        /// 禁用购买按钮
        /// </summary>
        [DataMember]
        public bool DisableBuyButton { get; set; }
        /// <summary>
        ///  价格
        /// </summary>
        [DataMember]
        public decimal Price
        {
            [DebuggerStepThrough]
            get
            {
                return this.GetMergedDataValue<decimal>("Price", _price);
            }
            set
            {
                _price = value;
            }
        }

        /// <summary>
        /// 旧价格
        /// </summary>
        [DataMember]
        public decimal OldPrice { get; set; }

        /// <summary>
        ///产品成本
        /// </summary>
        [DataMember]
        public decimal ProductCost { get; set; }

        /// <summary>
        /// 特殊价格
        /// </summary>
        [DataMember]
        public decimal? SpecialPrice { get; set; }

        /// <summary>
        /// 特殊的价格开始日期
        /// </summary>
        [DataMember]
        public DateTime? SpecialPriceStartDateTimeUtc { get; set; }

        /// <summary>
        /// 特殊的价格结束日期
        /// </summary>
        [DataMember]
        public DateTime? SpecialPriceEndDateTimeUtc { get; set; }

        /// <summary>
        /// 客户输入价格咨询
        /// </summary>
        [DataMember]
        public bool CustomerEntersPrice { get; set; }

        /// <summary>
        /// 输入最小价格
        /// </summary>
        [DataMember]
        public decimal MinimumCustomerEnteredPrice { get; set; }

        /// <summary>
        /// 输入最大价格
        /// </summary>
        [DataMember]
        public decimal MaximumCustomerEnteredPrice { get; set; }

        #endregion

        #region 关联信息


        /// <summary>
        /// 评论
        /// </summary>
        private ICollection<ArticleComment> _articleComments;
        /// <summary>
        /// 图片相册
        /// </summary>
        private ICollection<ArticleAlbum> _articleAlbums;
        /// <summary>
        /// 内容附件
        /// </summary>
        private ICollection<ArticleAttach> _articleAttachs;
        /// <summary>
        /// 标签
        /// </summary>
        private ICollection<ArticleTag> _articleTags;
        /// <summary>
        /// 赞
        /// </summary>
        private ICollection<ArticleReview> _articleReviews;
        //商品规格属性
        private ICollection<ProductVariantAttribute> _productVariantAttributes;
        //商品规格属性值
        private ICollection<ProductVariantAttributeCombination> _productVariantAttributeCombinations;
        /// <summary>
        /// 文档属性
        /// </summary>
        private ICollection<ArticleSpecificationAttribute> _articleSpecificationAttributes;
        /// <summary>
        /// 评论
        /// </summary>
        public virtual ICollection<ArticleComment> ArticleComments
        {
            get { return _articleComments ?? (_articleComments = new HashSet<ArticleComment>()); }
            protected set { _articleComments = value; }
        }
        /// <summary>
        /// 图片相册
        /// </summary>
        public virtual ICollection<ArticleAlbum> ArticleAlbum
        {
            get { return _articleAlbums ?? (_articleAlbums = new HashSet<ArticleAlbum>()); }
            protected set { _articleAlbums = value; }
        }
        /// <summary>
        /// 内容附件
        /// </summary>
        public virtual ICollection<ArticleAttach> ArticleAttachs
        {
            get { return _articleAttachs ?? (_articleAttachs = new HashSet<ArticleAttach>()); }
            protected set { _articleAttachs = value; }
        }
        /// <summary>
        /// Gets or sets the article tags
        /// </summary>
        [DataMember]
        public virtual ICollection<ArticleTag> ArticleTags
        {
            get { return _articleTags ?? (_articleTags = new HashSet<ArticleTag>()); }
            protected set { _articleTags = value; }
        }

        /// <summary>
        /// Gets or sets the collection of article reviews
        /// </summary>
        public virtual ICollection<ArticleReview> ArticleReviews
        {
            get { return _articleReviews ?? (_articleReviews = new HashSet<ArticleReview>()); }
            protected set { _articleReviews = value; }
        }

        /// <summary>
        /// Gets or sets the product attributes
        /// </summary>
        [DataMember]
        public virtual ICollection<ProductVariantAttribute> ProductVariantAttributes
        {
            get { return _productVariantAttributes ?? (_productVariantAttributes = new HashSet<ProductVariantAttribute>()); }
            protected set { _productVariantAttributes = value; }
        }
        /// <summary>
        /// Gets or sets the product attribute combinations
        /// </summary>
        [DataMember]
        public virtual ICollection<ProductVariantAttributeCombination> ProductVariantAttributeCombinations
        {
            get { return _productVariantAttributeCombinations ?? (_productVariantAttributeCombinations = new List<ProductVariantAttributeCombination>()); }
            protected set { _productVariantAttributeCombinations = value; }
        }
        /// <summary>
        /// Gets or sets the product specification attribute
        /// </summary>
        [DataMember]
        public virtual ICollection<ArticleSpecificationAttribute> ArticleSpecificationAttributes
        {
            get { return _articleSpecificationAttributes ?? (_articleSpecificationAttributes = new HashSet<ArticleSpecificationAttribute>()); }
            protected set { _articleSpecificationAttributes = value; }
        }
        #endregion
    }
}
