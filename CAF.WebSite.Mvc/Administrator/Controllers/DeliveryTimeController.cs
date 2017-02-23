using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Mvc.Admin.Models.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class DeliveryTimeController : AdminControllerBase
    {
        #region Fields

        private readonly IDeliveryTimeService _deliveryTimeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Constructors

        public DeliveryTimeController(IDeliveryTimeService deliveryTimeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService)
        {
            this._deliveryTimeService = deliveryTimeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
        }

        #endregion

        #region Utilities

        [NonAction]
        public void UpdateLocales(DeliveryTime deliveryTime, DeliveryTimeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(deliveryTime,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }


        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDeliveryTimes))
                return AccessDeniedView();
 
            return View();
        }
        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDeliveryTimes))
                return AccessDeniedView();
            var links = _deliveryTimeService.GetAllDeliveryTimes();
            return DataTablesResult.Create(links.Select(x => x.ToModel()).AsQueryable(), dataTableParam);
        }
        

        #endregion

        #region Create / Edit / Delete / Save

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDeliveryTimes))
                return AccessDeniedView();

            var model = new DeliveryTimeModel();
            //locales
            AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(DeliveryTimeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDeliveryTimes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var deliveryTime = model.ToEntity();

                _deliveryTimeService.InsertDeliveryTime(deliveryTime);
                //locales
                UpdateLocales(deliveryTime, model);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.DeliveryTime.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = deliveryTime.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDeliveryTimes))
                return AccessDeniedView();

            var deliveryTime = _deliveryTimeService.GetDeliveryTimeById(id);
            if (deliveryTime == null)
                //No currency found with the specified id
                return RedirectToAction("List");

            var model = deliveryTime.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = deliveryTime.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(DeliveryTimeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDeliveryTimes))
                return AccessDeniedView();

            var deliveryTime = _deliveryTimeService.GetDeliveryTimeById(model.Id);
            if (deliveryTime == null)
                //No currency found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                deliveryTime = model.ToEntity(deliveryTime);

                UpdateLocales(deliveryTime, model);
                _deliveryTimeService.UpdateDeliveryTime(deliveryTime);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.DeliveryTimes.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = deliveryTime.Id }) : RedirectToAction("List");
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDeliveryTimes))
                return AccessDeniedView();

            var deliveryTime = _deliveryTimeService.GetDeliveryTimeById(id);
            if (deliveryTime == null)
                //No delivery time found with the specified id
                return RedirectToAction("List");

            try
            {

                _deliveryTimeService.DeleteDeliveryTime(deliveryTime);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.DeliveryTimes.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("Edit", new { id = deliveryTime.Id });
            }
        }

        [HttpPost]
        public ActionResult Save(FormCollection formValues)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDeliveryTimes))
                return AccessDeniedView();

            //_currencySettings.ActiveExchangeRateProviderSystemName = formValues["exchangeRateProvider"];
            //_currencySettings.AutoUpdateEnabled = formValues["autoUpdateEnabled"].Equals("false") ? false : true;
            //_settingService.SaveSetting(_deliveryTimeSettings);
            return RedirectToAction("List", "DeliveryTime");
        }

        #endregion
    }
}
