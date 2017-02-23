using CAF.Infrastructure.Core.Domain.Localization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents a specification attribute
    /// </summary>
	[DataContract]
	public partial class SpecificationAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<SpecificationAttributeOption> _specificationAttributeOptions;

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
        /// Gets or sets the specification attribute options
        /// </summary>
		[DataMember]
		public virtual ICollection<SpecificationAttributeOption> SpecificationAttributeOptions
        {
			get { return _specificationAttributeOptions ?? (_specificationAttributeOptions = new HashSet<SpecificationAttributeOption>()); }
            protected set { _specificationAttributeOptions = value; }
        }
    }
}
