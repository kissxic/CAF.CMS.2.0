

using CAF.WebSite.PageSettings.Domain;
using System.Data.Entity.ModelConfiguration;

namespace CAF.WebSite.PageSettings.Data
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
