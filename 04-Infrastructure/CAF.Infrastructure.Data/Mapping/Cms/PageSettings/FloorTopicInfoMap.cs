
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.PageSettings
{
    public class FloorTopicInfoMap : EntityTypeConfiguration<FloorTopicInfo>
    {
        public FloorTopicInfoMap()
        {
            this.ToTable("FloorTopicInfo");
            this.HasKey(x => x.Id);
            this.Property(a => a.TopicName).HasMaxLength(100);
            this.Property(a => a.Url).HasMaxLength(100);

            this.Ignore(p => p.TopicType);

            this.HasRequired(sao => sao.HomeFloorInfo)
              .WithMany(sa => sa.FloorTopicInfos)
              .HasForeignKey(sao => sao.FloorId);


            this.HasOptional(p => p.Picture)
                .WithMany()
                .HasForeignKey(p => p.PictureId)
                .WillCascadeOnDelete(false);
        }
    }
}
