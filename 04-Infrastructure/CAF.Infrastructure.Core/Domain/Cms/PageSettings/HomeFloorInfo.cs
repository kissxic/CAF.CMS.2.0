using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;

namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    public class HomeFloorInfo : AuditedBaseEntity
    {
        /// <summary>
        /// ����
        /// </summary>
        public string FloorName { get; set; }
        /// <summary>
        /// Ĭ��Tab��
        /// </summary>
        public string DefaultTabName { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// С��������
        /// </summary>
        public string SubName { get; set; }
        /// <summary>
        /// �Ƿ���ʾ
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// ¥����
        /// </summary>
        public int StyleLevel { get; set; }
        /// <summary>
        /// ����Ʒ��IDs
        /// </summary>
        public string RelateManufacturerIds { get; set; }
        /// <summary>
        /// ������Ŀ����IDs
        /// </summary>
        public string RelateCategoryIds { get; set; }
        /// <summary>
        /// ������ƷIDs
        /// </summary>
        public string RelateProductIds { get; set; }
        /// <summary>
        /// ������ƷIDs
        /// </summary>
        public string RelateVendorIds { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public int DisplayOrder { get; set; }

        private ICollection<FloorSlidesInfo> _floorSlidesInfos;
        /// <summary>
        /// ¥��õ�Ƭ
        /// </summary>
        public virtual ICollection<FloorSlidesInfo> FloorSlidesInfos
        {
            get { return _floorSlidesInfos ?? (_floorSlidesInfos = new HashSet<FloorSlidesInfo>()); }
            protected set { _floorSlidesInfos = value; }
        }
        private ICollection<FloorTopicInfo> _floorTopicInfos;
        /// <summary>
        /// ¥���ı�����
        /// </summary>
        public virtual ICollection<FloorTopicInfo> FloorTopicInfos
        {
            get { return _floorTopicInfos ?? (_floorTopicInfos = new HashSet<FloorTopicInfo>()); }
            protected set { _floorTopicInfos = value; }
        }
    }
}
