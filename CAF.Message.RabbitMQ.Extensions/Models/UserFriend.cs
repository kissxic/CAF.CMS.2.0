using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Message.Distributed.Extensions.Models
{
    /// <summary>
    /// 用户好友
    /// </summary>
    public class UserFriend : Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 好友列表 1,2,3,4 （不知道好友列表长度会不会超过限制，如果超过限制，就不能用string存储了）
        /// </summary>
        public string list { get; set; }
    }
}
