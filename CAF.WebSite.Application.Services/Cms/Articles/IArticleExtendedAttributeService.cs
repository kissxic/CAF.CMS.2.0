
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Generic attribute service interface
    /// </summary>
    public partial interface IArticleExtendedAttributeService
    {
        /// <summary>
        /// Deletes an attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        void DeleteAttribute(ArticleExtendedAttribute attribute);

        /// <summary>
        /// Gets an attribute
        /// </summary>
        /// <param name="attributeId">Attribute identifier</param>
        /// <returns>An attribute</returns>
        ArticleExtendedAttribute GetAttributeById(int attributeId);

        /// <summary>
        /// Inserts an attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        void InsertAttribute(ArticleExtendedAttribute attribute);

        /// <summary>
        /// Updates the attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        void UpdateAttribute(ArticleExtendedAttribute attribute);

        /// <summary>
        /// Get attributes
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="keyGroup">Key group</param>
        /// <returns>Get attributes</returns>
		IList<ArticleExtendedAttribute> GetAttributesForEntity(int entityId, string keyGroup);
        
        /// <summary>
        /// Save attribute value
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
		/// <param name="siteId">Site identifier; pass 0 if this attribute will be available for all stores</param>
		void SaveAttribute<TPropType>(BaseEntity entity, string key, TPropType value, int siteId = 0);
    }
}