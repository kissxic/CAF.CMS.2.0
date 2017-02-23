
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 用户内容发布数量
    /// </summary>
     [Serializable]
    public partial class UserArticlePublicNum : BaseEntity
    {

        /// <summary>
        /// 会员
        /// </summary>
        [DataMember]
         public int UserId { get; set; }

        /// <summary>
        /// 频道
        /// </summary>
        [DataMember]
        public int ChannelId { get; set; }

        /// <summary>
        /// 用于发布数量
        /// </summary>
        public int PublicTotal { get; set; }

        /// <summary>
        /// 已发布
        /// </summary>
        public int PublicedTotal { get; set; }

        /// <summary>
        /// 无限制
        /// </summary>
        public bool UnLimit { get; set; }
        /// <summary>
        /// 会员
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// 频道
        /// </summary>
        public virtual Channel Channel { get; set; }
    }
}
