
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.PageSettings
{
	public interface IHomeCategoryService
    {
        /// <summary>
        /// Deletes a homeCategory
        /// </summary>
        /// <param name="homeCategory">HomeCategory</param>
        void DeleteHomeCategory(HomeCategory homeCategory);

        /// <summary>
        /// Gets a homeCategory
        /// </summary>
        /// <param name="homeCategoryId">The homeCategory identifier</param>
        /// <returns>HomeCategory</returns>
        HomeCategory GetHomeCategoryById(int homeCategoryId);

        /// <summary>
        /// Gets all homeCategorys
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>HomeCategorys</returns>
		IList<HomeCategory> GetAllHomeCategorys();

        /// <summary>
        /// Inserts a homeCategory
        /// </summary>
        /// <param name="homeCategory">HomeCategory</param>
        void InsertHomeCategory(HomeCategory homeCategory);

        /// <summary>
        /// Inserts a homeCategoryItems
        /// </summary>
        /// <param name="homeCategoryId"></param>
        /// <param name="homeCategoryItems"></param>
        void InsertHomeCategoryItem(int homeCategoryId, List<HomeCategoryItem> homeCategoryItems);
        /// <summary>
        /// Updates the homeCategory
        /// </summary>
        /// <param name="homeCategory">HomeCategory</param>
        void UpdateHomeCategory(HomeCategory homeCategory);






        /// <summary>
        /// Deletes a homeCategory
        /// </summary>
        /// <param name="homeCategory">HomeCategoryItem</param>
        void DeleteHomeCategoryItem(HomeCategoryItem homeCategoryItem);

        /// <summary>
        /// Gets a homeCategoryItem
        /// </summary>
        /// <param name="homeCategoryItemId">The homeCategoryItem identifier</param>
        /// <returns>HomeCategoryItem</returns>
        HomeCategoryItem GetHomeCategoryItemById(int homeCategoryItemId);

        /// <summary>
        /// Gets all homeCategoryItems
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>HomeCategoryItems</returns>
		IList<HomeCategoryItem> GetAllHomeCategoryItems();

        IList<HomeCategoryItem> GetAllHomeCategoryItemsByHomeCategoryId(int homeCategoryId);

        /// <summary>
        /// Inserts a homeCategoryItem
        /// </summary>
        /// <param name="homeCategoryItem">HomeCategoryItem</param>
        void InsertHomeCategoryItem(HomeCategoryItem homeCategoryItem);

        /// <summary>
        /// Updates the homeCategoryItem
        /// </summary>
        /// <param name="homeCategoryItem">HomeCategoryItem</param>
        void UpdateHomeCategoryItem(HomeCategoryItem homeCategoryItem);
    }
}
