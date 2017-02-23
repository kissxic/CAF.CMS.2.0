using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Application.Services.Channels;



namespace CAF.WebSite.Application.Services.Cms.Channels
{
    public class ChannelEventConsumer : IConsumer<EntityInserted<User>>, IConsumer<EntityInserted<Article>>, IConsumer<EntityDeleted<Article>>
    {
        private readonly IRepository<UserArticlePublicNum> _userArticlePublicNumRepository;
        private readonly IChannelService _channelService;
        private readonly IWorkContext _workContext;

        public ChannelEventConsumer(IChannelService channelService, IWorkContext workContext,
               IRepository<UserArticlePublicNum> userArticlePublicNumRepository)
        {
            this._channelService = channelService;
            this._workContext = workContext;
            this._userArticlePublicNumRepository = userArticlePublicNumRepository;
        }
        public void HandleEvent(EntityInserted<Article> eventMessage)
        {
            var article = eventMessage.Entity;
            if (_workContext.CurrentVendor == null) return;
            var uaPublicNum = this._channelService.GetUserArticlePublicNumByChannelIdAndUserId(eventMessage.Entity.ChannelId, _workContext.CurrentUser.Id);
            if (uaPublicNum != null && !uaPublicNum.UnLimit && uaPublicNum.PublicedTotal < uaPublicNum.PublicTotal)
            {
                uaPublicNum.PublicedTotal = uaPublicNum.PublicedTotal + 1;
                _channelService.UpdateUserArticlePublicNum(uaPublicNum);
            }
        }
        public void HandleEvent(EntityDeleted<Article> eventMessage)
        {
            var article = eventMessage.Entity;
            if (_workContext.CurrentVendor == null) return;
            var uaPublicNum = this._channelService.GetUserArticlePublicNumByChannelIdAndUserId(eventMessage.Entity.ChannelId, _workContext.CurrentUser.Id);
            if (uaPublicNum != null && !uaPublicNum.UnLimit && uaPublicNum.PublicedTotal > 0)
            {
                uaPublicNum.PublicedTotal = uaPublicNum.PublicedTotal - 1;
                _channelService.UpdateUserArticlePublicNum(uaPublicNum);
            }
        }

        public void HandleEvent(EntityInserted<User> eventMessage)
        {
            var user = eventMessage.Entity;

            foreach (var item in _channelService.GetAllChannels())
            {
                var uaPublicNum = new UserArticlePublicNum();
                uaPublicNum.UserId = user.Id;
                uaPublicNum.PublicedTotal = item.LimitNum;
                uaPublicNum.ChannelId = item.Id;
                _channelService.InsertUserArticlePublicNum(uaPublicNum);
            }

        }

    }
}
