using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Application.Services.Filter;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services;
using CAF.Infrastructure.SearchModule.Model.Indexing;
using CAF.Infrastructure.SearchModule.Model;
using CAF.Infrastructure.SearchModule.Core.Model;

namespace CAF.WebSite.Application.Searchs.Services
{
    public class CatalogItemIndexBuilder : ISearchIndexBuilder
    {
        private const int _partitionSizeCount = 100; // the maximum partition size, keep it smaller to prevent too big of the sql requests and too large messages in the queue
        public static string ShortcutSpecAttribute { get { return "_SpecId"; } }
        private readonly ISearchProvider _searchProvider;
        private readonly IProductCategoryService _catalogSearchService;
        private readonly IArticleService _articleService;
        private readonly IChannelService _channelService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ICommonServices _services;
        public CatalogItemIndexBuilder(ISearchProvider searchProvider,
            IProductCategoryService catalogSearchService,
            IArticleService articleService,
            IChannelService channelService,
            ISpecificationAttributeService specificationAttributeService,
            ILocalizedEntityService localizedEntityService,
            ICommonServices services)
        {
            _searchProvider = searchProvider;
            _catalogSearchService = catalogSearchService;
            _articleService = articleService;
            _channelService = channelService;
            _specificationAttributeService = specificationAttributeService;
            _localizedEntityService = localizedEntityService;
            _services = services;
        }

        #region ISearchIndexBuilder Members

        public string DocumentType
        {
            get
            {
                return CatalogItemSearchCriteria.DocType;
            }
        }

        public IEnumerable<Partition> GetPartitions(bool rebuild, DateTime startDate, DateTime endDate)
        {
            var partitions = (rebuild || startDate == DateTime.MinValue)
                ? GetPartitionsForAllProducts()
                : GetPartitionsForModifiedProducts(startDate, endDate);

            return partitions;
        }

        public IEnumerable<IDocument> CreateDocuments(Partition partition)
        {
            if (partition == null)
                throw new ArgumentNullException("partition");

            var documents = new ConcurrentBag<IDocument>();

          
            partition.Keys.Each(key => {
                //Trace.TraceInformation(string.Format("Processing documents starting {0} of {1} - {2}%", partition.Start, partition.Total, (partition.Start * 100 / partition.Total)));
                if (key != null)
                {
                    var doc = new ResultDocument();
                    IndexItem(doc, key);
                    documents.Add(doc);
                }
            });
            //  var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 5 };
            //Parallel.ForEach(partition.Keys, parallelOptions, key =>
            //{
            //    //Trace.TraceInformation(string.Format("Processing documents starting {0} of {1} - {2}%", partition.Start, partition.Total, (partition.Start * 100 / partition.Total)));
            //    if (key != null)
            //    {
            //        var doc = new ResultDocument();
            //        IndexItem(doc, key);
            //        documents.Add(doc);
            //    }
            //});

            return documents;
        }

        public void PublishDocuments(string scope, IDocument[] documents)
        {
            foreach (var doc in documents)
            {
                _searchProvider.Index(scope, DocumentType, doc);
            }

            _searchProvider.Commit(scope);
            _searchProvider.Close(scope, DocumentType);
        }

        public void RemoveDocuments(string scope, string[] documents)
        {
            foreach (var doc in documents)
            {
                _searchProvider.Remove(scope, DocumentType, "__key", doc);
            }
            _searchProvider.Commit(scope);
        }

        public void RemoveAll(string scope)
        {
            _searchProvider.RemoveAll(scope, DocumentType);
        }

        #endregion

