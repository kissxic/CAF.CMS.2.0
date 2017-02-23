

using CAF.Infrastructure.Core.Domain.Localization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Directory
{
    /// <summary>
    /// Represents a state/province
    /// </summary>
	[DataContract]
    public partial class City : BaseEntity, ILocalizedEntity
    {
        private ICollection<District> _districts;
        /// <summary>
        /// Gets or sets the  Province identifier
        /// </summary>
		[DataMember]
        public int ProvinceId { get; set; }
        /// <summary>
        /// Gets or sets the  Province Code
        /// </summary>
        [DataMember]
        public string ProvinceCode { get; set; }
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
        public virtual StateProvince StateProvince { get; set; }

        /// <summary>
        /// Gets or sets the state/provinces
        /// </summary>
        public virtual ICollection<District> Districts
        {
            get { return _districts ?? (_districts = new HashSet<District>()); }
            protected set { _districts = value; }
        }

    }

}
