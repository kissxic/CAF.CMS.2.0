
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.PageSettings
{
    public class BannerInfoMap : EntityTypeConfiguration<BannerInfo>
    {
        public BannerInfoMap()
        {
            this.ToTable("BannerInfo");
            this.HasKey(x => x.Id);
            this.Property(a => a.Name).HasMaxLength(100);
            this.Property(a => a.Url).HasMaxLength(100);

            this.Ignore(p => p.BannerUrltypes);
        }
    }
}
