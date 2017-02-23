using System;
using CAF.Infrastructure.SearchModule.Domain.Search;
using CAF.Infrastructure.SearchModule.Domain.Search.Model;

namespace CAF.Infrastructure.SearchModule.Data.Providers.ElasticSearch
{
    /// <summary>
    /// General Elastic Search Exception
    /// </summary>
    public class ElasticSearchException : SearchException
    {

        public ElasticSearchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ElasticSearchException(string message)
            : base(message)
        {
        }

    }
}
