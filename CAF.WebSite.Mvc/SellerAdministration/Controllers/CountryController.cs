using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Seller.Controllers
{

    public class CountryController : SellerAdminControllerBase
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly IDistrictService _districtService;
        private readonly ILocalizationService _localizationService;
        private readonly IAddressService _addressService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Constructors

        public CountryController(ICountryService countryService,
            IStateProvinceService stateProvinceService, ILocalizationService localizationService,
            IAddressService addressService, IPermissionService permissionService,
            ICityService cityService,
            IDistrictService districtService,
            ILocalizedEntityService localizedEntityService, ILanguageService languageService)
        {
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _districtService = districtService;
            this._localizationService = localizationService;
            this._addressService = addressService;
            this._permissionService = permissionService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
        }

        #endregion



        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetStatesByCountryId(string countryId, bool? addEmptyStateIfRequired, bool? addAsterisk)
        {
            // permission validation is not required here
            // This action method gets called via an ajax request

            var country = _countryService.GetCountryById(countryId.ToInt());

            var states = country != null ? _stateProvinceService.GetStateProvincesByCountryId(country.Id, true).ToList() : new List<StateProvince>();
            var result = (from s in states select new { id = s.Id, name = s.Name }).ToList();

            if (addEmptyStateIfRequired.HasValue && addEmptyStateIfRequired.Value && result.Count == 0)
                result.Insert(0, new { id = 0, name = T("Seller.Address.OtherNonUS").Text });

            if (addAsterisk.HasValue && addAsterisk.Value)
                result.Insert(0, new { id = 0, name = "*" });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCitysByProvinceId(string stateId, bool? addEmptyStateIfRequired)
        {

            if (String.IsNullOrEmpty(stateId))
                throw new ArgumentNullException("stateId");

            var stateProvince = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateId));
            var citys = _cityService.GetCitysByProvinceId(stateProvince != null ? stateProvince.Id : 0).ToList();
            var result = (from s in citys select new { id = s.Id, name = s.Name }).ToList();

            if (addEmptyStateIfRequired.HasValue && addEmptyStateIfRequired.Value && result.Count == 0)
                result.Insert(0, new { id = 0, name = T("Seller.Address.OtherNonUS").Text });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetDistrictsByCityId(string cityId, bool? addEmptyStateIfRequired)
        {
            if (String.IsNullOrEmpty(cityId))
                throw new ArgumentNullException("cityId");

            var city = _cityService.GetCityById(cityId.ToInt());

            var districts = _districtService.GetDistrictsByCityId(city != null ? city.Id : 0).ToList();
            var result = (from s in districts select new { id = s.Id, name = s.Name }).ToList();

            if (addEmptyStateIfRequired.HasValue && addEmptyStateIfRequired.Value && result.Count == 0)
                result.Insert(0, new { id = 0, name = T("Seller.Address.OtherNonUS").Text });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
