using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ProductCategoryMap : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
            this.ToTable("ProductCategory");
            this.HasKey(a => a.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(400);
            this.Property(c => c.FullName).HasMaxLength(400);
            this.Property(c => c.Alias).HasMaxLength(100);
            this.Property(a => a.Description).IsMaxLength();
            this.Property(a => a.Path).HasMaxLength(100);
            this.Property(c => c.PageSizeOptions).HasMaxLength(200);
            this.Property(c => c.PriceRanges).HasMaxLength(400);
            // Relationships

            this.HasOptional(p => p.Picture)
                .WithMany()
                .HasForeignKey(p => p.PictureId)
                .WillCascadeOnDelete(false);
        }
    }
}
