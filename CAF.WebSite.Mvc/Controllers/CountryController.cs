
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Globalization;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Directory;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.Mvc.Controllers
{

    public partial class CountryController : PublicControllerBase
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly ICityService _cityService;
        private readonly IDistrictService _districtService;
        #endregion

        #region Constructors

        public CountryController(ICountryService countryService,
            IStateProvinceService stateProvinceService,
              ICityService cityService,
            IDistrictService districtService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            _cityService = cityService;
            _districtService = districtService;
        }

        #endregion

        #region States / provinces

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetStatesByCountryId(string countryId, bool addEmptyStateIfRequired)
        {
            //this action method gets called via an ajax request
            if (String.IsNullOrEmpty(countryId))
                throw new ArgumentNullException("countryId");

            string cacheKey = string.Format(ModelCacheEventConsumer.STATEPROVINCES_BY_COUNTRY_MODEL_KEY, countryId, addEmptyStateIfRequired, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
                var states = _stateProvinceService.GetStateProvincesByCountryId(country != null ? country.Id : 0).ToList();
                var result = (from s in states
                              select new { id = s.Id, name = s.GetLocalized(x => x.Name) })
                              .ToList();

                if (addEmptyStateIfRequired && result.Count == 0)
                    result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                return result;

            });

            return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCitysByProvinceId(string stateId, bool? addEmptyStateIfRequired)
        {

            if (String.IsNullOrEmpty(stateId))
                throw new ArgumentNullException("stateId");
            string cacheKey = string.Format(ModelCacheEventConsumer.CITYS_BY_STATEPROVINCE_MODEL_KEY, stateId, addEmptyStateIfRequired, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var stateProvince = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateId));
                var citys = _cityService.GetCitysByProvinceId(stateProvince != null ? stateProvince.Id : 0).ToList();
                var result = (from s in citys select new { id = s.Id, name = s.Name }).ToList();

                if (addEmptyStateIfRequired.HasValue && addEmptyStateIfRequired.Value && result.Count == 0)
                    result.Insert(0, new { id = 0, name = T("Admin.Address.OtherNonUS").Text });


                return result;

            });
            return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetDistrictsByCityId(string cityId, bool? addEmptyStateIfRequired)
        {
            if (String.IsNullOrEmpty(cityId))
                throw new ArgumentNullException("cityId");
            string cacheKey = string.Format(ModelCacheEventConsumer.DISTRICTS_BY_CITY_MODEL_KEY, cityId, addEmptyStateIfRequired, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var city = _cityService.GetCityById(cityId.ToInt());

                var districts = _districtService.GetDistrictsByCityId(city != null ? city.Id : 0).ToList();
                var result = (from s in districts select new { id = s.Id, name = s.Name }).ToList();

                if (addEmptyStateIfRequired.HasValue && addEmptyStateIfRequired.Value && result.Count == 0)
                    result.Insert(0, new { id = 0, name = T("Admin.Address.OtherNonUS").Text });

                return result;

            });
            return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}

