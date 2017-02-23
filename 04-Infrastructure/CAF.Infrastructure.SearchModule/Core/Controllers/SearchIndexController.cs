using System;
using System.Globalization;
using System.Linq;
using CAF.Infrastructure.Core.Domain.Search;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core;
using System.Collections.Generic;
using CAF.Infrastructure.SearchModule.Model.Indexing;

namespace CAF.Infrastructure.SearchModule.Core.Controllers
{
    public class SearchIndexController : ISearchIndexController
    {
        private readonly ISearchIndexBuilder _searchIndexBuilder;
        private readonly ITypeFinder _typeFinder;
        private readonly SearchSettings _searchSettings;
        private readonly ISettingService _settingService;
        
        public SearchIndexController(ISettingService settingService, SearchSettings searchSettings, ISearchIndexBuilder searchIndexBuilder, ITypeFinder typeFinder)
        {
            _settingService = settingService;
            _searchSettings = searchSettings;
            this._typeFinder = typeFinder;
            _searchIndexBuilder = searchIndexBuilder;
        }

        #region ISearchIndexController

        /// <summary>
        /// Processes the staged indexes.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="documentType"></param>
        /// <param name="rebuild"></param>
        public void Process(string scope, string documentType, bool rebuild)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }

          //  var searchIndexBuilderTypes = _typeFinder.FindClassesOfType<ISearchIndexBuilder>();

            var searchIndexBuilders = new List<ISearchIndexBuilder>();
            //foreach (var searchIndexBuilder in searchIndexBuilderTypes)
            //{
            //    var provider = Activator.CreateInstance(searchIndexBuilder) as ISearchIndexBuilder;
            //    searchIndexBuilders.Add(provider);

            //}
            searchIndexBuilders.Add(_searchIndexBuilder);
            var validBuilders = string.IsNullOrEmpty(documentType) ? searchIndexBuilders : searchIndexBuilders.Where(b => string.Equals(b.DocumentType, documentType, StringComparison.OrdinalIgnoreCase)).ToList();

            var lastBuildTimeName = string.Format(CultureInfo.InvariantCulture, "VirtoCommerce.Search.LastBuildTime_{0}_{1}", scope, documentType);
            var lastBuildTime = _searchSettings.LastBuildTime;
            var nowUtc = DateTime.UtcNow;

            foreach (var indexBuilder in validBuilders)
            {
                if (rebuild)
                {
                    indexBuilder.RemoveAll(scope);

                }

                var startDate = rebuild ? DateTime.MinValue : lastBuildTime;
                var partitions = indexBuilder.GetPartitions(rebuild, startDate, nowUtc);

                foreach (var partition in partitions)
                {
                    if (partition.OperationType == OperationType.Remove)
                    {
                        indexBuilder.RemoveDocuments(scope, partition.Keys);
                    }
                    else
                    {
                        // create index docs
                        var docs = indexBuilder.CreateDocuments(partition);

                        // submit docs to the provider
                        var docsArray = docs.ToArray();
                        indexBuilder.PublishDocuments(scope, docsArray);
                    }
                }
            }
            _searchSettings.LastBuildTime = nowUtc;
            _settingService.SaveSetting(_searchSettings);

        }
        public void Process(string scope, string documentType, string documentId)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }

            if (documentType == null)
            {
                throw new ArgumentNullException("documentType");
            }

            if (documentId == null)
            {
                throw new ArgumentNullException("documentId");
            }

            // var indexBuilder = _indexBuilders.Where(b => string.Equals(b.DocumentType, documentType, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            var indexBuilder = _searchIndexBuilder;
            // remove existing index
            indexBuilder.RemoveDocuments(scope, new[] { documentId });

            // create new index
            var partition = new Partition(OperationType.Index, new[] { documentId });

            // create index docs
            var docs = indexBuilder.CreateDocuments(partition);

            // submit docs to the provider
            var docsArray = docs.ToArray();
            indexBuilder.PublishDocuments(scope, docsArray);
        }
        #endregion
    }
}
