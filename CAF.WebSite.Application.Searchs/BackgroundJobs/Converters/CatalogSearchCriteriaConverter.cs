using CAF.Infrastructure.SearchModule.Core.Catalogs.Search;
using CAF.Infrastructure.SearchModule.Core.Model;
using CAF.Infrastructure.SearchModule.Data.Model;
using CAF.WebSite.Application.Searchs.Infrastructure;
using CAF.WebSite.Application.Searchs.Models.Searchs;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Searchs.Converters
{
    public static class CatalogSearchCriteriaConverter
    {
        public static SearchCriteria ToCatalogApiModel(this CatalogSearchCriteria criteria, AppWorkContext workContext)
        {
            var result = new SearchCriteria
            {
                //  StoreId = workContext.CurrentStore.Id,
                Keyword = criteria.Keyword,
                //  ResponseGroup = criteria.ResponseGroup.ToString(),
                SearchInChildren = criteria.SearchInChildren,
                CategoryId = criteria.CategoryId,
                CatalogId = criteria.CatalogId,
                VendorId = criteria.VendorId,
                //   Currency = criteria.Currency == null ? workContext.CurrentCurrency.Code : criteria.Currency.Code,
                HideDirectLinkedCategories = true,
                Terms = criteria.Terms.ToStrings().ToArray(),
                //   PricelistIds = workContext.CurrentPricelists.Where(p => p.Currency == workContext.CurrentCurrency.Code).Select(p => p.Id).ToList(),
                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.SortBy
            };

            if (criteria.VendorIds != null)
            {
                //result.VendorIds = criteria.VendorIds.ToList();
            }

            return result;
        }

        public static ProductSearch ToSearchApiModel(this ProductSearchCriteria criteria, AppWorkContext workContext)
        {
            var result = new ProductSearch()
            {
                SearchPhrase = criteria.Keyword,
                Outline = criteria.Outline,
                //  Currency = criteria.Currency == null ? workContext.CurrentCurrency.Code : criteria.Currency.Code,
                Terms = criteria.Terms.ToStrings().ToArray(),
                //  PriceLists = workContext.CurrentPricelists.Where(p => p.Currency == workContext.CurrentCurrency.Code).Select(p => p.Id).ToList(),
                Skip = criteria.Start,
                Take = criteria.PageSize
            };

            //// Add vendor id to terms
            //if (!string.IsNullOrEmpty(criteria.VendorId))
            //{
            //    if (result.Terms == null)
            //    {
            //        result.Terms = new List<string>();
            //    }

            //    result.Terms.Add(string.Format("vendor:{0}", criteria.VendorId));
            //}

            if (criteria.SortBy != null)
                result.Sort = new string[] { criteria.SortBy };

            return result;
        }

        public static CategorySearch ToSearchApiModel(this ProductCategorySearchCriteria criteria, AppWorkContext workContext)
        {

            var result = new CategorySearch()
            {
                Skip = criteria.Start,
                Take = criteria.PageSize,
                Outline = criteria.Outline
            };

            if (criteria.SortBy != null)
                result.Sort = new string[] { criteria.SortBy };

            return result;
        }
    }
}
