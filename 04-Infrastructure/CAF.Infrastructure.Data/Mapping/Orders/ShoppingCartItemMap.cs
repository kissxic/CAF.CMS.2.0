using CAF.Infrastructure.Core.Domain.Orders;
using System.Data.Entity.ModelConfiguration;
 

namespace CAF.Infrastructure.Data.Mapping.Orders
{
    public partial class ShoppingCartItemMap : EntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            this.ToTable("ShoppingCartItem");
            this.HasKey(sci => sci.Id);

            this.Property(sci => sci.UserEnteredPrice).HasPrecision(18, 4);
			this.Property(sci => sci.AttributesXml).IsMaxLength();

            this.Ignore(sci => sci.ShoppingCartType);
            this.Ignore(sci => sci.IsFreeShipping);
            this.Ignore(sci => sci.IsShipEnabled);

            this.HasRequired(sci => sci.User)
                .WithMany(c => c.ShoppingCartItems)
                .HasForeignKey(sci => sci.UserId);

            this.HasRequired(sci => sci.Article)
                .WithMany()
                .HasForeignKey(sci => sci.ArticleId);
        }
    }
}
