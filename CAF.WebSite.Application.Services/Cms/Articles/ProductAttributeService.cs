using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Caching;
using CAF.Infrastructure.Core.Collections;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.WebSite.Application.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Articles
{

    public partial class ProductAttributeService : IProductAttributeService
    {
        private const string PRODUCTATTRIBUTES_ALL_KEY = "CAF.WebSite.productattribute.all";
        private const string PRODUCTATTRIBUTES_BY_ID_KEY = "CAF.WebSite.productattribute.id-{0}";
        private const string PRODUCTATTRIBUTES_PATTERN_KEY = "CAF.WebSite.productattribute.";



        private const string PRODUCTVARIANTATTRIBUTES_ALL_KEY = "CAF.WebSite.productvariantattribute.all-{0}";
        private const string PRODUCTVARIANTATTRIBUTES_BY_ID_KEY = "CAF.WebSite.productvariantattribute.id-{0}";
        // 0 = ArticleId, 1 = PageIndex, 2 = PageSize
        private const string PRODUCTVARIANTATTRIBUTES_COMBINATIONS_BY_ID_KEY = "CAF.WebSite.productvariantattribute.combinations.id-{0}-{1}-{2}";
        private const string PRODUCTVARIANTATTRIBUTES_PATTERN_KEY = "CAF.WebSite.productvariantattribute.";

        private const string PRODUCTVARIANTATTRIBUTEVALUES_ALL_KEY = "CAF.WebSite.productvariantattributevalue.all-{0}";
        private const string PRODUCTVARIANTATTRIBUTEVALUES_BY_ID_KEY = "CAF.WebSite.productvariantattributevalue.id-{0}";
        private const string PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY = "CAF.WebSite.productvariantattributevalue.";

        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeOption> _productAttributeOptionRepository;
        private readonly IRepository<ProductVariantAttribute> _productVariantAttributeRepository;
        private readonly IRepository<ProductVariantAttributeCombination> _pvacRepository;
        private readonly IRepository<ProductVariantAttributeValue> _productVariantAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestCache _requestCache;
        private readonly IPictureService _pictureService;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="productAttributeRepository">Product attribute repository</param>
        /// <param name="productVariantAttributeRepository">Product variant attribute mapping repository</param>
        /// <param name="pvacRepository">Product variant attribute combination repository</param>
        /// <param name="productVariantAttributeValueRepository">Product variant attribute value repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ProductAttributeService(IRequestCache requestCache,
            IRepository<ProductAttribute> productAttributeRepository,
                  IRepository<ProductAttributeOption> productAttributeOptionRepository,
            IRepository<ProductVariantAttribute> productVariantAttributeRepository,
            IRepository<ProductVariantAttributeCombination> pvacRepository,
            IRepository<ProductVariantAttributeValue> productVariantAttributeValueRepository,
            IEventPublisher eventPublisher,
            IPictureService pictureService)
        {
            _requestCache = requestCache;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeOptionRepository = productAttributeOptionRepository;
            _productVariantAttributeRepository = productVariantAttributeRepository;
            _pvacRepository = pvacRepository;
            _productVariantAttributeValueRepository = productVariantAttributeValueRepository;
            _eventPublisher = eventPublisher;
            _pictureService = pictureService;
        }

        #region Utilities

        private IList<ProductVariantAttribute> GetSwitchedLoadedAttributeMappings(ICollection<int> productVariantAttributeIds)
        {
            if (productVariantAttributeIds != null && productVariantAttributeIds.Count > 0)
            {
                if (productVariantAttributeIds.Count == 1)
                {
                    var pva = GetProductVariantAttributeById(productVariantAttributeIds.ElementAt(0));
                    if (pva != null)
                    {
                        return new List<ProductVariantAttribute> { pva };
                    }
                }
                else
                {
                    return _productVariantAttributeRepository.GetMany(productVariantAttributeIds).ToList();
                }
            }

            return new List<ProductVariantAttribute>();
        }

        #endregion

        #region Methods

        #region Product attributes

        public virtual void DeleteProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Delete(productAttribute);

            //cache
            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productAttribute);
        }

        public virtual IList<ProductAttribute> GetAllProductAttributes()
        {
            string key = PRODUCTATTRIBUTES_ALL_KEY;
            return _requestCache.Get(key, () =>
            {
                var query = from pa in _productAttributeRepository.Table
                            orderby pa.Name
                            select pa;
                var productAttributes = query.ToList();
                return productAttributes;
            });
        }

        public virtual ProductAttribute GetProductAttributeById(int productAttributeId)
        {
            if (productAttributeId == 0)
                return null;

            string key = string.Format(PRODUCTATTRIBUTES_BY_ID_KEY, productAttributeId);
            return _requestCache.Get(key, () =>
            {
                return _productAttributeRepository.GetById(productAttributeId);
            });
        }

        public virtual void InsertProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Insert(productAttribute);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productAttribute);
        }

        public virtual void UpdateProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Update(productAttribute);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productAttribute);
        }

        #endregion

        #region Product attribute option

        /// <summary>
        /// Gets a product attribute option
        /// </summary>
        /// <param name="productAttributeOptionId">The product attribute option identifier</param>
        /// <returns>Product attribute option</returns>
        public virtual ProductAttributeOption GetProductAttributeOptionById(int productAttributeOptionId)
        {
            if (productAttributeOptionId == 0)
                return null;

            return _productAttributeOptionRepository.GetById(productAttributeOptionId);
        }

        /// <summary>
        /// Gets a product attribute option by product attribute id
        /// </summary>
        /// <param name="productAttributeId">The product attribute identifier</param>
        /// <returns>Product attribute option</returns>
        public virtual IList<ProductAttributeOption> GetProductAttributeOptionsByProductAttribute(int productAttributeId)
        {
            var query = from sao in _productAttributeOptionRepository.Table
                        orderby sao.DisplayOrder
                        where sao.ProductAttributeId == productAttributeId
                        select sao;
            var productAttributeOptions = query.ToList();
            return productAttributeOptions;
        }

        /// <summary>
        /// Deletes a product attribute option
        /// </summary>
        /// <param name="productAttributeOption">The product attribute option</param>
        public virtual void DeleteProductAttributeOption(ProductAttributeOption productAttributeOption)
        {
            if (productAttributeOption == null)
                throw new ArgumentNullException("productAttributeOption");

            _productAttributeOptionRepository.Delete(productAttributeOption);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productAttributeOption);
        }

        /// <summary>
        /// Inserts a product attribute option
        /// </summary>
        /// <param name="productAttributeOption">The product attribute option</param>
        public virtual void InsertProductAttributeOption(ProductAttributeOption productAttributeOption)
        {
            if (productAttributeOption == null)
                throw new ArgumentNullException("productAttributeOption");

            _productAttributeOptionRepository.Insert(productAttributeOption);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productAttributeOption);
        }

        /// <summary>
        /// Updates the product attribute
        /// </summary>
        /// <param name="productAttributeOption">The product attribute option</param>
        public virtual void UpdateProductAttributeOption(ProductAttributeOption productAttributeOption)
        {
            if (productAttributeOption == null)
                throw new ArgumentNullException("productAttributeOption");

            _productAttributeOptionRepository.Update(productAttributeOption);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productAttributeOption);
        }

        #endregion

        #region Product variant attributes mappings (ProductVariantAttribute)

        public virtual void DeleteProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            _productVariantAttributeRepository.Delete(productVariantAttribute);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productVariantAttribute);
        }

        public virtual IList<ProductVariantAttribute> GetProductVariantAttributesByArticleId(int articleId)
        {
            string key = string.Format(PRODUCTVARIANTATTRIBUTES_ALL_KEY, articleId);

            return _requestCache.Get(key, () =>
            {
                var query = from pva in _productVariantAttributeRepository.Table
                            orderby pva.DisplayOrder
                            where pva.ArticleId == articleId
                            select pva;
                var productVariantAttributes = query.ToList();
                return productVariantAttributes;
            });
        }

        public virtual Multimap<int, ProductVariantAttribute> GetProductVariantAttributesByArticleIds(int[] articleIds, AttributeControlType? controlType)
        {
            Guard.ArgumentNotNull(() => articleIds);

            var query =
                from pva in _productVariantAttributeRepository.TableUntracked.Expand(x => x.ProductAttribute).Expand(x => x.ProductVariantAttributeValues)
                where articleIds.Contains(pva.ArticleId)
                select pva;

            if (controlType.HasValue)
            {
                query = query.Where(x => x.AttributeControlTypeId == ((int)controlType.Value));
            }

            var map = query
                .OrderBy(x => x.ArticleId)
                .ThenBy(x => x.DisplayOrder)
                .ToList()
                .ToMultimap(x => x.ArticleId, x => x);

            return map;
        }

        public virtual ProductVariantAttribute GetProductVariantAttributeById(int productVariantAttributeId)
        {
            if (productVariantAttributeId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTATTRIBUTES_BY_ID_KEY, productVariantAttributeId);

            return _requestCache.Get(key, () =>
            {
                return _productVariantAttributeRepository.GetById(productVariantAttributeId);
            });
        }

        public virtual IList<ProductVariantAttribute> GetProductVariantAttributesByIds(IEnumerable<int> productVariantAttributeIds, IEnumerable<ProductVariantAttribute> attributes = null)
        {
            if (productVariantAttributeIds != null)
            {
                if (attributes != null)
                {
                    var ids = new List<int>();
                    var result = new List<ProductVariantAttribute>();

                    foreach (var id in productVariantAttributeIds)
                    {
                        var pva = attributes.FirstOrDefault(x => x.Id == id);
                        if (pva == null)
                            ids.Add(id);
                        else
                            result.Add(pva);
                    }

                    var newLoadedMappings = GetSwitchedLoadedAttributeMappings(ids);

                    result.AddRange(newLoadedMappings);

                    return result;
                }

                return GetSwitchedLoadedAttributeMappings(productVariantAttributeIds.ToList());
            }

            return new List<ProductVariantAttribute>();
        }

        public virtual IEnumerable<ProductVariantAttributeValue> GetProductVariantAttributeValuesByIds(params int[] productVariantAttributeValueIds)
        {
            if (productVariantAttributeValueIds == null || productVariantAttributeValueIds.Length == 0)
            {
                return Enumerable.Empty<ProductVariantAttributeValue>();
            }

            return _productVariantAttributeValueRepository.GetMany(productVariantAttributeValueIds);
        }

        public virtual void InsertProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            _productVariantAttributeRepository.Insert(productVariantAttribute);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productVariantAttribute);
        }

        public virtual void InsertProductVariantAttribute(Article article, List<ProductVariantAttribute> productVariantAttributes)
        {
            // delete all existing combinations
            _productVariantAttributeRepository.DeleteAll(x => x.ArticleId == article.Id, true);


            using (var scope = new DbContextScope(ctx: _pvacRepository.Context, autoCommit: false, autoDetectChanges: false, validateOnSave: false, hooksEnabled: false))
            {
                foreach (var values in productVariantAttributes)
                {
                    _productVariantAttributeRepository.Insert(values);

                }
                scope.Commit();
            }


        }


        public virtual void UpdateProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            _productVariantAttributeRepository.Update(productVariantAttribute);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productVariantAttribute);
        }

        #endregion

        #region Product variant attribute values (ProductVariantAttributeValue)

        public virtual void DeleteProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            _productVariantAttributeValueRepository.Delete(productVariantAttributeValue);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productVariantAttributeValue);
        }

        public virtual IList<ProductVariantAttributeValue> GetProductVariantAttributeValues(int productVariantAttributeId)
        {
            string key = string.Format(PRODUCTVARIANTATTRIBUTEVALUES_ALL_KEY, productVariantAttributeId);
            return _requestCache.Get(key, () =>
            {
                var query = from pvav in _productVariantAttributeValueRepository.Table
                            orderby pvav.DisplayOrder
                            where pvav.ProductVariantAttributeId == productVariantAttributeId
                            select pvav;
                var productVariantAttributeValues = query.ToList();
                return productVariantAttributeValues;
            });
        }

        public virtual ProductVariantAttributeValue GetProductVariantAttributeValueById(int productVariantAttributeValueId)
        {
            if (productVariantAttributeValueId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTATTRIBUTEVALUES_BY_ID_KEY, productVariantAttributeValueId);
            return _requestCache.Get(key, () =>
            {
                return _productVariantAttributeValueRepository.GetById(productVariantAttributeValueId);
            });
        }

        public virtual void InsertProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            _productVariantAttributeValueRepository.Insert(productVariantAttributeValue);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productVariantAttributeValue);
        }

        public virtual void UpdateProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            _productVariantAttributeValueRepository.Update(productVariantAttributeValue);

            _requestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _requestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productVariantAttributeValue);
        }

        #endregion

        #region Product variant attribute combinations (ProductVariantAttributeCombination)

        private void CombineAll(List<List<ProductVariantAttributeValue>> toCombine, List<List<ProductVariantAttributeValue>> result, int y, List<ProductVariantAttributeValue> tmp)
        {
            var combine = toCombine[y];

            for (int i = 0; i < combine.Count; ++i)
            {
                var lst = new List<ProductVariantAttributeValue>(tmp);
                lst.Add(combine[i]);

                if (y == (toCombine.Count - 1))
                    result.Add(lst);
                else
                    CombineAll(toCombine, result, y + 1, lst);
            }
        }

        public virtual void DeleteProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _pvacRepository.Delete(combination);

            //event notification
            _eventPublisher.EntityDeleted(combination);
        }

        public virtual IPagedList<ProductVariantAttributeCombination> GetAllProductVariantAttributeCombinations(
            int articleId,
            int pageIndex,
            int pageSize,
            bool untracked = true)
        {
            if (articleId == 0)
            {
                return new PagedList<ProductVariantAttributeCombination>(new List<ProductVariantAttributeCombination>(), pageIndex, pageSize);
            }

            string key = string.Format(PRODUCTVARIANTATTRIBUTES_COMBINATIONS_BY_ID_KEY, articleId, 0, int.MaxValue);
            return _requestCache.Get(key, () =>
            {
                var query = from pvac in (untracked ? _pvacRepository.TableUntracked : _pvacRepository.Table)
                            orderby pvac.Id
                            where pvac.ArticleId == articleId
                            select pvac;

                var combinations = new PagedList<ProductVariantAttributeCombination>(query, pageIndex, pageSize);
                return combinations;
            });
        }
        public virtual IList<ProductVariantAttributeCombination> GetAllProductVariantAttributeCombinations(
          int articleId,
          bool untracked = true)
        {
            if (articleId == 0)
            {
                return new List<ProductVariantAttributeCombination>(new List<ProductVariantAttributeCombination>());
            }

            string key = string.Format(PRODUCTVARIANTATTRIBUTES_COMBINATIONS_BY_ID_KEY, articleId, 0, int.MaxValue);
            return _requestCache.Get(key, () =>
            {
                var query = from pvac in (untracked ? _pvacRepository.TableUntracked : _pvacRepository.Table)
                            orderby pvac.Id
                            where pvac.ArticleId == articleId
                            select pvac;

                var combinations = query.ToList();
                return combinations;
            });
        }

        public virtual IList<int> GetAllProductVariantAttributeCombinationPictureIds(int articleId)
        {
            var pictureIds = new List<int>();

            if (articleId == 0)
                return pictureIds;

            var query = from pvac in _pvacRepository.TableUntracked
                        where
                            pvac.ArticleId == articleId
                            && pvac.IsActive
                            && !String.IsNullOrEmpty(pvac.AssignedPictureIds)
                        select pvac.AssignedPictureIds;

            var data = query.ToList();
            if (data.Any())
            {
                int id;
                var ids = string.Join(",", data).SplitSafe(",").Distinct();

                foreach (string str in ids)
                {
                    if (int.TryParse(str, out id) && !pictureIds.Exists(i => i == id))
                        pictureIds.Add(id);
                }
            }

            return pictureIds;
        }

        public virtual Multimap<int, ProductVariantAttributeCombination> GetProductVariantAttributeCombinations(int[] articleIds)
        {
            Guard.ArgumentNotNull(() => articleIds);

            var query =
                from pvac in _pvacRepository.TableUntracked
                where articleIds.Contains(pvac.ArticleId)
                select pvac;

            var map = query
                .OrderBy(x => x.ArticleId)
                .ToList()
                .ToMultimap(x => x.ArticleId, x => x);

            return map;
        }

        public virtual decimal? GetLowestCombinationPrice(int articleId)
        {
            if (articleId == 0)
                return null;

            var query =
                from pvac in _pvacRepository.Table
                where pvac.ArticleId == articleId && pvac.Price != null && pvac.IsActive
                orderby pvac.Price ascending
                select pvac.Price;

            var price = query.FirstOrDefault();
            return price;
        }

        public virtual ProductVariantAttributeCombination GetProductVariantAttributeCombinationById(int productVariantAttributeCombinationId)
        {
            if (productVariantAttributeCombinationId == 0)
                return null;

            var combination = _pvacRepository.GetById(productVariantAttributeCombinationId);
            return combination;
        }

        public virtual ProductVariantAttributeCombination GetProductVariantAttributeCombinationBySku(string sku)
        {
            if (sku.IsEmpty())
                return null;

            var combination = _pvacRepository.Table.FirstOrDefault(x => x.Sku == sku);
            return combination;
        }

        public virtual void InsertProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            //if (combination.IsDefaultCombination)
            //{
            //	EnsureSingleDefaultVariant(combination);
            //}

            _pvacRepository.Insert(combination);

            //event notification
            _eventPublisher.EntityInserted(combination);
        }

        public virtual void UpdateProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            //if (combination.IsDefaultCombination)
            //{
            //	EnsureSingleDefaultVariant(combination);
            //}
            //else
            //{
            //	// check if it was default before modification...
            //	// but make it Type-Safe (resistant to code refactoring ;-))
            //	Expression<Func<ProductVariantAttributeCombination, bool>> expr = x => x.IsDefaultCombination;
            //	string propertyToCheck = expr.ExtractPropertyInfo().Name;

            //	object originalValue = null;
            //	if (_productVariantAttributeCombinationRepository.GetModifiedProperties(combination).TryGetValue(propertyToCheck, out originalValue))
            //	{
            //		bool wasDefault = (bool)originalValue;
            //		if (wasDefault) 
            //		{
            //			// we can't uncheck the default variant within a combination list,
            //			// we would't have a default combination anymore.
            //			combination.IsDefaultCombination = true;
            //		}
            //	}
            //}

            _pvacRepository.Update(combination);

            //event notification
            _eventPublisher.EntityUpdated(combination);
        }

        public virtual void CreateAllProductVariantAttributeCombinations(Article article)
        {
            // delete all existing combinations
            _pvacRepository.DeleteAll(x => x.ArticleId == article.Id,true);

            var attributes = GetProductVariantAttributesByArticleId(article.Id);
            if (attributes == null || attributes.Count <= 0)
                return;

            var toCombine = new List<List<ProductVariantAttributeValue>>();
            var resultMatrix = new List<List<ProductVariantAttributeValue>>();
            var tmp = new List<ProductVariantAttributeValue>();

            foreach (var attr in attributes)
            {
                var attributeValues = attr.ProductVariantAttributeValues.ToList();
                if (attributeValues.Count > 0)
                    toCombine.Add(attributeValues);
            }

            if (toCombine.Count > 0)
            {
                CombineAll(toCombine, resultMatrix, 0, tmp);

                using (var scope = new DbContextScope(ctx: _pvacRepository.Context, autoCommit: false, autoDetectChanges: false, validateOnSave: false, hooksEnabled: false))
                {
                    ProductVariantAttributeCombination combination = null;

                    var idx = 0;
                    foreach (var values in resultMatrix)
                    {
                        idx++;

                        string attrXml = "";
                        for (var i = 0; i < values.Count; ++i)
                        {
                            var value = values[i];
                            attrXml = attributes[i].AddProductAttribute(attrXml, value.Id.ToString());
                        }

                        combination = new ProductVariantAttributeCombination
                        {
                            ArticleId = article.Id,
                            AttributesXml = attrXml,
                            StockQuantity = 10000,
                            AllowOutOfStockOrders = true,
                            IsActive = true
                        };

                        _pvacRepository.Insert(combination);
                    }

                    scope.Commit();

                    if (combination != null)
                    {
                        // Perf: publish event for last one only
                        _eventPublisher.EntityInserted(combination);
                    }
                }

            }

            //foreach (var y in resultMatrix) {
            //	StringBuilder sb = new StringBuilder();
            //	foreach (var x in y) {
            //		sb.AppendFormat("{0} ", x.Name);
            //	}
            //	sb.ToString().Dump();
            //}
        }

        public virtual void CreateAllProductVariantAttributeCombinations(Article article, List<ProductVariantAttributeCombination> productVariantAttributeCombination)
        {
            // delete all existing combinations
 
            _pvacRepository.DeleteAll(x => x.ArticleId == article.Id, true);
            var attributes = GetProductVariantAttributesByArticleId(article.Id);
            if (attributes == null || attributes.Count <= 0)
                return;

            var toCombine = new List<List<ProductVariantAttributeValue>>();
            var resultMatrix = new List<List<ProductVariantAttributeValue>>();
            var tmp = new List<ProductVariantAttributeValue>();

            foreach (var attr in attributes)
            {
                var attributeValues = attr.ProductVariantAttributeValues.ToList();
                if (attributeValues.Count > 0)
                    toCombine.Add(attributeValues);
            }

            if (toCombine.Count > 0)
            {
                CombineAll(toCombine, resultMatrix, 0, tmp);

                using (var scope = new DbContextScope(ctx: _pvacRepository.Context, autoCommit: false, autoDetectChanges: false, validateOnSave: false, hooksEnabled: false))
                {
                    ProductVariantAttributeCombination combination = null;

                    var idx = 0;
                    foreach (var values in resultMatrix)
                    {
                        idx++;

                        string attrXml = "";
                        string tmpId = "";
                        for (var i = 0; i < values.Count; ++i)
                        {
                            var value = values[i];
                            attrXml = attributes[i].AddProductAttribute(attrXml, value.Id.ToString());
                            tmpId = tmpId.IsEmpty() ? value.ProductAttributeOptionId.ToString() : tmpId + "-" + value.ProductAttributeOptionId.ToString();
                        }
                        var pvaItemQuery = productVariantAttributeCombination.Where(p => p.AttributesXml == tmpId);
                        if (!pvaItemQuery.Any())
                            continue;
                        var pvaItem = pvaItemQuery.FirstOrDefault();
                        combination = new ProductVariantAttributeCombination
                        {
                            ArticleId = article.Id,
                            AttributesXml = attrXml,
                            StockQuantity = pvaItem.StockQuantity,
                            Sku = pvaItem.Sku,
                            Price = pvaItem.Price,
                            AssignedPictureIds = pvaItem.AssignedPictureIds,
                            AllowOutOfStockOrders = true,
                            IsActive = true
                        };

                        _pvacRepository.Insert(combination);
                    }

                    scope.Commit();

                    if (combination != null)
                    {
                        // Perf: publish event for last one only
                        _eventPublisher.EntityInserted(combination);
                    }
                }

            }

            //foreach (var y in resultMatrix) {
            //	StringBuilder sb = new StringBuilder();
            //	foreach (var x in y) {
            //		sb.AppendFormat("{0} ", x.Name);
            //	}
            //	sb.ToString().Dump();
            //}
        }

        public virtual bool VariantHasAttributeCombinations(int articleId)
        {
            if (articleId == 0)
                return false;

            var query =
                from c in _pvacRepository.Table
                where c.ArticleId == articleId
                select c;

            return query.Select(x => x.Id).Any();
        }

        #endregion


        #endregion
    }
}
