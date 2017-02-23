using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Cms.Agents
{
    /// <summary>
    /// 机构代理留言
    /// </summary>
    public partial class AgencyContent : UserContent
    {
      
        /// <summary>
        /// 主表ID
        /// </summary>
        [DataMember]
        public int? ArticleId { get; set; }
        public virtual Article Article { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        [DataMember]
        public string TrueName { get; set; }
       /// <summary>
       /// 电话
       /// </summary>
        [DataMember]
        public string TelePhone { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        [DataMember]
        public string QQ { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember]
        public string Email { get; set; }
        /// <summary>
        /// 代理区域
        /// </summary>
        [DataMember]
        public string ProvinceName { get; set; }
        /// <summary>
        /// 代理区域
        /// </summary>
        [DataMember]
        public string CityName { get; set; }
        /// <summary>
        /// 销售渠道
        /// </summary>
        [DataMember]
        public string DistributionChannelId { get; set; }
        /// <summary>
        /// 代理性质
        /// </summary>
        [DataMember]
        public string AgentPropertyId { get; set; }
        /// <summary>
        /// 留言信息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// 查看授权
        /// </summary>
        [DataMember]
        public int ShowAuthId { get; set; }
        [DataMember]
        public ShowAuthType ShowAuthType
        {
            get
            {
                return (ShowAuthType)this.ShowAuthId;
            }
            set
            {
                this.ShowAuthId = (int)value;
            }
        }
        /// <summary>
        /// 其他渠道
        /// </summary>
        [DataMember]
        public string OtherChannel { get; set; }
        

    }
}
