using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.SearchModule.Model.Filters;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Common;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CAF.WebSite.Application.Searchs.Services
{
    public class FilterService : IBrowseFilterService
    {
        private readonly ISiteService _siteService;
        private ISearchFilter[] _filters;

        public FilterService(ISiteService siteService)
        {
            _siteService = siteService;
        }

        public ISearchFilter[] GetFilters(IDictionary<string, object> context)
        {
            if (_filters != null)
            {
                return _filters;
            }

            var filters = new List<ISearchFilter>();

            var site = GetObjectValue(context, "Site") as Site;
            if (site != null)
            {
                var browsing = GetFilteredBrowsing(site);
                if (browsing != null)
                {
                    if (browsing.Attributes != null)
                    {
                        filters.AddRange(browsing.Attributes);
                    }

                    if (browsing.AttributeRanges != null)
                    {
                        filters.AddRange(browsing.AttributeRanges);
                    }

                    if (browsing.Prices != null)
                    {
                        filters.AddRange(browsing.Prices);
                    }
                }
            }

            _filters = filters.ToArray();
            return _filters;
        }


        private static object GetObjectValue(IDictionary<string, object> context, string key)
        {
            object result = null;

            if (context.ContainsKey(key))
            {
                var value = context[key];

                if (value != null)
                {
                    result = value;
                }
            }

            return result;
        }

        private static FilteredBrowsing GetFilteredBrowsing(Site site)
        {
            FilteredBrowsing result = null;

            var filterSettingValue = site.GetAttribute<string>("FilteredBrowsing");

            if (!string.IsNullOrEmpty(filterSettingValue))
            {
                var reader = new StringReader(filterSettingValue);
                var serializer = new XmlSerializer(typeof(FilteredBrowsing));
                result = serializer.Deserialize(reader) as FilteredBrowsing;
            }

            return result;
        }
    }
}
