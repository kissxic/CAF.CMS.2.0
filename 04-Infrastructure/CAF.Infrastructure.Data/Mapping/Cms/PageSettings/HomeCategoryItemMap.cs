using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.PageSettings
{
    public partial class HomeCategoryItemMap : EntityTypeConfiguration<HomeCategoryItem>
    {
        public HomeCategoryItemMap()
        {
            this.ToTable("HomeCategoryItem");
            this.HasKey(sao => sao.Id);
            this.Property(a => a.Name).HasMaxLength(100);
            this.Property(a => a.SeName).HasMaxLength(100);

            this.HasRequired(sao => sao.HomeCategory)
                .WithMany(sa => sa.HomeCategoryItems)
                .HasForeignKey(sao => sao.HomeCategoryId);
        }
    }
}