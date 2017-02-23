using Autofac;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CAF.WebSite.Application.Services.PageSettings
{
    public class BannerInfoService : IBannerInfoService
    {
        private readonly IRepository<BannerInfo> _bannerInfoRepository;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IEventPublisher _eventPublisher;

        public BannerInfoService(IRepository<BannerInfo> bannerInfoRepository, 
            AdminAreaSettings adminAreaSettings, 
            IComponentContext ctx,
            IEventPublisher eventPublisher)
        {
            this._bannerInfoRepository = bannerInfoRepository;
            this._adminAreaSettings = adminAreaSettings;
            _eventPublisher = eventPublisher;
        }
        #region Methods

        /// <summary>
        /// Deletes a bannerInfo
        /// </summary>
        /// <param name="bannerInfo">BannerInfo</param>
        public virtual void DeleteBannerInfo(BannerInfo bannerInfo)
        {
            if (bannerInfo == null)
                throw new ArgumentNullException("bannerInfo");

            _bannerInfoRepository.Delete(bannerInfo);

            //event notification
            _eventPublisher.EntityDeleted(bannerInfo);
        }

        /// <summary>
        /// Gets a bannerInfo
        /// </summary>
        /// <param name="bannerInfoId">The bannerInfo identifier</param>
        /// <returns>BannerInfo</returns>
        public virtual BannerInfo GetBannerInfoById(int bannerInfoId)
        {
            if (bannerInfoId == 0)
                return null;

            return _bannerInfoRepository.GetById(bannerInfoId);
        }

        

        /// <summary>
        /// Gets all bannerInfos
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>BannerInfos</returns>
		public virtual IList<BannerInfo> GetAllBannerInfos()
        {
            var query = _bannerInfoRepository.Table;
            query = query.OrderBy(t => t.CreatedOnUtc).ThenBy(t => t.DisplayOrder);

            return query.ToList();
        }

        /// <summary>
        /// Inserts a bannerInfo
        /// </summary>
        /// <param name="bannerInfo">BannerInfo</param>
        public virtual void InsertBannerInfo(BannerInfo bannerInfo)
        {
            if (bannerInfo == null)
                throw new ArgumentNullException("bannerInfo");

            _bannerInfoRepository.Insert(bannerInfo);

            //event notification
            _eventPublisher.EntityInserted(bannerInfo);
        }

        /// <summary>
        /// Updates the bannerInfo
        /// </summary>
        /// <param name="bannerInfo">BannerInfo</param>
        public virtual void UpdateBannerInfo(BannerInfo bannerInfo)
        {
            if (bannerInfo == null)
                throw new ArgumentNullException("bannerInfo");

            _bannerInfoRepository.Update(bannerInfo);

            //event notification
            _eventPublisher.EntityUpdated(bannerInfo);
        }

        #endregion
    }
}
