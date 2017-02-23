using System;
using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core.Caching;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core;

namespace CAF.WebSite.Application.Services.Directory
{
	/// <summary>
	/// City service
	/// </summary>
	public partial class CityService : ICityService
    {
        #region Constants
        private const string STATEPROVINCES_ALL_KEY = "CAF.WebSite.citys.all-{0}";
        private const string STATEPROVINCES_PATTERN_KEY = "CAF.WebSite.citys.";
        #endregion

        #region Fields

        private readonly IRepository<City> _cityRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestCache _requestCache;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="cityRepository">State/province repository</param>
        /// <param name="eventPublisher">Event published</param>
        public CityService(IRequestCache requestCache,
            IRepository<City> cityRepository,
            IEventPublisher eventPublisher)
        {
            _requestCache = requestCache;
            _cityRepository = cityRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="city">The state/province</param>
        public virtual void DeleteCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");
            
            _cityRepository.Delete(city);

            _requestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(city);
        }

		public virtual IQueryable<City> GetAllCitys()
		{
			var query = _cityRepository.Table;

			return query;
		}

		/// <summary>
		/// Gets a state/province
		/// </summary>
		/// <param name="cityId">The state/province identifier</param>
		/// <returns>State/province</returns>
		public virtual City GetCityById(int cityId)
        {
            if (cityId == 0)
                return null;

            return _cityRepository.GetById(cityId);
        }

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        public virtual City GetCityByCityCode(string cityCode)
        {
            var query = from sp in _cityRepository.Table
                        where sp.Code == cityCode
                        select sp;
            var city = query.FirstOrDefault();
            return city;
        }
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>State/province collection</returns>
        public virtual IList<City> GetCitysByProvinceId(int provinceId )
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, provinceId);
            return _requestCache.Get(key, () =>
            {
                var query = from sp in _cityRepository.Table
                            orderby sp.DisplayOrder
                            where sp.ProvinceId == provinceId
                            select sp;
                var citys = query.ToList();
                return citys;
            });
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="city">State/province</param>
        public virtual void InsertCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");

            _cityRepository.Insert(city);

            _requestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(city);
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="city">State/province</param>
        public virtual void UpdateCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");

            _cityRepository.Update(city);

            _requestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(city);
        }

        #endregion
    }
}
