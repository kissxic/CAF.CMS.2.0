using System;
using System.Collections.Generic;

namespace CAF.Infrastructure.SearchModule.Model.Filters
{
   public interface IBrowseFilterService
    {
        ISearchFilter[] GetFilters(IDictionary<string, object> context);
    }
}
