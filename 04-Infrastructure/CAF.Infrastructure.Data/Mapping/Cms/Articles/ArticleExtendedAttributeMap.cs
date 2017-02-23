
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleExtendedAttributeMap : EntityTypeConfiguration<ArticleExtendedAttribute>
    {
        public ArticleExtendedAttributeMap()
        {
            this.ToTable("ArticleExtendedAttribute");
            this.HasKey(ga => ga.Id);

            this.Property(ga => ga.KeyGroup).IsRequired().HasMaxLength(400);
            this.Property(ga => ga.Key).IsRequired().HasMaxLength(400);
            this.Property(ga => ga.Value).IsRequired().IsMaxLength();
        }
    }
}