using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    /// <summary>
    /// ��ҳ����˵�
    /// </summary>
    [Serializable]
    public partial class HomeCategory : AuditedBaseEntity
    {
        /// <summary>
        /// ��������
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ����URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// �Ƽ��̼�
        /// </summary>
        public string RequiredVendorIds { get; set; }

        /// <summary>
        /// �Ƽ�Ʒ��
        /// </summary>
        public string RequiredManufacturerIds { get; set; }


        /// <summary>
        /// ����
        /// </summary>
        private ICollection<HomeCategoryItem> _homeCategoryItems;
        /// <summary>
        /// ����
        /// </summary>
        [DataMember]
        public virtual ICollection<HomeCategoryItem> HomeCategoryItems
        {
            get { return _homeCategoryItems ?? (_homeCategoryItems = new HashSet<HomeCategoryItem>()); }
            set { _homeCategoryItems = value; }
        }
    }
}
