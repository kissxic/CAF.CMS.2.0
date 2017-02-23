using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Localization;
using System;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Directory
{
    /// <summary>
    /// Represents a currency
    /// </summary>
	[DataContract]
	public partial class DeliveryTime : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
		[DataMember]
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hex value
        /// </summary>
		[DataMember]
		public string ColorHexValue { get; set; }

        /// <summary>
        /// Gets or sets the display locale
        /// </summary>
		[DataMember]
		public string DisplayLocale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        //public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
		[DataMember]
		public int DisplayOrder { get; set; }
        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        [DataMember]
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
		[DataMember]
        public DateTime UpdatedOnUtc { get; set; }
    }

}
