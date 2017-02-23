using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Logging;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Data;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
 

namespace CAF.WebSite.Mvc.Admin.Controllers
{
 
    public class SpecificationAttributeController : AdminControllerBase
    {
        #region Fields

        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public SpecificationAttributeController(ISpecificationAttributeService specificationAttributeService,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, IUserActivityService customerActivityService,
            IPermissionService permissionService)
        {
            this._specificationAttributeService = specificationAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        [NonAction]
        public void UpdateAttributeLocales(SpecificationAttribute specificationAttribute, SpecificationAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(specificationAttribute, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        [NonAction]
        public void UpdateOptionLocales(SpecificationAttributeOption specificationAttributeOption, SpecificationAttributeOptionModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(specificationAttributeOption, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        private void AddMultipleOptionNames(SpecificationAttributeOptionModel model)
        {
            var values = model.Name.SplitSafe(";");
            int order = model.DisplayOrder;

            for (int i = 0; i < values.Length; ++i)
            {
                var sao = new SpecificationAttributeOption()
                {
                    Name = values[i].Trim(),
                    DisplayOrder = order++,
                    SpecificationAttributeId = model.SpecificationAttributeId
                };

                _specificationAttributeService.InsertSpecificationAttributeOption(sao);

                foreach (var localized in model.Locales.Where(l => l.Name.HasValue()))
                {
                    var localizedValues = localized.Name.SplitSafe(";");
                    string value = (i < localizedValues.Length ? localizedValues[i].Trim() : sao.Name);

                    _localizedEntityService.SaveLocalizedValue(sao, x => x.Name, value, localized.LanguageId);
                }
            }
        }

        #endregion

        #region Methods

        #region Article attribute 
        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

           
            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDeliveryTimes))
                return AccessDeniedView();
            var data = _specificationAttributeService.GetSpecificationAttributes()
                     .Expand(x => x.SpecificationAttributeOptions).AsEnumerable()
                     .Select(x=> {
                         var model = x.ToModel();
                         model.OptionCount = x.SpecificationAttributeOptions.Count;
                         return model;
                     })
                     .ToList();

            return DataTablesResult.Create(data.AsQueryable(), dataTableParam);
        }
        
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var model = new SpecificationAttributeModel();

            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttribute = model.ToEntity();
                _specificationAttributeService.InsertSpecificationAttribute(specificationAttribute);
                UpdateAttributeLocales(specificationAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewSpecAttribute", _localizationService.GetResource("ActivityLog.AddNewSpecAttribute"), specificationAttribute.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = specificationAttribute.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();


            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            var model = specificationAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = specificationAttribute.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();


            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(model.Id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttribute = model.ToEntity(specificationAttribute);
                _specificationAttributeService.UpdateSpecificationAttribute(specificationAttribute);

                UpdateAttributeLocales(specificationAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditSpecAttribute", _localizationService.GetResource("ActivityLog.EditSpecAttribute"), specificationAttribute.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Updated"));
                return continueEditing ? RedirectToAction("Edit", specificationAttribute.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            _specificationAttributeService.DeleteSpecificationAttribute(specificationAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteSpecAttribute", _localizationService.GetResource("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name);

            NotifySuccess(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (selectedIds != null && selectedIds.Count > 0)
            {
                var attributes = _specificationAttributeService.GetSpecificationAttributesByIds(selectedIds.ToArray()).ToList();
                string deletedNames = string.Join(", ", attributes.Select(x => x.Name));

                foreach (var attribute in attributes)
                    _specificationAttributeService.DeleteSpecificationAttribute(attribute);

                _customerActivityService.InsertActivity("DeleteSpecAttribute", _localizationService.GetResource("ActivityLog.DeleteSpecAttribute"), deletedNames);
            }

            return Json(new { Result = true });
        }
        [HttpPost]
        public ActionResult ArticleMappingEdit(int specificationAttributeId, string field, bool value)
        {
            _specificationAttributeService.UpdateArticleSpecificationMapping(specificationAttributeId, field, value);

            return Json(new
            {
                message = _localizationService.GetResource("Admin.Common.DataEditSuccess"),
                notificationType = "success"
            });
        }
        #endregion

        #region Article attribute options

        [HttpPost]
        public ActionResult OptionList(int specificationAttributeId, DataTablesParam dataTableParam)
        {          
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(specificationAttributeId);

            var total = options.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = options.Select(x =>
                {
                    var model = x.ToModel();
                    
                    return model;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public ActionResult OptionCreatePopup(int specificationAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var model = new SpecificationAttributeOptionModel();
            model.SpecificationAttributeId = specificationAttributeId;
            //locales
            AddLocales(_languageService, model.Locales);

            ViewBag.MultipleEnabled = true;

            return View(model);
        }

        [HttpPost]
        public ActionResult OptionCreatePopup(string btnId, string formId, SpecificationAttributeOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(model.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                if (model.Multiple)
                {
                    AddMultipleOptionNames(model);
                }
                else
                {
                    var sao = model.ToEntity();

                    _specificationAttributeService.InsertSpecificationAttributeOption(sao);
                    UpdateOptionLocales(sao, model);
                }

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult OptionEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var sao = _specificationAttributeService.GetSpecificationAttributeOptionById(id);
            if (sao == null)
                return RedirectToAction("List");

            var model = sao.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = sao.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult OptionEditPopup(string btnId, string formId, SpecificationAttributeOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var sao = _specificationAttributeService.GetSpecificationAttributeOptionById(model.Id);
            if (sao == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                sao = model.ToEntity(sao);
                _specificationAttributeService.UpdateSpecificationAttributeOption(sao);

                UpdateOptionLocales(sao, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

    
        public ActionResult OptionDelete(int optionId, int specificationAttributeId)
        {
            
            if (_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
            {
                var sao = _specificationAttributeService.GetSpecificationAttributeOptionById(optionId);

                _specificationAttributeService.DeleteSpecificationAttributeOption(sao);
            }

            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Catalog.Attributes.ArticleAttributeOption.Deleted") }, JsonRequestBehavior.AllowGet);
        }

        //ajax
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetOptionsByAttributeId(string attributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            // This action method gets called via an ajax request
            if (String.IsNullOrEmpty(attributeId))
                throw new ArgumentNullException("attributeId");

            var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(Convert.ToInt32(attributeId));
            var result = (from o in options
                          select new { id = o.Id, name = o.Name, text = o.Name }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetAttributeValue(string pk, string value, string name, FormCollection form)
        {
            try
            {
                //name is the entity id of product specification attribute mapping
                var specificationAttribute = _specificationAttributeService.GetArticleSpecificationAttributeById(Convert.ToInt32(name));
                specificationAttribute.SpecificationAttributeOptionId = Convert.ToInt32(value);
                _specificationAttributeService.UpdateArticleSpecificationAttribute(specificationAttribute);
                Response.StatusCode = 200;

                // we give back the name to xeditable to overwrite the grid data in success event when a grid element got updated
                return Json(new { name = specificationAttribute.SpecificationAttributeOption.Name });
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(501, ex.Message);
            }
        }

        #endregion

        #endregion



    }
}
