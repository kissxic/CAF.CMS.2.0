using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Users;


namespace CAF.WebSite.Mvc.Admin.Models.Users
{

    public class UserArticlePublicNumModel : EntityModelBase
    {
        public UserArticlePublicNumModel()
        {

        }

        [LangResourceDisplayName("Admin.Users.UserArticlePublicNum.Fields.ChannelName","频道分类")]
        public int ChannelId { get; set; }

        [LangResourceDisplayName("Admin.Users.UserRoles.UserArticlePublicNum.ChannelName", "频道分类")]
        public string ChannelName { get; set; }
        /// <summary>
        /// 用于发布数量
        /// </summary>
        [LangResourceDisplayName("Admin.Users.UserRoles.UserArticlePublicNum.PublicTotal", "发布总数")]
        public int PublicTotal { get; set; }

        /// <summary>
        /// 已发布
        /// </summary>
        [LangResourceDisplayName("Admin.Users.UserRoles.UserArticlePublicNum.PublicedTotal", "已发布")]
        public int PublicedTotal { get; set; }

        /// <summary>
        /// 无限制
        /// </summary>
        [LangResourceDisplayName("Admin.Users.UserRoles.UserArticlePublicNum.UnLimit", "无限制(1/0)")]
        public int UnLimit { get; set; }
    }
}