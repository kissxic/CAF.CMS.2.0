using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;

namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    public class HomeFloorInfo : AuditedBaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string FloorName { get; set; }
        /// <summary>
        /// 默认Tab名
        /// </summary>
        public string DefaultTabName { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 小标题名称
        /// </summary>
        public string SubName { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 楼层风格
        /// </summary>
        public int StyleLevel { get; set; }
        /// <summary>
        /// 关联品牌IDs
        /// </summary>
        public string RelateManufacturerIds { get; set; }
        /// <summary>
        /// 关联栏目分类IDs
        /// </summary>
        public string RelateCategoryIds { get; set; }
        /// <summary>
        /// 关联商品IDs
        /// </summary>
        public string RelateProductIds { get; set; }
        /// <summary>
        /// 关联商品IDs
        /// </summary>
        public string RelateVendorIds { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        private ICollection<FloorSlidesInfo> _floorSlidesInfos;
        /// <summary>
        /// 楼层幻灯片
        /// </summary>
        public virtual ICollection<FloorSlidesInfo> FloorSlidesInfos
        {
            get { return _floorSlidesInfos ?? (_floorSlidesInfos = new HashSet<FloorSlidesInfo>()); }
            protected set { _floorSlidesInfos = value; }
        }
        private ICollection<FloorTopicInfo> _floorTopicInfos;
        /// <summary>
        /// 楼层文本内容
        /// </summary>
        public virtual ICollection<FloorTopicInfo> FloorTopicInfos
        {
            get { return _floorTopicInfos ?? (_floorTopicInfos = new HashSet<FloorTopicInfo>()); }
            protected set { _floorTopicInfos = value; }
        }
    }
}
