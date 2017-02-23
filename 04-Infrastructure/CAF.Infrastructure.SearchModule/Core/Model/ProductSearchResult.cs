using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.SearchModule.Core.Catalogs.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CAF.Infrastructure.SearchModule.Data.Model
{
    public class ProductSearchResult
    {
        public ProductSearchResult()
        {

        }

        public Aggregation[] Aggregations { get; set; }

        public Article[] Products { get; set; }

        public long TotalCount { get; set; }
    }
}