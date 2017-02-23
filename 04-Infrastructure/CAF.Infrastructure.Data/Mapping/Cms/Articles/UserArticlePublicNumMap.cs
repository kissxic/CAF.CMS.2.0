using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class UserArticlePublicNumMap : EntityTypeConfiguration<UserArticlePublicNum>
    {
        public UserArticlePublicNumMap()
        {
            this.ToTable("UserArticlePublicNum");
            this.HasKey(pp => pp.Id);

            this.HasRequired(pp => pp.User)
                .WithMany()
                .HasForeignKey(pp => pp.UserId);

            this.HasRequired(pp => pp.Channel)
                .WithMany()
                .HasForeignKey(pp => pp.ChannelId);
        }
    }
}
