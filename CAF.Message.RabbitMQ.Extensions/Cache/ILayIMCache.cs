using System.Web;
using CAF.Message.Distributed.Extensions.Models;
using CAF.Message.Distributed.Extensions;

namespace CAF.Message.Distributed.Extensions.Core
{
    public partial interface ILayIMCache
    {
        void CacheUserAfterLogin(string userId);
        BaseListResult GetUserBaseInfo();
        string GetCurrentUserId();
        string GetUserFriendList(string userId);
        void ApplyToClient(ApplyEntity apply);
        bool IsOnline(string userId);
        void OperateOnlineUser(OnlineUser user, bool isDelete = false);
    }
}