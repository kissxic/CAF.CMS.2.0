using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Caching;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Sites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Directory
{
    /// <summary>
    /// Currency service
    /// </summary>
    public partial class CurrencyService : ICurrencyService
    {
        #region Constants
        private const string CURRENCIES_ALL_KEY = "CAF.WebSite.currency.all-{0}";
        private const string CURRENCIES_PATTERN_KEY = "CAF.WebSite.currency.";
        private const string CURRENCIES_BY_ID_KEY = "CAF.WebSite.currency.id-{0}";
        #endregion

        #region Fields

        private readonly IRepository<Currency> _currencyRepository;
		private readonly ISiteMappingService _siteMappingService;
        private readonly IRequestCache _requestCache;
        private readonly CurrencySettings _currencySettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IEventPublisher _eventPublisher;
		private readonly IProviderManager _providerManager;
		private readonly ISiteContext _siteContext;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="currencyRepository">Currency repository</param>
		/// <param name="siteMappingRepository">Site mapping repository</param>
        /// <param name="currencySettings">Currency settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="eventPublisher">Event published</param>
        public CurrencyService(IRequestCache requestCache,
            IRepository<Currency> currencyRepository,
			ISiteMappingService siteMappingService,
            CurrencySettings currencySettings,
            IPluginFinder pluginFinder,
            IEventPublisher eventPublisher,
			IProviderManager providerManager,
			ISiteContext siteContext)
        {
            this._requestCache = requestCache;
            this._currencyRepository = currencyRepository;
			this._siteMappingService = siteMappingService;
            this._currencySettings = currencySettings;
            this._pluginFinder = pluginFinder;
            this._eventPublisher = eventPublisher;
			this._providerManager = providerManager;
			this._siteContext = siteContext;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        public virtual IList<ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode)
        {
            var exchangeRateProvider = LoadActiveExchangeRateProvider();
			if (exchangeRateProvider != null)
			{
				return exchangeRateProvider.Value.GetCurrencyLiveRates(exchangeRateCurrencyCode);
			}
			return new List<ExchangeRate>();
        }

        /// <summary>
        /// Deletes currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual void DeleteCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");
            
            _currencyRepository.Delete(currency);

            _requestCache.RemoveByPattern(CURRENCIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(currency);
        }

        /// <summary>
        /// Gets a currency
        /// </summary>
        /// <param name="currencyId">Currency identifier</param>
        /// <returns>Currency</returns>
        public virtual Currency GetCurrencyById(int currencyId)
        {
            if (currencyId == 0)
                return null;

            string key = string.Format(CURRENCIES_BY_ID_KEY, currencyId);
            return _requestCache.Get(key, () => _currencyRepository.GetById(currencyId));
        }

        /// <summary>
        /// Gets a currency by code
        /// </summary>
        /// <param name="currencyCode">Currency code</param>
        /// <returns>Currency</returns>
        public virtual Currency GetCurrencyByCode(string currencyCode)
        {
            if (String.IsNullOrEmpty(currencyCode))
                return null;
            return GetAllCurrencies(true).FirstOrDefault(c => c.CurrencyCode.ToLower() == currencyCode.ToLower());
        }

        /// <summary>
        /// Gets all currencies
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
		/// <param name="siteId">Load records allows only in specified site; pass 0 to load all records</param>
        /// <returns>Currency collection</returns>
		public virtual IList<Currency> GetAllCurrencies(bool showHidden = false, int siteId = 0)
        {
			string key = string.Format(CURRENCIES_ALL_KEY, showHidden);
			var currencies = _requestCache.Get(key, () =>
			{
				var query = _currencyRepository.Table;
				if (!showHidden)
					query = query.Where(c => c.Published);
				query = query.OrderBy(c => c.DisplayOrder);
				return query.ToList();
			});

			//site mapping
			if (siteId > 0)
			{
				currencies = currencies
					.Where(c => _siteMappingService.Authorize(c, siteId))
					.ToList();
			}
			return currencies;
        }

        /// <summary>
        /// Inserts a currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual void InsertCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");

            _currencyRepository.Insert(currency);

            _requestCache.RemoveByPattern(CURRENCIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(currency);
        }

        /// <summary>
        /// Updates the currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual void UpdateCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");

            _currencyRepository.Update(currency);

            _requestCache.RemoveByPattern(CURRENCIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(currency);
        }


        /// <summary>
        /// Converts currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="exchangeRate">Currency exchange rate</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertCurrency(decimal amount, decimal exchangeRate)
        {
            if (amount != decimal.Zero && exchangeRate != decimal.Zero)
                return amount * exchangeRate;
            return decimal.Zero;
        }

        /// <summary>
        /// Converts currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
		/// <param name="site">Site to get the primary currencies from</param>
        /// <returns>Converted value</returns>
		public virtual decimal ConvertCurrency(decimal amount, Currency sourceCurrencyCode, Currency targetCurrencyCode, Site site = null)
        {
            decimal result = amount;
           // if (sourceCurrencyCode.Id == targetCurrencyCode.Id)
                return result;
          
        }

        /// <summary>
        /// Converts to primary site currency 
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
		/// <param name="site">Site to get the primary site currency from</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertToPrimarySiteCurrency(decimal amount, Currency sourceCurrencyCode, Site site = null)
        {
            decimal result = amount;
			var primarySiteCurrency = (site == null ? _siteContext.CurrentSite.PrimarySiteCurrency : site.PrimarySiteCurrency);

            if (result != decimal.Zero && sourceCurrencyCode.Id != primarySiteCurrency.Id)
            {
                decimal exchangeRate = sourceCurrencyCode.Rate;
                if (exchangeRate == decimal.Zero)
                    throw new WorkException(string.Format("Exchange rate not found for currency [{0}]", sourceCurrencyCode.Name));
                result = result / exchangeRate;
            }
            return result;
        }

        /// <summary>
        /// Converts from primary site currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
		public virtual decimal ConvertFromPrimarySiteCurrency(decimal amount, Currency targetCurrencyCode, Site site = null)
        {
            decimal result = amount;
			var primarySiteCurrency = (site == null ? _siteContext.CurrentSite.PrimarySiteCurrency : site.PrimarySiteCurrency);
            result = ConvertCurrency(amount, primarySiteCurrency, targetCurrencyCode, site);
            return result;
        }
       

        /// <summary>
        /// Load active exchange rate provider
        /// </summary>
        /// <returns>Active exchange rate provider</returns>
        public virtual Provider<IExchangeRateProvider> LoadActiveExchangeRateProvider()
        {
			return LoadExchangeRateProviderBySystemName(_currencySettings.ActiveExchangeRateProviderSystemName) ?? LoadAllExchangeRateProviders().FirstOrDefault();
        }

        /// <summary>
        /// Load exchange rate provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found exchange rate provider</returns>
        public virtual Provider<IExchangeRateProvider> LoadExchangeRateProviderBySystemName(string systemName)
        {
			return _providerManager.GetProvider<IExchangeRateProvider>(systemName);
        }

        /// <summary>
        /// Load all exchange rate providers
        /// </summary>
        /// <returns>Exchange rate providers</returns>
        public virtual IEnumerable<Provider<IExchangeRateProvider>> LoadAllExchangeRateProviders()
        {
			return _providerManager.GetAllProviders<IExchangeRateProvider>();
        }
        #endregion
    }
}