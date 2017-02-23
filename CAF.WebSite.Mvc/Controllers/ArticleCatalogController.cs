﻿using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Security;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Models.ArticleCatalog;
using CAF.WebSite.Mvc.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAF.WebSite.Mvc.Models.Articles;
using CAF.WebSite.Application.Services.Clients;
using CAF.Infrastructure.Core.Domain.Cms.Clients;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Filter;
using CAF.WebSite.Application.Services.Manufacturers;
using System.Drawing;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Application.WebUI.Controllers;

namespace CAF.WebSite.Mvc.Controllers
{
    [WhitespaceFilterAttribute]
    public class ArticleCatalogController : PublicControllerBase
    {
        #region Fields
        private readonly IManufacturerService _manufacturerService;
        private readonly ICurrencyService _currencyService;
        private readonly ICommonServices _services;
        private readonly IClientService _clientService;
        private readonly IArticleCategoryService _articlecategoryService;
        private readonly IChannelService _channelService;
        private readonly IProductCategoryService _productcategoryService;
        private readonly IModelTemplateService _modelTemplateService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articlesTagService;
        private readonly IPictureService _pictureService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAclService _aclService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly MediaSettings _mediaSettings;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly IRecentlyViewedArticlesService _recentlyViewedArticlesService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IFilterService _filterService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ArticleCatalogHelper _helper;
        #endregion

        #region Constructors

        public ArticleCatalogController(
            IManufacturerService manufacturerService,
            ICurrencyService currencyService,
            ICommonServices services,
            IClientService clientService,
            IArticleCategoryService articlecategoryService,
            IChannelService channelService,
            IProductCategoryService productcategoryService,
            IModelTemplateService modelTemplateService,
            IArticleService articleService,
            IArticleTagService articlesTagService,
            IStateProvinceService stateProvinceService,
            IPictureService pictureService,
            IGenericAttributeService genericAttributeService,
            IAclService aclService,
            ISiteMappingService siteMappingService,
            MediaSettings mediaSettings,
            ArticleCatalogSettings catalogSettings,
            IRecentlyViewedArticlesService recentlyViewedArticlesService,
            ISpecificationAttributeService specificationAttributeService,
            IFilterService filterService,
             IPriceFormatter priceFormatter,
            ArticleCatalogHelper helper
            )
        {
            this._manufacturerService = manufacturerService;
            this._currencyService = currencyService;
            this._services = services;
            this._clientService = clientService;
            this._articlecategoryService = articlecategoryService;
            this._channelService = channelService;
            this._productcategoryService = productcategoryService;
            this._articleService = articleService;
            this._articlesTagService = articlesTagService;
            this._stateProvinceService = stateProvinceService;
            this._modelTemplateService = modelTemplateService;
            this._pictureService = pictureService;
            this._genericAttributeService = genericAttributeService;
            this._aclService = aclService;
            this._siteMappingService = siteMappingService;
            this._recentlyViewedArticlesService = recentlyViewedArticlesService;
            this._specificationAttributeService = specificationAttributeService;
            this._filterService = filterService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._priceFormatter = priceFormatter;
            this._helper = helper;

            T = NullLocalizer.Instance;
        }

        #endregion

        #region Properties

        public Localizer T
        {
            get;
            set;
        }

        #endregion

        #region Utilities
        [NonAction]
        private ArticleCategoryModel CategoryFilter(int? categoryId, int? channelId, int? articleThumbPictureSize, ArticleCatalogPagingFilteringModel command, string partialView = null)
        {
            //var cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_MODEL_KEY, _services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id, categoryId ?? 0, channelId ?? 0);
            //if (partialView.IsEmpty())
            //    cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PARTIALVIEW_MODEL_PARTVIEW_KEY, _services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id, categoryId ?? 0, channelId ?? 0, partialView);
            //var cachedModel = _services.Cache.Get(cacheKey, () =>
            //{
            var category = new ArticleCategory() { PageSize = 24 };
            if (categoryId.HasValue)
            {
                category = _articlecategoryService.GetArticleCategoryById(categoryId.Value);
                if (category == null || category.Deleted)
                    return null;

                //Check whether the current user has a "Manage catalog" permission
                //It allows him to preview a category before publishing
                if (!category.Published)
                    return null;

                ////ACL (access control list)
                if (!_aclService.Authorize(category))
                    return null;

                //Site mapping
                if (!_siteMappingService.Authorize(category))
                    return null;
            }

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_services.WorkContext.CurrentUser,
                SystemUserAttributeNames.LastContinueShoppingPage,
                _services.WebHelper.GetThisPageUrl(false),
                _services.SiteContext.CurrentSite.Id);

            if (command.PageNumber <= 0)
                command.PageNumber = 1;



