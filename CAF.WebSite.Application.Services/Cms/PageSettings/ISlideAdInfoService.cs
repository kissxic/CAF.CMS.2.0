
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.PageSettings
{
    public interface ISlideAdInfoService
    {
        /// <summary>
        /// Deletes a slideAdInfo
        /// </summary>
        /// <param name="slideAdInfo">SlideAdInfo</param>
        void DeleteSlideAdInfo(SlideAdInfo slideAdInfo);

        /// <summary>
        /// Gets a slideAdInfo
        /// </summary>
        /// <param name="slideAdInfoId">The slideAdInfo identifier</param>
        /// <returns>SlideAdInfo</returns>
        SlideAdInfo GetSlideAdInfoById(int slideAdInfoId);

        /// <summary>
        /// Gets all slideAdInfos
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>SlideAdInfos</returns>
		IList<SlideAdInfo> GetAllSlideAdInfos(int slideAdTypeId = 0);

        /// <summary>
        /// Inserts a slideAdInfo
        /// </summary>
        /// <param name="slideAdInfo">SlideAdInfo</param>
        void InsertSlideAdInfo(SlideAdInfo slideAdInfo);

        /// <summary>
        /// Updates the slideAdInfo
        /// </summary>
        /// <param name="slideAdInfo">SlideAdInfo</param>
        void UpdateSlideAdInfo(SlideAdInfo slideAdInfo);



        /// <summary>
        /// Deletes a imageAdInfo
        /// </summary>
        /// <param name="imageAdInfo">ImageAdInfo</param>
        void DeleteImageAdInfo(ImageAdInfo imageAdInfo);

        /// <summary>
        /// Gets a imageAdInfo
        /// </summary>
        /// <param name="imageAdInfoId">The imageAdInfo identifier</param>
        /// <returns>ImageAdInfo</returns>
        ImageAdInfo GetImageAdInfoById(int imageAdInfoId);
        /// <summary>
        /// Gets a imageAdInfo
        /// </summary>
        /// <param name="postionId">The postion identifier</param>
        /// <returns>ImageAdInfo</returns>
        ImageAdInfo GetImageAdInfoByPostionId(int postionId);
        /// <summary>
        /// Gets all imageAdInfos
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>ImageAdInfos</returns>
		IList<ImageAdInfo> GetAllImageAdInfos();

        /// <summary>
        /// Inserts a imageAdInfo
        /// </summary>
        /// <param name="imageAdInfo">ImageAdInfo</param>
        void InsertImageAdInfo(ImageAdInfo imageAdInfo);

        /// <summary>
        /// Updates the imageAdInfo
        /// </summary>
        /// <param name="imageAdInfo">ImageAdInfo</param>
        void UpdateImageAdInfo(ImageAdInfo imageAdInfo);
    }
}
