using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Collections.Generic;

namespace CAF.Infrastructure.SearchModule.Core.Catalogs.Search
{
    public class SearchResult
    {
        public SearchResult()
        {
            Products = new List<Article>();
            Categories = new List<ProductCategory>();
            //   Catalogs = new List<Catalog>();
        }

        public int ProductsTotalCount { get; set; }
        /// <summary>
        /// Type used in search result and represent properties search result aggregation 
        /// </summary>
        public ICollection<Article> Products { get; set; }
        public ICollection<ProductCategory> Categories { get; set; }
        //  public ICollection<Catalog> Catalogs { get; set; }

        /// <summary>
        /// Represent aggregations for product properties
        /// </summary>
        public Aggregation[] Aggregations { get; set; }
    }
}
