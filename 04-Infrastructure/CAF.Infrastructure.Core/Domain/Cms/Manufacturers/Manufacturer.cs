using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System.Collections.Generic;
using CAF.Infrastructure.Core.Domain.Cms.Articles;

namespace CAF.Infrastructure.Core.Domain.Cms.Manufacturers
{
    /// <summary>
    /// Represents a manufacturer
    /// </summary>
	[DataContract]
	public partial class Manufacturer : AuditedBaseEntity, ISoftDeletable, ILocalizedEntity, ISlugSupported
    {
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

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
		[DataMember]
		public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
		[DataMember]
		public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
		[DataMember]
		public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the parent picture identifier
        /// </summary>
		[DataMember]
		public int? PictureId { get; set; }

		/// <summary>
		/// Gets or sets the picture
		/// </summary>
		[DataMember]
		public virtual Picture Picture { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
		[DataMember]
		public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
		[DataMember]
		public bool AllowUsersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
		[DataMember]
		public string PageSizeOptions { get; set; }
 
 
        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
		[DataMember]
		public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
		[Index]
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
		[DataMember]
		public int DisplayOrder { get; set; }

        /// <summary>
        /// 文章列表
        /// </summary>
        private ICollection<Article> _articles;
        /// <summary>
        /// 文章列表
        /// </summary>
        public virtual ICollection<Article> Articles
        {
            get { return _articles ?? (_articles = new HashSet<Article>()); }
            protected set { _articles = value; }
        }

    }
}
