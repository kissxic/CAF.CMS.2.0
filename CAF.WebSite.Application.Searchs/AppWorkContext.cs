using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Searchs.Models.Common;
using CAF.WebSite.Application.Searchs.Models.Searchs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Application.Searchs.Infrastructure
{
    public class AppWorkContext : IDisposable
    {
        /// <summary>
        /// Current request url example: http:/host/app/store/en-us/search?page=2
        /// </summary>
        public Uri RequestUrl { get; set; }

        public NameValueCollection QueryString { get; set; }
        /// <summary>
        /// Represent bucket, aggregated data based on a search query resulted by current search criteria CurrentCatalogSearchCriteria (example  color 33, gr
        /// </summary>
        public IMutablePagedList<Aggregation> Aggregations { get; set; }

        /// <summary>
        /// Current search product search criterias
        /// </summary>
        public ProductSearchCriteria CurrentProductSearchCriteria { get; set; }

        /// <summary>
        /// Represent products filtered by current search criteria CurrentCatalogSearchCriteria (loaded on first access by lazy loading)
        /// </summary>
        public IMutablePagedList<Article> Products { get; set; }
        /// <summary>
        /// Represent all store catalog categories filtered by current search criteria CurrentCatalogSearchCriteria (loaded on first access by lazy loading)
        /// </summary>
        public IMutablePagedList<ProductCategory> ProductCategorys { get; set; }
        /// <summary>
        /// Current page number
        /// </summary>
        public int? PageNumber { get; set; }
        /// <summary>
        /// Current page size
        /// </summary>
        public int? PageSize { get; set; }

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        #endregion
    }
}