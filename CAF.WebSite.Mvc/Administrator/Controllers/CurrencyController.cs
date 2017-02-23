using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Mvc.Admin.Models.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class CurrencyController : AdminControllerBase
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly ICommonServices _services;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        #endregion

        #region Constructors

        public CurrencyController(ICurrencyService currencyService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService,
            ICommonServices services,
            ISiteMappingService siteMappingService,
            IDateTimeHelper dateTimeHelper)
        {
            this._currencyService = currencyService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._services = services;
            this._siteMappingService = siteMappingService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        [NonAction]
        public void UpdateLocales(Currency currency, CurrencyModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(currency,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }


        private void PrepareCurrencyModel(CurrencyModel model, Currency currency, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var allSites = _services.SiteService.GetAllSites();

            model.AvailableSites = allSites.Select(s => s.ToModel()).ToList();

            if (currency != null)
            {
                model.PrimarySiteCurrencySites = allSites
                    .Where(x => x.PrimarySiteCurrencyId == currency.Id)
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = Url.Action("Edit", "Site", new { id = x.Id })
                    })
                    .ToList();

               
            }

            if (!excludeProperties)
            {
                model.SelectedSiteIds = (currency == null ? new int[0] : _siteMappingService.GetSitesIdsWithAccess(currency));
            }
        }
        private bool IsAttachedToSite(Currency currency, IList<Site> sites, bool force)
        {
            var attachedSite = sites.FirstOrDefault(x => x.PrimarySiteCurrencyId == currency.Id);

            if (attachedSite != null)
            {
                if (force || (!force && !currency.Published))
                {
                    NotifyError(T("Admin.Configuration.Currencies.DeleteOrPublishSiteConflict", attachedSite.Name));
                    return true;
                }

                // Must site limitations include the site where the currency is attached as primary or exchange rate currency?
                //if (currency.LimitedToSites)
                //{
                //	if (selectedSiteIds == null)
                //		selectedSiteIds = _siteMappingService.GetSiteMappingsFor("Currency", currency.Id).Select(x => x.SiteId).ToArray();

                //	if (!selectedSiteIds.Contains(attachedSite.Id))
                //	{
                //		NotifyError(T("Admin.Configuration.Currencies.SiteLimitationConflict", attachedSite.Name));
                //		return true;
                //	}
                //}
            }
            return false;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();
            var Site = _services.SiteContext.CurrentSite;
            var models = _currencyService.GetAllCurrencies(true).Select(x => x.ToModel()).ToList();

            foreach (var model in models)
            {
                model.IsPrimarySiteCurrency = (Site.PrimarySiteCurrencyId == model.Id);
            }

            return View();
        }
        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();
            var currencies = _currencyService.GetAllCurrencies(true);

            return DataTablesResult.Create(currencies.Select(x => x.ToModel()).AsQueryable(), dataTableParam);
        }


        #endregion

        #region Create / Edit / Delete / Save

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var model = new CurrencyModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //Sites
            PrepareCurrencyModel(model, null, false);
            //default values
            model.Published = true;
            model.Rate = 1;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(CurrencyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var currency = model.ToEntity();
                currency.CreatedOnUtc = DateTime.UtcNow;
                currency.UpdatedOnUtc = DateTime.UtcNow;

                _currencyService.InsertCurrency(currency);

                //locales
                UpdateLocales(currency, model);

                //Sites
                _siteMappingService.SaveSiteMappings<Currency>(currency, model.SelectedSiteIds);

                NotifySuccess(_services.Localization.GetResource("Admin.Configuration.Currencies.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = currency.Id }) : RedirectToAction("List");
            }
            //Sites
            PrepareCurrencyModel(model, null, true);
            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyById(id);
            if (currency == null)
                return RedirectToAction("List");

            var model = currency.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(currency.CreatedOnUtc, DateTimeKind.Utc);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = currency.GetLocalized(x => x.Name, languageId, false, false);
            });

            foreach (var ending in model.DomainEndings.SplitSafe(","))
            {
                var item = model.AvailableDomainEndings.FirstOrDefault(x => x.Value.IsCaseInsensitiveEqual(ending));
                if (item == null)
                    model.AvailableDomainEndings.Add(new SelectListItem() { Text = ending, Value = ending, Selected = true });
                else
                    item.Selected = true;
            }

            //Sites
            PrepareCurrencyModel(model, currency, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(CurrencyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();
            var currency = _currencyService.GetCurrencyById(model.Id);
            if (currency == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                currency = model.ToEntity(currency);

                if (!IsAttachedToSite(currency, _services.SiteService.GetAllSites(), false))
                {
                    currency.UpdatedOnUtc = DateTime.UtcNow;

                    _currencyService.UpdateCurrency(currency);

                    //locales
                    UpdateLocales(currency, model);

                    //Sites
                    _siteMappingService.SaveSiteMappings<Currency>(currency, model.SelectedSiteIds);

                    NotifySuccess(_services.Localization.GetResource("Admin.Configuration.Currencies.Updated"));
                    return continueEditing ? RedirectToAction("Edit", new { id = currency.Id }) : RedirectToAction("List");
                }
            }

            //If we got this far, something failed, redisplay form
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(currency.CreatedOnUtc, DateTimeKind.Utc);

            //Sites
            PrepareCurrencyModel(model, currency, true);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyById(id);
            if (currency == null)
                return RedirectToAction("List");

            try
            {
                if (!IsAttachedToSite(currency, _services.SiteService.GetAllSites(), true))
                {
                    _currencyService.DeleteCurrency(currency);

                    NotifySuccess(_services.Localization.GetResource("Admin.Configuration.Currencies.Deleted"));
                    return RedirectToAction("List");
                }
            }
            catch (Exception exc)
            {
                NotifyError(exc);
            }

            return RedirectToAction("Edit", new { id = currency.Id });
        }
 
        #endregion
    }
}
