using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleCategorySpecificationAttributeMap : EntityTypeConfiguration<ArticleCategorySpecificationAttribute>
    {
        public ArticleCategorySpecificationAttributeMap()
        {
            this.ToTable("ArticleCategorySpecificationAttribute");
            this.HasKey(psa => psa.Id);

            this.HasRequired(psa => psa.SpecificationAttributeOption)
                .WithMany(sao => sao.ArticleCategorySpecificationAttributes)
                .HasForeignKey(psa => psa.SpecificationAttributeOptionId);


            this.HasRequired(psa => psa.ArticleCategory)
                .WithMany(p => p.ArticleCategorySpecificationAttributes)
                .HasForeignKey(psa => psa.CategoryId);
        }
    }
}