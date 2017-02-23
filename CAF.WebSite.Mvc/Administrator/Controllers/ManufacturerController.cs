using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Mvc.Admin.Models.Topics;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Application.Services.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Manufacturers;
using CAF.WebSite.Mvc.Admin.Models.Manufacturers;
using CAF.Infrastructure.Core.Domain.Cms.Manufacturers;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Application.Services.Helpers;
using CAF.Infrastructure.Core.Logging;
using CAF.Mvc.JQuery.Datatables.Models;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class ManufacturerController : AdminControllerBase
    {
        #region Fields
        private readonly IUrlRecordService _urlRecordService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPictureService _pictureService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IUserActivityService _userActivityService;
        #endregion Fields

        #region Constructors

        public ManufacturerController(IManufacturerService manufacturerService, ILanguageService languageService,
            IModelTemplateService modelTemplateService, IUrlRecordService urlRecordService,
            ILocalizedEntityService localizedEntityService, ILocalizationService localizationService,
            IPictureService pictureService, AdminAreaSettings adminAreaSettings,
            IDateTimeHelper dateTimeHelper, IUserActivityService userActivityService,
            IPermissionService permissionService, IEventPublisher eventPublisher)
        {

            this._manufacturerService = manufacturerService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._urlRecordService = urlRecordService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._eventPublisher = eventPublisher;
            this._pictureService = pictureService;
            this._adminAreaSettings = adminAreaSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._userActivityService = userActivityService;
        }

        #endregion

        #region Utilities
        [NonAction]
        protected void UpdateLocales(Manufacturer manufacturer, ManufacturerModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = manufacturer.ValidateSeName(localized.SeName, localized.Name, false, localized.LanguageId);
                _urlRecordService.SaveSlug(manufacturer, seName, localized.LanguageId);
            }
        }


        [NonAction]
        protected void UpdatePictureSeoNames(Manufacturer manufacturer)
        {
            var picture = _pictureService.GetPictureById(manufacturer.PictureId.GetValueOrDefault());
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(manufacturer.Name));
        }

        [NonAction]
        private void PrepareManufacturerModel(ManufacturerModel model, Manufacturer manufacturer, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.GridPageSize = _adminAreaSettings.GridPageSize;


            if (manufacturer != null)
            {
                model.CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(manufacturer.CreatedOnUtc, DateTimeKind.Utc);
                model.ModifiedOnUtc = _dateTimeHelper.ConvertToUserTime(manufacturer.ModifiedOnUtc.Value, DateTimeKind.Utc);
            }
        }

        #endregion

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();
            var model = new ManufacturerListModel
            {
                GridPageSize = _adminAreaSettings.GridPageSize
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam, ManufacturerListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var manufacturers = _manufacturerService.GetAllManufacturers(model.SearchManufacturerName, dataTableParam.PageIndex, dataTableParam.PageSize, true);
          
            var total = manufacturers.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = manufacturers.Select(x => x.ToModel()).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        //ajax
        // codehint: sm-edit
        public ActionResult AllManufacturers(string label, int selectedId)
        {
            var categories = _manufacturerService.GetAllManufacturers(showHidden: true);
            var cquery = categories.AsQueryable();
            if (label.HasValue())
            {
                categories.Insert(0, new Manufacturer { Name = label, Id = 0 });
            }

            var query =
                from c in cquery
                select new
                {
                    id = c.Id.ToString(),
                    text = c.Name,
                    selected = c.Id == selectedId
                };

            var data = query.ToList();

            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var model = new ManufacturerModel();
            //locales
            AddLocales(_languageService, model.Locales);

            PrepareManufacturerModel(model, null, false);

            //default values
            model.PageSize = 12;
            model.Published = true;

            model.AllowUsersToSelectPageSize = true;

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(ManufacturerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var manufacturer = model.ToEntity();
                manufacturer.CreatedOnUtc = DateTime.UtcNow;
                manufacturer.ModifiedOnUtc = DateTime.UtcNow;

                MediaHelper.UpdatePictureTransientStateFor(manufacturer, m => m.PictureId);

                _manufacturerService.InsertManufacturer(manufacturer);

                //search engine name
                model.SeName = manufacturer.ValidateSeName(model.SeName, manufacturer.Name, true);
                _urlRecordService.SaveSlug(manufacturer, model.SeName, 0);

                //locales
                UpdateLocales(manufacturer, model);

                //update picture seo file name
                UpdatePictureSeoNames(manufacturer);


                //activity log
                _userActivityService.InsertActivity("AddNewManufacturer", _localizationService.GetResource("ActivityLog.AddNewManufacturer"), manufacturer.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Catalog.Manufacturers.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = manufacturer.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareManufacturerModel(model, null, true);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var manufacturer = _manufacturerService.GetManufacturerById(id);
            if (manufacturer == null || manufacturer.Deleted)
                return RedirectToAction("List");

            var model = manufacturer.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = manufacturer.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = manufacturer.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = manufacturer.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = manufacturer.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = manufacturer.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = manufacturer.GetSeName(languageId, false, false);
            });

            PrepareManufacturerModel(model, manufacturer, false);

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(ManufacturerModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var manufacturer = _manufacturerService.GetManufacturerById(model.Id);
            if (manufacturer == null)
                //No manufacturer found with the specified id
                return RedirectToAction("List");




            if (ModelState.IsValid)
            {
                manufacturer = model.ToEntity(manufacturer);
                MediaHelper.UpdatePictureTransientStateFor(manufacturer, m => m.PictureId);
                manufacturer.ModifiedOnUtc = DateTime.UtcNow;
                _manufacturerService.UpdateManufacturer(manufacturer);

                //search engine name
                model.SeName = manufacturer.ValidateSeName(model.SeName, manufacturer.Name, true);
                _urlRecordService.SaveSlug(manufacturer, model.SeName, 0);

                //locales
                UpdateLocales(manufacturer, model);

                //update picture seo file name
                UpdatePictureSeoNames(manufacturer);


                //activity log
                _userActivityService.InsertActivity("EditManufacturer", _localizationService.GetResource("ActivityLog.EditManufacturer"), manufacturer.Name);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Manufacturers.Updated"));
                return continueEditing ? RedirectToAction("Edit", manufacturer.Id) : RedirectToAction("List");
            }


            PrepareManufacturerModel(model, manufacturer, true);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var manufacturer = _manufacturerService.GetManufacturerById(id);
            if (manufacturer == null)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            _manufacturerService.DeleteManufacturer(manufacturer);
            //activity log
            _userActivityService.InsertActivity("DeleteManufacturer", _localizationService.GetResource("ActivityLog.DeleteManufacturer"), manufacturer.Name);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Manufacturers.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}
