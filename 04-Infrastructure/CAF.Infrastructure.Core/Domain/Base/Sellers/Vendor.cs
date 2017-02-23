
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Sellers
{
    /// <summary>
    /// Represents a vendor
    /// </summary>
    public partial class Vendor : BaseEntity, ILocalizedEntity, ISlugSupported
    {
        private ICollection<VendorNote> _vendorNotes;

        /// <summary>
        /// ��������
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ����˵��
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ����Logo
        /// </summary>
        public int? PictureId { get; set; }
        /// <summary>
        /// Gets or sets the picture
        /// </summary>
        [DataMember]
        public virtual Picture Picture { get; set; }

        /// <summary>
        /// ���̹���Ա˵��
        /// </summary>
        public string AdminComment { get; set; }
        /// <summary>
        /// ��˾����
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// ��˾���ڵ�ʡ����ID
        /// </summary>
        public int CompanyRegionId { get; set; }

        /// <summary>
        /// ��˾��ַ
        /// </summary>
        public string CompanyWebSite { get; set; }
        /// <summary>
        /// ��˾��ϸ��ַ
        /// </summary>
        public string CompanyAddress { get; set; }
        /// <summary>
        /// ��˾�绰
        /// </summary>
        public string CompanyPhone { get; set; }
        /// <summary>
        /// Ա������
        /// </summary>
        public int CompanyEmployeeCountId { get; set; }
        /// <summary>
        /// Gets or sets the password format
        /// </summary>
        public CompanyEmployeeCount CompanyEmployeeCount
        {
            get { return (CompanyEmployeeCount)CompanyEmployeeCountId; }
            set { this.CompanyEmployeeCountId = (int)value; }
        }
        /// <summary>
        /// ע���ʽ�
        /// </summary>
        public decimal CompanyRegisteredCapital { get; set; }
        /// <summary>
        /// ����������
        /// </summary>
        public string LegalPerson { get; set; }
        /// <summary>
        /// ��ϵ������
        /// </summary>
        public string ContactsName { get; set; }
        /// <summary>
        /// ��ϵ�˵绰
        /// </summary>
        public string ContactsPhone { get; set; }
        /// <summary>
        /// ��˾����
        /// </summary>
        public string ContactsFax { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string ContactsEmail { get; set; }
        /// <summary>
        /// ���п�����
        /// </summary>
        public string BankAccountName { get; set; }
        /// <summary>
        /// ��˾�����˺�
        /// </summary>
        public string BankAccountNumber { get; set; }
        /// <summary>
        /// ��������֧������
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// ֧�����к�
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// �����������ڵ�
        /// </summary>
        public int BankRegionId { get; set; }
        /// <summary>
        /// ���Ӱ�
        /// </summary>
        public int BankPictureId { get; set; }
        /// <summary>
        /// ���̵ȼ�
        /// </summary>
        public int VendorGradeId { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// ������Ч��
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// ������פ����
        /// </summary>
        public int StageId { get; set; }

        /// <summary>
        /// Gets or sets the password format
        /// </summary>
        public VendorStage VendorStage
        {
            get { return (VendorStage)StageId; }
            set { this.StageId = (int)value; }
        }
        /// <summary>
        /// ����״̬
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }


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
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        public string PageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets vendor notes
        /// </summary>
        public virtual ICollection<VendorNote> VendorNotes
        {
            get { return _vendorNotes ?? (_vendorNotes = new List<VendorNote>()); }
            protected set { _vendorNotes = value; }
        }

    }
}
