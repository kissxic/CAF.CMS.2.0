using CAF.Infrastructure.SearchModule.Core.Catalogs.Search;
using System;
using System.Linq;
using Omu.ValueInjecter;
using CAF.Infrastructure.SearchModule.Model.Search;
using searchModel = CAF.WebSite.Application.Searchs.Models.Searchs;
namespace CAF.WebSite.Application.Searchs.Converters
{
    public static class AggregationConverters
    {
        public static Aggregation ToModuleModel(this FacetGroup facetGroup, params string[] appliedFilters)
        {
            var result = new Aggregation
            {
                AggregationType = facetGroup.FacetType,
                Field = facetGroup.FieldName,
                Items = facetGroup.Facets.Select(f => f.ToModuleModel(appliedFilters)).ToArray()
            };

            if (facetGroup.Labels != null)
            {
                result.Labels = facetGroup.Labels.Select(ToModuleModel).ToArray();
            }

            return result;
        }

        public static AggregationItem ToModuleModel(this Facet facet, params string[] appliedFilters)
        {
            var result = new AggregationItem
            {
                Value = facet.Key,
                Count = facet.Count,
                IsApplied = appliedFilters.Any(x => x.Equals(facet.Key, StringComparison.OrdinalIgnoreCase))
            };

            if (facet.Labels != null)
            {
                result.Labels = facet.Labels.Select(ToModuleModel).ToArray();
            }

            return result;
        }

        public static AggregationLabel ToModuleModel(this FacetLabel label)
        {
            return new AggregationLabel
            {
                Language = label.Language,
                Label = label.Label,
            };
        }

        public static searchModel.Aggregation ToWebModel(this Aggregation aggregation, string currentLanguage)
        {
            var result = new searchModel.Aggregation();
            result.InjectFrom<NullableAndEnumValueInjecter>(aggregation);

            if (aggregation.Items != null)
            {
                result.Items = aggregation.Items
                    .Select(i => i.ToWebModel(currentLanguage))
                    .ToArray();
            }

            if (aggregation.Labels != null)
            {
                result.Label =
                    aggregation.Labels.Where(l => string.Equals(l.Language, currentLanguage, StringComparison.OrdinalIgnoreCase))
                        .Select(l => l.Label)
                        .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(result.Label))
            {
                result.Label = aggregation.Field;
            }

            return result;
        }

        public static searchModel.AggregationItem ToWebModel(this AggregationItem item, string currentLanguage)
        {
            var result = new searchModel.AggregationItem();
            result.InjectFrom<NullableAndEnumValueInjecter>(item);

            if (item.Labels != null)
            {
                result.Label =
                    item.Labels.Where(l => string.Equals(l.Language, currentLanguage, StringComparison.OrdinalIgnoreCase))
                        .Select(l => l.Label)
                        .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(result.Label) && item.Value != null)
            {
                result.Label = item.Value.ToString();
            }

            return result;
        }
    }
}
