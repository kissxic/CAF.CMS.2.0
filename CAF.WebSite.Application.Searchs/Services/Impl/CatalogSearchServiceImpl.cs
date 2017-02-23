using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.SearchModule.Core.Model;
using CAF.Infrastructure.SearchModule.Model;
using CAF.WebSite.Application.Searchs.Models.Searchs;
using PagedList;
using CAF.WebSite.Application.Searchs.Infrastructure;
using CAF.WebSite.Application.Searchs.Converters;
using CAF.Infrastructure.SearchModule.Data.Model;
using CAF.Infrastructure.SearchModule.Model.Filters;
 

namespace CAF.WebSite.Application.Searchs.Services
{
    public class CatalogSearchServiceImpl : ICatalogSearchService
    {
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchConnection _searchConnection;
        private readonly IBrowseFilterService _browseFilterService;
        private readonly IItemBrowsingService _browseService;
        private readonly Func<AppWorkContext> _workContextFactory;

        public CatalogSearchServiceImpl(ISearchProvider searchProvider, ISearchConnection searchConnection,
                       IBrowseFilterService browseFilterService, IItemBrowsingService browseService,
            Func<AppWorkContext> workContextFactory)
        {
            _searchProvider = searchProvider;
            _searchConnection = searchConnection;
            _browseFilterService = browseFilterService;
            _browseService = browseService;
            _workContextFactory = workContextFactory;
        }

        public Task<Article[]> GetProductsAsync(string[] ids, ItemResponseGroup responseGroup)
        {
            throw new NotImplementedException();
        }

        public Task<ProductCategory[]> GetCategoriesAsync(string[] ids, CategoryResponseGroup responseGroup)
        {
            throw new NotImplementedException();
        }

        public Task<CatalogSearchResult> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<ProductCategory>> SearchCategoriesAsync(CategorySearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public CatalogSearchResult SearchProducts(ProductSearchCriteria criteria)
        {
            criteria = criteria.Clone();
            var workContext = _workContextFactory();
            var searchCriteria = criteria.ToSearchApiModel(workContext);
          
            var result = SearchProducts(_searchConnection.Scope, searchCriteria, ItemResponseGroup.ItemLarge);
            //可自行转换
            var products = result.Products.ToList();

            return new CatalogSearchResult
            {
                Products = new StaticPagedList<Article>(products, criteria.PageNumber, criteria.PageSize, (int?)result.TotalCount ?? 0),
                Aggregations = !result.Aggregations.IsNullOrEmpty() ? result.Aggregations.Select(x => x.ToWebModel("1")).ToArray() : new Aggregation[] { }
            };
        }

        public IPagedList<ProductCategory> SearchCategories(CategorySearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        private ProductSearchResult SearchProducts(string scope, ProductSearch criteria, ItemResponseGroup responseGroup)
        {
            var context = new Dictionary<string, object>
            {
                { "Store", 1 },
            };

            var filters = _browseFilterService.GetFilters(context);

            var serviceCriteria = criteria.AsCriteria<CatalogItemSearchCriteria>("0", filters);

            var searchResults = _browseService.SearchItems(scope, serviceCriteria, responseGroup);
            return searchResults;
        }
    }
}
