using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Mvc.Seller.Models.Common
{

    public partial class MemberInfo
    {
        public MemberInfo()
        {
            UserArticlePublicNumModels = new List<Common.UserArticlePublicNum>();
        }

        public List<UserArticlePublicNum> UserArticlePublicNumModels { get; set; }
    }

    /// <summary>
    /// 用户发布数量
    /// </summary>
    public partial class UserArticlePublicNum
    {
        public string ChannelName { get; set; }

        public int PublicNum { get; set; }

        public int PublicedNum { get; set; }
    }
}