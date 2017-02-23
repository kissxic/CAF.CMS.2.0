﻿using CAF.Infrastructure.SearchModule.Core.Model;
using CAF.Infrastructure.SearchModule.Model.Search.Criterias;
using CAF.Infrastructure.SearchModule.Providers.ElasticSearch.Nest;
using Nest;


namespace CAF.WebSite.Application.Searchs.Services.Providers.ElasticSearch.Nest
{
    public class CatalogElasticSearchQueryBuilder : ElasticSearchQueryBuilder
    {
        protected override QueryContainer GetQuery<T>(ISearchCriteria criteria)
        {
            var result = base.GetQuery<T>(criteria);

            result &= GetCategoryQuery<T>(criteria as CategorySearchCriteria);
            result &= GetCatalogItemQuery<T>(criteria as CatalogItemSearchCriteria);

            return result;
        }

        protected virtual QueryContainer GetCategoryQuery<T>(CategorySearchCriteria criteria)
            where T : class
        {
            QueryContainer result = null;

            if (criteria != null)
            {
                if (criteria.Outlines != null && criteria.Outlines.Count > 0)
                {
                    result = CreateQuery("__outline", criteria.Outlines);
                }
            }

            return result;
        }

        protected virtual QueryContainer GetCatalogItemQuery<T>(CatalogItemSearchCriteria criteria)
            where T : class
        {
            QueryContainer result = null;

            if (criteria != null)
            {
                result &= new DateRangeQuery { Field = "startdate", LessThanOrEqualTo = criteria.StartDate };

                if (criteria.StartDateFrom.HasValue)
                {
                    result &= new DateRangeQuery { Field = "startdate", GreaterThan = criteria.StartDateFrom.Value };
                }

                if (criteria.EndDate.HasValue)
                {
                    result &= new DateRangeQuery { Field = "enddate", GreaterThan = criteria.EndDate.Value };
                }

                result &= new TermQuery { Field = "__hidden", Value = false };

                if (criteria.Outlines != null && criteria.Outlines.Count > 0)
                {
                    result &= CreateQuery("__outline", criteria.Outlines);
                }

                if (!string.IsNullOrEmpty(criteria.Catalog))
                {
                    result &= CreateQuery("catalog", criteria.Catalog);
                }

                if (criteria.ClassTypes != null && criteria.ClassTypes.Count > 0)
                {
                    result &= CreateQuery("__type", criteria.ClassTypes, false);
                }
            }

            return result;
        }
    }
}
