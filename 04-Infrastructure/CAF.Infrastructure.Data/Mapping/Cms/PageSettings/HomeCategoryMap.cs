using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.PageSettings
{
    public partial class HomeCategoryMap : EntityTypeConfiguration<HomeCategory>
    {
        public HomeCategoryMap()
        {
            this.ToTable("HomeCategory");
            this.HasKey(sa => sa.Id);
            this.Property(sa => sa.Name).IsRequired();
            this.Property(a => a.RequiredVendorIds).HasMaxLength(100);
            this.Property(a => a.RequiredManufacturerIds).HasMaxLength(100);
        }
    }
}