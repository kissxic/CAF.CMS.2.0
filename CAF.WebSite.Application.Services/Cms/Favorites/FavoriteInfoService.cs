using System;
using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Caching;
using CAF.WebSite.Application.Services.Favorites;
using CAF.Infrastructure.Core.Domain.Cms.Favorites;

namespace CAF.WebSite.Application.Services.Favorites
{
    /// <summary>
    /// FavoriteInfo service
    /// </summary>
    public partial class FavoriteInfoService : IFavoriteInfoService
    {
        #region Constants

        #endregion

        #region Fields

        private readonly IRepository<FavoriteInfo> _favoriteInfoRepository;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestCache _requestCache;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache"></param>
        /// <param name="favoriteInfoRepository"></param>
        /// <param name="workContext"></param>
        /// <param name="siteContext"></param>
        /// <param name="eventPublisher"></param>
        public FavoriteInfoService(IRequestCache requestCache,
            IRepository<FavoriteInfo> favoriteInfoRepository,
            IWorkContext workContext,
            ISiteContext siteContext,
            IEventPublisher eventPublisher)
        {
            _requestCache = requestCache;
            _favoriteInfoRepository = favoriteInfoRepository;
            _workContext = workContext;
            _siteContext = siteContext;
            _eventPublisher = eventPublisher;

            this.QuerySettings = DbQuerySettings.Default;
        }

        public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a favoriteInfo
        /// </summary>
        /// <param name="favoriteInfo">FavoriteInfo</param>
        public virtual void DeleteFavoriteInfo(FavoriteInfo favoriteInfo)
        {
            if (favoriteInfo == null)
                throw new ArgumentNullException("favoriteInfo");

            _favoriteInfoRepository.Delete(favoriteInfo);
 
        }

        /// <summary>
        /// Gets all favoriteInfos
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>FavoriteInfo collection</returns>
        public virtual IList<FavoriteInfo> GetAllFavoriteInfos(bool showHidden = false)
        {
            return GetAllFavoriteInfos(null, showHidden);
        }

        /// <summary>
        /// Gets all favoriteInfos
        /// </summary>
        /// <param name="favoriteInfoName">FavoriteInfo name</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>FavoriteInfo collection</returns>
        public virtual IList<FavoriteInfo> GetAllFavoriteInfos(string favoriteInfoName, bool showHidden = false)
        {
            var query = _favoriteInfoRepository.Table;

            var favoriteInfos = query.ToList();
            return favoriteInfos;
        }

        /// <summary>
        /// Gets all favoriteInfos
        /// </summary>
        /// <param name="favoriteInfoName">FavoriteInfo name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>FavoriteInfos</returns>
        public virtual IPagedList<FavoriteInfo> GetAllFavoriteInfos(string favoriteInfoName,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            var favoriteInfos = GetAllFavoriteInfos(favoriteInfoName, showHidden);
            return new PagedList<FavoriteInfo>(favoriteInfos, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets a favoriteInfo
        /// </summary>
        /// <param name="favoriteInfoId">FavoriteInfo identifier</param>
        /// <returns>FavoriteInfo</returns>
        public virtual FavoriteInfo GetFavoriteInfoById(int favoriteInfoId)
        {
           
            if (favoriteInfoId == 0)
                return null;
       
         
                return _favoriteInfoRepository.GetById(favoriteInfoId);
           
        }

        /// <summary>
        /// Inserts a favoriteInfo
        /// </summary>
        /// <param name="favoriteInfo">FavoriteInfo</param>
        public virtual void InsertFavoriteInfo(FavoriteInfo favoriteInfo)
        {
            if (favoriteInfo == null)
                throw new ArgumentNullException("favoriteInfo");

            _favoriteInfoRepository.Insert(favoriteInfo);

            //event notification
            _eventPublisher.EntityInserted(favoriteInfo);
        }

        /// <summary>
        /// Updates the favoriteInfo
        /// </summary>
        /// <param name="favoriteInfo">FavoriteInfo</param>
        public virtual void UpdateFavoriteInfo(FavoriteInfo favoriteInfo)
        {
            if (favoriteInfo == null)
                throw new ArgumentNullException("favoriteInfo");

            _favoriteInfoRepository.Update(favoriteInfo);
           
            //event notification
            _eventPublisher.EntityUpdated(favoriteInfo);
        }


        #endregion
    }
}
