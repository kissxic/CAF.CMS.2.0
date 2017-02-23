using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Cms.Channels;

namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 产品分类
    /// </summary>
    [DataContract]
    //[DebuggerDisplay("{Id}: {Name} (Parent: {ParentCategoryId})")]
    public partial class ProductCategory : AuditedBaseEntity, ILocalizedEntity, ISlugSupported
    {

        /// <summary>
        /// 类别标题
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        [DataMember]
        public string Alias { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        [DataMember]
        public string FullName { get; set; }
        /// <summary>
        /// 父类别ID
        /// </summary>
        [DataMember]
        public int ParentCategoryId { get; set; }

        /// <summary>
        /// 层次深度
        /// </summary>
        [DataMember]
        public int Depth { get; set; }
        /// <summary>
        /// 层次深度
        /// </summary>
        [DataMember]
        public string Path { get; set; }
        /// <summary>
        /// 排序数字
        /// </summary>
        [DataMember]
        public int DisplayOrder { get; set; }

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
        /// 详细
        /// </summary>
        [DataMember]
        public string Description { get; set; }


        /// <summary>
        /// 获取或设置一个值,该值指示该实体是否发表
        /// </summary>
        [DataMember]
        public bool Published { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// 客户是否客户可以选择页面大小
        /// </summary>
        [DataMember]
        public bool AllowUsersToSelectPageSize { get; set; }

        /// <summary>
        ///客户可选择的页面大小选项
        /// </summary>
        [DataMember]
        public string PageSizeOptions { get; set; }
        /// <summary>
        /// 价格范围
        /// </summary>
        [DataMember]
        public string PriceRanges { get; set; }
        /// <summary>
        /// 文章列表
        /// </summary>
        private ICollection<Article> _articles;
        /// <summary>
        /// 文章列表
        /// </summary>
        public virtual ICollection<Article> Articles
        {
            get { return _articles ?? (_articles = new HashSet<Article>()); }
            protected set { _articles = value; }
        }



    }
}
