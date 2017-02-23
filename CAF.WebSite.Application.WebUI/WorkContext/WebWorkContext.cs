using System;
using System.Linq;
using System.Web;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Fakes;
using CAF.Infrastructure.Core.Collections;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Common;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.WebSite.Application.Services.Authentication;
using CAF.Infrastructure.Core.Domain.Security;
using System.Web.Security;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.WebSite.Application.Services.Directory;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Sellers;
using CAF.WebSite.Application.Services.Vendors;
using CAF.WebSite.Application.Services.Members;
using CAF.Infrastructure.Core.Domain.Cms.Members;
using CAF.Infrastructure.Core.Utilities;
using CachingFramework.Redis.Contracts;

namespace CAF.WebSite.Application.WebUI
{
    /// <summary>
    /// Work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        private const string UserCookieName = "caf.user";
  
        private const string AppSystemCookieName = "caf.appsystem";
        private readonly IUserService _userService;
        private readonly ISiteContext _siteContext;
        private readonly HttpContextBase _httpContext;
        private readonly ILanguageService _languageService;
        private readonly ICacheManager _cacheManager;
        private readonly IGenericAttributeService _attrService;
        private readonly ISiteService _siteService;
        private readonly TimeSpan _expirationTimeSpan;
        private readonly SecuritySettings _securitySettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly CurrencySettings _currencySettings;
        private readonly ISettingService _settingService;
        private readonly TaxSettings _taxSettings;
        private readonly IUserAgent _userAgent;
        private readonly IVendorService _vendorService;
        private readonly IMemberGradeService _memberGradeService;
        private readonly ICurrencyService _currencyService;
  
        private TaxDisplayType? _cachedTaxDisplayType;
        private Language _cachedLanguage;
        private Currency _cachedCurrency;
        private User _cachedUser;
        private Vendor _cachedVendor;
        private MemberGrade _cachedMemberGrade;
        private User _originalUserIfImpersonated;

        public WebWorkContext(Func<string, ICacheManager> cacheManager,
            HttpContextBase httpContext,
            IUserService userService,
            LocalizationSettings localizationSettings,
            SecuritySettings securitySettings, CurrencySettings currencySettings,
            ISiteContext siteContext,
            ILanguageService languageService,
            ISiteService siteService,
            IAuthenticationService authenticationService,
            IGenericAttributeService attrService,
            ISettingService settingService,
            TaxSettings taxSettings,
            IUserAgent userAgent,
            IVendorService vendorService,
            IMemberGradeService memberGradeService,
            ICurrencyService currencyService
            )
        {

            this._cacheManager = cacheManager("static");
            this._userService = userService;
            this._httpContext = httpContext;
            this._siteContext = siteContext;
            this._languageService = languageService;
            this._siteService = siteService;
            this._currencySettings = currencySettings;
            this._currencyService = currencyService;
            this._taxSettings = taxSettings;
            this._localizationSettings = localizationSettings;
            this._securitySettings = securitySettings;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
            this._authenticationService = authenticationService;
            this._attrService = attrService;
            this._settingService = settingService;
            this._userAgent = userAgent;
            this._vendorService = vendorService;
            this._memberGradeService = memberGradeService;
          
        }

