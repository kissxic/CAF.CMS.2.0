using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Seo;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Application.Services.Media;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.Mvc.JQuery.Datatables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Channels;
using CAF.Infrastructure.Core.Domain;
using Newtonsoft.Json;
using CAF.Infrastructure.Core.Domain.Cms.Channels;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class ArticleCategoryController : AdminControllerBase
    {
        #region Fields
        private readonly IArticleCategoryService _categoryService;
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
        private readonly ISpecificationAttributeService _specificationAttributeService;
        #endregion

        #region Ctor

        public ArticleCategoryController(
            IModelTemplateService modelTemplateService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IUserService userService,
            UserSettings userSettings,
            IArticleCategoryService categoryService,
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
            SiteInformationSettings siteSettings,
            ISpecificationAttributeService specificationAttributeService)
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
            this._specificationAttributeService = specificationAttributeService;
        }
        #endregion

        #region Utilities

        [NonAction]
        protected void UpdateLocales(ArticleCategory category, ArticleCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category, x => x.Name, localized.Name, localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category, x => x.FullName, localized.FullName, localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category, x => x.Description, localized.Description, localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category, x => x.BottomDescription, localized.BottomDescription, localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category, x => x.MetaKeywords, localized.MetaKeywords, localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category, x => x.MetaDescription, localized.MetaDescription, localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category, x => x.MetaTitle, localized.MetaTitle, localized.LanguageId);

                //search engine name
                var seName = category.ValidateSeName(localized.SeName, localized.Name, false, localized.LanguageId);
                _urlRecordService.SaveSlug(category, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected void PrepareTemplatesModel(ArticleCategoryModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var channel = _channelService.GetChannelById(model.ChannelId);
            var templates = _modelTemplateService.GetAllModelTemplates();
            var listTemplate = templates.Where(p => p.TemplageTypeId == (int)TemplateTypeFormat.List).ToList();
            foreach (var template in listTemplate)
            {
                model.AvailableModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString(),

                });
            }
            var detailTemplate = templates.Where(p => p.TemplageTypeId == (int)TemplateTypeFormat.Detail).ToList();
            foreach (var template in detailTemplate)
            {
                model.AvailableDetailModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString(),

                });
            }
            if (channel != null && channel.ModelTemplateId == 0)
            {
                model.ModelTemplateId = channel.ModelTemplateId;
                model.DetailModelTemplateId = channel.DetailModelTemplateId;
            }
        }

        [NonAction]
        protected void PrepareChannelsModel(ArticleCategoryModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var channels = _channelService.GetAllChannels();
            foreach (var channel in channels)
            {
                model.AvailableChannels.Add(new SelectListItem()
                {
                    Text = channel.Title,
                    Value = channel.Id.ToString()
                });
            }
        }

        private void PrepareCategoryModel(ArticleCategoryModel model, ArticleCategory category)
        {

            if (model == null)
                throw new ArgumentNullException("model");

            if (category != null)
            {
                model.CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(category.CreatedOnUtc, DateTimeKind.Utc);
                model.ModifiedOnUtc = (category.ModifiedOnUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(category.ModifiedOnUtc.Value, DateTimeKind.Utc) : category.ModifiedOnUtc);
            }

            model.AvailableDefaultViewModes.Add(
                new SelectListItem { Value = "grid", Text = _localizationService.GetResource("Common.Grid"), Selected = model.DefaultViewMode.IsCaseInsensitiveEqual("grid") }
            );
            model.AvailableDefaultViewModes.Add(
                new SelectListItem { Value = "list", Text = _localizationService.GetResource("Common.List"), Selected = model.DefaultViewMode.IsCaseInsensitiveEqual("list") }
            );

        }

        [NonAction]
        private void PrepareAclModel(ArticleCategoryModel model, ArticleCategory category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (category != null)
                {
                    model.SelectedUserRoleIds = _aclService.GetUserRoleIdsWithAccess(category);
                }
                else
                {
                    model.SelectedUserRoleIds = new int[0];
                }
            }
        }

        [NonAction]
        private void PrepareSitesMappingModel(ArticleCategoryModel model, ArticleCategory category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableSites = _siteService
                .GetAllSites()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (category != null)
                {
                    model.SelectedSiteIds = _siteMappingService.GetSitesIdsWithAccess(category);
                }
                else
                {
                    model.SelectedSiteIds = new int[0];
                }
            }
            model.SiteContentShare = _siteSettings.SiteContentShare;
        }
        /// <summary>
        /// 装配文档属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        private void PrerpareArticleSpecifications(ArticleCategoryModel model, ArticleCategory category)
        {
            
            #region specification attributes
            //specification attributes
            var specificationAttributes = _specificationAttributeService.GetSpecificationAttributes().ToList();
            for (int i = 0; i < specificationAttributes.Count; i++)
            {
                var sa = specificationAttributes[i];
                model.AddSpecificationAttributeModel.AvailableAttributes.Add(new SelectListItem { Text = sa.Name, Value = sa.Id.ToString() });
                if (i == 0)
                {
                    //attribute options
                    foreach (var sao in _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(sa.Id))
                    {
                        model.AddSpecificationAttributeModel.AvailableOptions.Add(new SelectListItem { Text = sao.Name, Value = sao.Id.ToString() });
                    }
                }
            }
            //default specs values
            model.AddSpecificationAttributeModel.ShowOnProductPage = true;
            #endregion

            if (category != null)
            {
                var productrSpecs = _specificationAttributeService.GetCategorySpecificationAttributesById(category.Id);

                var productrSpecsModel = productrSpecs
                    .Select(x =>
                    {
                        var psaModel = new ArticleCategorySpecificationAttributeModel
                        {
                            Id = x.Id,
                            SpecificationAttributeName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                            SpecificationAttributeOptionName = x.SpecificationAttributeOption.Name,
                            SpecificationAttributeOptionAttributeId = x.SpecificationAttributeOption.SpecificationAttributeId,
                            SpecificationAttributeOptionId = x.SpecificationAttributeOptionId,
                            AllowFiltering = x.AllowFiltering,
                            ShowOnArticlePage = x.ShowOnArticlePage,
                            DisplayOrder = x.DisplayOrder
                        };
                        return psaModel;
                    })
                    .ToList();

                foreach (var attr in productrSpecsModel)
                {
                    var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(attr.SpecificationAttributeOptionAttributeId);

                    foreach (var option in options)
                    {
                        attr.SpecificationAttributeOptions.Add(new ArticleCategorySpecificationAttributeModel.SpecificationAttributeOption
                        {
                            id = option.Id,
                            name = option.Name,
                            text = option.Name,
                            select = attr.SpecificationAttributeOptionId == option.Id
                        });
                    }

                    attr.SpecificationAttributeOptionsJsonString = JsonConvert.SerializeObject(attr.SpecificationAttributeOptions);
                }
                model.ArticleCategorySpecificationAttributeModel = productrSpecsModel;
            }
        }



        #region 更新

    
        [NonAction]
        protected void UpdatePictureSeoNames(ArticleCategory category)
        {
            var picture = _pictureService.GetPictureById(category.PictureId.GetValueOrDefault());
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(category.Name));
        }
        [NonAction]
        protected void UpdateCategoryAcl(ArticleCategory category, ArticleCategoryModel model)
        {
            var existingAclRecords = _aclService.GetAclRecords(category);
            var allUserRoles = _userService.GetAllUserRoles(true);
            foreach (var userRole in allUserRoles)
            {
                if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                {
                    //new role
                    if (existingAclRecords.Where(acl => acl.UserRoleId == userRole.Id).Count() == 0)
                        _aclService.InsertAclRecord(category, userRole.Id);
                }
                else
                {
                    //removed role
                    var aclRecordToDelete = existingAclRecords.Where(acl => acl.UserRoleId == userRole.Id).FirstOrDefault();
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }
        [NonAction]
        protected void UpdateSiteMappings(ArticleCategory category, ArticleCategoryModel model)
        {
            //网站限制
            if (this._siteSettings.SiteContentShare)
            {
                category.LimitedToSites = model.LimitedToSites;
            }
            else
            {
                category.LimitedToSites = true;
                var siteIds = new List<int>();
                siteIds.Add(this._siteContext.CurrentSite.Id);
                model.SelectedSiteIds = siteIds.ToArray();
            }

            _siteMappingService.SaveSiteMappings<ArticleCategory>(category, model.SelectedSiteIds);
        }
        /// <summary>
        /// 更新文档属性数据
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateArticleSpecifications(ArticleCategoryModel model, ArticleCategory category)
        {
            if (model.SpaValues.IsEmpty())
                return;
            var channelSpecificationAttributes = JsonConvert.DeserializeObject<List<ArticleCategorySpecificationAttribute>>(model.SpaValues);
            var list = new List<ArticleCategorySpecificationAttribute>();
            foreach (var item in channelSpecificationAttributes)
            {
                var psa = new ArticleCategorySpecificationAttribute()
                {
                    SpecificationAttributeOptionId = item.SpecificationAttributeOptionId,
                    CategoryId = category.Id,
                    AllowFiltering = item.AllowFiltering,
                    ShowOnArticlePage = item.ShowOnArticlePage,
                    DisplayOrder = item.DisplayOrder,
                };
                list.Add(psa);
            }
            _specificationAttributeService.InsertCategorySpecificationAttribute(category.Id, list);
        }
        #endregion
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
            var model = new ArticleCategoryModel();

            #region categories
            //categories
            var allChannels = _channelService.GetAllChannels();
            foreach (var c in allChannels)
            {
                model.AvailableChannels.Add(new SelectListItem { Text = c.Title, Value = c.Id.ToString() });
            }

            #endregion


            return View(model);
        }
        public ActionResult Center(int? channelId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var model = new ArticleCategoryModel();

            #region templates

            //var templates = _modelTemplateService.GetAllModelTemplates();
            //foreach (var template in templates)
            //{
            //    model.AvailableModelTemplates.Add(new SelectListItem()
            //    {
            //        Text = template.Name,
            //        Value = template.Id.ToString()
            //    });
            //}

            #endregion

            #region Channels
            if (channelId.HasValue)
            {
                var channel = _channelService.GetChannelById(channelId.Value);
                model.ChannelId = channel.Id;
                model.ModelTemplateId = channel.ModelTemplateId;
                model.DetailModelTemplateId = channel.DetailModelTemplateId;
            }

            #endregion


            return View(model);
        }
        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam, CategoryListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var categories = _categoryService.GetAllCategories(model.SearchCategoryName, 0, int.MaxValue, true, model.SearchAlias, model.SearchChannelId, true, false);
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
                    categoryModel.ChannelName = x.Channel.Title;
                    return categoryModel;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }


        #endregion

        #region Create / Edit / Delete

        public ActionResult Create(int? channelId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var model = new ArticleCategoryModel();
            if (channelId.HasValue)
                model.ChannelId = channelId.Value;
            //locales
            AddLocales(_languageService, model.Locales);
            //templates
            PrepareTemplatesModel(model);
            //channel
            PrepareChannelsModel(model);
            PrepareCategoryModel(model, null);
            //ACL
            PrepareAclModel(model, null, false);
            //Sites
            PrepareSitesMappingModel(model, null, false);

            PrerpareArticleSpecifications(model, null);

            //default values

            model.PageSize = 12;
            model.Published = true;
            model.DisplayOrder = 1;
            model.AllowUsersToSelectPageSize = true;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing", "save-create", "continueCreate")]
        [ValidateInput(false)]
        public ActionResult Create(ArticleCategoryModel model, bool continueEditing, bool continueCreate, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                var category = model.ToEntity();
                if (category.ParentCategoryId != 0 && category.ChannelId == 0)
                    category.ChannelId = _categoryService.GetArticleCategoryById(category.ParentCategoryId).ChannelId;
                category.AddEntitySysParam();
                _categoryService.InsertArticleCategory(category);
                //search engine name
                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);

                //locales
                UpdateLocales(category, model);

                //update picture seo file name
                UpdatePictureSeoNames(category);

                //ACL (customer roles)
                UpdateCategoryAcl(category, model);

                //SiteMap
                UpdateSiteMappings(category, model);

                UpdateArticleSpecifications(model, category);

                _categoryService.UpdateArticleCategory(category);


                _eventPublisher.Publish(new ModelBoundEvent(model, category, form));

                //activity log
                _userActivityService.InsertActivity("AddNewArticleCategory", _localizationService.GetResource("ActivityLog.AddNewArticleCategory"), category.Name);


                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.ArticleCategorys.Added"));
                if (continueCreate)
                {
                    return RedirectToAction("Create", new { channelId = model.ChannelId });
                }

                return continueEditing ? RedirectToAction("Edit", new { id = category.Id }) : RedirectToAction("Center", new { channelId = model.ChannelId });

            }
            //templates
            PrepareTemplatesModel(model);
            //channel
            PrepareChannelsModel(model);
            //parent categories
            if (model.ParentCategoryId.HasValue)
            {
                var parentCategory = _categoryService.GetArticleCategoryById(model.ParentCategoryId.Value);
                if (parentCategory != null && !parentCategory.Deleted)
                    model.ParentCategoryBreadcrumb = parentCategory.GetCategoryBreadCrumb(_categoryService);
                else
                    model.ParentCategoryId = 0;
            }

            PrepareCategoryModel(model, null);
            //ACL
            PrepareAclModel(model, null, true);
            //Sites
            PrepareSitesMappingModel(model, null, true);
            PrerpareArticleSpecifications(model, null);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var category = _categoryService.GetArticleCategoryById(id);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            var model = category.ToModel();
            if (model.ParentCategoryId.HasValue)
            {
                // codehint: sm-edit
                var parentCategory = _categoryService.GetArticleCategoryById(model.ParentCategoryId.Value);
                if (parentCategory != null && !parentCategory.Deleted)
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
                locale.BottomDescription = category.GetLocalized(x => x.BottomDescription, languageId, false, false);
                locale.MetaKeywords = category.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = category.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = category.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = category.GetSeName(languageId, false, false);
            });
            //templates
            PrepareTemplatesModel(model);
            //channel
            PrepareChannelsModel(model);
            //ACL
            PrepareAclModel(model, category, false);
            //Sites
            PrepareSitesMappingModel(model, category, false);
            PrepareCategoryModel(model, category);
            PrerpareArticleSpecifications(model, category);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing", "save-create", "continueCreate")]
        [ValidateInput(false)]
        public ActionResult Edit(ArticleCategoryModel model, bool continueEditing, bool continueCreate, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var category = _categoryService.GetArticleCategoryById(model.Id);
            if (category == null)
                //No category found with the specified id
                return RedirectToAction("List");
            if (ModelState.IsValid)
            {
                int prevPictureId = category.PictureId.GetValueOrDefault();
                category = model.ToEntity(category);
                category.AddEntitySysParam(false, true);
                if (category.ParentCategoryId != 0 && category.ChannelId == 0)
                    category.ChannelId = _categoryService.GetArticleCategoryById(category.ParentCategoryId).ChannelId;

                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);
                //locales
                UpdateLocales(category, model);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //update picture seo file name
                UpdatePictureSeoNames(category);

                //ACL
                UpdateCategoryAcl(category, model);

                //SiteMap
                UpdateSiteMappings(category, model);

                UpdateArticleSpecifications(model, category);

                _categoryService.UpdateArticleCategory(category);

                _eventPublisher.Publish(new ModelBoundEvent(model, category, form));

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.ArticleCategorys.Updated"));
                if (continueCreate)
                {
                    return RedirectToAction("Create", new { channelId = model.ChannelId });
                }
                return continueEditing ? RedirectToAction("Edit", new { id = category.Id }) : RedirectToAction("Center", new { channelId = model.ChannelId });
            }


            //parent categories
            if (model.ParentCategoryId.HasValue)
            {
                var parentCategory = _categoryService.GetArticleCategoryById(model.ParentCategoryId.Value);
                if (parentCategory != null && !parentCategory.Deleted)
                    model.ParentCategoryBreadcrumb = parentCategory.GetCategoryBreadCrumb(_categoryService);
                else
                    model.ParentCategoryId = 0;
            }
            //templates
            PrepareTemplatesModel(model);
            //channel
            PrepareChannelsModel(model);
            PrepareCategoryModel(model, category);
            //ACL
            PrepareAclModel(model, category, true);
            //Sites
            PrepareSitesMappingModel(model, category, true);
            PrerpareArticleSpecifications(model, category);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BulkEditSave(ArticleCategoryModel.BatchCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (model.OpenTemplateCheckBox && model.TemplateId.HasValue)
            {
                foreach (var id in model.SelectedIds)
                {
                    //update
                    var articleCategory = _categoryService.GetArticleCategoryById(id);
                    if (articleCategory != null)
                    {
                        articleCategory.ModelTemplateId = model.TemplateId.Value;
                        _categoryService.UpdateArticleCategory(articleCategory);
                    }
                }
            }

            return Json(new { state = 1 }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var category = _categoryService.GetArticleCategoryById(id);
            if (category == null)
                //No category found with the specified id
                return RedirectToAction("Center");

            _categoryService.DeleteArticleCategory(category);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Categorys.Deleted"));
            return RedirectToAction("Center", new { channelId = category.ChannelId });
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
                    _categoryService.DeleteArticleCategory(categoryItem);
            }

            return Json(new { Result = true });
        }
        #endregion

        public ActionResult ParentArticleCategory(int id)
        {
            var category = _categoryService.GetArticleCategoryById(id);
            var data = new
            {
                ModelTemplateId = category.ModelTemplateId,
                DetailModelTemplateId = category.DetailModelTemplateId
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}