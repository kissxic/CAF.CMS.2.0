using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Filter;
using CAF.WebSite.Mvc.Models.Filter;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Controllers
{
    public partial class FilterController : Controller // not BaseController cause of performance
    {
        private readonly IFilterService _filterService;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly IArticleCategoryService _articleCategoryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ArticleCatalogHelper _helper;
        public FilterController(IFilterService filterService, ArticleCatalogSettings catalogSettings,
            IArticleCategoryService articleCategoryService,
            IProductCategoryService productCategoryService,
            ArticleCatalogHelper helper)
        {
            this._filterService = filterService;
            this._catalogSettings = catalogSettings;
            this._articleCategoryService = articleCategoryService;
            this._productCategoryService = productCategoryService;
            this._helper = helper;
        }

        private bool IsShowAllText(ICollection<FilterCriteria> criteriaGroup)
        {
            if (criteriaGroup.Any(c => c.Entity.IsCaseInsensitiveEqual(FilterService.ShortcutPrice)))
                return false;

            return (criteriaGroup.Count >= _catalogSettings.MaxFilterItemsToDisplay || criteriaGroup.Any(c => !c.IsInactive));
        }

        public ActionResult Articles(string filter, int categoryID, int productCategoryID, string path, int? pagesize, int? orderby, string viewmode)
        {
            var context = _filterService.CreateFilterArticleContext(filter, categoryID, productCategoryID, path, pagesize, orderby, viewmode);
            if (_catalogSettings.ShowArticlesFromSubcategories && categoryID > 0)
            {
                context.CategoryIds.AddRange(_helper.GetChildCategoryIds(categoryID));
            }
            if (_catalogSettings.ShowArticlesFromSubcategories && productCategoryID > 0)
            {
                context.ProductCategoryIds.AddRange(_helper.GetChildProductCategoryIds(productCategoryID));
            }
            _filterService.ArticleFilterable(context);

            return PartialView(new ArticleFilterModel
            {
                Context = context,
                IsShowAllText = IsShowAllText(context.Criteria),
                MaxFilterItemsToDisplay = _catalogSettings.MaxFilterItemsToDisplay,
                ExpandAllFilterGroups = _catalogSettings.ExpandAllFilterCriteria
            });
        }

        [HttpPost]
        public ActionResult Articles(string active, string inactive, int categoryID, int productCategoryID, int? pagesize, int? orderby, string viewmode)
        {
            // TODO: needed later for ajax based filtering... see example below
            //System.Threading.Thread.Sleep(3000);

            var context = new FilterArticleContext
            {
                ParentCategoryID = categoryID,
                CategoryIds = new List<int> { categoryID },
                ParentProductCategoryID = productCategoryID,
                ProductCategoryIds = new List<int> { productCategoryID },
                Criteria = _filterService.Deserialize(active),
                OrderBy = orderby,
            };

            context.Criteria.AddRange(_filterService.Deserialize(inactive));

            //var query = _filterService.ArticleFilter(context);
            //var Articles = new PagedList<Article>(query, 0, pagesize ?? 4);

            //ArticleListModel model = new ArticleListModel {
            //	PagingFilteringContext = new CatalogPagingFilteringModel()
            //};

            //model.Articles = PrepareArticleOverviewModels(Articles).ToList();
            //model.PagingFilteringContext.LoadPagedList(Articles);

            //string htmlArticles = this.RenderPartialViewToString("~/Views/Catalog/_ArticleBoxContainer.cshtml", model);

            //return Json(new {
            //	Articles = htmlArticles
            //});

            return null;
        }

        public ActionResult ArticlesMultiSelect(string filter, int categoryID, int productCategoryID, string path, int? pagesize, int? orderby, string viewmode, string filterMultiSelect)
        {
            var context = _filterService.CreateFilterArticleContext(filter, categoryID, productCategoryID, path, pagesize, orderby, viewmode);
            if (_catalogSettings.ShowArticlesFromSubcategories && categoryID > 0)
            {
                context.CategoryIds.AddRange(_helper.GetChildCategoryIds(categoryID));
            }
            if (_catalogSettings.ShowArticlesFromSubcategories && productCategoryID > 0)
            {
                context.ProductCategoryIds.AddRange(_helper.GetChildProductCategoryIds(productCategoryID));
            }
            _filterService.ArticleFilterableMultiSelect(context, filterMultiSelect);

            return PartialView(new ArticleFilterModel
            {
                Context = context,
                IsShowAllText = IsShowAllText(context.Criteria),
                MaxFilterItemsToDisplay = _catalogSettings.MaxFilterItemsToDisplay,
                ExpandAllFilterGroups = _catalogSettings.ExpandAllFilterCriteria
            });
        }
    }
}
