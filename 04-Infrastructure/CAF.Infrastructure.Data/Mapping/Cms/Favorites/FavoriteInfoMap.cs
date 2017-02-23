using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Favorites;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Favorites
{
    public class FavoriteInfoMap : EntityTypeConfiguration<FavoriteInfo>
    {
        public FavoriteInfoMap()
        {
            this.ToTable("FavoriteInfo");
            this.HasKey(t => t.Id);

            //用户=》收藏夹  一对多
            this.HasRequired(bc => bc.User)
              .WithMany(bp => bp.FavoriteInfos)
            .HasForeignKey(bc => bc.UserId);

            //收藏夹=》文档 一对一
            //this.HasOptional(p => p.Article)
            //.WithMany()
            //.HasForeignKey(p => p.ArticleId);

            ///收藏夹=》文档 一对一
            ///  this.HasOptional<Article>(c => c.Article);
        }
    }
}
