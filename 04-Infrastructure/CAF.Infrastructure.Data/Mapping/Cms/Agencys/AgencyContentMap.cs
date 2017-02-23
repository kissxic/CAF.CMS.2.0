using CAF.Infrastructure.Core.Domain.Cms.Agents;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Agencys
{
    public partial class AgencyContentMap : EntityTypeConfiguration<AgencyContent>
    {
        public AgencyContentMap()
        {
            this.ToTable("AgencyContent");

            this.Property(bc => bc.Message).IsMaxLength();
            this.Property(a => a.TrueName).HasMaxLength(50);
            this.Property(a => a.TelePhone).HasMaxLength(50);
            this.Property(a => a.Mobile).HasMaxLength(50);
            this.Property(a => a.QQ).HasMaxLength(50);
            this.Property(a => a.Email).HasMaxLength(50);
            this.Property(a => a.ProvinceName).HasMaxLength(50);
            this.Property(a => a.CityName).HasMaxLength(50);
            this.Property(a => a.DistributionChannelId).HasMaxLength(100);
            this.Property(a => a.AgentPropertyId).HasMaxLength(100);
            this.Property(a => a.OtherChannel).HasMaxLength(200);

            this.HasRequired(bc => bc.Article)
                .WithMany()
                .HasForeignKey(bc => bc.ArticleId);

            this.Ignore(p => p.ShowAuthType);
        }
    }
}
