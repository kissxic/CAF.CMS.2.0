using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleMap : EntityTypeConfiguration<Article>
    {
        public ArticleMap()
        {
            this.ToTable("Article");
            this.HasKey(a => a.Id);
            this.Property(a => a.Title).IsRequired().HasMaxLength(100);
            this.Property(a => a.LinkUrl).HasMaxLength(50);
            this.Property(a => a.ImgUrl).HasMaxLength(50);
            this.Property(a => a.MetaTitle).HasMaxLength(50);
            this.Property(a => a.MetaKeywords).HasMaxLength(255);
            this.Property(a => a.MetaDescription).HasMaxLength(500);
            this.Property(a => a.ShortContent).HasMaxLength(255);
            this.Property(a => a.FullContent).IsMaxLength();
            this.Property(a => a.GroupidsView).HasMaxLength(255);
            this.Property(a => a.Author).HasMaxLength(50);


            #region 商品
            this.Property(p => p.Sku).HasMaxLength(400);
            this.Property(p => p.Price).HasPrecision(18, 4);
            this.Property(p => p.OldPrice).HasPrecision(18, 4);
            this.Property(p => p.ProductCost).HasPrecision(18, 4);
            this.Property(p => p.SpecialPrice).HasPrecision(18, 4);
            this.Property(p => p.MinimumCustomerEnteredPrice).HasPrecision(18, 4);
            this.Property(p => p.MaximumCustomerEnteredPrice).HasPrecision(18, 4);
            this.Property(p => p.Weight).HasPrecision(18, 4);
            this.Property(p => p.Length).HasPrecision(18, 4);
            this.Property(p => p.Width).HasPrecision(18, 4);
            this.Property(p => p.Height).HasPrecision(18, 4);


            this.HasOptional(p => p.DeliveryTime)
                .WithMany()
                .HasForeignKey(p => p.DeliveryTimeId)
                .WillCascadeOnDelete(false);

            this.HasOptional(p => p.QuantityUnit)
                .WithMany()
                .HasForeignKey(p => p.QuantityUnitId)
                .WillCascadeOnDelete(false);

            #endregion

            this.Ignore(p => p.ArticleType);
            this.Ignore(p => p.ArticleTypeLabelHint);
            this.Ignore(a => a.ArticleStatus);
            this.Ignore(u => u.StatusFormat);



            this.HasOptional(p => p.Picture)
            .WithMany()
            .HasForeignKey(p => p.PictureId)
            .WillCascadeOnDelete(false);
            // Relationships
            this.HasOptional(a => a.ArticleCategory)
                .WithMany(t => t.Articles)
                .HasForeignKey(t => t.CategoryId)
                .WillCascadeOnDelete(false);

            this.HasOptional(a => a.ProductCategory)
             .WithMany(t => t.Articles)
             .HasForeignKey(t => t.ProductCategoryId)
             .WillCascadeOnDelete(false);

            this.HasOptional(a => a.Manufacturer)
              .WithMany(t => t.Articles)
              .HasForeignKey(t => t.ManufacturerId)
              .WillCascadeOnDelete(false);

            this.HasMany(p => p.ArticleTags)
            .WithMany(pt => pt.Articles)
            .Map(m => m.ToTable("Article_ArticleTag_Mapping"));

            this.HasOptional(a => a.StateProvince)
                .WithMany()
                .HasForeignKey(a => a.StateProvinceId).WillCascadeOnDelete(false);

            this.HasOptional(a => a.City)
               .WithMany()
               .HasForeignKey(a => a.CityId).WillCascadeOnDelete(false);

            this.HasOptional(a => a.District)
               .WithMany()
               .HasForeignKey(a => a.DistrictId).WillCascadeOnDelete(false);


        }
    }
}
