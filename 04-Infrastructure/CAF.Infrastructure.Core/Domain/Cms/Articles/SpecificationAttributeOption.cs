using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Domain.Localization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents a specification attribute option
    /// </summary>
	[DataContract]
    public partial class SpecificationAttributeOption : BaseEntity, ILocalizedEntity
    {
        private ICollection<ArticleSpecificationAttribute> _articleSpecificationAttributes;
        private ICollection<ChannelSpecificationAttribute> _channelSpecificationAttributes;
        private ICollection<ArticleCategorySpecificationAttribute> _articleCategorySpecificationAttributes;
        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        [DataMember]
        public int SpecificationAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
		[DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
		[DataMember]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute
        /// </summary>
        [DataMember]
        public virtual SpecificationAttribute SpecificationAttribute { get; set; }

        /// <summary>
        /// Gets or sets the article specification attribute
        /// </summary>
		[DataMember]
        public virtual ICollection<ArticleSpecificationAttribute> ArticleSpecificationAttributes
        {
            get { return _articleSpecificationAttributes ?? (_articleSpecificationAttributes = new HashSet<ArticleSpecificationAttribute>()); }
            protected set { _articleSpecificationAttributes = value; }
        }

        /// <summary>
        /// Gets or sets the article specification attribute
        /// </summary>
        [DataMember]
        public virtual ICollection<ChannelSpecificationAttribute> ChannelSpecificationAttributes
        {
            get { return _channelSpecificationAttributes ?? (_channelSpecificationAttributes = new HashSet<ChannelSpecificationAttribute>()); }
            protected set { _channelSpecificationAttributes = value; }
        }

        /// <summary>
        /// Gets or sets the article specification attribute
        /// </summary>
        [DataMember]
        public virtual ICollection<ArticleCategorySpecificationAttribute> ArticleCategorySpecificationAttributes
        {
            get { return _articleCategorySpecificationAttributes ?? (_articleCategorySpecificationAttributes = new HashSet<ArticleCategorySpecificationAttribute>()); }
            protected set { _articleCategorySpecificationAttributes = value; }
        }

    }
}