        protected virtual void IndexItem(ResultDocument doc, string productId)
        {

            var item = _articleService.GetArticleById(productId.ToInt());
            if (item == null)
                return;

            var indexStoreNotAnalyzed = new[] { IndexStore.Yes, IndexType.NotAnalyzed };
            var indexStoreNotAnalyzedStringCollection = new[] { IndexStore.Yes, IndexType.NotAnalyzed, IndexDataType.StringCollection };
            var indexStoreAnalyzedStringCollection = new[] { IndexStore.Yes, IndexType.Analyzed, IndexDataType.StringCollection };

            doc.Add(new DocumentField("__key", item.Id.ToString().ToLower(), indexStoreNotAnalyzed));
            doc.Add(new DocumentField("__type", item.GetType().Name, indexStoreNotAnalyzed));
            doc.Add(new DocumentField("__sort", item.Title, indexStoreNotAnalyzed));
            doc.Add(new DocumentField("__hidden", (item.StatusFormat != StatusFormat.Norma).ToString().ToLower(), indexStoreNotAnalyzed));
            doc.Add(new DocumentField("code", item.Sku, indexStoreNotAnalyzed));
            doc.Add(new DocumentField("name", item.Title, indexStoreNotAnalyzed));
            doc.Add(new DocumentField("startdate", item.EndDateUtc.HasValue ? item.EndDateUtc : DateTime.MaxValue, indexStoreNotAnalyzed));
            doc.Add(new DocumentField("enddate", item.EndDateUtc.HasValue ? item.EndDateUtc : DateTime.MaxValue, indexStoreNotAnalyzed));
            doc.Add(new DocumentField("createddate", item.CreatedOnUtc, indexStoreNotAnalyzed));
            doc.Add(new DocumentField("lastmodifieddate", item.ModifiedOnUtc ?? DateTime.MaxValue, indexStoreNotAnalyzed));
            // doc.Add(new DocumentField("priority", item.Priority, indexStoreNotAnalyzed));
            doc.Add(new DocumentField("vendor", item.VendorId, indexStoreNotAnalyzed));

            // Add priority in virtual categories to search index
            //foreach (var link in item.Links)
            //{
            //    doc.Add(new DocumentField(string.Format(CultureInfo.InvariantCulture, "priority_{0}_{1}", link.CatalogId, link.CategoryId), link.Priority, indexStoreNotAnalyzed));
            //}

            // Add catalogs to search index
            //var catalogs = item.Outlines
            //    .Select(o => o.Items.First().Id)
            //    .Distinct(StringComparer.OrdinalIgnoreCase)
            //    .ToArray();
            if (item.ProductCategoryId.HasValue)
                doc.Add(new DocumentField("catalog", item.ProductCategoryId.ToString().ToLower(), indexStoreNotAnalyzedStringCollection));


            // Add outlines to search index
            //var outlineStrings = GetOutlineStrings(item.Outlines);
            //foreach (var outline in outlineStrings)
            //{
            //    doc.Add(new DocumentField("__outline", outline.ToLower(), indexStoreNotAnalyzedStringCollection));
            //}

            // Index custom properties
            IndexItemCustomProperties(doc, item);

            //if (item.Variations != null)
            //{
            //    if (item.Variations.Any(c => c.ProductType == "Physical"))
            //    {
            //        doc.Add(new DocumentField("producttype", "Physical", new[] { IndexStore.Yes, IndexType.NotAnalyzed, IndexDataType.StringCollection }));
            //    }

            //    if (item.Variations.Any(c => c.ProductType == "Digital"))
            //    {
            //        doc.Add(new DocumentField("producttype", "Digital", new[] { IndexStore.Yes, IndexType.NotAnalyzed, IndexDataType.StringCollection }));
            //    }

            //    foreach (var variation in item.Variations)
            //    {
            //        //  IndexItemCustomProperties(doc, variation);
            //    }
            //}

            // Index item prices
            //  IndexItemPrices(doc, item);


            // add to content
            doc.Add(new DocumentField("__content", item.Title, indexStoreAnalyzedStringCollection));
            doc.Add(new DocumentField("__content", item.Sku, indexStoreAnalyzedStringCollection));
        }



