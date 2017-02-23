
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Caching;

namespace CAF.WebSite.Application.Services.Channels
{

    public partial class ChannelService : IChannelService
    {

        #region Constants
        private const string CHANNELS_PATTERN_KEY = "cms.channel.";
        private const string CHANNELS_BY_ID_KEY = "cms.channel.id-{0}";

        #endregion

        #region Fields
        private readonly IRepository<Channel> _channelRepository;
        private readonly IRequestCache _requestCache;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<UserArticlePublicNum> _userArticlePublicNumRepository;
        #endregion

        #region Ctor


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache"></param>
        /// <param name="channelRepository"></param>
        /// <param name="eventPublisher"></param>
        public ChannelService(IRequestCache requestCache,
            IRepository<Channel> channelRepository,
            IEventPublisher eventPublisher,
            IRepository<UserArticlePublicNum> userArticlePublicNumRepository)
        {
            this._requestCache = requestCache;
            this._channelRepository = channelRepository;
            this._eventPublisher = eventPublisher;
            this._userArticlePublicNumRepository = userArticlePublicNumRepository;
        }

        #endregion

        #region Methods

        #region Utilities

        #endregion

        #region Channels

        public IPagedList<Channel> GetAllChannels(int pageIndex, int pageSize)
        {
            var query = _channelRepository.Table;
            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var Channels = new PagedList<Channel>(query, pageIndex, pageSize);
            return Channels;
        }

        public void DeleteChannel(Channel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            // channel.IsDeleted = true;
            // UpdateChannel(channel);

            _channelRepository.Delete(channel);

            //event notification
            _eventPublisher.EntityDeleted(channel);
        }

        public Channel GetChannelById(int channelId)
        {
            if (channelId == 0)
                return null;

            string key = string.Format(CHANNELS_BY_ID_KEY, channelId);
            return _requestCache.Get(key, () =>
            {
                return _channelRepository.GetById(channelId);
            });
        }


        public IList<Channel> GetChannelsByIds(int[] channelIds)
        {
            if (channelIds == null || channelIds.Length == 0)
                return new List<Channel>();

            var query = from c in _channelRepository.Table
                        where channelIds.Contains(c.Id)
                        select c;
            var channels = query.ToList();
            //sort by passed identifiers
            var sortedChannel = new List<Channel>();
            foreach (int id in channelIds)
            {
                var channel = channels.Find(x => x.Id == id);
                if (channel != null)
                    sortedChannel.Add(channel);
            }
            return sortedChannel;
        }

        /// <summary>
        /// Gets all channels
        /// </summary>
        /// <returns>Channels</returns>
        public IList<Channel> GetAllChannels()
        {
            var query = from s in _channelRepository.Table
                        orderby s.CreatedOnUtc
                        select s;
            var channels = query.ToList();
            return channels;
        }

        public IQueryable<Channel> GetAllChannelQ()
        {
            var query = _channelRepository.Table;
            return query;
        }

        public void InsertChannel(Channel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            _channelRepository.Insert(channel);

            //event notification
            _eventPublisher.EntityInserted(channel);
        }


        /// <summary>
        /// Updates the channel
        /// </summary>
        /// <param name="channel">Channel</param>
        public virtual void UpdateChannel(Channel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            _channelRepository.Update(channel);

            //event notification
            _eventPublisher.EntityUpdated(channel);
        }


        #endregion

        #region UserArticlePublicNum

        /// <summary>
        /// Deletes a Article picture
        /// </summary>
        /// <param name="UserArticlePublicNum">Article picture</param>
        public virtual void DeleteUserArticlePublicNum(UserArticlePublicNum userArticlePublicNum)
        {
            if (userArticlePublicNum == null)
                throw new ArgumentNullException("userArticlePublicNum");

            this._userArticlePublicNumRepository.Delete(userArticlePublicNum);

            //event notification
            _eventPublisher.EntityDeleted(userArticlePublicNum);
        }
        public virtual UserArticlePublicNum GetUserArticlePublicNumsById(int id)
        {
          return  this._userArticlePublicNumRepository.GetById(id);
        }

        /// <summary>
        /// Gets a Article pictures by Article identifier
        /// </summary>
        /// <param name="ArticleId">The Article identifier</param>
        /// <returns>Article pictures</returns>
        public virtual IList<UserArticlePublicNum> GetUserArticlePublicNumsByUserId(int userId)
        {
            var query = from pp in this._userArticlePublicNumRepository.Table
                        where pp.UserId == userId
                        select pp;
            var userArticlePublicNums = query.ToList();
            return userArticlePublicNums;
        }
        public virtual IList<UserArticlePublicNum> GetUserArticlePublicNumsByChannelId(int channelId)
        {
            var query = from pp in this._userArticlePublicNumRepository.Table
                        where pp.ChannelId == channelId
                        select pp;
            var userArticlePublicNums = query.ToList();
            return userArticlePublicNums;
        }


        public virtual UserArticlePublicNum GetUserArticlePublicNumByChannelIdAndUserId(int channelId, int userId)
        {
            if (channelId == 0)
                return null;
            var query = from pp in this._userArticlePublicNumRepository.Table
                        where pp.ChannelId == channelId && pp.UserId == userId
                        select pp;
            var userArticlePublicNum = query.FirstOrDefault();
            return userArticlePublicNum;
        }

        public virtual bool IsPublicArticle(int channelId, int userId)
        {
            if (channelId == 0)
                return false;
            var query = from pp in this._userArticlePublicNumRepository.Table
                        where pp.ChannelId == channelId && pp.UserId == userId
                        select pp;
            var userArticlePublicNum = query.FirstOrDefault();
            if (userArticlePublicNum.UnLimit)
                return true;
            return userArticlePublicNum.PublicedTotal < userArticlePublicNum.PublicTotal;
        }

        /// <summary>
        /// Inserts a Article picture
        /// </summary>
        /// <param name="UserArticlePublicNum">Article picture</param>
        public virtual void InsertUserArticlePublicNum(UserArticlePublicNum userArticlePublicNum)
        {
            if (userArticlePublicNum == null)
                throw new ArgumentNullException("UserArticlePublicNum");

            this._userArticlePublicNumRepository.Insert(userArticlePublicNum);

            //event notification
            _eventPublisher.EntityInserted(userArticlePublicNum);
        }

        /// <summary>
        /// Updates a Article picture
        /// </summary>
        /// <param name="UserArticlePublicNum">Article picture</param>
        public virtual void UpdateUserArticlePublicNum(UserArticlePublicNum userArticlePublicNum)
        {
            if (userArticlePublicNum == null)
                throw new ArgumentNullException("userArticlePublicNum");

            this._userArticlePublicNumRepository.Update(userArticlePublicNum);

            //event notification
            _eventPublisher.EntityUpdated(userArticlePublicNum);
        }

        #endregion
        #endregion



    }
}