            var model = category.ToModel();
            _helper.PreparePagingFilteringModel(model.PagingFilteringContext, command, new PageSizeContext
            {
                AllowUsersToSelectPageSize = category.AllowUsersToSelectPageSize,
                PageSize = category.PageSize,
                PageSizeOptions = category.PageSizeOptions
            });

            //category breadcrumb
            model.DisplayCategoryBreadcrumb = _catalogSettings.CategoryBreadcrumbEnabled;
            if (model.DisplayCategoryBreadcrumb)
            {
                model.CategoryBreadcrumb = _helper.GetCategoryBreadCrumb(category.Id, 0);
            }
            // use old filter
            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_services.WebHelper);

            // Articles

            var ctx2 = new ArticleSearchContext();
            if (category.Id > 0)
            {
                ctx2.CategoryIds.Add(category.Id);
                if (_catalogSettings.ShowArticlesFromSubcategories)
                {
                    // include subcategories
                    ctx2.CategoryIds.AddRange(_helper.GetChildCategoryIds(category.Id));
                }
                ctx2.Origin = categoryId.ToString();
            }
            if (channelId.HasValue && channelId > 0)
            {
                ctx2.ChannelIds.Add(channelId.Value);

            }
            ctx2.LanguageId = _services.WorkContext.WorkingLanguage.Id;

            ctx2.OrderBy = (ArticleSortingEnum)command.OrderBy; // ArticleSortingEnum.Position;
            ctx2.PageIndex = command.PageNumber - 1;
            ctx2.PageSize = command.PageSize;
            ctx2.SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode;
            ctx2.IsTop = command.IsTop;
            ctx2.IsHot = command.IsHot;
            ctx2.IsRed = command.IsRed;
            ctx2.IsSlide = command.IsSlide;
            ctx2.AreaId = command.AreaId;
            ctx2.Keywords = command.Q.UrlDecode();
            ctx2.LoadFilterableSpecificationAttributeOptionIds = true;
            ctx2.FilteredSpecs = alreadyFilteredSpecOptionIds;
            var articles = _articleService.SearchArticles(ctx2);

            model.PagingFilteringContext.Q = command.Q.UrlDecode() ;

            model.PagingFilteringContext.LoadPagedList(articles);

            model.Articles = _helper.PrepareArticlePostModels(
                articles,
                preparePictureModel: true, articleThumbPictureSize: new Size(articleThumbPictureSize ?? 0, articleThumbPictureSize ?? 0)).ToList();

