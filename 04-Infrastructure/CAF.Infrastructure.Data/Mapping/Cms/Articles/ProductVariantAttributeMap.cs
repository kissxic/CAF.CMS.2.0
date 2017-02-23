using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ProductVariantAttributeMap : EntityTypeConfiguration<ProductVariantAttribute>
    {
        public ProductVariantAttributeMap()
        {
			this.ToTable("Product_ProductAttribute_Mapping");
            this.HasKey(pva => pva.Id);
	        this.Ignore(pva => pva.AttributeControlType);

            this.HasRequired(pva => pva.Article)
                .WithMany(pva => pva.ProductVariantAttributes)
                .HasForeignKey(pva => pva.ArticleId);
            
            this.HasRequired(pva => pva.ProductAttribute)
                .WithMany()
                .HasForeignKey(pva => pva.ProductAttributeId);
        }
    }
}