﻿using System;
using System.Collections.Specialized;
using Lucene.Net.Index;
using Lucene.Net.Search;
using CAF.Infrastructure.SearchModule.Model.Search;
using CAF.Infrastructure.SearchModule.Model.Search.Criterias;
using CAF.Infrastructure.SearchModule.Model.Filters;

namespace CAF.Infrastructure.SearchModule.Providers.Lucene
{
    public class BaseSearchQueryBuilder : ISearchQueryBuilder
    {
        public virtual string DocumentType
        {
            get
            {
                return string.Empty; // default implementation, can handle generic queries
            }
        }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public virtual object BuildQuery<T>(string scope, ISearchCriteria criteria) where T : class
        {
            var queryBuilder = new QueryBuilder();
            var queryFilter = new BooleanFilter();
            var query = new BooleanQuery();
            queryBuilder.Query = query;
            queryBuilder.Filter = queryFilter;

            if (criteria.CurrentFilters != null)
            {
                foreach (var filter in criteria.CurrentFilters)
                {
                    // Skip currencies that are not part of the filter
                    if (filter.GetType() == typeof(PriceRangeFilter)) // special filtering 
                    {
                        var currency = (filter as PriceRangeFilter).Currency;
                        if (!currency.Equals(criteria.Currency, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }

                    var filterQuery = LuceneQueryHelper.CreateQuery(criteria, filter, Occur.SHOULD);

                    // now add other values that should also be counted?

                    if (filterQuery != null)
                    {
                        var clause = new FilterClause(filterQuery, Occur.MUST);
                        queryFilter.Add(clause);
                    }
                }
            }

            return queryBuilder;
        }

        /// <summary>
        ///     Adds the query.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="query">The query.</param>
        /// <param name="filter">The filter.</param>
        protected void AddQuery(string fieldName, BooleanQuery query, StringCollection filter)
        {
            fieldName = fieldName.ToLower();
            if (filter.Count > 0)
            {
                if (filter.Count != 1)
                {
                    var booleanQuery = new BooleanQuery();
                    var containsFilter = false;
                    foreach (var index in filter)
                    {
                        if (String.IsNullOrEmpty(index))
                        {
                            continue;
                        }

                        var nodeQuery = new WildcardQuery(new Term(fieldName, index));
                        booleanQuery.Add(nodeQuery, Occur.SHOULD);
                        containsFilter = true;
                    }
                    if (containsFilter)
                    {
                        query.Add(booleanQuery, Occur.MUST);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(filter[0]))
                    {
                        this.AddQuery(fieldName, query, filter[0].ToLower());
                    }
                }
            }
        }

        /// <summary>
        ///     Adds the query.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="query">The query.</param>
        /// <param name="filter">The filter.</param>
        protected void AddQuery(string fieldName, BooleanQuery query, string filter)
        {
            fieldName = fieldName.ToLower();
            var nodeQuery = new WildcardQuery(new Term(fieldName, filter.ToLower()));
            query.Add(nodeQuery, Occur.MUST);
        }

    }
}