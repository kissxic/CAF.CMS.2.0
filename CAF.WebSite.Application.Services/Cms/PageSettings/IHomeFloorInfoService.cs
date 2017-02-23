
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.PageSettings
{
    public interface IHomeFloorInfoService
    {
        /// <summary>
        /// Deletes a homeFloorInfo
        /// </summary>
        /// <param name="homeFloorInfo">HomeFloorInfo</param>
        void DeleteHomeFloorInfo(HomeFloorInfo homeFloorInfo);

        /// <summary>
        /// Gets a homeFloorInfo
        /// </summary>
        /// <param name="homeFloorInfoId">The homeFloorInfo identifier</param>
        /// <returns>HomeFloorInfo</returns>
        HomeFloorInfo GetHomeFloorInfoById(int homeFloorInfoId);


        /// <summary>
        /// Gets all homeFloorInfos
        /// </summary>
        /// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>HomeFloorInfos</returns>
        IList<HomeFloorInfo> GetAllHomeFloorInfos(bool showHidden = false);

        /// <summary>
        /// 更新/新增 楼层信息
        /// </summary>
        /// <param name="homeFloorInfo">HomeFloorInfo</param>
        void UpdateHomeFloorInfo(HomeFloorInfo homeFloorInfo);

      
        //更新排序
        void UpdateHomeFloorSequence(int sourceSequence, int destiSequence);
    }
}
