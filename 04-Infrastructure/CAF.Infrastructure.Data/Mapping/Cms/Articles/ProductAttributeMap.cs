using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ProductAttributeMap : EntityTypeConfiguration<ProductAttribute>
    {
        public ProductAttributeMap()
        {
            this.ToTable("ProductAttribute");
            this.HasKey(pa => pa.Id);
            this.Property(pa => pa.Alias).HasMaxLength(100);
            this.Property(pa => pa.Name).IsRequired();
        }
    }
}