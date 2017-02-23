using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Specification attribute service interface
    /// </summary>
    public partial interface ISpecificationAttributeService
    {
        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId);

        /// <summary>
        /// Gets specification attributes
        /// </summary>
		/// <returns>Specification attribute query</returns>
        IQueryable<SpecificationAttribute> GetSpecificationAttributes();

		/// <summary>
		/// Gets specification attributes by identifier
		/// </summary>
		/// <param name="ids">Identifiers</param>
		/// <returns>Specification attribute query</returns>
		IQueryable<SpecificationAttribute> GetSpecificationAttributesByIds(int[] ids);

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute);

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute);

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute);

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        /// <returns>Specification attribute option</returns>
        SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOption);

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId);

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption);

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption);

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        void UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption);

        #endregion

        #region Article specification attribute

        /// <summary>
        /// Deletes a article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute</param>
        void DeleteArticleSpecificationAttribute(ArticleSpecificationAttribute articleSpecificationAttribute);

        /// <summary>
        /// Gets a article specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article specification attribute mapping collection</returns>
        IList<ArticleSpecificationAttribute> GetArticleSpecificationAttributesByArticleId(int articleId);

        /// <summary>
        /// Gets a article specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnArticlePage">0 to load attributes with ShowOnArticlePage set to false, 0 to load attributes with ShowOnArticlePage set to true, null to load all attributes</param>
        /// <returns>Article specification attribute mapping collection</returns>
        IList<ArticleSpecificationAttribute> GetArticleSpecificationAttributesByArticleId(int articleId,
            bool? allowFiltering, bool? showOnArticlePage);

        /// <summary>
        /// Gets a article specification attribute mapping 
        /// </summary>
        /// <param name="articleSpecificationAttributeId">Article specification attribute mapping identifier</param>
        /// <returns>Article specification attribute mapping</returns>
        ArticleSpecificationAttribute GetArticleSpecificationAttributeById(int articleSpecificationAttributeId);

        /// <summary>
        /// Inserts a article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute mapping</param>
        void InsertArticleSpecificationAttribute(ArticleSpecificationAttribute articleSpecificationAttribute);
        /// <summary>
        /// Inserts a article specification attribute mapping
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="articleSpecificationAttributes"></param>
        void InsertArticleSpecificationAttribute(int articleId, List<ArticleSpecificationAttribute> articleSpecificationAttributes);

        /// <summary>
        /// Updates the article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute mapping</param>
        void UpdateArticleSpecificationAttribute(ArticleSpecificationAttribute articleSpecificationAttribute);

		void UpdateArticleSpecificationMapping(int specificationAttributeId, string field, bool value);

        #endregion

        #region Channel specification attribute

        /// <summary>
        /// Gets a Channel specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article specification attribute mapping collection</returns>
        IList<ChannelSpecificationAttribute> GetChannelSpecificationAttributesById(int channelId);
        /// <summary>
        /// Gets a Channel specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnArticlePage">0 to load attributes with ShowOnArticlePage set to false, 0 to load attributes with ShowOnArticlePage set to true, null to load all attributes</param>
        /// <returns>Article specification attribute mapping collection</returns>
        IList<ChannelSpecificationAttribute> GetChannelSpecificationAttributesById(int channelId,
            bool? allowFiltering, bool? showOnArticlePage);
        /// Inserts a Channel specification attribute mapping
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="articleSpecificationAttributes"></param>
        void InsertChannelSpecificationAttribute(int channelId, List<ChannelSpecificationAttribute> channelSpecificationAttributes);


        #endregion

        #region Category specification attribute

        /// <summary>
        /// Gets a Channel specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article specification attribute mapping collection</returns>
        IList<ArticleCategorySpecificationAttribute> GetCategorySpecificationAttributesById(int categoryId);
        /// <summary>
        /// Gets a Channel specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnArticlePage">0 to load attributes with ShowOnArticlePage set to false, 0 to load attributes with ShowOnArticlePage set to true, null to load all attributes</param>
        /// <returns>Article specification attribute mapping collection</returns>
        IList<ArticleCategorySpecificationAttribute> GetCategorySpecificationAttributesById(int categoryId,
            bool? allowFiltering, bool? showOnArticlePage);
        /// Inserts a Channel specification attribute mapping
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="articleSpecificationAttributes"></param>
        void InsertCategorySpecificationAttribute(int categoryId, List<ArticleCategorySpecificationAttribute> categorySpecificationAttributes);


        #endregion

        
    }
}
