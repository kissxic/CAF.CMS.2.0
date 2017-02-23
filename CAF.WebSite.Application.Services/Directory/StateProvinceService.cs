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
	/// State province service
	/// </summary>
	public partial class StateProvinceService : IStateProvinceService
    {
        #region Constants
        private const string STATEPROVINCES_ALL_KEY = "CAF.WebSite.stateprovince.all-{0}";
        private const string STATEPROVINCES_PATTERN_KEY = "CAF.WebSite.stateprovince.";
        #endregion

        #region Fields

        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestCache _requestCache;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="stateProvinceRepository">State/province repository</param>
        /// <param name="eventPublisher">Event published</param>
        public StateProvinceService(IRequestCache requestCache,
            IRepository<StateProvince> stateProvinceRepository,
            IEventPublisher eventPublisher)
        {
            _requestCache = requestCache;
            _stateProvinceRepository = stateProvinceRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        public virtual void DeleteStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");
            
            _stateProvinceRepository.Delete(stateProvince);

            _requestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(stateProvince);
        }

		public virtual IQueryable<StateProvince> GetAllStateProvinces(bool showHidden = false)
		{
			var query = _stateProvinceRepository.Table;

			if (!showHidden)
				query = query.Where(x => x.Published);

			return query;
		}

		/// <summary>
		/// Gets a state/province
		/// </summary>
		/// <param name="stateProvinceId">The state/province identifier</param>
		/// <returns>State/province</returns>
		public virtual StateProvince GetStateProvinceById(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return null;

            return _stateProvinceRepository.GetById(stateProvinceId);
        }

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        public virtual StateProvince GetStateProvinceByAbbreviation(string abbreviation)
        {
            var query = from sp in _stateProvinceRepository.Table
                        where sp.Abbreviation == abbreviation
                        select sp;
            var stateProvince = query.FirstOrDefault();
            return stateProvince;
        }
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>State/province collection</returns>
        public virtual IList<StateProvince> GetStateProvincesByCountryId(int countryId, bool showHidden = false)
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, countryId);
            return _requestCache.Get(key, () =>
            {
                var query = from sp in _stateProvinceRepository.Table
                            orderby sp.DisplayOrder
                            where sp.CountryId == countryId &&
                            (showHidden || sp.Published)
                            select sp;
                var stateProvinces = query.ToList();
                return stateProvinces;
            });
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public virtual void InsertStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            _stateProvinceRepository.Insert(stateProvince);

            _requestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(stateProvince);
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public virtual void UpdateStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            _stateProvinceRepository.Update(stateProvince);

            _requestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(stateProvince);
        }

        #endregion
    }
}