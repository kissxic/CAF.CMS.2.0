using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ChannelSpecificationAttributeMap : EntityTypeConfiguration<ChannelSpecificationAttribute>
    {
        public ChannelSpecificationAttributeMap()
        {
            this.ToTable("Channel_SpecificationAttribute_Mapping");
            this.HasKey(psa => psa.Id);

            this.HasRequired(psa => psa.SpecificationAttributeOption)
                .WithMany(sao => sao.ChannelSpecificationAttributes)
                .HasForeignKey(psa => psa.SpecificationAttributeOptionId);


            this.HasRequired(psa => psa.Channel)
                .WithMany(p => p.ChannelSpecificationAttributes)
                .HasForeignKey(psa => psa.ChannelId);
        }
    }
}