            //specs
            model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
                ctx2.FilterableSpecificationAttributeOptionIds,
                _specificationAttributeService, _services.WebHelper, _services.WorkContext);

            //areas
            model.PagingFilteringContext.BaseFilter.LoadBaseFilters(command.AreaId ?? 0, _stateProvinceService, ctx2.FilterableSpecificationAttributeOptionIds.Count > 0, _services.WebHelper);

            var tempId = 0;
            if (channelId.HasValue)
                tempId = _channelService.GetChannelById(channelId.Value).ModelTemplateId;
            else
                tempId = category.ModelTemplateId;
            // template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.ARTICLECATEGORY_TEMPLATE_MODEL_KEY, tempId);
            var templateViewPath = _services.Cache.Get(templateCacheKey, () =>
            {
                var template = _modelTemplateService.GetModelTemplateById(tempId);
                if (template == null)
                    template = _modelTemplateService.GetAllModelTemplates().FirstOrDefault();
                return template.ViewPath;
            });
            model.TemplateName = templateViewPath;
            return model;
            //});

            //return cachedModel;
        }
        #endregion

        #region Categories
        /// <summary>
        /// 下拉列表获取栏目分类
        /// </summary>
        /// <param name="label"></param>
        /// <param name="selectedId"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public ActionResult AllCategories(string label, int selectedId, int? channelId)
        {

            var categories = _articlecategoryService.GetAllCategories(showHidden: false, channelId: channelId);
            var mappedCategories = categories.ToDictionary(x => x.Id);
            var cquery = categories.AsQueryable();

            if (label.HasValue())
            {
                categories.Insert(0, new ArticleCategory { Name = label, Id = 0 });
            }

            var query =
                from c in cquery
                select new
                {
                    id = c.Id.ToString(),
                    text = c.GetCategoryBreadCrumb(_articlecategoryService, mappedCategories),
                    selected = c.Id == selectedId
                };

            var data = query.ToList();

            var mru = new MostRecentlyUsedList<string>(_services.WorkContext.CurrentUser.GetAttribute<string>(SystemUserAttributeNames.MostRecentlyUsedCategories),
                _catalogSettings.MostRecentlyUsedCategoriesMaxSize);

            for (int i = mru.Count - 1; i >= 0; --i)
            {
                string id = mru[i];
                var item = categories.FirstOrDefault(x => x.Id.ToString() == id);
                if (item != null)
                {
                    data.Insert(0, new
                    {
                        id = id,
                        text = item.GetCategoryBreadCrumb(_articlecategoryService, mappedCategories),
                        selected = false
                    });
                }
            }

            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        /// <summary>
        /// 根据频道ID获取信息
        /// </summary>
        /// <param name="channelId">频道ID</param>
        /// <param name="partialView">部分试图名称</param>
        /// <param name="command">条件</param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult AllCategory(int? channelId, ArticleCatalogPagingFilteringModel command)
        {
            var model = new ArticleCategoryModel();
            model = CategoryFilter(null, channelId, null, command);
            return View(model.TemplateName, model);
        }
        /// <summary>
        /// 根据分类ID及条件获取文章数量
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="command"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult Category(int categoryId, ArticleCatalogPagingFilteringModel command, string filter)
        {
            var category = _articlecategoryService.GetArticleCategoryById(categoryId);
            if (category == null || category.Deleted)
                return HttpNotFound();

            //判断内容访问权限，实际项目中需求及去掉注释启用
            //It allows him to preview a category before publishing
            //if (!category.Published )
            //    return HttpNotFound();

            ////ACL (access control list)
            //if (!_aclService.Authorize(category))
            //    return HttpNotFound();

            //site mapping
            if (!_siteMappingService.Authorize(category))
                return HttpNotFound();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_services.WorkContext.CurrentUser,
                SystemUserAttributeNames.LastContinueShoppingPage,
                _services.WebHelper.GetThisPageUrl(false),
                _services.SiteContext.CurrentSite.Id);

            if (command.PageNumber <= 0)
                command.PageNumber = 1;



            var model = category.ToModel();

            _helper.PreparePagingFilteringModel(model.PagingFilteringContext, command, new PageSizeContext
            {
                AllowUsersToSelectPageSize = category.AllowUsersToSelectPageSize,
                PageSize = category.PageSize,
                PageSizeOptions = category.PageSizeOptions
            });
            //价格范围帅选条件
            model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(category.PriceRanges, _services.WebHelper, _priceFormatter);
            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_services.WebHelper, category.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;

            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimarySiteCurrency(selectedPriceRange.From.Value, _services.WorkContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimarySiteCurrency(selectedPriceRange.To.Value, _services.WorkContext.WorkingCurrency);
            }

            //category breadcrumb
            model.DisplayCategoryBreadcrumb = _catalogSettings.CategoryBreadcrumbEnabled;
            if (model.DisplayCategoryBreadcrumb)
            {
                model.CategoryBreadcrumb = _helper.GetCategoryBreadCrumb(category.Id, 0);
            }

            model.DisplayFilter = _catalogSettings.FilterEnabled;
            model.ShowSubcategoriesAboveArticleLists = _catalogSettings.ShowSubcategoriesAboveArticleLists;

            var customerRolesIds = _services.WorkContext.CurrentUser.UserRoles.Where(x => x.Active).Select(x => x.Id).ToList();


            // Articles
            if (filter.HasValue())
            {
                var context = new FilterArticleContext
                {
                    ParentCategoryID = category.Id,
                    CategoryIds = new List<int> { category.Id },
                    Criteria = _filterService.Deserialize(filter),
                    OrderBy = command.OrderBy
                };

                if (_catalogSettings.ShowArticlesFromSubcategories)
                    context.CategoryIds.AddRange(_helper.GetChildCategoryIds(category.Id));

                var filterQuery = _filterService.ArticleFilter(context);
                var articles = new PagedList<Article>(filterQuery, command.PageIndex, command.PageSize);

                model.Articles = _helper.PrepareArticlePostModels(
                    articles,
                    prepareColorAttributes: true,
                    prepareManufacturers: command.ViewMode.IsCaseInsensitiveEqual("list")).ToList();
                model.PagingFilteringContext.LoadPagedList(articles);
            }
            else
            {
                // use old filter
                IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_services.WebHelper);

                var ctx2 = new ArticleSearchContext();

                if (category.Id > 0)
                {
                    ctx2.CategoryIds.Add(category.Id);
                    if (_catalogSettings.ShowArticlesFromSubcategories)
                    {
                        // include subcategories
                        ctx2.CategoryIds.AddRange(_helper.GetChildCategoryIds(category.Id));
                    }
                }
                ctx2.LanguageId = _services.WorkContext.WorkingLanguage.Id;
                ctx2.PriceMin = minPriceConverted;
                ctx2.PriceMax = maxPriceConverted;
                ctx2.FilteredSpecs = alreadyFilteredSpecOptionIds;

                ctx2.OrderBy = (ArticleSortingEnum)command.OrderBy; // ArticleSortingEnum.Position;
                ctx2.PageIndex = command.PageNumber - 1;
                ctx2.PageSize = command.PageSize;
                ctx2.SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode;
                ctx2.Origin = categoryId.ToString();
                ctx2.LoadFilterableSpecificationAttributeOptionIds = true;
                ctx2.VisibleIndividuallyOnly = true;

                var articles = _articleService.SearchArticles(ctx2);

                model.Articles = _helper.PrepareArticlePostModels(
                    articles,
                    prepareManufacturers: command.ViewMode.IsCaseInsensitiveEqual("list")).ToList();

                model.PagingFilteringContext.LoadPagedList(articles);

                //specs
                model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
                    ctx2.FilterableSpecificationAttributeOptionIds,
                    _specificationAttributeService, _services.WebHelper, _services.WorkContext);

                //areas
                model.PagingFilteringContext.BaseFilter.LoadBaseFilters(command.AreaId ?? 0, _stateProvinceService, ctx2.FilterableSpecificationAttributeOptionIds.Count > 0, _services.WebHelper);

            }



            // template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.ARTICLECATEGORY_TEMPLATE_MODEL_KEY, category.ModelTemplateId);
            var templateViewPath = _services.Cache.Get(templateCacheKey, () =>
            {
                var template = _modelTemplateService.GetModelTemplateById(category.ModelTemplateId);
                if (template == null)
                    template = _modelTemplateService.GetAllModelTemplates().FirstOrDefault();
                return template.ViewPath;
            });

            // activity log
            _services.UserActivity.InsertActivity("PublicSite.ViewCategory", T("ActivityLog.PublicSite.ViewCategory"), category.Name);

            if (IsAjaxRequest())
                return Json(model);
            else
                return View(templateViewPath, model);

        }

        /// <summary>
        /// 获取分类信息
        /// </summary>
        /// <param name="currentCategoryId">当前分类ID</param>
        /// <param name="currentArticleId">当前文章ID</param>
        /// <param name="channelId">频道ID</param>
        /// <param name="onlyShowChildren">只显示当前分类，不显示全分类导航</param>
        /// <param name="showParent">显示父级分类</param>
        /// <param name="templateView">部分试图</param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult CategoryNavigation(int currentCategoryId, int currentArticleId, int? channelId, bool onlyShowChildren = false, bool showParent = false, string templateView = "")
        {

            var model = new NavigationModel();
            if (channelId.HasValue)
                model = _helper.PrepareCategoryModels(channelId.Value, currentCategoryId, currentArticleId);
            else
                model = _helper.PrepareCategoryNavigationModel(currentCategoryId, currentArticleId, onlyShowChildren, showParent);

            if (templateView.IsEmpty())
                return PartialView(model);
            else
                return PartialView(templateView, model);
        }

        /// <summary>
        /// 分类菜单
        /// </summary>
        /// <param name="currentCategoryId">当前分类ID</param>
        /// <param name="currentArticleId">当前文章ID</param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Megamenu(int currentCategoryId, int currentArticleId)
        {
            var model = _helper.PrepareCategoryNavigationModel(currentCategoryId, currentArticleId);
            return PartialView(model);
        }

        /// <summary>
        /// 文章内容分类导航
        /// </summary>
        /// <param name="articleId">当前文章ID</param>
        /// <param name="OnlyCurrentCategory">只显示当前分类</param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ArticleBreadcrumb(int articleId, bool OnlyCurrentCategory = false, string templateView = "")
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            if (!_catalogSettings.CategoryBreadcrumbEnabled)
                return Content("");

            var model = new ArticlePostModel.ArticleBreadcrumbModel
            {
                ArticleId = article.Id,
                ArticleName = article.GetLocalized(x => x.Title),
                ArticleSeName = article.GetSeName()
            };

            var breadcrumb = _helper.GetCategoryBreadCrumb(article.CategoryId ?? 0, articleId, OnlyCurrentCategory);
            model.CategoryBreadcrumb = breadcrumb;
            model.OnlyCurrentCategory = OnlyCurrentCategory;

            if (templateView.IsEmpty())
                return PartialView(model);
            else
                return PartialView(templateView, model);
        }
        /// <summary>
        /// 获取首页分类
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult HomepageCategories()
        {
            var categories = _articlecategoryService.GetAllCategoriesDisplayedOnHomePage()
                .Where(c => _aclService.Authorize(c) && _siteMappingService.Authorize(c))
                .ToList();

            var listModel = categories
                .Select(x =>
                {
                    var catModel = x.ToModel();

                    //prepare picture model
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true,
                        _services.WorkContext.WorkingLanguage.Id, _services.WebHelper.IsCurrentConnectionSecured(), _services.SiteContext.CurrentSite.Id);
                    catModel.PictureModel = _services.Cache.Get(categoryPictureCacheKey, () =>
                    {
                        var pictureModel = new PictureModel()
                        {
                            PictureId = x.PictureId.GetValueOrDefault(),
                            FullSizeImageUrl = _pictureService.GetPictureUrl(x.PictureId.GetValueOrDefault()),
                            ImageUrl = _pictureService.GetPictureUrl(x.PictureId.GetValueOrDefault(), pictureSize),
                            Title = string.Format(T("Media.ArticleCategory.ImageLinkTitleFormat"), catModel.Name),
                            AlternateText = string.Format(T("Media.ArticleCategory.ImageAlternateTextFormat"), catModel.Name)
                        };
                        return pictureModel;
                    });

                    return catModel;
                })
                .ToList();

            if (listModel.Count == 0)
                return Content("");

            return PartialView(listModel);
        }

        #region Searching
        /// <summary>
        /// 站内搜索
        /// </summary>
        /// <param name="model"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        [ValidateInput(false)]
        public ActionResult Search(SearchModel model, SearchPagingFilteringModel command)
        {
            if (model == null)
                model = new SearchModel();

            // 'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_services.WorkContext.CurrentUser,
                SystemUserAttributeNames.LastContinueShoppingPage,
                _services.WebHelper.GetThisPageUrl(false),
                _services.SiteContext.CurrentSite.Id);

            if (command.PageSize <= 0)
                command.PageSize = _catalogSettings.SearchPageArticlesPerPage;
            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            _helper.PreparePagingFilteringModel(model.PagingFilteringContext, command, new PageSizeContext
            {
                AllowUsersToSelectPageSize = _catalogSettings.ArticleSearchAllowUsersToSelectPageSize,
                PageSize = _catalogSettings.SearchPageArticlesPerPage,
                PageSizeOptions = _catalogSettings.ArticleSearchPageSizeOptions
            });

            if (model.Q == null)
                model.Q = "";
            model.Q = model.Q.Trim();

            // Build AvailableCategories
            // first empty entry
            model.AvailableArticleCategories.Add(new SelectListItem
            {
                Value = "0",
                Text = T("Common.All")
            });

            var navModel = _helper.PrepareCategoryNavigationModel(0, 0);

            navModel.Root.TraverseTree((node) =>
            {
                if (node.IsRoot)
                    return;

                int id = node.Value.EntityId;
                var breadcrumb = node.GetBreadcrumb().Select(x => x.Text).ToArray();

                model.AvailableArticleCategories.Add(new SelectListItem
                {
                    Value = id.ToString(),
                    Text = String.Join(" > ", breadcrumb),
                    Selected = model.Cid == id
                });
            });

            var manufacturers = _manufacturerService.GetAllManufacturers();
            if (manufacturers.Count > 0)
            {
                model.AvailableManufacturers.Add(new SelectListItem
                {
                    Value = "0",
                    Text = T("Common.All")
                });
                foreach (var m in manufacturers)
                    model.AvailableManufacturers.Add(new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.GetLocalized(x => x.Name),
                        Selected = model.Mid == m.Id
                    });
            }



            IPagedList<Article> articles = new PagedList<Article>(new List<Article>(), 0, 1);
            // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
            if (Request.Params["Q"] != null)
            {
                if (model.Q.Length < _catalogSettings.ArticleSearchTermMinimumLength)
                {
                    model.Warning = string.Format(T("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ArticleSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    int manufacturerId = 0;
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    bool searchInDescriptions = false;
                    if (model.As)
                    {
                        // advanced search
                        var categoryId = model.Cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (model.Isc)
                            {
                                // include subcategories
                                categoryIds.AddRange(_helper.GetChildCategoryIds(categoryId));
                            }
                        }
                        manufacturerId = model.Mid;
                        // min price
                        if (!string.IsNullOrEmpty(model.Pf))
                        {
                            decimal minPrice = decimal.Zero;
                            if (decimal.TryParse(model.Pf, out minPrice))
                                minPriceConverted = _currencyService.ConvertToPrimarySiteCurrency(minPrice, _services.WorkContext.WorkingCurrency);
                        }
                        // max price
                        if (!string.IsNullOrEmpty(model.Pt))
                        {
                            decimal maxPrice = decimal.Zero;
                            if (decimal.TryParse(model.Pt, out maxPrice))
                                maxPriceConverted = _currencyService.ConvertToPrimarySiteCurrency(maxPrice, _services.WorkContext.WorkingCurrency);
                        }

                        searchInDescriptions = model.Sid;
                    }

                    //var searchInArticleTags = false;
                    var searchInArticleTags = searchInDescriptions;

                    //articles
                    #region Text Search


                    var ctx = new ArticleSearchContext();
                    ctx.CategoryIds = categoryIds;
                    ctx.ManufacturerId = manufacturerId;
                    ctx.PriceMin = minPriceConverted;
                    ctx.PriceMax = maxPriceConverted;
                    ctx.Keywords = model.Q;
                    ctx.SearchDescriptions = searchInDescriptions;
                    ctx.SearchArticleTags = searchInArticleTags;
                    ctx.LanguageId = _services.WorkContext.WorkingLanguage.Id;
                    ctx.OrderBy = (ArticleSortingEnum)command.OrderBy; // ArticleSortingEnum.Position;
                    ctx.PageIndex = command.PageNumber - 1;
                    ctx.PageSize = command.PageSize;
                    ctx.SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode;
                    ctx.VisibleIndividuallyOnly = true;

                    articles = _articleService.SearchArticles(ctx);
                    #endregion
                    #region Lucene

                    //var searchQuery = new SearchQuery(model.Q)
                    // {
                    //     NumberOfHitsToReturn = command.PageSize,
                    //     ReturnFromPosition = command.PageSize * (command.PageNumber - 1)
                    // };

                    //// Perform the searh
                    //var result = this._searchProvider.Value.Search(searchQuery);
                    #endregion

                    model.Articles = _helper.PrepareArticlePostModels(
                        articles,
                        prepareManufacturers: command.ViewMode.IsCaseInsensitiveEqual("list")).ToList();

                    model.NoResults = !model.Articles.Any();
                }
            }

            model.PagingFilteringContext.LoadPagedList(articles);
            return View(model);
        }
        /// <summary>
        /// 搜索内容部分页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult SearchBox()
        {
            var model = new SearchBoxModel
            {
                AutoCompleteEnabled = _catalogSettings.ArticleSearchAutoCompleteEnabled,
                ShowArticleImagesInSearchAutoComplete = _catalogSettings.ShowArticleImagesInSearchAutoComplete,
                SearchTermMinimumLength = _catalogSettings.ArticleSearchTermMinimumLength
            };
            return PartialView(model);
        }
        /// <summary>
        /// 搜索下拉提醒
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult SearchTermAutoComplete(string term)
        {
            if (String.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ArticleSearchTermMinimumLength)
                return Content("");

            // articles
            var pageSize = _catalogSettings.ArticleSearchAutoCompleteNumberOfArticles > 0 ? _catalogSettings.ArticleSearchAutoCompleteNumberOfArticles : 10;

            var ctx = new ArticleSearchContext();
            ctx.LanguageId = _services.WorkContext.WorkingLanguage.Id;
            ctx.Keywords = term;
            ctx.OrderBy = ArticleSortingEnum.Position;
            ctx.PageSize = pageSize;
            ctx.SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode;
            ctx.VisibleIndividuallyOnly = true;

            var articles = _articleService.SearchArticles(ctx);

            var models = _helper.PrepareArticlePostModels(
                articles,
                false,
                _catalogSettings.ShowArticleImagesInSearchAutoComplete,
                new System.Drawing.Size(_mediaSettings.ThumbPictureSizeOnDetailsPage, _mediaSettings.ThumbPictureSizeOnDetailsPage)).ToList();

            var result = (from p in models
                          select new
                          {
                              label = p.Title,
                              secondary = p.ShortContent.Truncate(70, "...") ?? "",
                              articleurl = Url.RouteUrl("Article", new { SeName = p.SeName }),
                              articlepictureurl = p.DefaultPictureModel.ImageUrl
                          })
                          .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region Articles by Tag
        /// <summary>
        /// 标签云部分页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult PopularArticleTags()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.ARTICLETAG_POPULAR_MODEL_KEY, _services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id);
            var cacheModel = _services.Cache.Get(cacheKey, () =>
            {
                var model = new PopularArticlesTagsModel();

                //get all tags
                var allTags = _articlesTagService
                    .GetAllArticleTags()
                    //filter by current site
                    .Where(x => _articlesTagService.GetArticleCount(x.Id, _services.SiteContext.CurrentSite.Id) > 0)
                    //order by article count
                    .OrderByDescending(x => _articlesTagService.GetArticleCount(x.Id, _services.SiteContext.CurrentSite.Id))
                    .ToList();

                var tags = allTags
                    .Take(_catalogSettings.NumberOfArticleTags)
                    .ToList();
                //sorting
                tags = tags.OrderBy(x => x.GetLocalized(y => y.Name)).ToList();

                model.TotalTags = allTags.Count;

                foreach (var tag in tags)
                    model.Tags.Add(new ArticleTagModel()
                    {
                        Id = tag.Id,
                        Name = tag.GetLocalized(y => y.Name),
                        SeName = tag.GetSeName(),
                        ArticleCount = _articlesTagService.GetArticleCount(tag.Id, _services.SiteContext.CurrentSite.Id)
                    });
                return model;
            });

            return PartialView(cacheModel);
        }
        /// <summary>
        /// 根据标签获取内容数据
        /// </summary>
        /// <param name="articleTagId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult ArticlesByTag(int articleTagId, ArticleCatalogPagingFilteringModel command)
        {
            var articlesTag = _articlesTagService.GetArticleTagById(articleTagId);
            if (articlesTag == null)
                return HttpNotFound();

            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            var model = new ArticleByTagModel()
            {
                Id = articlesTag.Id,
                TagName = articlesTag.GetLocalized(y => y.Name)
            };

            _helper.PreparePagingFilteringModel(model.PagingFilteringContext, command, new PageSizeContext
            {
                AllowUsersToSelectPageSize = _catalogSettings.ArticlesByTagAllowUsersToSelectPageSize,
                PageSize = _catalogSettings.ArticlesByTagPageSize,
                PageSizeOptions = _catalogSettings.ArticlesByTagPageSizeOptions.IsEmpty()
                    ? _catalogSettings.DefaultPageSizeOptions
                    : _catalogSettings.ArticlesByTagPageSizeOptions
            });

            //articless

            var ctx = new ArticleSearchContext();
            ctx.ArticleTagId = articlesTag.Id;
            ctx.LanguageId = _services.WorkContext.WorkingLanguage.Id;
            ctx.OrderBy = (ArticleSortingEnum)command.OrderBy;
            ctx.PageIndex = command.PageNumber - 1;
            ctx.PageSize = command.PageSize;
            ctx.SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode;
            ctx.VisibleIndividuallyOnly = true;

            var articless = _articleService.SearchArticles(ctx);

            model.Articles = _helper.PrepareArticlePostModels(
                articless).ToList();

            model.PagingFilteringContext.LoadPagedList(articless);
            //model.PagingFilteringContext.ViewMode = viewMode;
            return View(model);
        }
        /// <summary>
        /// 标签列表页面
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult ArticlesTagsAll()
        {
            var model = new PopularArticlesTagsModel();
            model.Tags = _articlesTagService
                .GetAllArticleTags()
                //filter by current site
                .Where(x => _articlesTagService.GetArticleCount(x.Id, _services.SiteContext.CurrentSite.Id) > 0)
                //sort by name
                .OrderBy(x => x.GetLocalized(y => y.Name))
                .Select(x =>
                {
                    var ptModel = new ArticleTagModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                        ArticleCount = _articlesTagService.GetArticleCount(x.Id, _services.SiteContext.CurrentSite.Id)
                    };
                    return ptModel;
                })
                .ToList();
            return View(model);
        }
        #endregion

        #region Recently[...]Articles
        /// <summary>
        /// 获取最近查看缓存下来的内容列表数据
        /// </summary>
        /// <param name="articleThumbPictureSize"></param>
        /// <param name="partialView"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult RecentlyViewedArticles(int? articleThumbPictureSize, string partialView)
        {
            var model = new List<ArticleOverviewModel>();
            if (_catalogSettings.RecentlyViewedArticlesEnabled)
            {
                var articles = _recentlyViewedArticlesService.GetRecentlyViewedArticles(_catalogSettings.RecentlyViewedArticlesNumber);
                if (articleThumbPictureSize.HasValue)
                    model.AddRange(_helper.PrepareArticlePostModels(articles, true, false, new System.Drawing.Size(articleThumbPictureSize ?? 0, articleThumbPictureSize ?? 0)));
                else
                    model.AddRange(_helper.PrepareArticlePostModels(articles));
            }
            return PartialView(partialView, model);
        }
        /// <summary>
        /// 获取内容列表数据
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="articleThumbPictureSize"></param>
        /// <param name="partialView"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RecentlyArticleViewed(int categoryId, int? channelId, int? articleThumbPictureSize, string partialView, ArticleCatalogPagingFilteringModel command)
        {
            var model = new ArticleCategoryModel();
            model = CategoryFilter(categoryId, channelId, articleThumbPictureSize, command, partialView);
            return PartialView(partialView, model);
        }
        /// <summary>
        /// Ajax获取内容列表数据
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="articleThumbPictureSize"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult AjaxHomeArticleBlock(int categoryId, int? channelId, int? articleThumbPictureSize, ArticleCatalogPagingFilteringModel command)
        {
            var model = new ArticleCategoryModel();
            model = CategoryFilter(categoryId, channelId, articleThumbPictureSize, command);
            return Json(model);
        }
        #endregion

        #region Clients
        /// <summary>
        /// 获取服务客户明细
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult Client(int clientId)
        {
            var client = _clientService.GetClientById(clientId);
            if (client == null || client.Deleted)
                return HttpNotFound();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a client before publishing
            if (!client.Published)
                return HttpNotFound();

            //Store mapping
            if (!_siteMappingService.Authorize(client))
                return HttpNotFound();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_services.WorkContext.CurrentUser,
                SystemUserAttributeNames.LastContinueShoppingPage,
                _services.WebHelper.GetThisPageUrl(false),
                _services.SiteContext.CurrentSite.Id);
            var model = client.ToModel();
            // prepare picture model
            model.PictureModel = this.PrepareClientPictureModel(client, model.Name);
            //activity log
            _services.UserActivity.InsertActivity("PublicStore.ViewClient", T("ActivityLog.PublicStore.ViewClient"), client.Name);

            return View(model);
        }
        /// <summary>
        /// 服务客户列表页
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult ClientAll()
        {
            var model = new List<ClientModel>();
            var clients = _clientService.GetAllClients();
            foreach (var client in clients)
            {
                var modelMan = client.ToModel();

                // prepare picture model
                modelMan.PictureModel = this.PrepareClientPictureModel(client, modelMan.Name);
                model.Add(modelMan);
            }

            return View(model);
        }
        /// <summary>
        /// 获取服务客户图片信息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="localizedName"></param>
        /// <returns></returns>
        private PictureModel PrepareClientPictureModel(Client client, string localizedName)
        {
            var model = new PictureModel();

            int pictureSize = _mediaSettings.ClientThumbPictureSize;
            var clientPictureCacheKey = string.Format(ModelCacheEventConsumer.CLIENT_PICTURE_MODEL_KEY,
                client.Id,
                pictureSize,
                true,
                _services.WorkContext.WorkingLanguage.Id,
                _services.WebHelper.IsCurrentConnectionSecured(),
                _services.SiteContext.CurrentSite.Id);

            model = _services.Cache.Get(clientPictureCacheKey, () =>
            {
                var pictureModel = new PictureModel()
                {
                    PictureId = client.PictureId.GetValueOrDefault(),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(client.PictureId.GetValueOrDefault()),
                    ImageUrl = _pictureService.GetPictureUrl(client.PictureId.GetValueOrDefault(), pictureSize),
                    Title = string.Format(T("Media.Client.ImageLinkTitleFormat"), localizedName),
                    AlternateText = string.Format(T("Media.Client.ImageAlternateTextFormat"), localizedName)
                };
                return pictureModel;
            });

            return model;
        }
        /// <summary>
        /// 服务客户导航页面
        /// </summary>
        /// <param name="currentClientId"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ClientNavigation(int currentClientId)
        {
            if (_catalogSettings.ClientsBlockItemsToDisplay == 0)
                return Content("");

            string cacheKey = string.Format(ModelCacheEventConsumer.CLIENT_NAVIGATION_MODEL_KEY, currentClientId, _services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id);
            var cacheModel = _services.Cache.Get(cacheKey, () =>
            {
                var currentClient = _clientService.GetClientById(currentClientId);

                var clients = _clientService.GetAllClients();
                var model = new ClientNavigationModel()
                {
                    TotalClients = clients.Count
                };

                foreach (var client in clients.Take(_catalogSettings.ClientsBlockItemsToDisplay))
                {
                    var modelMan = client.ToModel();

                    modelMan.IsActive = currentClient != null && currentClient.Id == modelMan.Id;
                    modelMan.PictureModel = this.PrepareClientPictureModel(client, modelMan.Name);

                    model.Clients.Add(modelMan);
                }
                return model;
            });

            if (cacheModel.Clients.Count == 0)
                return Content("");

            return PartialView(cacheModel);
        }
        /// <summary>
        /// Ajax获取客户信息
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult AjaxClientItem(int clientId)
        {

            var client = _clientService.GetClientById(clientId);
            var modelMan = client.ToModel();
            modelMan.PictureModel = this.PrepareClientPictureModel(client, modelMan.Name);
            //var model = new List<ClientModel>();
            //model.Add(modelMan);
            //foreach (var client in clients)
            //{
            //    var modelMan = client.ToModel();

            //    // prepare picture model
            //    modelMan.PictureModel = this.PrepareClientPictureModel(client, modelMan.Name);
            //    model.Add(modelMan);
            //}

            return Json(modelMan);
        }
        /// <summary>
        /// 获取所有服务客户部分页面
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult PartialClientAll()
        {
            var model = new List<ClientModel>();
            var clients = _clientService.GetAllClients();
            foreach (var client in clients)
            {
                var modelMan = client.ToModel();

                // prepare picture model
                modelMan.PictureModel = this.PrepareClientPictureModel(client, modelMan.Name);
                model.Add(modelMan);
            }

            return PartialView(model);
        }
        #endregion


    }
}