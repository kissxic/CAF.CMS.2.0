using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Caching;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Manufacturers;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Manufacturers
{
    /// <summary>
    /// Manufacturer service
    /// </summary>
    public partial class ManufacturerService : IManufacturerService
    {
        #region Constants
        private const string PRODUCTMANUFACTURERS_ALLBYMANUFACTURERID_KEY = "CAF.WebSite.articlemanufacturer.allbymanufacturerid-{0}-{1}-{2}-{3}-{4}";
        private const string PRODUCTMANUFACTURERS_ALLBYPRODUCTID_KEY = "CAF.WebSite.articlemanufacturer.allbyarticleid-{0}-{1}-{2}";
        private const string MANUFACTURERS_PATTERN_KEY = "CAF.WebSite.manufacturer.";
        private const string MANUFACTURERS_BY_ID_KEY = "CAF.WebSite.manufacturer.id-{0}";
        private const string PRODUCTMANUFACTURERS_PATTERN_KEY = "CAF.WebSite.articlemanufacturer.";

        #endregion

        #region Fields

        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IWorkContext _workContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestCache _requestCache;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="manufacturerRepository">Category repository</param>
        /// <param name="articleRepository">Article repository</param>
		/// <param name="workContext">Work context</param>
        /// <param name="eventPublisher">Event published</param>
        public ManufacturerService(IRequestCache requestCache,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Article> articleRepository,
            IWorkContext workContext,
            IEventPublisher eventPublisher)
        {
            _requestCache = requestCache;
            _manufacturerRepository = manufacturerRepository;
            _articleRepository = articleRepository;
            _workContext = workContext;
            _eventPublisher = eventPublisher;

            this.QuerySettings = DbQuerySettings.Default;
        }

        public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void DeleteManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");

            manufacturer.Deleted = true;
            UpdateManufacturer(manufacturer);
        }

        public virtual IQueryable<Manufacturer> GetManufacturers(bool showHidden = false)
        {
            var query = _manufacturerRepository.Table
                .Where(m => !m.Deleted);

            if (!showHidden)
                query = query.Where(m => m.Published);


            return query;
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer collection</returns>
        public virtual IList<Manufacturer> GetAllManufacturers(bool showHidden = false)
        {
            return GetAllManufacturers(null, showHidden);
        }


        public virtual IList<Manufacturer> GetAllManufacturersById(int[] manufacturerIds)
        {
            if (manufacturerIds == null || manufacturerIds.Length == 0)
                return new List<Manufacturer>();

            var query = from c in _manufacturerRepository.Table
                        where manufacturerIds.Contains(c.Id)
                        select c;
            var manufacturers = query.ToList();

            var sortedManufacturer = new List<Manufacturer>();
            foreach (int id in manufacturerIds)
            {
                var manufacturer = manufacturers.Find(x => x.Id == id);
                if (manufacturer != null)
                    sortedManufacturer.Add(manufacturer);
            }
            return sortedManufacturer;
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name</param>
		/// <param name="storeId">Whether to filter result by store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer collection</returns>
        public virtual IList<Manufacturer> GetAllManufacturers(string manufacturerName, bool showHidden = false)
        {
            var query = GetManufacturers(showHidden);

            if (manufacturerName.HasValue())
                query = query.Where(m => m.Name.Contains(manufacturerName));

            query = query.OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.Name);

            var manufacturers = query.ToList();
            return manufacturers;
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="storeId">Whether to filter result by store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturers</returns>
        public virtual IPagedList<Manufacturer> GetAllManufacturers(string manufacturerName,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            var manufacturers = GetAllManufacturers(manufacturerName, showHidden);
            return new PagedList<Manufacturer>(manufacturers, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets a manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer</returns>
        public virtual Manufacturer GetManufacturerById(int manufacturerId)
        {
            if (manufacturerId == 0)
                return null;

            string key = string.Format(MANUFACTURERS_BY_ID_KEY, manufacturerId);
            return _requestCache.Get(key, () =>
            {
                return _manufacturerRepository.GetById(manufacturerId);
            });
        }

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void InsertManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");

            _manufacturerRepository.Insert(manufacturer);

            //cache
            _requestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(manufacturer);
        }

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void UpdateManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");

            _manufacturerRepository.Update(manufacturer);

            //cache
            _requestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(manufacturer);
        }


        #endregion
    }
}
