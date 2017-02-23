using CAF.Infrastructure.Core.Domain.Orders;
using System.Data.Entity.ModelConfiguration;
 

namespace CAF.Infrastructure.Data.Mapping.Orders
{
    public partial class OrderItemMap : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemMap()
        {
			this.ToTable("OrderItem");
            this.HasKey(orderItem => orderItem.Id);
            this.Property(orderItem => orderItem.AttributeDescription);
            this.Property(orderItem => orderItem.AttributesXml).IsMaxLength();
			this.Property(orderItem => orderItem.BundleData).IsMaxLength();

            this.Property(orderItem => orderItem.UnitPriceInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.UnitPriceExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.PriceInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.PriceExclTax).HasPrecision(18, 4);
			this.Property(orderItem => orderItem.TaxRate).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.DiscountAmountInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.DiscountAmountExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.ItemWeight).HasPrecision(18, 4);
			this.Property(orderItem => orderItem.ProductCost).HasPrecision(18, 4);

            this.HasRequired(orderItem => orderItem.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId);

            this.HasRequired(orderItem => orderItem.Article)
                .WithMany()
                .HasForeignKey(orderItem => orderItem.ArticleId);
        }
    }
}