using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.SearchModule.Core.Model;
using CAF.Infrastructure.SearchModule.Model;
using CAF.WebSite.Application.Searchs.Models.Searchs;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Searchs.Services
{
    /// <summary>
    /// Represent abstraction to search in catalog api (products, categories etc)
    /// </summary>
    public interface ICatalogSearchService
    {
        Task<Article[]> GetProductsAsync(string[] ids, ItemResponseGroup responseGroup);

        Task<ProductCategory[]> GetCategoriesAsync(string[] ids, CategoryResponseGroup responseGroup);

        Task<CatalogSearchResult> SearchProductsAsync(ProductSearchCriteria criteria);

        Task<IPagedList<ProductCategory>> SearchCategoriesAsync(CategorySearchCriteria criteria);

        CatalogSearchResult SearchProducts(ProductSearchCriteria criteria);

        IPagedList<ProductCategory> SearchCategories(CategorySearchCriteria criteria);
    }
}
