
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.PageSettings
{
	public interface IBannerInfoService
    {
        /// <summary>
        /// Deletes a bannerInfo
        /// </summary>
        /// <param name="bannerInfo">BannerInfo</param>
        void DeleteBannerInfo(BannerInfo bannerInfo);

        /// <summary>
        /// Gets a bannerInfo
        /// </summary>
        /// <param name="bannerInfoId">The bannerInfo identifier</param>
        /// <returns>BannerInfo</returns>
        BannerInfo GetBannerInfoById(int bannerInfoId);

        /// <summary>
        /// Gets all bannerInfos
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>BannerInfos</returns>
		IList<BannerInfo> GetAllBannerInfos();

        /// <summary>
        /// Inserts a bannerInfo
        /// </summary>
        /// <param name="bannerInfo">BannerInfo</param>
        void InsertBannerInfo(BannerInfo bannerInfo);

        /// <summary>
        /// Updates the bannerInfo
        /// </summary>
        /// <param name="bannerInfo">BannerInfo</param>
        void UpdateBannerInfo(BannerInfo bannerInfo);
    }
}
