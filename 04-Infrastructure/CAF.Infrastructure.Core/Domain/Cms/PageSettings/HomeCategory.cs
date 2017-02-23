using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    /// <summary>
    /// 首页分类菜单
    /// </summary>
    [Serializable]
    public partial class HomeCategory : AuditedBaseEntity
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分类URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 推荐商家
        /// </summary>
        public string RequiredVendorIds { get; set; }

        /// <summary>
        /// 推荐品牌
        /// </summary>
        public string RequiredManufacturerIds { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        private ICollection<HomeCategoryItem> _homeCategoryItems;
        /// <summary>
        /// 分类
        /// </summary>
        [DataMember]
        public virtual ICollection<HomeCategoryItem> HomeCategoryItems
        {
            get { return _homeCategoryItems ?? (_homeCategoryItems = new HashSet<HomeCategoryItem>()); }
            set { _homeCategoryItems = value; }
        }
    }
}
