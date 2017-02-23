using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ProductAttributeOptionMap : EntityTypeConfiguration<ProductAttributeOption>
    {
        public ProductAttributeOptionMap()
        {
            this.ToTable("ProductAttributeOption");
            this.HasKey(sao => sao.Id);
            this.Property(sao => sao.Name).IsRequired();
            this.Property(pvav => pvav.ColorSquaresRgb).HasMaxLength(100);

            this.HasRequired(sao => sao.ProductAttribute)
                .WithMany(sa => sa.ProductAttributeOptions)
                .HasForeignKey(sao => sao.ProductAttributeId);
        }
    }
}