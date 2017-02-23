
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.PageSettings
{
    public class FloorSlidesInfoMap : EntityTypeConfiguration<FloorSlidesInfo>
    {
        public FloorSlidesInfoMap()
        {
            this.ToTable("FloorSlidesInfo");
            this.HasKey(x => x.Id);
            this.Property(a => a.Name).HasMaxLength(100);
           
        }
    }
}
