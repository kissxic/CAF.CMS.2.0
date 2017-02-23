﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Nest;
using CAF.Infrastructure.SearchModule.Model.Search.Criterias;
using CAF.Infrastructure.SearchModule.Model.Search;
using CAF.Infrastructure.SearchModule.Model.Filters;

namespace CAF.Infrastructure.SearchModule.Providers.ElasticSearch.Nest
{
    public class ElasticSearchQueryBuilder : ISearchQueryBuilder
    {
        public virtual string DocumentType
        {
            get
            {
                return string.Empty; // default implementation, can handle generic queries
            }
        }

        #region ISearchQueryBuilder Members

        public virtual object BuildQuery<T>(string scope, ISearchCriteria criteria)
            where T : class
        {
            var builder = new SearchRequest(scope, criteria.DocumentType)
            {
                From = criteria.StartingRecord,
                Size = criteria.RecordsToRetrieve,
                Sort = GetSorting<T>(criteria.Sort),
                PostFilter = GetPostFilter<T>(criteria),
                Query = GetQuery<T>(criteria),
                Aggregations = GetAggregations<T>(criteria),
            };

            return builder;
        }

        #endregion

        protected virtual IList<ISort> GetSorting<T>(SearchSort sorting)
            where T : class
        {
            IList<ISort> result = null;

            if (sorting != null)
            {
                var fields = sorting.GetSort();

                foreach (var field in fields)
                {
                    if (result == null)
                    {
                        result = new List<ISort>();
                    }

                    result.Add(
                        new SortField
                        {
                            Field = field.FieldName,
                            Order = field.IsDescending ? SortOrder.Descending : SortOrder.Ascending,
                            Missing = "_last",
                            IgnoreUnmappedFields = true
                        });
                }
            }

            return result;
        }

        protected virtual QueryContainer GetPostFilter<T>(ISearchCriteria criteria)
            where T : class
        {
            QueryContainer result = null;

            // Perform facet filters
            if (criteria.CurrentFilters != null && criteria.CurrentFilters.Any())
            {
                foreach (var filter in criteria.CurrentFilters)
                {
                    // Skip currencies that are not part of the filter
                    if (filter.GetType() == typeof(PriceRangeFilter)) // special filtering 
                    {
                        var priceRangeFilter = filter as PriceRangeFilter;
                        if (priceRangeFilter != null)
                        {
                            var currency = priceRangeFilter.Currency;
                            if (!currency.Equals(criteria.Currency, StringComparison.OrdinalIgnoreCase))
                                continue;
                        }
                    }

                    var filterQuery = ElasticQueryHelper.CreateQuery<T>(criteria, filter);

                    if (filterQuery != null)
                    {
                        result &= filterQuery;
                    }
                }
            }

            return result;
        }

        protected virtual QueryContainer GetQuery<T>(ISearchCriteria criteria)
            where T : class
        {
            QueryContainer result = null;

            var keywordSearchCriteria = criteria as KeywordSearchCriteria;
            if (keywordSearchCriteria != null)
            {
                if (!string.IsNullOrEmpty(keywordSearchCriteria.SearchPhrase))
                {
                    var searchFields = new List<string> { "__content" };

                    if (!string.IsNullOrEmpty(criteria.Locale))
                    {
                        searchFields.Add(string.Concat("__content_", criteria.Locale.ToLower()));
                    }

                    result = CreateKeywordQuery<T>(keywordSearchCriteria, searchFields.ToArray());
                }
            }

            return result;
        }

        #region Aggregations

        protected virtual AggregationDictionary GetAggregations<T>(ISearchCriteria criteria)
            where T : class
        {
            var container = new Dictionary<string, AggregationContainer>();

            foreach (var filter in criteria.Filters)
            {
                var fieldName = filter.Key.ToLower();
                var attributeFilter = filter as AttributeFilter;
                var priceRangeFilter = filter as PriceRangeFilter;
                var rangeFilter = filter as RangeFilter;

                if (attributeFilter != null)
                {
                    AddAttributeAggregationQueries<T>(container, fieldName, attributeFilter.FacetSize, criteria);
                }
                else if (priceRangeFilter != null)
                {
                    var currency = priceRangeFilter.Currency;
                    if (currency.Equals(criteria.Currency, StringComparison.OrdinalIgnoreCase))
                    {
                        AddPriceAggregationQueries<T>(container, fieldName, priceRangeFilter.Values, criteria);
                    }
                }
                else if (rangeFilter != null)
                {
                    AddRangeAggregationQueries<T>(container, fieldName, rangeFilter.Values, criteria);
                }
            }

            return container;
        }

        protected virtual void AddAttributeAggregationQueries<T>(Dictionary<string, AggregationContainer> container, string field, int? facetSize, ISearchCriteria criteria)
            where T : class
        {
            var existingFilters = GetExistingFilters<T>(criteria, field);

            var agg = new FilterAggregation(field)
            {
                Filter = new BoolQuery { Must = existingFilters },
                Aggregations = new TermsAggregation(field)
                {
                    Field = field,
                    Size = facetSize
                },
            };

            container.Add(field, agg);
        }

