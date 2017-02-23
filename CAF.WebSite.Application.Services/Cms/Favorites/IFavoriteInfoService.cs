using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Favorites;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Favorites
{
    /// <summary>
    /// FavoriteInfo service
    /// </summary>
    public partial interface IFavoriteInfoService
    {
        /// <summary>
        /// Deletes a favoriteInfo
        /// </summary>
        /// <param name="favoriteInfo">FavoriteInfo</param>
        void DeleteFavoriteInfo(FavoriteInfo favoriteInfo);

        /// <summary>
        /// Gets all favoriteInfos
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>FavoriteInfo collection</returns>
        IList<FavoriteInfo> GetAllFavoriteInfos(bool showHidden = false);

        /// <summary>
        /// Gets all favoriteInfos
        /// </summary>
        /// <param name="favoriteInfoName">FavoriteInfo name</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>FavoriteInfo collection</returns>
        IList<FavoriteInfo> GetAllFavoriteInfos(string favoriteInfoName, bool showHidden = false);
        
        /// <summary>
        /// Gets all favoriteInfos
        /// </summary>
        /// <param name="favoriteInfoName">FavoriteInfo name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>FavoriteInfos</returns>
        IPagedList<FavoriteInfo> GetAllFavoriteInfos(string favoriteInfoName,
            int pageIndex, int pageSize, bool showHidden = false);

        /// <summary>
        /// Gets a favoriteInfo
        /// </summary>
        /// <param name="favoriteInfoId">FavoriteInfo identifier</param>
        /// <returns>FavoriteInfo</returns>
        FavoriteInfo GetFavoriteInfoById(int favoriteInfoId);

        /// <summary>
        /// Inserts a favoriteInfo
        /// </summary>
        /// <param name="favoriteInfo">FavoriteInfo</param>
        void InsertFavoriteInfo(FavoriteInfo favoriteInfo);

        /// <summary>
        /// Updates the favoriteInfo
        /// </summary>
        /// <param name="favoriteInfo">FavoriteInfo</param>
        void UpdateFavoriteInfo(FavoriteInfo favoriteInfo);

        
    }
}
