using System.Linq;
using System.Web.Mvc;

using System;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Logging;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
 
    public class ProductAttributeController : AdminControllerBase
    {
        #region Fields

        private readonly IProductAttributeService _productAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public ProductAttributeController(IProductAttributeService productAttributeService,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, IUserActivityService customerActivityService,
            IPermissionService permissionService)
        {
            this._productAttributeService = productAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities
        private void AddMultipleOptionNames(ProductAttributeOptionModel model)
        {
            var values = model.Name.SplitSafe(";");
            int order = model.DisplayOrder;

            for (int i = 0; i < values.Length; ++i)
            {
                var sao = new ProductAttributeOption()
                {
                    Name = values[i].Trim(),
                    DisplayOrder = order++,
                    ProductAttributeId = model.ProductAttributeId
                };

                _productAttributeService.InsertProductAttributeOption(sao);

                foreach (var localized in model.Locales.Where(l => l.Name.HasValue()))
                {
                    var localizedValues = localized.Name.SplitSafe(";");
                    string value = (i < localizedValues.Length ? localizedValues[i].Trim() : sao.Name);

                    _localizedEntityService.SaveLocalizedValue(sao, x => x.Name, value, localized.LanguageId);
                }
            }
        }
        [NonAction]
        public void UpdateLocales(ProductAttribute productAttribute, ProductAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(productAttribute,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);
            }
        }

        [NonAction]
        public void UpdateOptionLocales(ProductAttributeOption productAttributeOption, ProductAttributeOptionModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productAttributeOption, x => x.Name, localized.Name, localized.LanguageId);
            }
        }
        #endregion

        #region Methods

        #region Product attribute 
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
            var productAttributes = _productAttributeService.GetAllProductAttributes();
            return DataTablesResult.Create(productAttributes.Select(x => x.ToModel()).AsQueryable(), dataTableParam);
        }
        
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var model = new ProductAttributeModel();

            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(ProductAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var productAttribute = model.ToEntity();
                _productAttributeService.InsertProductAttribute(productAttribute);
                UpdateLocales(productAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewProductAttribute", _localizationService.GetResource("ActivityLog.AddNewProductAttribute"), productAttribute.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = productAttribute.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            var model = productAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = productAttribute.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = productAttribute.GetLocalized(x => x.Description, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(ProductAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(model.Id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                productAttribute = model.ToEntity(productAttribute);
                _productAttributeService.UpdateProductAttribute(productAttribute);

                UpdateLocales(productAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditProductAttribute", _localizationService.GetResource("ActivityLog.EditProductAttribute"), productAttribute.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Updated"));
                return continueEditing ? RedirectToAction("Edit", productAttribute.Id) : RedirectToAction("List");
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

            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            _productAttributeService.DeleteProductAttribute(productAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteProductAttribute", _localizationService.GetResource("ActivityLog.DeleteProductAttribute"), productAttribute.Name);

            NotifySuccess(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Deleted"));
            return RedirectToAction("List");
        }
        #endregion

        #region Product attribute options

        [HttpPost]
        public ActionResult OptionList(int productAttributeId,DataTablesParam dataTableParam)
        {          
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var options = _productAttributeService.GetProductAttributeOptionsByProductAttribute(productAttributeId);

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

        public ActionResult OptionCreatePopup(int productAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var model = new ProductAttributeOptionModel();
            model.ProductAttributeId = productAttributeId;
            //locales
            AddLocales(_languageService, model.Locales);

            ViewBag.MultipleEnabled = true;

            return View(model);
        }

        [HttpPost]
        public ActionResult OptionCreatePopup(string btnId, string formId, ProductAttributeOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(model.ProductAttributeId);
            if (productAttribute == null)
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

                    _productAttributeService.InsertProductAttributeOption(sao);
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

            var sao = _productAttributeService.GetProductAttributeOptionById(id);
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
        public ActionResult OptionEditPopup(string btnId, string formId, ProductAttributeOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var sao = _productAttributeService.GetProductAttributeOptionById(model.Id);
            if (sao == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                sao = model.ToEntity(sao);
                _productAttributeService.UpdateProductAttributeOption(sao);

                UpdateOptionLocales(sao, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

    
        public ActionResult OptionDelete(int optionId, int productAttributeId)
        {
            if (_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
            {
                var sao = _productAttributeService.GetProductAttributeOptionById(optionId);

                _productAttributeService.DeleteProductAttributeOption(sao);
            }

            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributeOption.Deleted") }, JsonRequestBehavior.AllowGet);
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

            var options = _productAttributeService.GetProductAttributeOptionsByProductAttribute(Convert.ToInt32(attributeId));
            var result = (from o in options
                          select new { id = o.Id, name = o.Name, text = o.Name }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #endregion



    }
}