        protected virtual void IndexItemCustomProperties(ResultDocument doc, Article article)
        {
            var query = _specificationAttributeService.GetArticleSpecificationAttributesByArticleId(article.Id, null, true);


            List<FilterCriteria> criterias = null;
            var languageId = _services.WorkContext.WorkingLanguage.Id;

            var attributes =
             from p in query
             where p.AllowFiltering
             select p.SpecificationAttributeOption;
            if (attributes.Count() == 0) return;

            var grouped =
                from a in attributes
                group a by new { a.SpecificationAttributeId, a.Id } into g
                select new FilterCriteria
                {
                    Name = g.FirstOrDefault().SpecificationAttribute.Name,
                    Value = g.FirstOrDefault().Name,
                    ID = g.Key.Id,
                    PId = g.FirstOrDefault().SpecificationAttribute.Id,
                    //  MatchCount = g.Count(),
                    DisplayOrder = g.FirstOrDefault().SpecificationAttribute.DisplayOrder,
                    DisplayOrderValues = g.FirstOrDefault().DisplayOrder
                };
            criterias = grouped
                  .OrderBy(a => a.DisplayOrder)
                  .ThenBy(a => a.DisplayOrderValues)
                  .ToList();
            //多语言化处理
            criterias.ForEach(c =>
            {
                c.Entity = ShortcutSpecAttribute;
                c.IsInactive = true;

                if (c.PId.HasValue)
                    c.NameLocalized = _localizedEntityService.GetLocalizedValue(languageId, c.PId.Value, "SpecificationAttribute", "Name");

                if (c.ID.HasValue)
                    c.ValueLocalized = _localizedEntityService.GetLocalizedValue(languageId, c.ID.Value, "SpecificationAttributeOption", "Name");
            });


            foreach (var propValue in criterias.Where(x => x.Value != null))
            {
                var propertyName = propValue.Name.ToLower();
                var contentField = string.Concat("__content", propValue.PId.HasValue ? "_" + propValue.PId.ToString().ToLower() : string.Empty);


                var stringValue = propValue.Value.ToString();

                if (!string.IsNullOrWhiteSpace(stringValue)) // don't index empty values
                {
                    doc.Add(new DocumentField(contentField, stringValue.ToLower(), new[] { IndexStore.Yes, IndexType.Analyzed, IndexDataType.StringCollection }));
                }


                //switch (propValue.ValueType)
                //{
                //    case PropertyValueType.Boolean:
                //    case PropertyValueType.DateTime:
                //    case PropertyValueType.Number:
                //        doc.Add(new DocumentField(propertyName, propValue.Value, new[] { IndexStore.Yes, IndexType.Analyzed }));
                //        break;
                //    case PropertyValueType.LongText:
                // doc.Add(new DocumentField(propertyName, propValue.Value.ToString().ToLowerInvariant(), new[] { IndexStore.Yes, IndexType.Analyzed }));
                //    break;
                //case PropertyValueType.ShortText: // do not tokenize small values as they will be used for lookups and filters
                doc.Add(new DocumentField(propertyName, propValue.Value.ToString(), new[] { IndexStore.Yes, IndexType.NotAnalyzed }));
                //    break;

            }
        }



        /// <summary>
        /// 获取所有产品
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Partition> GetPartitionsForAllProducts()
        {
            var partitions = new List<Partition>();

            var searchContext = new ArticleSearchContext
            {
                OrderBy = ArticleSortingEnum.Position,
                ShowHidden = true,
                PageSize = _partitionSizeCount,
            };

            var articles = _articleService.SearchArticles(searchContext);
            for (var start = 0; start < articles.TotalPages; start += _partitionSizeCount)
            {
                searchContext.PageIndex = start;
                searchContext.PageSize = _partitionSizeCount;

                // TODO: Need optimize search to return only product ids
                articles = _articleService.SearchArticles(searchContext);

                var productIds = articles.Select(p => p.Id.ToString()).ToArray();
                partitions.Add(new Partition(OperationType.Index, productIds));
            }
            return partitions;
        }

        private IEnumerable<Partition> GetPartitionsForModifiedProducts(DateTime startDate, DateTime endDate)
        {
            var partitions = new List<Partition>();

            //var productChanges = GetProductChanges(startDate, endDate);
            //var deletedProductIds = productChanges.Where(c => c.OperationType == EntryState.Deleted).Select(c => c.ObjectId).ToList();
            //var modifiedProductIds = productChanges.Where(c => c.OperationType != EntryState.Deleted).Select(c => c.ObjectId).ToList();

            //partitions.AddRange(CreatePartitions(OperationType.Remove, deletedProductIds));
            //partitions.AddRange(CreatePartitions(OperationType.Index, modifiedProductIds));

            return partitions;
        }

        //private List<OperationLog> GetProductChanges(DateTime startDate, DateTime endDate)
        //{
        //    var allProductChanges = _changeLogService.FindChangeHistory("Item", startDate, endDate).ToList();
        //    var allPriceChanges = _changeLogService.FindChangeHistory("Price", startDate, endDate).ToList();

        //    var priceIds = allPriceChanges.Select(c => c.ObjectId).ToArray();
        //    var prices = GetPrices(priceIds);

        //    // TODO: How to get product for deleted price?
        //    var productsWithChangedPrice = allPriceChanges
        //        .Select(c => new { c.ModifiedDate, Price = prices.ContainsKey(c.ObjectId) ? prices[c.ObjectId] : null })
        //        .Where(x => x.Price != null)
        //        .Select(x => new OperationLog { ObjectId = x.Price.ProductId, ModifiedDate = x.ModifiedDate, OperationType = EntryState.Modified })
        //        .ToList();

        //    allProductChanges.AddRange(productsWithChangedPrice);

        //    // Return latest operation type for each product
        //    var result = allProductChanges
        //        .GroupBy(c => c.ObjectId)
        //        .Select(g => new OperationLog { ObjectId = g.Key, OperationType = g.OrderByDescending(c => c.ModifiedDate).Select(c => c.OperationType).First() })
        //        .ToList();

        //    return result;
        //}


    }
}
