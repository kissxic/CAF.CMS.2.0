
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.PageSettings
{
    public class HomeFloorInfoMap : EntityTypeConfiguration<HomeFloorInfo>
    {
        public HomeFloorInfoMap()
        {
            this.ToTable("HomeFloorInfo");
            this.HasKey(x => x.Id);
            this.Property(a => a.FloorName).HasMaxLength(100);
            this.Property(a => a.DefaultTabName).HasMaxLength(100);
            this.Property(a => a.SubName).HasMaxLength(100);
            this.Property(a => a.RelateManufacturerIds).HasMaxLength(100);
            this.Property(a => a.RelateCategoryIds).HasMaxLength(100);
            this.Property(a => a.RelateProductIds).HasMaxLength(100);
            this.Property(a => a.RelateVendorIds).HasMaxLength(100);
            this.Property(a => a.Url).HasMaxLength(100);

        }
    }
}
