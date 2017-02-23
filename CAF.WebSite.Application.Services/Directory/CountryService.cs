using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core.Caching;

namespace CAF.WebSite.Application.Services.Directory
{
    /// <summary>
    /// Country service
    /// </summary>
    public partial class CountryService : ICountryService
    {
        #region Constants
        private const string COUNTRIES_ALL_KEY = "caf.country.all-{0}";
        private const string COUNTRIES_BILLING_KEY = "caf.country.billing-{0}";
        private const string COUNTRIES_SHIPPING_KEY = "caf.country.shipping-{0}";
        private const string COUNTRIES_PATTERN_KEY = "caf.country.";
        #endregion
        
        #region Fields
        
        private readonly IRepository<Country> _countryRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestCache _requestCache;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="countryRepository">Country repository</param>
        /// <param name="eventPublisher">Event published</param>
        public CountryService(IRequestCache requestCache,
            IRepository<Country> countryRepository,
            IEventPublisher eventPublisher)
        {
            _requestCache = requestCache;
            _countryRepository = countryRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void DeleteCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Delete(country);

            _requestCache.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(country);
        }

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Country collection</returns>
        public virtual IList<Country> GetAllCountries(bool showHidden = false)
        {
            string key = string.Format(COUNTRIES_ALL_KEY, showHidden);
            return _requestCache.Get(key, () =>
            {
                var query = from c in _countryRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where showHidden || c.Published
                            select c;
                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow billing
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Country collection</returns>
        public virtual IList<Country> GetAllCountriesForBilling(bool showHidden = false)
        {
            string key = string.Format(COUNTRIES_BILLING_KEY, showHidden);
            return _requestCache.Get(key, () =>
            {
                var query = from c in _countryRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where (showHidden || c.Published) && c.AllowsBilling
                            select c;
                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow shipping
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Country collection</returns>
        public virtual IList<Country> GetAllCountriesForShipping(bool showHidden = false)
        {
            string key = string.Format(COUNTRIES_SHIPPING_KEY, showHidden);
            return _requestCache.Get(key, () =>
            {
                var query = from c in _countryRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where (showHidden || c.Published) && c.AllowsShipping
                            select c;
                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryById(int countryId)
        {
            if (countryId == 0)
                return null;

            return _countryRepository.GetById(countryId);
        }

		/// <summary>
		/// Gets a country by two or three letter ISO code
		/// </summary>
		/// <param name="letterIsoCode">Country two or three letter ISO code</param>
		/// <returns>Country</returns>
		public virtual Country GetCountryByTwoOrThreeLetterIsoCode(string letterIsoCode)
		{
			if (letterIsoCode.HasValue())
			{
				if (letterIsoCode.Length == 2)
					return GetCountryByTwoLetterIsoCode(letterIsoCode);
				else if (letterIsoCode.Length == 3)
					return GetCountryByThreeLetterIsoCode(letterIsoCode);
			}
			return null;
		}

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryByTwoLetterIsoCode(string twoLetterIsoCode)
        {
            var query = from c in _countryRepository.Table
                        where c.TwoLetterIsoCode == twoLetterIsoCode
                        select c;
            var country = query.FirstOrDefault();

            return country;
        }

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryByThreeLetterIsoCode(string threeLetterIsoCode)
        {
            var query = from c in _countryRepository.Table
                        where c.ThreeLetterIsoCode == threeLetterIsoCode
                        select c;
            var country = query.FirstOrDefault();
            return country;
        }

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void InsertCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Insert(country);

            _requestCache.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(country);
        }

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void UpdateCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Update(country);

            _requestCache.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(country);
        }

        #endregion
    }
}