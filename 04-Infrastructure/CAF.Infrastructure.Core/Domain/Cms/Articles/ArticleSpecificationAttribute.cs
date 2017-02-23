using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Œƒµµ Ù–‘
    /// </summary>
	[DataContract]
	public partial class ArticleSpecificationAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the Article identifier
        /// </summary>
		[DataMember]
		public int ArticleId { get; set; }

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
        /// Gets or sets the Article
        /// </summary>
        public virtual Article Article { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option
        /// </summary>
        public virtual SpecificationAttributeOption SpecificationAttributeOption { get; set; }
    }
}
