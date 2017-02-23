using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleSpecificationAttributeMap : EntityTypeConfiguration<ArticleSpecificationAttribute>
    {
        public ArticleSpecificationAttributeMap()
        {
            this.ToTable("Article_SpecificationAttribute_Mapping");
            this.HasKey(psa => psa.Id);

            this.HasRequired(psa => psa.SpecificationAttributeOption)
                .WithMany(sao => sao.ArticleSpecificationAttributes)
                .HasForeignKey(psa => psa.SpecificationAttributeOptionId);


            this.HasRequired(psa => psa.Article)
                .WithMany(p => p.ArticleSpecificationAttributes)
                .HasForeignKey(psa => psa.ArticleId);
        }
    }
}