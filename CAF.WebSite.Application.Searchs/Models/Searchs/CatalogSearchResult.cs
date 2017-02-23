using System.Collections.Generic;
using PagedList;
using CAF.Infrastructure.Core.Domain.Cms.Articles;

namespace CAF.WebSite.Application.Searchs.Models.Searchs
{
    public class CatalogSearchResult
    {
        public IPagedList<Article> Products { get; set; }
        public Aggregation[] Aggregations { get; set; }
    }
}
