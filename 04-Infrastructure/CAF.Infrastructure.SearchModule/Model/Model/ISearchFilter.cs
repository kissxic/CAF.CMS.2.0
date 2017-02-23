namespace CAF.Infrastructure.SearchModule.Domain.Search.Model
{
    public interface ISearchFilter
    {
        string Key { get; }

        string CacheKey { get; }
    }
}
