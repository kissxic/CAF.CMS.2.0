
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.PageSettings
{
    public class FloorSlideDetailsInfoMap : EntityTypeConfiguration<FloorSlideDetailsInfo>
    {
        public FloorSlideDetailsInfoMap()
        {
            this.ToTable("FloorSlideDetailsInfo");
            this.HasKey(x => x.Id);
            this.Property(a => a.Url).HasMaxLength(100);

            this.HasRequired(sao => sao.FloorSlidesInfo)
              .WithMany(sa => sa.FloorSlideDetailsInfos)
              .HasForeignKey(sao => sao.FloorSlideId);

            this.HasOptional(sao => sao.Article)
           .WithMany()
           .HasForeignKey(sao => sao.ArticleId);


            this.HasOptional(p => p.Picture)
                .WithMany()
                .HasForeignKey(p => p.PictureId)
                .WillCascadeOnDelete(false);
        }
    }
}
