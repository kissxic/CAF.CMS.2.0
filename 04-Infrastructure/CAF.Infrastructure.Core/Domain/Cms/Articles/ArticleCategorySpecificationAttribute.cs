using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Channels
{
    /// <summary>
    /// ��������
    /// </summary>
	[DataContract]
	public partial class ArticleCategorySpecificationAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the Channel identifier
        /// </summary>
		[DataMember]
		public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
		[DataMember]
		public int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute can be filtered by
        /// </summary>
		[DataMember]
		public bool AllowFiltering { get; set; }

        /// <summary>
        /// Gets or sets whether the attrbiute will be shown on the Article page
        /// </summary>
		[DataMember]
		public bool ShowOnArticlePage { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
		[DataMember]
		public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets or sets the Channel
        /// </summary>
        public virtual ArticleCategory ArticleCategory { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option
        /// </summary>
        public virtual SpecificationAttributeOption SpecificationAttributeOption { get; set; }
    }
}
