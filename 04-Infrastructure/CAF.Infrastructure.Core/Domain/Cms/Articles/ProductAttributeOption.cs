
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Localization;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 商品规格属性选项
    /// </summary>
	[DataContract]
    public partial class ProductAttributeOption : BaseEntity, ILocalizedEntity
    {
        
        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
		[DataMember]
		public int ProductAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
		[DataMember]
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary>
        [DataMember]
        public virtual string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
		[DataMember]
		public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets or sets the specification attribute
        /// </summary>
		[DataMember]
		public virtual ProductAttribute ProductAttribute { get; set; }

      
    }
}
