using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CAF.Infrastructure.SearchModule.Domain.Search.Model;

namespace CAF.Infrastructure.SearchModule.Domain.Search.Services
{
    public interface ISearchQueryBuilder
    {
        object BuildQuery(ISearchCriteria criteria);
    }

}
