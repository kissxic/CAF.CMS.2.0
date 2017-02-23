using System;
using System.Collections.Generic;
using CAF.Infrastructure.SearchModule.Domain.Search.Model;

namespace CAF.Infrastructure.SearchModule.Domain.Search.Services
{
   public interface IBrowseFilterService
    {
        ISearchFilter[] GetFilters(IDictionary<string, object> context);
    }
}