        protected HttpCookie GetUserCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[UserCookieName];
        }

        protected void SetUserCookie(Guid userGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(UserCookieName);
                cookie.HttpOnly = true;
                cookie.Value = userGuid.ToString();
                if (userGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    //int cookieExpires = 24 * 365; //通过Forms配置
                    cookie.Expires = DateTime.Now.Add(_expirationTimeSpan);
                }

                _httpContext.Response.Cookies.Remove(UserCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }


        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public User CurrentUser
        {
            get
            {
                if (_cachedUser != null)
                    return _cachedUser;

                User user = null;

                // check whether request is made by a background task
                // in this case return built-in user record for background task
                if (_httpContext == null || _httpContext.IsFakeContext())
                {
                    user = _userService.GetUserByUserName(SystemUserNames.BackgroundTask);
                }

                // check whether request is made by a search engine
                // in this case return built-in user record for search engines 
                if (user == null || user.Deleted || !user.Active)
                {
                    if (_userAgent.IsBot)
                    {
                        user = _userService.GetUserByUserName(SystemUserNames.SearchEngine);
                    }
                }

                // check whether request is made by the PDF converter
                // in this case return built-in user record for the converter
                if (user == null || user.Deleted || !user.Active)
                {
                    if (_userAgent.IsPdfConverter)
                    {
                        user = _userService.GetUserByUserName(SystemUserNames.PdfConverter);
                    }
                }

                //registered user
                if (user == null || user.Deleted || !user.Active)
                {
                    user = _authenticationService.GetAuthenticatedUser();
                }

                // impersonate user if required (currently used for 'phone order' support)
                if (user != null && !user.Deleted && user.Active)
                {
                    int? impersonatedUserId = user.GetAttribute<int?>(SystemUserAttributeNames.ImpersonatedUserId);
                    if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
                    {
                        var impersonatedUser = _userService.GetUserById(impersonatedUserId.Value);
                        if (impersonatedUser != null && !impersonatedUser.Deleted && impersonatedUser.Active)
                        {
                            //set impersonated user
                            _originalUserIfImpersonated = user;
                            user = impersonatedUser;
                        }
                    }
                }
                //load guest user
                if (user == null || user.Deleted || !user.Active)
                {
                    var userCookie = GetUserCookie();
                    if (userCookie != null && !String.IsNullOrEmpty(userCookie.Value))
                    {
                        Guid userGuid;
                        if (Guid.TryParse(userCookie.Value, out userGuid))
                        {
                            var userByCookie = _userService.GetUserByGuid(userGuid);
                            if (userByCookie != null &&
                                //this user (from cookie) should not be registered
                                !userByCookie.IsRegistered() &&
                                //it should not be a built-in 'search engine' user account
                                !userByCookie.IsSearchEngineAccount())
                                user = userByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (user == null || user.Deleted || !user.Active)
                {
                    user = _userService.InsertGuestUser();

                }


                //validation
                if (user != null && !user.Deleted && user.Active)
                {
                    SetUserCookie(user.UserGuid);
                    _cachedUser = user;
                }

                return _cachedUser;
            }
            set
            {
                if (!value.IsSystemAccount)
                {
                    SetUserCookie(value.UserGuid);
                }
                //SetUserCookie(value == null ? Guid.Empty : value.UserGuid);
                _cachedUser = value;
            }
        }

        /// <summary>
        /// Gets or sets the original user (in case the current one is impersonated)
        /// </summary>
        public User OriginalUserIfImpersonated
        {
            get
            {
                return _originalUserIfImpersonated;
            }
        }

        /// <summary>
        /// Get or set current tax display type
        /// </summary>
        public TaxDisplayType TaxDisplayType
        {
            get
            {
                return GetTaxDisplayTypeFor(this.CurrentUser, _siteContext.CurrentSite.Id);
            }
            set
            {
                if (!_taxSettings.AllowUsersToSelectTaxDisplayType)
                    return;

                _attrService.SaveAttribute(this.CurrentUser,
                     SystemUserAttributeNames.TaxDisplayTypeId,
                     (int)value, _siteContext.CurrentSite.Id);
            }
        }

        public TaxDisplayType GetTaxDisplayTypeFor(User user, int siteId)
        {
            if (_cachedTaxDisplayType.HasValue)
            {
                return _cachedTaxDisplayType.Value;
            }

            int? taxDisplayType = null;

            if (_taxSettings.AllowUsersToSelectTaxDisplayType && user != null)
            {
                taxDisplayType = user.GetAttribute<int?>(SystemUserAttributeNames.TaxDisplayTypeId, siteId);
            }

            //if (!taxDisplayType.HasValue && _taxSettings.EuVatEnabled)
            //{
            //    if (user != null && _taxService.Value.IsVatExempt(null, user))
            //    {
            //        taxDisplayType = (int)TaxDisplayType.ExcludingTax;
            //    }
            //}

            if (!taxDisplayType.HasValue)
            {
                var userRoles = user.UserRoles;
                string key = string.Format(ModelCacheEventConsumer.USERRROLES_TAX_DISPLAY_TYPES_KEY, String.Join(",", userRoles.Select(x => x.Id)), siteId);
                var cacheResult = _cacheManager.Get(key, () =>
                {
                    var roleTaxDisplayTypes = userRoles
                        .Where(x => x.TaxDisplayType.HasValue)
                        .OrderByDescending(x => x.TaxDisplayType.Value)
                        .Select(x => x.TaxDisplayType.Value);

                    if (roleTaxDisplayTypes.Any())
                    {
                        return (TaxDisplayType)roleTaxDisplayTypes.FirstOrDefault();
                    }

                    return _taxSettings.TaxDisplayType;
                });

                taxDisplayType = (int)cacheResult;
            }

            _cachedTaxDisplayType = (TaxDisplayType)taxDisplayType.Value;
            return _cachedTaxDisplayType.Value;
        }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Gets or sets the current shop (logged-in manager)
        /// </summary>
        public virtual Vendor CurrentVendor
        {
            get
            {
                if (_cachedVendor != null)
                    return _cachedVendor;

                var currentUser = this.CurrentUser;
                if (currentUser == null)
                    return null;

                var shop = _vendorService.GetVendorById(currentUser.VendorId);

                //validation
                if (shop != null && !shop.Deleted)
                    _cachedVendor = shop;

                return _cachedVendor;
            }
        }

        /// <summary>
        /// Gets or sets the MemberGrade shop (logged-in manager)
        /// </summary>
        public virtual MemberGrade CurrentMemberGrade
        {
            get
            {
                if (_cachedMemberGrade != null)
                    return _cachedMemberGrade;

                var currentUser = this.CurrentUser;
                if (currentUser == null)
                    return null;

                var memberGrade = _memberGradeService.GetMemberGradeById(currentUser.MemberGradeId);

                //validation
                if (memberGrade != null && !memberGrade.Deleted)
                    _cachedMemberGrade = memberGrade;

                return _cachedMemberGrade;
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                int siteId = _siteContext.CurrentSite.Id;
                int customerLangId = 0;

                if (this.CurrentUser != null)
                {
                    customerLangId = this.CurrentUser.GetAttribute<int>(
                        SystemUserAttributeNames.LanguageId,
                        _attrService,
                        _siteContext.CurrentSite.Id);
                }

                #region Get language from URL (if possible)

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && _httpContext != null && _httpContext.Request != null)
                {
                    var helper = new LocalizedUrlHelper(_httpContext.Request, true);
                    string seoCode;
                    if (helper.IsLocalizedUrl(out seoCode))
                    {
                        if (_languageService.IsPublishedLanguage(seoCode, siteId))
                        {
                            // the language is found. now we need to save it
                            var langBySeoCode = _languageService.GetLanguageBySeoCode(seoCode);

                            if (this.CurrentUser != null && customerLangId != langBySeoCode.Id)
                            {
                                customerLangId = langBySeoCode.Id;
                                this.SetUserLanguage(langBySeoCode.Id, siteId);
                            }
                            _cachedLanguage = langBySeoCode;
                            return langBySeoCode;
                        }
                    }
                }

                #endregion

                if (_localizationSettings.DetectBrowserUserLanguage && (customerLangId == 0 || !_languageService.IsPublishedLanguage(customerLangId, siteId)))
                {
                    #region Get Browser UserLanguage

                    // Fallback to browser detected language
                    Language browserLanguage = null;

                    if (_httpContext != null && _httpContext.Request != null && _httpContext.Request.UserLanguages != null)
                    {
                        var userLangs = _httpContext.Request.UserLanguages.Select(x => x.Split(new[] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries)[0]);
                        if (userLangs.Any())
                        {
                            foreach (var culture in userLangs)
                            {
                                browserLanguage = _languageService.GetLanguageByCulture(culture);
                                if (browserLanguage != null && _languageService.IsPublishedLanguage(browserLanguage.Id, siteId))
                                {
                                    // the language is found. now we need to save it
                                    if (this.CurrentUser != null && customerLangId != browserLanguage.Id)
                                    {
                                        customerLangId = browserLanguage.Id;
                                        SetUserLanguage(customerLangId, siteId);
                                    }
                                    _cachedLanguage = browserLanguage;
                                    return browserLanguage;
                                }
                            }
                        }
                    }

                    #endregion
                }

                if (customerLangId > 0 && _languageService.IsPublishedLanguage(customerLangId, siteId))
                {
                    _cachedLanguage = _languageService.GetLanguageById(customerLangId);
                    return _cachedLanguage;
                }

                // Fallback
                customerLangId = _languageService.GetDefaultLanguageId(siteId);
                SetUserLanguage(customerLangId, siteId);

                _cachedLanguage = _languageService.GetLanguageById(customerLangId);
                return _cachedLanguage;
            }
            set
            {
                var languageId = value != null ? value.Id : 0;
                this.SetUserLanguage(languageId, _siteContext.CurrentSite.Id);
                _cachedLanguage = null;
            }
        }


        private void SetUserLanguage(int languageId, int siteId)
        {
            _attrService.SaveAttribute(
               this.CurrentUser,
               SystemUserAttributeNames.LanguageId,
               languageId,
               siteId);
        }

        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        public Currency WorkingCurrency
        {
            get
            {
                if (_cachedCurrency != null)
                    return _cachedCurrency;

                Currency currency = null;

                // return primary site currency when we're in admin area/mode
                if (this.IsAdmin)
                {
                    currency = _siteContext.CurrentSite.PrimarySiteCurrency;
                }

                if (currency == null)
                {
                    // find current customer language
                    var customer = this.CurrentUser;
                    var siteCurrenciesMap = _currencyService.GetAllCurrencies(siteId: _siteContext.CurrentSite.Id).ToDictionary(x => x.Id);

                    if (customer != null && !customer.IsSearchEngineAccount())
                    {
                        // search engines should always crawl by primary site currency
                        var customerCurrencyId = customer.GetAttribute<int?>(SystemUserAttributeNames.CurrencyId, _attrService, _siteContext.CurrentSite.Id);
                        if (customerCurrencyId.GetValueOrDefault() > 0)
                        {
                            if (siteCurrenciesMap.TryGetValue(customerCurrencyId.Value, out currency))
                            {
                                currency = VerifyCurrency(currency);
                                if (currency == null)
                                {
                                    _attrService.SaveAttribute<int?>(customer, SystemUserAttributeNames.CurrencyId, null, _siteContext.CurrentSite.Id);
                                }
                            }
                        }
                    }

                    // if there's only one currency for current site it dominates the primary currency
                    if (siteCurrenciesMap.Count == 1)
                    {
                        currency = siteCurrenciesMap[siteCurrenciesMap.Keys.First()];
                    }

                    // find currency by domain ending
                    if (currency == null && _httpContext != null && _httpContext.Request != null && _httpContext.Request.Url != null)
                    {
                        currency = siteCurrenciesMap.Values.GetByDomainEnding(_httpContext.Request.Url.Authority);
                    }

                    // get PrimarySiteCurrency
                    if (currency == null)
                    {
                        currency = VerifyCurrency(_siteContext.CurrentSite.PrimarySiteCurrency);
                    }

                    // get the first published currency for current site
                    if (currency == null)
                    {
                        currency = siteCurrenciesMap.Values.FirstOrDefault();
                    }
                }

                // if not found in currencies filtered by the current site, then return any currency
                if (currency == null)
                {
                    currency = _currencyService.GetAllCurrencies().FirstOrDefault();
                }

                // no published currency available (fix it)
                if (currency == null)
                {
                    currency = _currencyService.GetAllCurrencies(true).FirstOrDefault();
                    if (currency != null)
                    {
                        currency.Published = true;
                        _currencyService.UpdateCurrency(currency);
                    }
                }

                _cachedCurrency = currency;
                return _cachedCurrency;
            }
            set
            {
                int? id = value != null ? value.Id : (int?)null;
                _attrService.SaveAttribute<int?>(this.CurrentUser, SystemUserAttributeNames.CurrencyId, id, _siteContext.CurrentSite.Id);
                _cachedCurrency = null;
            }
        }
        private Currency VerifyCurrency(Currency currency)
        {
            if (currency != null && !currency.Published)
            {
                return null;
            }
            return currency;
        }

        [Obsolete("Use ILanguageService.IsPublishedLanguage() instead")]
        public bool IsPublishedLanguage(string seoCode, int storeId = 0)
        {
            return _languageService.IsPublishedLanguage(seoCode, storeId);
        }

        [Obsolete("Use ILanguageService.GetDefaultLanguageSeoCode() instead")]
        public string GetDefaultLanguageSeoCode(int storeId = 0)
        {
            return _languageService.GetDefaultLanguageSeoCode(storeId);
        }

    }
}
