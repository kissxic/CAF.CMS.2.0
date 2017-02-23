using Autofac;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.PageSettings
{
    public class HomeFloorInfoService : IHomeFloorInfoService
    {
        private readonly IRepository<HomeFloorInfo> _homeFloorInfoRepository;
        private readonly IRepository<FloorSlidesInfo> _floorSlidesInfoRepository;
        private readonly IRepository<FloorSlideDetailsInfo> _floorSlideDetailsInfoRepository;
        private readonly IRepository<FloorTopicInfo> _floorTopicInfoRepository;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IEventPublisher _eventPublisher;

        public HomeFloorInfoService(IRepository<HomeFloorInfo> homeFloorInfoRepository,
            IRepository<FloorSlidesInfo> floorSlidesInfoRepository,
            IRepository<FloorSlideDetailsInfo> floorSlideDetailsInfoRepository,
            IRepository<FloorTopicInfo> floorTopicInfoRepository,
            AdminAreaSettings adminAreaSettings,
            IComponentContext ctx,
            IEventPublisher eventPublisher)
        {
            this._homeFloorInfoRepository = homeFloorInfoRepository;
            this._floorSlidesInfoRepository = floorSlidesInfoRepository;
            this._floorSlideDetailsInfoRepository = floorSlideDetailsInfoRepository;
            this._floorTopicInfoRepository = floorTopicInfoRepository;
            this._adminAreaSettings = adminAreaSettings;
            _eventPublisher = eventPublisher;
        }
        #region Methods

        /// <summary>
        /// Deletes a homeFloorInfo
        /// </summary>
        /// <param name="homeFloorInfo">HomeFloorInfo</param>
        public virtual void DeleteHomeFloorInfo(HomeFloorInfo homeFloorInfo)
        {
            if (homeFloorInfo == null)
                throw new ArgumentNullException("homeFloorInfo");

            _homeFloorInfoRepository.Delete(homeFloorInfo);

            //event notification
            _eventPublisher.EntityDeleted(homeFloorInfo);
        }

        /// <summary>
        /// Gets a homeFloorInfo
        /// </summary>
        /// <param name="homeFloorInfoId">The homeFloorInfo identifier</param>
        /// <returns>HomeFloorInfo</returns>
        public virtual HomeFloorInfo GetHomeFloorInfoById(int homeFloorInfoId)
        {
            if (homeFloorInfoId == 0)
                return null;

            return _homeFloorInfoRepository.GetById(homeFloorInfoId);
        }



        /// <summary>
        /// Gets all homeFloorInfos
        /// </summary>
        /// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>HomeFloorInfos</returns>
        public virtual IList<HomeFloorInfo> GetAllHomeFloorInfos(bool showHidden = false)
        {
            var query = _homeFloorInfoRepository.Table;
            if (!showHidden)
                query = query.Where(x => x.IsShow);
            query = query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.CreatedOnUtc);

            return query.ToList();
        }

        /// <summary>
        /// Updates the homeFloorInfo
        /// </summary>
        /// <param name="homeFloorInfo">HomeFloorInfo</param>
        public virtual void UpdateHomeFloorInfo(HomeFloorInfo homeFloorInfo)
        {
            if (homeFloorInfo == null)
                throw new ArgumentNullException("homeFloorInfo");


            using (var scope = new DbContextScope(autoDetectChanges: false, proxyCreation: true, validateOnSave: false, forceNoTracking: true))
            {

                try
                {


                    if (homeFloorInfo.Id == 0)
                    {
                        homeFloorInfo.CreatedOnUtc = DateTime.UtcNow;
                        homeFloorInfo.ModifiedOnUtc = DateTime.UtcNow;
                        homeFloorInfo.DisplayOrder = GetMaxHomeFloorSequence() + 1;
                        _homeFloorInfoRepository.Insert(homeFloorInfo);

                        //event notification
                        _eventPublisher.EntityInserted(homeFloorInfo);
                    }
                    else
                    {
                        if (homeFloorInfo.StyleLevel == 1)
                        {
                            UpdateProducts(homeFloorInfo.Id, homeFloorInfo.FloorSlidesInfos);
                        }

                        UpdateTextLink(homeFloorInfo.Id,
                          from item in homeFloorInfo.FloorTopicInfos
                          where item.TopicType == Position.Top
                          select item);

                        UpdataProductLink(homeFloorInfo.Id,
                         from item in homeFloorInfo.FloorTopicInfos
                         where item.TopicType != Position.Top
                         select item);

                        var defaultHomeFloor = _homeFloorInfoRepository.GetById(homeFloorInfo.Id);
                        defaultHomeFloor.DefaultTabName = homeFloorInfo.DefaultTabName;
                        defaultHomeFloor.FloorName = homeFloorInfo.FloorName;
                        defaultHomeFloor.SubName = homeFloorInfo.SubName;
                        // defaultHomeFloor.IsShow = homeFloorInfo.IsShow;
                        defaultHomeFloor.RelateManufacturerIds = homeFloorInfo.RelateManufacturerIds;
                        defaultHomeFloor.RelateCategoryIds = homeFloorInfo.RelateCategoryIds;
                        defaultHomeFloor.RelateProductIds = homeFloorInfo.RelateProductIds;
                        defaultHomeFloor.RelateVendorIds = homeFloorInfo.RelateVendorIds;
                        defaultHomeFloor.Url = homeFloorInfo.Url;
                        defaultHomeFloor.ModifiedOnUtc = DateTime.UtcNow;

                        _homeFloorInfoRepository.Update(defaultHomeFloor);

                        //event notification
                        _eventPublisher.EntityUpdated(homeFloorInfo);
                    }


                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                finally
                {

                }

            }
        }


       
        /// <summary>
        /// ¸üÐÂÅÅÐò
        /// </summary>
        /// <param name="sourceSequence"></param>
        /// <param name="destiSequence"></param>
        public void UpdateHomeFloorSequence(int sourceSequence, int destiSequence)
        {
            var homeFloorInfo1 = _homeFloorInfoRepository.Table.Where(x => x.DisplayOrder == sourceSequence).FirstOrDefault();
            var homeFloorInfo2 = _homeFloorInfoRepository.Table.Where(x => x.DisplayOrder == destiSequence).FirstOrDefault();
            homeFloorInfo1.DisplayOrder = destiSequence;
            UpdateHomeFloorInfo(homeFloorInfo1);
            homeFloorInfo2.DisplayOrder = sourceSequence;
            UpdateHomeFloorInfo(homeFloorInfo2);


        }

        private int GetMaxHomeFloorSequence()
        {
            int num = 0;
            if (_homeFloorInfoRepository.Table.Count() > 0)
            {
                num = _homeFloorInfoRepository.Table.Max(x => x.DisplayOrder);
            }
            return num;
        }

        private void UpdateProducts(int floorId, IEnumerable<FloorSlidesInfo> tabs)
        {
            _floorTopicInfoRepository.DeleteAll(x => x.FloorId == floorId, true);
            _floorSlidesInfoRepository.InsertRange(tabs);


        }
        private void UpdataProductLink(int floorId, IEnumerable<FloorTopicInfo> productLink)
        {

            _floorTopicInfoRepository.DeleteAll(x => x.FloorId == floorId && x.TopicTypeId != 15);
            _floorTopicInfoRepository.InsertRange(productLink);
        }

        private void UpdateTextLink(int floorId, IEnumerable<FloorTopicInfo> textLinks)
        {
            _floorTopicInfoRepository.DeleteAll(x => x.FloorId == floorId, true);
            _floorTopicInfoRepository.InsertRange(textLinks);
        }

        #endregion
    }
}
