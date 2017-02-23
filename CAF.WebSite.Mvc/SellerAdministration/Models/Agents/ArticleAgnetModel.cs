using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Seller.Models.Articles
{
    #region 代理

    public partial class ArticleAgnetModel : EntityModelBase
    {
        /// <summary>
        /// 主表ID
        /// </summary>
        public int? ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string Url{ get; set; }
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
        public string CityName { get; set; }
        /// <summary>
        /// 销售渠道
        /// </summary>
        public string DistributionChannelId { get; set; }
        /// <summary>
        /// 代理性质
        /// </summary>
        public string AgentPropertyId { get; set; }
        /// <summary>
        /// 留言信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 查看授权
        /// </summary>
        public int ShowAuthId { get; set; }
        public string ShowAuth { get; set; }
        /// <summary>
        /// 其他渠道
        /// </summary>
        public string OtherChannel { get; set; }

        public string CheckCode { get; set; }


        /// <summary>
        /// 用户
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Creator of this entity.
        /// </summary>
        public virtual long? CreatedUserID { get; set; }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public virtual DateTime? ModifiedOnUtc { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public virtual long? ModifiedUserID { get; set; }
    }


    #endregion


}