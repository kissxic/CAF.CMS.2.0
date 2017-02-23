
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
    /// 商品规格属性
    /// </summary>
    [DataContract]
    public partial class ProductAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<ProductAttributeOption> _productAttributeOptions;
        /// <summary>
        /// Gets or sets the product attribute alias 
        /// (an optional key for advanced customization)
        /// </summary>
        [DataMember]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public virtual ICollection<ProductAttributeOption> ProductAttributeOptions
        {
            get { return _productAttributeOptions ?? (_productAttributeOptions = new HashSet<ProductAttributeOption>()); }
            protected set { _productAttributeOptions = value; }
        }
    }
}
