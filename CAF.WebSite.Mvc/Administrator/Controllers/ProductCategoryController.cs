using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Authentication;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.WebSite.Application.WebUI.MvcCaptcha;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.Channels;
using CAF.WebSite.Mvc.Admin.Models.Users;
using CAF.WebSite.Application.Services.Media;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.Mvc.JQuery.Datatables.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Channels;
using CAF.Infrastructure.Core.Domain;
using System.Threading.Tasks;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class ProductCategoryController : AdminControllerBase
    {
        #region Fields
        private readonly IProductCategoryService _categoryService;
        private readonly IModelTemplateService _modelTemplateService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly IUserActivityService _userActivityService;
        private readonly IAclService _aclService;
        private readonly ISiteService _siteService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;
        private readonly IChannelService _channelService;
        private readonly SiteInformationSettings _siteSettings;
        #endregion

        #region Ctor

        public ProductCategoryController(
            IModelTemplateService modelTemplateService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IUserService userService,
            UserSettings userSettings,
            IProductCategoryService categoryService,
            IUrlRecordService urlRecordService,
            ILanguageService languageService,
            IPictureService pictureService,
            IAclService aclService,
            IUserActivityService userActivityService,
            ISiteService siteService, ISiteMappingService siteMappingService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            ArticleCatalogSettings catalogSettings,
            IPermissionService permissionService,
            IChannelService channelService,
            IWorkContext workContext,
            ISiteContext siteContext,
            SiteInformationSettings siteSettings)
        {
            this._modelTemplateService = modelTemplateService;
            this._categoryService = categoryService;
            this._localizedEntityService = localizedEntityService;
            this._urlRecordService = urlRecordService;
            this._localizationService = localizationService;
            this._userService = userService;
            this._userSettings = userSettings;
            this._userActivityService = userActivityService;
            this._aclService = aclService;
            this._languageService = languageService;
            this._pictureService = pictureService;
            this._siteService = siteService;
            this._siteMappingService = siteMappingService;
            this._dateTimeHelper = dateTimeHelper;
            this._eventPublisher = eventPublisher;
            this._workContext = workContext;
            this._catalogSettings = catalogSettings;
            this._permissionService = permissionService;
            this._channelService = channelService;
            this._siteContext = siteContext;
            this._siteSettings = siteSettings;
        }
        #endregion

        #region Utilities

        private void PrepareCategoryModel(ProductCategoryModel model, ProductCategory category)
        {

            if (model == null)
                throw new ArgumentNullException("model");

            if (category != null)
            {
                model.CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(category.CreatedOnUtc, DateTimeKind.Utc);
                model.ModifiedOnUtc = (category.ModifiedOnUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(category.ModifiedOnUtc.Value, DateTimeKind.Utc) : category.ModifiedOnUtc);
            }

        }
        /// <summary>
        /// 获取排序
        /// </summary>
        /// <param name="model"></param>
        /// <param name="category"></param>
        private void ProcessingDisplayOrder(ProductCategoryModel model)
        {
            int num = _categoryService.GetAllProductCategoriesByParentCategoryId(model.ParentCategoryId ?? 0).Count() + 1;
            model.DisplayOrder = num;
        }
        /// <summary>
        /// 更新分类路径
        /// </summary>
        /// <param name="model"></param>
        /// <param name="category"></param>
        private void UpdateCategoryPath(ProductCategoryModel model, ProductCategory category)
        {

            if (!model.ParentCategoryId.HasValue || model.ParentCategoryId == 0)
            {
                category.Path = category.Id.ToString();
                return;
            }
            var parentCategory = _categoryService.GetProductCategoryById(model.ParentCategoryId.Value);
            category.Path = string.Format("{0}|{1}", parentCategory.Path, category.Id);
        }

        [NonAction]
        protected void UpdateLocales(ProductCategory category, ProductCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category, x => x.Name, localized.Name, localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category, x => x.FullName, localized.FullName, localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category, x => x.Description, localized.Description, localized.LanguageId);
                //search engine name
                var seName = category.ValidateSeName(localized.SeName, localized.Name, false, localized.LanguageId);
                _urlRecordService.SaveSlug(category, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected void UpdatePictureSeoNames(ProductCategory category)
        {
            var picture = _pictureService.GetPictureById(category.PictureId.GetValueOrDefault());
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(category.Name));
        }

        protected void UpdateProductCategory(ProductCategoryModel model, ProductCategory category)
        {
            if (!model.ParentCategoryId.HasValue || model.ParentCategoryId == 0)
            {
                category.Depth = 1;
            }
            else
            {
                category.Depth = _categoryService.GetProductCategoryById(model.ParentCategoryId.Value).Depth + 1;
            }
        }

        #endregion Utilities

        #region List
        // GET: Edit
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var categories = _categoryService.GetAllProductCategoriesByParentCategoryId(0, true);
            var model = categories.Select(x =>
              {
                  var categoryModel = x.ToModel();
                  return categoryModel;
              }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam, CategoryListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var categories = _categoryService.GetAllCategories(model.SearchCategoryName, dataTableParam.PageIndex, dataTableParam.PageSize, true, model.SearchAlias, true, false);
            var mappedCategories = categories.ToDictionary(x => x.Id);
            Dictionary<int, string> breadCrumbDir = new Dictionary<int, string>();

            var total = categories.TotalCount;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = categories.Select(x =>
                {
                    var categoryModel = x.ToModel();
                    categoryModel.Breadcrumb = x.GetCategoryBreadCrumb(_categoryService, mappedCategories);
                    return categoryModel;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public JsonResult GetAuthorizationCategory(int? channelId)
        {
            var businessCategory = _categoryService.GetAllCategories(showHidden: false);

            List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();

            foreach (ProductCategory categoryInfo in businessCategory)
            {
                string[] strArrays = categoryInfo.Path.Split(new char[] { '|' });
                int num = int.Parse(strArrays.Length > 0 ? strArrays[0] : "0");
                int num1 = int.Parse(strArrays.Length > 1 ? strArrays[1] : "0");
                var category = _categoryService.GetProductCategoryById(num);
                var categoryJsonModel = new CategoryJsonModel()
                {
                    Name = category.Name,
                    Id = category.Id.ToString(),
                    SubCategory = new List<SecondLevelCategory>()
                };
                SecondLevelCategory secondLevelCategory = null;
                ThirdLevelCategoty thirdLevelCategoty = null;
                ProductCategory category1 = _categoryService.GetProductCategoryById(num1);
                if (category1 != null)
                {
                    secondLevelCategory = new SecondLevelCategory()
                    {
                        Name = category1.Name,
                        Id = category1.Id.ToString(),
                        SubCategory = new List<ThirdLevelCategoty>()
                    };

                    if (strArrays.Length >= 3)
                    {
                        thirdLevelCategoty = new ThirdLevelCategoty()
                        {
                            Id = categoryInfo.Id.ToString(),
                            Name = categoryInfo.Name
                        };

                    }
                    if (thirdLevelCategoty != null)
                    {
                        secondLevelCategory.SubCategory.Add(thirdLevelCategoty);
                    }
                }
                CategoryJsonModel categoryJsonModel2 = categoryJsonModels.FirstOrDefault((CategoryJsonModel j) => j.Id == category.Id.ToString());
                if (categoryJsonModel2 != null && category1 != null && categoryJsonModel2.SubCategory.Any((SecondLevelCategory j) => j.Id == category1.Id.ToString()))
                {
                    categoryJsonModel2.SubCategory.FirstOrDefault((SecondLevelCategory j) => j.Id == category1.Id.ToString()).SubCategory.Add(thirdLevelCategoty);
                }
                else if (categoryJsonModel2 != null && secondLevelCategory != null)
                {
                    categoryJsonModel2.SubCategory.Add(secondLevelCategory);
                }
                else if (secondLevelCategory != null)
                {
                    categoryJsonModel.SubCategory.Add(secondLevelCategory);
                }
                if (categoryJsonModels.FirstOrDefault((CategoryJsonModel j) => j.Id == category.Id.ToString()) != null)
                {
                    continue;
                }
                categoryJsonModels.Add(categoryJsonModel);
            }
            return Json(new { json = categoryJsonModels }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create / Edit / Delete

        public ActionResult Create(int? parentId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var model = new ProductCategoryModel();

            //locales
            AddLocales(_languageService, model.Locales);
            model.PageSize = 12;
            model.Published = true;
            model.DisplayOrder = 1;
            model.AllowUsersToSelectPageSize = true;
            if (parentId.HasValue)
            {
                model.ParentCategoryId = parentId;
                var parentCategory = _categoryService.GetProductCategoryById(model.ParentCategoryId.Value);
                model.ParentCategoryBreadcrumb = parentCategory.GetCategoryBreadCrumb(_categoryService);
            }
            ProcessingDisplayOrder(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing", "save-create", "continueCreate")]
        [ValidateInput(false)]
        public ActionResult Create(ProductCategoryModel model, bool continueEditing, bool continueCreate, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                var category = model.ToEntity();

                category.AddEntitySysParam();
                UpdateProductCategory(model, category);
                _categoryService.InsertProductCategory(category);
                //search engine name
                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);
                //locales
                UpdateLocales(category, model);

                //update picture seo file name
                UpdatePictureSeoNames(category);

                UpdateCategoryPath(model, category);
                _categoryService.UpdateProductCategory(category);


                _eventPublisher.Publish(new ModelBoundEvent(model, category, form));

                //activity log
                _userActivityService.InsertActivity("AddNewProductCategory", _localizationService.GetResource("ActivityLog.AddNewProductCategory"), category.Name);


                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.ProductCategorys.Added"));
                if (continueCreate)
                {
                    return RedirectToAction("Create");
                }

                return continueEditing ? RedirectToAction("Edit", new { id = category.Id }) : RedirectToAction("List");

            }

            //parent categories
            if (model.ParentCategoryId.HasValue)
            {
                var parentCategory = _categoryService.GetProductCategoryById(model.ParentCategoryId.Value);
                if (parentCategory != null)
                    model.ParentCategoryBreadcrumb = parentCategory.GetCategoryBreadCrumb(_categoryService);
                else
                    model.ParentCategoryId = 0;
            }

            PrepareCategoryModel(model, null);
            ProcessingDisplayOrder(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var category = _categoryService.GetProductCategoryById(id);
            if (category == null)
                return RedirectToAction("List");

            var model = category.ToModel();
            if (model.ParentCategoryId.HasValue)
            {
                // codehint: sm-edit
                var parentCategory = _categoryService.GetProductCategoryById(model.ParentCategoryId.Value);
                if (parentCategory != null)
                    model.ParentCategoryBreadcrumb = parentCategory.GetCategoryBreadCrumb(_categoryService);
                else
                    model.ParentCategoryId = 0;
            }
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = category.GetLocalized(x => x.Name, languageId, false, false);
                locale.FullName = category.GetLocalized(x => x.FullName, languageId, false, false);
                locale.Description = category.GetLocalized(x => x.Description, languageId, false, false);
                locale.SeName = category.GetSeName(languageId, false, false);
            });

            PrepareCategoryModel(model, category);
            ProcessingDisplayOrder(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing", "save-create", "continueCreate")]
        [ValidateInput(false)]
        public ActionResult Edit(ProductCategoryModel model, bool continueEditing, bool continueCreate, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var category = _categoryService.GetProductCategoryById(model.Id);
            if (category == null)
                //No category found with the specified id
                return RedirectToAction("List");
            if (ModelState.IsValid)
            {
                int prevPictureId = category.PictureId.GetValueOrDefault();
                category = model.ToEntity(category);
                UpdateProductCategory(model, category);
                category.AddEntitySysParam(false, true);
                //locales
                UpdateLocales(category, model);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);

                //update picture seo file name
                UpdatePictureSeoNames(category);

                UpdateCategoryPath(model, category);
                _categoryService.UpdateProductCategory(category);

                _eventPublisher.Publish(new ModelBoundEvent(model, category, form));

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.ProductCategorys.Updated"));
                if (continueCreate)
                {
                    return RedirectToAction("Create");
                }
                return continueEditing ? RedirectToAction("Edit", new { id = category.Id }) : RedirectToAction("List");
            }


            //parent categories
            if (model.ParentCategoryId.HasValue)
            {
                var parentCategory = _categoryService.GetProductCategoryById(model.ParentCategoryId.Value);
                if (parentCategory != null)
                    model.ParentCategoryBreadcrumb = parentCategory.GetCategoryBreadCrumb(_categoryService);
                else
                    model.ParentCategoryId = 0;
            }

            PrepareCategoryModel(model, category);
            ProcessingDisplayOrder(model);

            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
            {
                if (IsAjaxRequest())
                    return Json(new { Result = false });
                return AccessDeniedView();
            }
            var category = _categoryService.GetProductCategoryById(id);
            if (category == null)
                //No category found with the specified id
                return RedirectToAction("List");

            _categoryService.DeleteProductCategory(category);
            if (IsAjaxRequest())
                return Json(new { Result = true });
            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Categorys.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var categoryItems = _categoryService.GetCategorysByIds(selectedIds.ToArray());
                foreach (var categoryItem in categoryItems)
                    _categoryService.DeleteProductCategory(categoryItem);
            }

            return Json(new { Result = true });
        }


        public ActionResult UpdateName(string name, int id)
        {
            var category = _categoryService.GetProductCategoryById(id);
            category.Name = name;
            _categoryService.UpdateProductCategory(category);
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateOrder(int order, int id)
        {
 
            var category = _categoryService.GetProductCategoryById(id);
            category.DisplayOrder = order;
            _categoryService.UpdateProductCategory(category);
            return Json(new { Result =true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}