using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Validators.Members;

namespace CAF.WebSite.Mvc.Admin.Models.Members
{
    [Validator(typeof(MemberGradeValidator))]
    public partial class MemberGradeModel : EntityModelBase
    {
        [LangResourceDisplayName("Admin.Configuration.Members.Fields.SystemName","系统名称")]
        [AllowHtml]
        public string SystemName { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Members.Fields.GradeName", "等级名称")]
        [AllowHtml]
        public string GradeName { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Members.Fields.Integral", "发布数量")]
        public int Integral { get; set; }


        [LangResourceDisplayName("Admin.Configuration.Members.Fields.Comment", "备注")]
        [AllowHtml]
        public virtual string Comment { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Members.Fields.Deleted", "物流删除")]
        [AllowHtml]
        public bool Deleted { get; set; }
        [LangResourceDisplayName("Admin.Configuration.Members.Fields.IsDefault", "默认")]
        public bool IsDefault { get; set; }

    }
}