using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ProductVariantAttributeCombinationMap : EntityTypeConfiguration<ProductVariantAttributeCombination>
    {
        public ProductVariantAttributeCombinationMap()
        {
            this.ToTable("ProductVariantAttributeCombination");
            this.HasKey(pvac => pvac.Id);

            this.Property(pvac => pvac.Sku).HasMaxLength(400);
            this.Property(pvac => pvac.ManufacturerPartNumber).HasMaxLength(400);
            this.Property(pvac => pvac.Gtin).HasMaxLength(400);
			this.Property(pvac => pvac.Price).HasPrecision(18, 4);
            this.Property(pvac => pvac.AssignedPictureIds).HasMaxLength(1000);
            this.Property(pvac => pvac.Length).HasPrecision(18, 4);
            this.Property(pvac => pvac.Width).HasPrecision(18, 4);
            this.Property(pvac => pvac.Height).HasPrecision(18, 4);
            this.Property(pvac => pvac.BasePriceAmount).HasPrecision(18, 4);

            this.HasRequired(pvac => pvac.Article)
                .WithMany(pvac => pvac.ProductVariantAttributeCombinations)
                .HasForeignKey(pvac => pvac.ArticleId);

            this.HasOptional(pvac => pvac.DeliveryTime)
                .WithMany()
                .HasForeignKey(pvac => pvac.DeliveryTimeId)
                .WillCascadeOnDelete(false);
            this.HasOptional(pvac => pvac.QuantityUnit)
                .WithMany()
                .HasForeignKey(pvac => pvac.QuantityUnitId)
                .WillCascadeOnDelete(false);

        }
    }
}