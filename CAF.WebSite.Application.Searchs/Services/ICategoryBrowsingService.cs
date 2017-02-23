using CAF.Infrastructure.SearchModule.Data.Model;
using CAF.Infrastructure.SearchModule.Model;
using CAF.Infrastructure.SearchModule.Model.Search.Criterias;

namespace CAF.WebSite.Application.Searchs.Services
{
    public interface ICategoryBrowsingService
    {
        CategorySearchResult SearchCategories(string scope, ISearchCriteria criteria, CategoryResponseGroup responseGroup);
    }
}