        protected virtual void AddPriceAggregationQueries<T>(Dictionary<string, AggregationContainer> container, string fieldName, IEnumerable<RangeFilterValue> values, ISearchCriteria criteria)
            where T : class
        {
            if (values == null)
                return;

            var existingFilters = GetExistingFilters<T>(criteria, fieldName);

            foreach (var value in values)
            {
                var query = ElasticQueryHelper.CreatePriceRangeFilter<T>(criteria, fieldName, value);

                if (query != null)
                {
                    var allFilters = new List<QueryContainer>();
                    allFilters.AddRange(existingFilters);
                    allFilters.Add(query);

                    var boolQuery = new BoolQuery { Must = allFilters };
                    var agg = new FilterAggregation(string.Format(CultureInfo.InvariantCulture, "{0}-{1}", fieldName, value.Id)) { Filter = boolQuery };
                    container.Add(string.Format(CultureInfo.InvariantCulture, "{0}-{1}", fieldName, value.Id), agg);
                }
            }
        }

        protected virtual void AddRangeAggregationQueries<T>(Dictionary<string, AggregationContainer> container, string fieldName, IEnumerable<RangeFilterValue> values, ISearchCriteria criteria)
            where T : class
        {
            if (values == null)
                return;

            var existingFilters = GetExistingFilters<T>(criteria, fieldName);

            foreach (var value in values)
            {
                var query = new TermRangeQuery { Field = fieldName, GreaterThanOrEqualTo = value.Lower, LessThan = value.Upper };

                var allFilters = new List<QueryContainer>();
                allFilters.AddRange(existingFilters);
                allFilters.Add(query);

                var boolQuery = new BoolQuery { Must = allFilters };
                var agg = new FilterAggregation(string.Format(CultureInfo.InvariantCulture, "{0}-{1}", fieldName, value.Id)) { Filter = boolQuery };
                container.Add(string.Format(CultureInfo.InvariantCulture, "{0}-{1}", fieldName, value.Id), agg);
            }
        }

        #endregion

        #region Helper Query Methods

        protected virtual QueryContainer CreateQuery(string fieldName, StringCollection values, bool lowerCase = true)
        {
            return CreateQuery(fieldName, values.OfType<string>().ToList(), lowerCase);
        }

        protected virtual QueryContainer CreateQuery(string fieldName, IList<string> values, bool lowerCase)
        {
            QueryContainer result = null;

            if (values.Count > 0)
            {
                if (values.Count == 1)
                {
                    var value = values[0];
                    if (!string.IsNullOrEmpty(value))
                    {
                        result &= CreateWildcardQuery(fieldName, value, lowerCase);
                    }
                }
                else
                {
                    var containsFilter = false;
                    var valueContainer = new List<QueryContainer>();

                    foreach (var value in values.Where(v => !string.IsNullOrEmpty(v)))
                    {
                        valueContainer.Add(CreateWildcardQuery(fieldName, value, lowerCase));
                        containsFilter = true;
                    }

                    if (containsFilter)
                    {
                        result |= new BoolQuery { Should = valueContainer };
                    }

                }
            }

            return result;
        }
        protected virtual QueryContainer CreateQuery(string fieldName, string value, bool lowerCase = true)
        {
            QueryContainer query = null;
            query &= CreateWildcardQuery(fieldName, value, lowerCase);
            return query;
        }


        protected virtual QueryContainer CreateWildcardQuery(string fieldName, string value, bool lowerCaseValue)
        {
            return new WildcardQuery { Field = fieldName.ToLower(), Value = lowerCaseValue ? value.ToLower() : value };
        }

        protected virtual QueryContainer CreateKeywordQuery<T>(KeywordSearchCriteria criteria, params string[] fields)
            where T : class
        {
            QueryContainer query = null;
            var searchPhrase = criteria.SearchPhrase;
            MultiMatchQuery multiMatch;
            if (criteria.IsFuzzySearch)
            {
                multiMatch = new MultiMatchQuery
                {
                    Fields = fields,
                    Query = searchPhrase,
                    Fuzziness = Fuzziness.Auto,
                    Analyzer = "standard",
                    Operator = Operator.And
                };
            }
            else
            {
                multiMatch = new MultiMatchQuery
                {
                    Fields = fields,
                    Query = searchPhrase,
                    Analyzer = "standard",
                    Operator = Operator.And
                };
            }

            query &= multiMatch;
            return query;
        }

        protected virtual List<QueryContainer> GetExistingFilters<T>(ISearchCriteria criteria, string field)
            where T : class
        {
            var existingFilters = new List<QueryContainer>();

            foreach (var f in criteria.CurrentFilters)
            {
                if (!f.Key.Equals(field, StringComparison.OrdinalIgnoreCase))
                {
                    var q = ElasticQueryHelper.CreateQuery<T>(criteria, f);
                    existingFilters.Add(q);
                }
            }

            return existingFilters;
        }

        #endregion
    }
}
