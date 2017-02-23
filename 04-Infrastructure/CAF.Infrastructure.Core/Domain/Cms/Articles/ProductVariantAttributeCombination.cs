using CAF.Infrastructure.Core.Domain.Directory;
using System;
using System.Linq;
using System.Runtime.Serialization;
namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents a product variant attribute combination
    /// </summary>
    [DataContract]
	public partial class ProductVariantAttributeCombination : BaseEntity
    {
        public ProductVariantAttributeCombination()
        {
            this.IsActive = true;
        }
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
		[DataMember]
		public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
		[DataMember]
		public string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
		[DataMember]
		public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow orders when out of stock
        /// </summary>
		[DataMember]
		public bool AllowOutOfStockOrders { get; set; }

        [DataMember]
        public string Sku { get; set; }

        [DataMember]
        public string Gtin { get; set; }

        [DataMember]
        public string ManufacturerPartNumber { get; set; }

        [DataMember]
        public decimal? Price { get; set; }

        [DataMember]
        public decimal? Length { get; set; }

        [DataMember]
        public decimal? Width { get; set; }

        [DataMember]
        public decimal? Height { get; set; }

        [DataMember]
        public decimal? BasePriceAmount { get; set; }

        [DataMember]
        public int? BasePriceBaseAmount { get; set; }

        [DataMember]
        public string AssignedPictureIds { get; set; }

        [DataMember]
        public int? DeliveryTimeId { get; set; }

        [DataMember]
        public virtual DeliveryTime DeliveryTime { get; set; }

        [DataMember]
        public int? QuantityUnitId { get; set; }

        [DataMember]
        public virtual QuantityUnit QuantityUnit { get; set; }

        [DataMember]
        public bool IsActive { get; set; }
        //public bool IsDefaultCombination { get; set; }

        public int[] GetAssignedPictureIds()
        {
            if (string.IsNullOrEmpty(this.AssignedPictureIds))
            {
                return new int[] { };
            }

            var query = from id in this.AssignedPictureIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        let idx = id.ToInt()
                        where idx > 0
                        select idx;

            return query.Distinct().ToArray();
        }

        public void SetAssignedPictureIds(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                this.AssignedPictureIds = null;
            }
            else
            {
                this.AssignedPictureIds = String.Join<int>(",", ids);
            }
        }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Article Article { get; set; }
    }
}
