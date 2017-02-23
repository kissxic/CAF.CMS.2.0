
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
        /// 店铺名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 店铺邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 店铺说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 店铺Logo
        /// </summary>
        public int? PictureId { get; set; }
        /// <summary>
        /// Gets or sets the picture
        /// </summary>
        [DataMember]
        public virtual Picture Picture { get; set; }

        /// <summary>
        /// 店铺管理员说明
        /// </summary>
        public string AdminComment { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 公司所在地省市区ID
        /// </summary>
        public int CompanyRegionId { get; set; }

        /// <summary>
        /// 公司网址
        /// </summary>
        public string CompanyWebSite { get; set; }
        /// <summary>
        /// 公司详细地址
        /// </summary>
        public string CompanyAddress { get; set; }
        /// <summary>
        /// 公司电话
        /// </summary>
        public string CompanyPhone { get; set; }
        /// <summary>
        /// 员工总数
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
        /// 注册资金
        /// </summary>
        public decimal CompanyRegisteredCapital { get; set; }
        /// <summary>
        /// 法定代表人
        /// </summary>
        public string LegalPerson { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string ContactsName { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string ContactsPhone { get; set; }
        /// <summary>
        /// 公司传真
        /// </summary>
        public string ContactsFax { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string ContactsEmail { get; set; }
        /// <summary>
        /// 银行开户名
        /// </summary>
        public string BankAccountName { get; set; }
        /// <summary>
        /// 公司银行账号
        /// </summary>
        public string BankAccountNumber { get; set; }
        /// <summary>
        /// 开户银行支行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 支行联行号
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 开户银行所在地
        /// </summary>
        public int BankRegionId { get; set; }
        /// <summary>
        /// 电子版
        /// </summary>
        public int BankPictureId { get; set; }
        /// <summary>
        /// 店铺等级
        /// </summary>
        public int VendorGradeId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 店铺有效期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 店铺入驻步骤
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
        /// 店铺状态
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
