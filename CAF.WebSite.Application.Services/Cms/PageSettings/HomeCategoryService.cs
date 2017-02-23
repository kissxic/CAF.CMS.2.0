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
    public class HomeCategoryService : IHomeCategoryService
    {
        private readonly IRepository<HomeCategory> _homeCategoryRepository;
        private readonly IRepository<HomeCategoryItem> _homeCategoryItemRepository;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IEventPublisher _eventPublisher;

        public HomeCategoryService(IRepository<HomeCategory> homeCategoryRepository,
            IRepository<HomeCategoryItem> homeCategoryItemRepository,
            AdminAreaSettings adminAreaSettings,
            IComponentContext ctx,
            IEventPublisher eventPublisher)
        {
            this._homeCategoryRepository = homeCategoryRepository;
            this._homeCategoryItemRepository = homeCategoryItemRepository;
            this._adminAreaSettings = adminAreaSettings;

            _eventPublisher = eventPublisher;
        }
        #region Methods

        /// <summary>
        /// Deletes a homeCategory
        /// </summary>
        /// <param name="homeCategory">HomeCategory</param>
        public virtual void DeleteHomeCategory(HomeCategory homeCategory)
        {
            if (homeCategory == null)
                throw new ArgumentNullException("homeCategory");

            _homeCategoryRepository.Delete(homeCategory);

            //event notification
            _eventPublisher.EntityDeleted(homeCategory);
        }

        /// <summary>
        /// Gets a homeCategory
        /// </summary>
        /// <param name="homeCategoryId">The homeCategory identifier</param>
        /// <returns>HomeCategory</returns>
        public virtual HomeCategory GetHomeCategoryById(int homeCategoryId)
        {
            if (homeCategoryId == 0)
                return null;

            return _homeCategoryRepository.GetById(homeCategoryId);
        }



        /// <summary>
        /// Gets all homeCategorys
        /// </summary>
        /// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>HomeCategorys</returns>
        public virtual IList<HomeCategory> GetAllHomeCategorys()
        {
            var query = _homeCategoryRepository.Table;
            query = query.OrderBy(t => t.CreatedOnUtc).ThenBy(t => t.DisplayOrder);

            return query.ToList();
        }
 

        /// <summary>
        /// Inserts a homeCategory
        /// </summary>
        /// <param name="homeCategory">HomeCategory</param>
        public virtual void InsertHomeCategory(HomeCategory homeCategory)
        {
            if (homeCategory == null)
                throw new ArgumentNullException("homeCategory");

            _homeCategoryRepository.Insert(homeCategory);

            //event notification
            _eventPublisher.EntityInserted(homeCategory);
        }

        /// <summary>
        /// Inserts a homeCategorys
        /// </summary>
        /// <param name="homeCategorys">homeCategorys mapping</param>
        public virtual void InsertHomeCategoryItem(int homeCategoryId, List<HomeCategoryItem> homeCategoryItems)
        {
            if (homeCategoryItems.Count <= 0)
                throw new ArgumentNullException("homeCategoryItems");

            _homeCategoryItemRepository.DeleteAll(x => x.HomeCategoryId == homeCategoryId,true);
            
            using (var scope = new DbContextScope(ctx: _homeCategoryItemRepository.Context, autoCommit: false, autoDetectChanges: false, validateOnSave: false, hooksEnabled: false))
            {
                foreach (var values in homeCategoryItems)
                {
                    _homeCategoryItemRepository.Insert(values);
                    //event notification
                    _eventPublisher.EntityInserted(values);
                }
                scope.Commit();
            }

        }

        /// <summary>
        /// Updates the homeCategory
        /// </summary>
        /// <param name="homeCategory">HomeCategory</param>
        public virtual void UpdateHomeCategory(HomeCategory homeCategory)
        {
            if (homeCategory == null)
                throw new ArgumentNullException("homeCategory");

            _homeCategoryRepository.Update(homeCategory);

            //event notification
            _eventPublisher.EntityUpdated(homeCategory);
        }




        /// <summary>
        /// Deletes a homeCategoryItem
        /// </summary>
        /// <param name="homeCategoryItem">HomeCategoryItem</param>
        public virtual void DeleteHomeCategoryItem(HomeCategoryItem homeCategoryItem)
        {
            if (homeCategoryItem == null)
                throw new ArgumentNullException("homeCategoryItem");

            _homeCategoryItemRepository.Delete(homeCategoryItem);

            //event notification
            _eventPublisher.EntityDeleted(homeCategoryItem);
        }

        /// <summary>
        /// Gets a homeCategoryItem
        /// </summary>
        /// <param name="homeCategoryItemId">The homeCategoryItem identifier</param>
        /// <returns>HomeCategoryItem</returns>
        public virtual HomeCategoryItem GetHomeCategoryItemById(int homeCategoryItemId)
        {
            if (homeCategoryItemId == 0)
                return null;

            return _homeCategoryItemRepository.GetById(homeCategoryItemId);
        }



        /// <summary>
        /// Gets all homeCategoryItems
        /// </summary>
        /// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>HomeCategoryItems</returns>
        public virtual IList<HomeCategoryItem> GetAllHomeCategoryItems()
        {
            var query = _homeCategoryItemRepository.Table;
            query = query.OrderBy(t => t.Id);

            return query.ToList();
        }
        /// <summary>
        /// Gets  homeCategoryItems by homeCategoryId
        /// </summary>
        /// <param name="homeCategoryId"></param>
        /// <returns></returns>
        public virtual IList<HomeCategoryItem> GetAllHomeCategoryItemsByHomeCategoryId(int homeCategoryId)
        {
            var query = _homeCategoryItemRepository.Table;
            query = query.Where(c => c.HomeCategoryId == homeCategoryId);
            query = query.OrderBy(t => t.Id);

            return query.ToList();
        }

     
        /// <summary>
        /// Inserts a homeCategoryItem
        /// </summary>
        /// <param name="homeCategoryItem">HomeCategoryItem</param>
        public virtual void InsertHomeCategoryItem(HomeCategoryItem homeCategoryItem)
        {
            if (homeCategoryItem == null)
                throw new ArgumentNullException("homeCategoryItem");

            _homeCategoryItemRepository.Insert(homeCategoryItem);

            //event notification
            _eventPublisher.EntityInserted(homeCategoryItem);
        }



        /// <summary>
        /// Updates the homeCategoryItem
        /// </summary>
        /// <param name="homeCategoryItem">HomeCategoryItem</param>
        public virtual void UpdateHomeCategoryItem(HomeCategoryItem homeCategoryItem)
        {
            if (homeCategoryItem == null)
                throw new ArgumentNullException("homeCategoryItem");

            _homeCategoryItemRepository.Update(homeCategoryItem);

            //event notification
            _eventPublisher.EntityUpdated(homeCategoryItem);
        }

        #endregion
    }
}
