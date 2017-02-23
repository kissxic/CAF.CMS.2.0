using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAF.Infrastructure.SearchModule.Model.Search;
using CAF.Infrastructure.SearchModule.Model.Search.Criterias;

namespace CAF.Infrastructure.SearchModule.Model
{
    public interface ISearchProvider
    {
        ISearchQueryBuilder[] QueryBuilders { get; }

        void Close(string scope, string documentType);

        void Commit(string scope);

        void Index<T>(string scope, string documentType, T document);

        int Remove(string scope, string documentType, string key, string value);

        void RemoveAll(string scope, string documentType);

        ISearchResults<T> Search<T>(string scope, ISearchCriteria criteria) where T : class;
    }

}
