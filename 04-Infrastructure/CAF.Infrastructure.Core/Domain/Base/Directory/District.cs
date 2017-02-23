
using CAF.Infrastructure.Core.Domain.Localization;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Directory
{
    /// <summary>
    /// Represents a state/province
    /// </summary>
	[DataContract]
    public partial class District : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the City identifier
        /// </summary>
		[DataMember]
        public int CityId { get; set; }
        /// <summary>
        /// Gets or sets the City Code
        /// </summary>
        [DataMember]
        public string CityCode { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [DataMember]
        public string Code { get; set; }
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
        /// Gets or sets the country
        /// </summary>
		[DataMember]
        public virtual City City { get; set; }
    }

}
