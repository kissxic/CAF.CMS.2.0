using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;


namespace CAF.Infrastructure.Core.Domain.Orders
{
    /// <summary>
    /// Represents a shopping cart item
    /// </summary>
    public partial class ShoppingCartItem : BaseEntity
    {
		/// <summary>
		/// Gets or sets the store identifier
		/// </summary>
		public int SiteId { get; set; }

		/// <summary>
		/// The parent shopping cart item id
		/// </summary>
		public int? ParentItemId { get; set; }

		/// <summary>
		/// Gets or sets ths bundle item identifier
		/// </summary>
		public int? BundleItemId { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart type identifier
        /// </summary>
        public int ShoppingCartTypeId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
		public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the product variant attributes
        /// </summary>
        public string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the price enter by a customer
        /// </summary>
        public decimal UserEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets the log type
        /// </summary>
        public ShoppingCartType ShoppingCartType
        {
            get
            {
                return (ShoppingCartType)this.ShoppingCartTypeId;
            }
            set
            {
                this.ShoppingCartTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
		public virtual Article Article { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual User User { get; set; }

	 

        /// <summary>
        /// Gets a value indicating whether the shopping cart item is free shipping
        /// </summary>
        public bool IsFreeShipping
        {
            get
            {
				var product = this.Article;
				if (product != null)
					return product.IsFreeShipping;
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the shopping cart item is ship enabled
        /// </summary>
        public bool IsShipEnabled
        {
            get
            {
				var product = this.Article;
				if (product != null)
					return product.IsShipEnabled;
                return false;
            }
        }

    }
}
