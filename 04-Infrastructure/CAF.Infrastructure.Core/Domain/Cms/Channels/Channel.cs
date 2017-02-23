
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Members;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Channels
{
    /// <summary>
    /// 系统频道表
    /// </summary>
    [Serializable]
    public partial class Channel : AuditedBaseEntity, IMemberGradeMappingSupported
    {
        /// <summary>
        ///系统频道表主体ID
        /// </summary>
        [DataMember]
        public Guid ChannelGuid { get; set; }

        /// <summary>
        /// 栏目分类显示类型
        /// </summary>
        [DataMember]
        public int CategoryShowTypeId { get; set; }
        /// <summary>
        /// 商品分类显示类型
        /// </summary>
        [DataMember]
        public int ProductCategoryShowTypeId { get; set; }
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

        /// <summary>
        ///频道名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///频道标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        ///Font Awesome 图标名称
        /// </summary>
        [DataMember]
        public string IconName { get; set; }

        /// <summary>
        ///会员限制数量
        /// </summary>
        [DataMember]
        public int LimitNum { get; set; }

        /// <summary>
        ///排序数字
        /// </summary>
        [DataMember]
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Gets or sets a value of used category template identifier
        /// </summary>
        [DataMember]
        public int ModelTemplateId { get; set; }
        /// <summary>
        /// Gets or sets a value of used category template identifier
        /// </summary>
        [DataMember]
        public int DetailModelTemplateId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        [DataMember]
        public bool LimitedToMemberGrades { get; set; }
        /// <summary>
        /// Gets or sets the product type
        /// </summary>
        [DataMember]
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
        [DataMember]
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
        /// <summary>
        /// 扩展属性
        /// </summary>
        private ICollection<ExtendedAttribute> _extendedAttributes;
        /// <summary>
        /// 扩展属性
        /// </summary>
        [DataMember]
        public virtual ICollection<ExtendedAttribute> ExtendedAttributes
        {
            get { return _extendedAttributes ?? (_extendedAttributes = new HashSet<ExtendedAttribute>()); }
            protected set { _extendedAttributes = value; }
        }
        /// <summary>
        /// 内容类别
        /// </summary>
        private ICollection<ArticleCategory> _articleCategory;
        /// <summary>
        /// 类别
        /// </summary>
        public virtual ICollection<ArticleCategory> ArticleCategorys
        {
            get { return _articleCategory ?? (_articleCategory = new HashSet<ArticleCategory>()); }
            protected set { _articleCategory = value; }
        }

        /// <summary>
        /// 产品分类
        /// </summary>
        private ICollection<ProductCategory> _productCategorys;
        /// <summary>
        /// 产品分类
        /// </summary>
        [DataMember]
        public virtual ICollection<ProductCategory> ProductCategorys
        {
            get { return _productCategorys ?? (_productCategorys = new HashSet<ProductCategory>()); }
            set { _productCategorys = value; }
        }
        private ICollection<ChannelSpecificationAttribute> _channelSpecificationAttributes;
        /// <summary>
        /// Gets or sets the product specification attribute
        /// </summary>
        [DataMember]
        public virtual ICollection<ChannelSpecificationAttribute> ChannelSpecificationAttributes
        {
            get { return _channelSpecificationAttributes ?? (_channelSpecificationAttributes = new HashSet<ChannelSpecificationAttribute>()); }
            protected set { _channelSpecificationAttributes = value; }
        }
    }
}
