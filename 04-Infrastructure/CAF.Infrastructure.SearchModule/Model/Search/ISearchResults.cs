using System.Collections.Generic;
using CAF.Infrastructure.SearchModule.Model.Search.Criterias;

namespace CAF.Infrastructure.SearchModule.Model.Search
{
    public interface ISearchResults<T> where T : class
    {
        IEnumerable<T> Documents { get; }

        ISearchCriteria SearchCriteria { get; }

        long DocCount { get; }

        FacetGroup[] Facets { get; }

        string[] Suggestions { get;}

        long TotalCount { get; }
    }
}
