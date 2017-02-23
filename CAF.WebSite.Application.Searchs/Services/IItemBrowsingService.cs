using CAF.Infrastructure.SearchModule.Data.Model;
using CAF.Infrastructure.SearchModule.Model;
using CAF.Infrastructure.SearchModule.Model.Search.Criterias;

namespace CAF.WebSite.Application.Searchs.Services
{
    public interface IItemBrowsingService
    {
        ProductSearchResult SearchItems(string scope, ISearchCriteria criteria, ItemResponseGroup responseGroup);
    }
}
