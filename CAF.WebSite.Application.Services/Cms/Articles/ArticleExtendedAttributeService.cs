
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core.Caching;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Generic attribute service
    /// </summary>
    public partial class ArticleExtendedAttributeService : IArticleExtendedAttributeService
    {
        #region Constants
        
        private const string GENERICATTRIBUTE_KEY = "cafsite.articleattribute.{0}-{1}";
        private const string GENERICATTRIBUTE_PATTERN_KEY = "cafsite.articleattribute.";
        #endregion

        #region Fields

        private readonly IRepository<ArticleExtendedAttribute> _articleAttributeRepository;
        private readonly IRequestCache _requestCache;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="articleAttributeRepository">Generic attribute repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ArticleExtendedAttributeService(IRequestCache requestCache,
            IRepository<ArticleExtendedAttribute> articleAttributeRepository,
            IEventPublisher eventPublisher)
        {
            this._requestCache = requestCache;
            this._articleAttributeRepository = articleAttributeRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Deletes an attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        public virtual void DeleteAttribute(ArticleExtendedAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            _articleAttributeRepository.Delete(attribute);

            //cache
            _requestCache.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(attribute);
        }

        /// <summary>
        /// Gets an attribute
        /// </summary>
        /// <param name="attributeId">Attribute identifier</param>
        /// <returns>An attribute</returns>
        public virtual ArticleExtendedAttribute GetAttributeById(int attributeId)
        {
            if (attributeId == 0)
                return null;

            var attribute = _articleAttributeRepository.GetById(attributeId);
            return attribute;
        }

        /// <summary>
        /// Inserts an attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        public virtual void InsertAttribute(ArticleExtendedAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            _articleAttributeRepository.Insert(attribute);
            
            //cache
            _requestCache.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(attribute);
        }

        /// <summary>
        /// Updates the attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        public virtual void UpdateAttribute(ArticleExtendedAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            _articleAttributeRepository.Update(attribute);

            //cache
            _requestCache.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(attribute);
        }

        /// <summary>
        /// Get attributes
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="keyGroup">Key group</param>
        /// <returns>Get attributes</returns>
		public virtual IList<ArticleExtendedAttribute> GetAttributesForEntity(int entityId, string keyGroup)
        {
            string key = string.Format(GENERICATTRIBUTE_KEY, entityId, keyGroup);
            return _requestCache.Get(key, () =>
            {
                var query = from ga in _articleAttributeRepository.Table
                            where ga.EntityId == entityId &&
                            ga.KeyGroup == keyGroup
                            select ga;
                var attributes = query.ToList();
                return attributes;
            });
        }

        /// <summary>
        /// Save attribute value
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
		/// <param name="siteId">Site identifier; pass 0 if this attribute will be available for all stores</param>
		public virtual void SaveAttribute<TPropType>(BaseEntity entity, string key, TPropType value, int siteId = 0)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            string keyGroup = entity.GetUnproxiedEntityType().Name;

			var props = GetAttributesForEntity(entity.Id, keyGroup)
				 .ToList();
            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            string valueStr = value.Convert<string>();

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                {
                    //delete
                    DeleteAttribute(prop);
                }
                else
                {
                    //update
                    prop.Value = valueStr;
                    UpdateAttribute(prop);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(valueStr))
                {
                    //insert
                    prop = new ArticleExtendedAttribute()
                    {
                        EntityId = entity.Id,
                        Key = key,
                        KeyGroup = keyGroup,
                        Value = valueStr
                    };
                    InsertAttribute(prop);
                }
            }
        }

        #endregion
    }
}