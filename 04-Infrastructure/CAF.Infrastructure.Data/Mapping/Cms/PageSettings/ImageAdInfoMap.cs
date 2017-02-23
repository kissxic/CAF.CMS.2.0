
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.PageSettings
{
    public class ImageAdInfoMap : EntityTypeConfiguration<ImageAdInfo>
    {
        public ImageAdInfoMap()
        {
            this.ToTable("ImageAdInfo");
            this.HasKey(x => x.Id);
            this.Property(a => a.Url).HasMaxLength(100);

        }
    }
}
