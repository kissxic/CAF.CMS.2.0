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
	/// District service
	/// </summary>
	public partial class DistrictService : IDistrictService
    {
        #region Constants
        private const string STATEPROVINCES_ALL_KEY = "CAF.WebSite.districts.all-{0}";
        private const string STATEPROVINCES_PATTERN_KEY = "CAF.WebSite.districts.";
        #endregion

        #region Fields

        private readonly IRepository<District> _districtRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestCache _requestCache;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="districtRepository">District repository</param>
        /// <param name="eventPublisher">Event published</param>
        public DistrictService(IRequestCache requestCache,
            IRepository<District> districtRepository,
            IEventPublisher eventPublisher)
        {
            _requestCache = requestCache;
            _districtRepository = districtRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a city
        /// </summary>
        /// <param name="district">The city</param>
        public virtual void DeleteDistrict(District district)
        {
            if (district == null)
                throw new ArgumentNullException("district");
            
            _districtRepository.Delete(district);

            _requestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(district);
        }

		public virtual IQueryable<District> GetAllDistricts()
		{
			var query = _districtRepository.Table;

			return query;
		}

		/// <summary>
		/// Gets a city
		/// </summary>
		/// <param name="districtId">The city identifier</param>
		/// <returns>District</returns>
		public virtual District GetDistrictById(int districtId)
        {
            if (districtId == 0)
                return null;

            return _districtRepository.GetById(districtId);
        }

        /// <summary>
        /// Gets a city 
        /// </summary>
        /// <param name="abbreviation">The city abbreviation</param>
        /// <returns>District</returns>
        public virtual District GetDistrictByDistrictCode(string districtCode)
        {
            var query = from sp in _districtRepository.Table
                        where sp.Code == districtCode
                        select sp;
            var district = query.FirstOrDefault();
            return district;
        }
        
        /// <summary>
        /// Gets a city collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>District collection</returns>
        public virtual IList<District> GetDistrictsByCityId(int cityId)
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, cityId);
            return _requestCache.Get(key, () =>
            {
                var query = from sp in _districtRepository.Table
                            orderby sp.DisplayOrder
                            where sp.CityId == cityId 
                            select sp;
                var districts = query.ToList();
                return districts;
            });
        }

        /// <summary>
        /// Inserts a city
        /// </summary>
        /// <param name="district">District</param>
        public virtual void InsertDistrict(District district)
        {
            if (district == null)
                throw new ArgumentNullException("district");

            _districtRepository.Insert(district);

            _requestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(district);
        }

        /// <summary>
        /// Updates a city
        /// </summary>
        /// <param name="district">District</param>
        public virtual void UpdateDistrict(District district)
        {
            if (district == null)
                throw new ArgumentNullException("district");

            _districtRepository.Update(district);

            _requestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(district);
        }

        #endregion
    }
}
