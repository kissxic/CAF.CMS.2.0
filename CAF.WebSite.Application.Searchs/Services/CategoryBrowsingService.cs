using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.SearchModule.Core.Model;
using CAF.Infrastructure.SearchModule.Data.Model;
using CAF.Infrastructure.SearchModule.Model;
using CAF.Infrastructure.SearchModule.Model.Indexing;
using CAF.Infrastructure.SearchModule.Model.Search;
using CAF.Infrastructure.SearchModule.Model.Search.Criterias;
using CAF.WebSite.Application.Services.Articles;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Searchs.Services
{
    public class CategoryBrowsingService : ICategoryBrowsingService
    {
        private readonly IProductCategoryService _categoryService;
        private readonly ISearchProvider _searchProvider;
     
        public CategoryBrowsingService(
            IProductCategoryService categoryService,
            ISearchProvider searchService)
        {
            _searchProvider = searchService;
            _categoryService = categoryService;
           
        }
        
        public virtual CategorySearchResult SearchCategories(string scope, ISearchCriteria criteria, CategoryResponseGroup responseGroup)
        {
            var items = new List<ProductCategory>();
            var itemsOrderedList = new List<string>();

            var foundItemCount = 0;
            var dbItemCount = 0;
            var searchRetry = 0;

            //var myCriteria = criteria.Clone();
            var myCriteria = criteria;

            ISearchResults<DocumentDictionary> searchResults = null;

            do
            {
                // Search using criteria, it will only return IDs of the items
                searchResults = _searchProvider.Search<DocumentDictionary>(scope, criteria);

                searchRetry++;

                if (searchResults == null || searchResults.Documents == null)
                {
                    continue;
                }

                //Get only new found itemIds
                var uniqueKeys = searchResults.Documents.Select(x => x.Id.ToString()).Except(itemsOrderedList).ToArray();
                foundItemCount = uniqueKeys.Length;

                if (!searchResults.Documents.Any())
                {
                    continue;
                }

                itemsOrderedList.AddRange(uniqueKeys);

                // if we can determine catalog, pass it to the service
                string catalog = null;
                if (criteria is CatalogItemSearchCriteria)
                {
                    catalog = (criteria as CatalogItemSearchCriteria).Catalog;
                }

                // Now load items from repository
                var currentItems = _categoryService.GetCategorysByIds(uniqueKeys.Convert<int[]>());

                var orderedList = currentItems.OrderBy(i => itemsOrderedList.IndexOf(i.Id.ToString()));
                items.AddRange(orderedList);
                dbItemCount = currentItems.Count;

                //If some items where removed and search is out of sync try getting extra items
                if (foundItemCount > dbItemCount)
                {
                    //Retrieve more items to fill missing gap
                    myCriteria.RecordsToRetrieve += (foundItemCount - dbItemCount);
                }
            }
            while (foundItemCount > dbItemCount && searchResults != null && searchResults.Documents.Any() && searchRetry <= 3 &&
                (myCriteria.RecordsToRetrieve + myCriteria.StartingRecord) < searchResults.TotalCount);

            var response = new CategorySearchResult();

            if (items != null)
            {
                response.Categories = items.ToArray();
            }

            if (searchResults != null)
            {
                response.TotalCount = searchResults.TotalCount;
            }

            return response;
        }
    }
}
