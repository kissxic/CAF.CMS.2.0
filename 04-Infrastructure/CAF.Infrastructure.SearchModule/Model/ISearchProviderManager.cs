using System;
using System.Collections.Generic;
using CAF.Infrastructure.SearchModule.Domain.Search;
using CAF.Infrastructure.SearchModule.Domain.Search.Model;
using CAF.Infrastructure.SearchModule.Domain.Search.Services;

namespace CAF.Infrastructure.SearchModule.Domain.Search
{
    public interface ISearchProviderManager
    {
        void RegisterSearchProvider(string name, Func<ISearchConnection, ISearchProvider> factory);
        IEnumerable<string> RegisteredProviders { get; }
        ISearchProvider CurrentProvider { get; }
        ISearchConnection CurrentConnection { get; }
    }
}
