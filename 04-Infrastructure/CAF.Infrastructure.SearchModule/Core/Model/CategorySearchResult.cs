using CAF.Infrastructure.Core.Domain.Cms.Articles;

namespace CAF.Infrastructure.SearchModule.Data.Model
{
    public class CategorySearchResult
    {
        public CategorySearchResult()
        {

        }

        public ProductCategory[] Categories { get; set; }

        public long TotalCount { get; set; }
    }
}