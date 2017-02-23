using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Models.Articles
{
    #region 代理

    public partial class ArticleAgnetModel : EntityModelBase
    {
        /// <summary>
        /// 商家ID
        /// </summary>
        public int? VendorId { get; set; }
        /// <summary>
        /// 主表ID
        /// </summary>
        public int? ArticleId { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string TrueName { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string TelePhone { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 代理区域
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 代理区域
        /// </summary>
        public List<string> CityName { get; set; }
        /// <summary>
        /// 销售渠道
        /// </summary>
        public List<string> DistributionChannelId { get; set; }
        /// <summary>
        /// 代理性质
        /// </summary>
        public List<string> AgentPropertyId { get; set; }
        /// <summary>
        /// 留言信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 查看授权
        /// </summary>
        public int ShowAuthId { get; set; }
      
        /// <summary>
        /// 其他渠道
        /// </summary>
        public string OtherChannel { get; set; }

        public string CheckCode { get; set; }
    }

   
    #endregion

   
}