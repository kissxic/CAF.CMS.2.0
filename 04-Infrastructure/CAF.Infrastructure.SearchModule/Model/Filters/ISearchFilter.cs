﻿namespace CAF.Infrastructure.SearchModule.Model.Filters
{
    public interface ISearchFilter
    {
        string Key { get; }

        string CacheKey { get; }
    }
}
