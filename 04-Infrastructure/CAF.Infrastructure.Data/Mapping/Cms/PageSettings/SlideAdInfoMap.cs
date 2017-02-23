
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.PageSettings
{
    public class SlideAdInfoMap : EntityTypeConfiguration<SlideAdInfo>
    {
        public SlideAdInfoMap()
        {
            this.ToTable("SlideAdInfo");
            this.HasKey(x => x.Id);
            this.Property(a => a.Description).HasMaxLength(200);
            this.Property(a => a.Url).HasMaxLength(100);

            this.Ignore(p => p.SlideAdType);
        }
    }
}
