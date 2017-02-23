

using CAF.WebSite.Application.WebUI.Mvc;
using System;
namespace CAF.WebSite.Mvc.Models.ShopProfile
{
    public partial class VendorModel : EntityModelBase
    {
        /// <summary>
        /// 绑定用户ID
        /// </summary>
        public int UserId { get; set; }

        public string SeName { get; set; }
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

        public string LogoPictureUrl { get; set; }

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
        public string ContactsFax{ get; set; }
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
        /// 店铺入驻步骤
        /// </summary>
        public int StageId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}