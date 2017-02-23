using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Caching;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Constants
        private const string PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY = "SmartStore.articlespecificationattribute.allbyarticleid-{0}-{1}-{2}";
        private const string PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY = "SmartStore.articlespecificationattribute.";
        #endregion

        #region Fields

        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository<ArticleSpecificationAttribute> _articleSpecificationAttributeRepository;
        private readonly IRepository<ChannelSpecificationAttribute> _channelSpecificationAttributeRepository;
        private readonly IRepository<ArticleCategorySpecificationAttribute> _articleCategorySpecificationAttribute;
        private readonly IRequestCache _requestCache;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="specificationAttributeRepository">Specification attribute repository</param>
        /// <param name="specificationAttributeOptionRepository">Specification attribute option repository</param>
        /// <param name="articleSpecificationAttributeRepository">Article specification attribute repository</param>
        /// <param name="eventPublisher">Event published</param>
        public SpecificationAttributeService(IRequestCache requestCache,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<ArticleSpecificationAttribute> articleSpecificationAttributeRepository,
            IRepository<ChannelSpecificationAttribute> channelSpecificationAttributeRepository,
            IRepository<ArticleCategorySpecificationAttribute> articleCategorySpecificationAttribute,
            IEventPublisher eventPublisher)
        {
            _requestCache = requestCache;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _articleSpecificationAttributeRepository = articleSpecificationAttributeRepository;
            _channelSpecificationAttributeRepository = channelSpecificationAttributeRepository;
            this._articleCategorySpecificationAttribute = articleCategorySpecificationAttribute;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public virtual SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            return _specificationAttributeRepository.GetById(specificationAttributeId);
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
		/// <returns>Specification attribute query</returns>
        public virtual IQueryable<SpecificationAttribute> GetSpecificationAttributes()
        {
            var query =
                from sa in _specificationAttributeRepository.Table
                orderby sa.DisplayOrder, sa.Name
                select sa;

            return query;
        }

        /// <summary>
        /// Gets specification attributes by identifier
        /// </summary>
        /// <param name="ids">Identifiers</param>
        /// <returns>Specification attribute query</returns>
        public virtual IQueryable<SpecificationAttribute> GetSpecificationAttributesByIds(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;

            var query =
                from sa in _specificationAttributeRepository.Table
                where ids.Contains(sa.Id)
                orderby sa.DisplayOrder, sa.Name
                select sa;

            return query;
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                return;

            // (delete localized properties of options)
            var options = GetSpecificationAttributeOptionsBySpecificationAttribute(specificationAttribute.Id);
            foreach (var itm in options)
            {
                DeleteSpecificationAttributeOption(itm);
            }

            _specificationAttributeRepository.Delete(specificationAttribute);

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttribute);
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Insert(specificationAttribute);

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttribute);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Update(specificationAttribute);

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttribute);
        }

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return null;

            return _specificationAttributeOptionRepository.GetById(specificationAttributeOptionId);
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRepository.Table
                        orderby sao.DisplayOrder
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.ToList();
            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Delete(specificationAttributeOption);

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttributeOption);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Insert(specificationAttributeOption);

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttributeOption);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Update(specificationAttributeOption);

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttributeOption);
        }

        #endregion

        #region Article specification attribute

        /// <summary>
        /// Deletes a article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute</param>
        public virtual void DeleteArticleSpecificationAttribute(ArticleSpecificationAttribute articleSpecificationAttribute)
        {
            if (articleSpecificationAttribute == null)
                throw new ArgumentNullException("articleSpecificationAttribute");

            _articleSpecificationAttributeRepository.Delete(articleSpecificationAttribute);

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(articleSpecificationAttribute);
        }

        /// <summary>
        /// Gets a article specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article specification attribute mapping collection</returns>
        public virtual IList<ArticleSpecificationAttribute> GetArticleSpecificationAttributesByArticleId(int articleId)
        {
            return GetArticleSpecificationAttributesByArticleId(articleId, null, null);
        }

        /// <summary>
        /// Gets a article specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnArticlePage">0 to load attributes with ShowOnArticlePage set to false, 0 to load attributes with ShowOnArticlePage set to true, null to load all attributes</param>
        /// <returns>Article specification attribute mapping collection</returns>
        public virtual IList<ArticleSpecificationAttribute> GetArticleSpecificationAttributesByArticleId(int articleId,
            bool? allowFiltering, bool? showOnArticlePage)
        {
            string allowFilteringCacheStr = "null";
            if (allowFiltering.HasValue)
                allowFilteringCacheStr = allowFiltering.ToString();
            string showOnArticlePageCacheStr = "null";
            if (showOnArticlePage.HasValue)
                showOnArticlePageCacheStr = showOnArticlePage.ToString();
            string key = string.Format(PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY, articleId, allowFilteringCacheStr, showOnArticlePageCacheStr);

            return _requestCache.Get(key, () =>
            {
                var query = _articleSpecificationAttributeRepository.Table;
                query = query.Where(psa => psa.ArticleId == articleId);
                if (allowFiltering.HasValue)
                    query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
                if (showOnArticlePage.HasValue)
                    query = query.Where(psa => psa.ShowOnArticlePage == showOnArticlePage.Value);
                query = query.OrderBy(psa => psa.DisplayOrder);

                var articleSpecificationAttributes = query.ToList();
                return articleSpecificationAttributes;
            });
        }

        /// <summary>
        /// Gets a article specification attribute mapping 
        /// </summary>
        /// <param name="articleSpecificationAttributeId">Article specification attribute mapping identifier</param>
        /// <returns>Article specification attribute mapping</returns>
        public virtual ArticleSpecificationAttribute GetArticleSpecificationAttributeById(int articleSpecificationAttributeId)
        {
            if (articleSpecificationAttributeId == 0)
                return null;

            var articleSpecificationAttribute = _articleSpecificationAttributeRepository.GetById(articleSpecificationAttributeId);
            return articleSpecificationAttribute;
        }

        /// <summary>
        /// Inserts a article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute mapping</param>
        public virtual void InsertArticleSpecificationAttribute(ArticleSpecificationAttribute articleSpecificationAttribute)
        {
            if (articleSpecificationAttribute == null)
                return;

            _articleSpecificationAttributeRepository.Insert(articleSpecificationAttribute);

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(articleSpecificationAttribute);
        }

        /// <summary>
        /// Inserts a article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute mapping</param>
        public virtual void InsertArticleSpecificationAttribute(int articleId, List<ArticleSpecificationAttribute> articleSpecificationAttributes)
        {
            if (articleSpecificationAttributes.Count <= 0)
                return;

            _articleSpecificationAttributeRepository.DeleteAll(x => x.ArticleId == articleId, true);
            using (var scope = new DbContextScope(ctx: _articleSpecificationAttributeRepository.Context, autoCommit: false, autoDetectChanges: false, validateOnSave: false, hooksEnabled: false))
            {
                foreach (var values in articleSpecificationAttributes)
                {
                    _articleSpecificationAttributeRepository.Insert(values);
                    //event notification
                    _eventPublisher.EntityInserted(values);
                }
                scope.Commit();
            }

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute mapping</param>
        public virtual void UpdateArticleSpecificationAttribute(ArticleSpecificationAttribute articleSpecificationAttribute)
        {
            if (articleSpecificationAttribute == null)
                return;

            _articleSpecificationAttributeRepository.Update(articleSpecificationAttribute);

            _requestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(articleSpecificationAttribute);
        }

        public virtual void UpdateArticleSpecificationMapping(int specificationAttributeId, string field, bool value)
        {
            if (specificationAttributeId == 0 || field.IsEmpty())
                return;

            bool isAllowFiltering = field.IsCaseInsensitiveEqual("AllowFiltering");
            bool isShowOnArticlePage = field.IsCaseInsensitiveEqual("ShowOnArticlePage");

            if (!isAllowFiltering && !isShowOnArticlePage)
                return;

            var optionIds = (
                from sao in _specificationAttributeOptionRepository.Table
                where sao.SpecificationAttributeId == specificationAttributeId
                select sao.Id).ToList();


            foreach (int optionId in optionIds)
            {
                var query =
                    from psa in _articleSpecificationAttributeRepository.Table
                    where psa.SpecificationAttributeOptionId == optionId
                    select psa;

                if (isAllowFiltering)
                {
                    query = query.Where(a => a.AllowFiltering != value);
                }
                else if (isShowOnArticlePage)
                {
                    query = query.Where(a => a.ShowOnArticlePage != value);
                }

                var attributes = query.ToList();

                foreach (var attribute in attributes)
                {
                    if (isAllowFiltering)
                    {
                        attribute.AllowFiltering = value;
                    }
                    else if (isShowOnArticlePage)
                    {
                        attribute.ShowOnArticlePage = value;
                    }

                    UpdateArticleSpecificationAttribute(attribute);
                }
            }
        }

        #endregion

        #region Channel specification attribute

        /// <summary>
        /// Gets a article specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article specification attribute mapping collection</returns>
        public virtual IList<ChannelSpecificationAttribute> GetChannelSpecificationAttributesById(int channelId)
        {
            return GetChannelSpecificationAttributesById(channelId, null, null);
        }

        /// <summary>
        /// Gets a article specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnArticlePage">0 to load attributes with ShowOnArticlePage set to false, 0 to load attributes with ShowOnArticlePage set to true, null to load all attributes</param>
        /// <returns>Article specification attribute mapping collection</returns>
        public virtual IList<ChannelSpecificationAttribute> GetChannelSpecificationAttributesById(int channelId,
            bool? allowFiltering, bool? showOnArticlePage)
        {
            string allowFilteringCacheStr = "null";
            if (allowFiltering.HasValue)
                allowFilteringCacheStr = allowFiltering.ToString();
            string showOnArticlePageCacheStr = "null";
            if (showOnArticlePage.HasValue)
                showOnArticlePageCacheStr = showOnArticlePage.ToString();


            var query = _channelSpecificationAttributeRepository.Table;
            query = query.Where(psa => psa.ChannelId == channelId);
            if (allowFiltering.HasValue)
                query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
            if (showOnArticlePage.HasValue)
                query = query.Where(psa => psa.ShowOnArticlePage == showOnArticlePage.Value);
            query = query.OrderBy(psa => psa.DisplayOrder);

            var channelSpecificationAttributes = query.ToList();
            return channelSpecificationAttributes;

        }
        /// <summary>
        /// Inserts a article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute mapping</param>
        public virtual void InsertChannelSpecificationAttribute(int channelId, List<ChannelSpecificationAttribute> channelSpecificationAttributes)
        {
            if (channelSpecificationAttributes.Count <= 0)
                // throw new ArgumentNullException("channelSpecificationAttributes");
                return;

            _channelSpecificationAttributeRepository.DeleteAll(x => x.ChannelId == channelId, true);

            using (var scope = new DbContextScope(ctx: _channelSpecificationAttributeRepository.Context, autoCommit: false, autoDetectChanges: false, validateOnSave: false, hooksEnabled: false))
            {
                foreach (var values in channelSpecificationAttributes)
                {
                    _channelSpecificationAttributeRepository.Insert(values);
                    //event notification
                    _eventPublisher.EntityInserted(values);
                }
                scope.Commit();
            }

        }
        #endregion

        #region Category specification attribute

        /// <summary>
        /// Gets a article specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article specification attribute mapping collection</returns>
        public virtual IList<ArticleCategorySpecificationAttribute> GetCategorySpecificationAttributesById(int categroyId)
        {
            return GetCategorySpecificationAttributesById(categroyId, null, null);
        }

        /// <summary>
        /// Gets a article specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnArticlePage">0 to load attributes with ShowOnArticlePage set to false, 0 to load attributes with ShowOnArticlePage set to true, null to load all attributes</param>
        /// <returns>Article specification attribute mapping collection</returns>
        public virtual IList<ArticleCategorySpecificationAttribute> GetCategorySpecificationAttributesById(int categroyId,
            bool? allowFiltering, bool? showOnArticlePage)
        {
            string allowFilteringCacheStr = "null";
            if (allowFiltering.HasValue)
                allowFilteringCacheStr = allowFiltering.ToString();
            string showOnArticlePageCacheStr = "null";
            if (showOnArticlePage.HasValue)
                showOnArticlePageCacheStr = showOnArticlePage.ToString();


            var query = _articleCategorySpecificationAttribute.Table;
            query = query.Where(psa => psa.CategoryId == categroyId);
            if (allowFiltering.HasValue)
                query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
            if (showOnArticlePage.HasValue)
                query = query.Where(psa => psa.ShowOnArticlePage == showOnArticlePage.Value);
            query = query.OrderBy(psa => psa.DisplayOrder);

            var categroySpecificationAttributes = query.ToList();
            return categroySpecificationAttributes;

        }
        /// <summary>
        /// Inserts a article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute mapping</param>
        public virtual void InsertCategorySpecificationAttribute(int categroyId, List<ArticleCategorySpecificationAttribute> categroySpecificationAttributes)
        {
            if (categroySpecificationAttributes.Count <= 0)
                // throw new ArgumentNullException("categroySpecificationAttributes");
                return;

            _articleCategorySpecificationAttribute.DeleteAll(x => x.CategoryId == categroyId, true);

            using (var scope = new DbContextScope(ctx: _articleCategorySpecificationAttribute.Context, autoCommit: false, autoDetectChanges: false, validateOnSave: false, hooksEnabled: false))
            {
                foreach (var values in categroySpecificationAttributes)
                {
                    _articleCategorySpecificationAttribute.Insert(values);
                    //event notification
                    _eventPublisher.EntityInserted(values);
                }
                scope.Commit();
            }

        }
        #endregion
       

        #endregion
    }
}
