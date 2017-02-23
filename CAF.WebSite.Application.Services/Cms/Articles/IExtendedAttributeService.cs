using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Extended attribute service
    /// </summary>
    public partial interface IExtendedAttributeService
    {
        #region Extended attributes

        /// <summary>
        /// Deletes a extended attribute
        /// </summary>
        /// <param name="ExtendedAttribute">Extended attribute</param>
        void DeleteExtendedAttribute(ExtendedAttribute ExtendedAttribute);

        /// <summary>
        /// Gets all extended attributes
        /// </summary>
        /// <returns>Extended attribute collection</returns>
        IList<ExtendedAttribute> GetAllExtendedAttributes(bool showHidden = false);
        /// <summary>
        /// Gets all extended attributes
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="showHidden"></param>
        /// <returns>Extended attribute collection</returns>
        IList<ExtendedAttribute> GetAllExtendedAttributes(int channelId, bool showHidden = false);
        /// <summary>
        /// Gets a extended attribute 
        /// </summary>
        /// <param name="ExtendedAttributeId">Extended attribute identifier</param>
        /// <returns>Extended attribute</returns>
        ExtendedAttribute GetExtendedAttributeById(int ExtendedAttributeId);

        /// <summary>
        /// Inserts a extended attribute
        /// </summary>
        /// <param name="ExtendedAttribute">Extended attribute</param>
        void InsertExtendedAttribute(ExtendedAttribute ExtendedAttribute);

        /// <summary>
        /// Updates the extended attribute
        /// </summary>
        /// <param name="ExtendedAttribute">Extended attribute</param>
        void UpdateExtendedAttribute(ExtendedAttribute ExtendedAttribute);

        /// <summary>
        /// Gets ExtendedAttribute tag by name
        /// </summary>
        /// <param name="name">ExtendedAttribute   name</param>
        /// <returns>ExtendedAttribute  </returns>
        ExtendedAttribute GetExtendedAttributeByName(string name);

        #endregion

        #region Extended variant attribute values

        /// <summary>
        /// Deletes a extended attribute value
        /// </summary>
        /// <param name="ExtendedAttributeValue">Extended attribute value</param>
        void DeleteExtendedAttributeValue(ExtendedAttributeValue ExtendedAttributeValue);

        /// <summary>
        /// Gets extended attribute values by extended attribute identifier
        /// </summary>
        /// <param name="ExtendedAttributeId">The extended attribute identifier</param>
        /// <returns>Extended attribute value collection</returns>
        IList<ExtendedAttributeValue> GetExtendedAttributeValues(int ExtendedAttributeId);

        /// <summary>
        /// Gets a extended attribute value
        /// </summary>
        /// <param name="ExtendedAttributeValueId">Extended attribute value identifier</param>
        /// <returns>Extended attribute value</returns>
        ExtendedAttributeValue GetExtendedAttributeValueById(int ExtendedAttributeValueId);

        /// <summary>
        /// Inserts a extended attribute value
        /// </summary>
        /// <param name="ExtendedAttributeValue">Extended attribute value</param>
        void InsertExtendedAttributeValue(ExtendedAttributeValue ExtendedAttributeValue);

        /// <summary>
        /// Updates the extended attribute value
        /// </summary>
        /// <param name="ExtendedAttributeValue">Extended attribute value</param>
        void UpdateExtendedAttributeValue(ExtendedAttributeValue ExtendedAttributeValue);

        #endregion
    }
}
