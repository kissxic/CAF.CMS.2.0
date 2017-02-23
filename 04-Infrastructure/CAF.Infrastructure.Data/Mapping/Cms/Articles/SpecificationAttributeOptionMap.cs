using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class SpecificationAttributeOptionMap : EntityTypeConfiguration<SpecificationAttributeOption>
    {
        public SpecificationAttributeOptionMap()
        {
            this.ToTable("SpecificationAttributeOption");
            this.HasKey(sao => sao.Id);
            this.Property(sao => sao.Name).IsRequired();
            
            this.HasRequired(sao => sao.SpecificationAttribute)
                .WithMany(sa => sa.SpecificationAttributeOptions)
                .HasForeignKey(sao => sao.SpecificationAttributeId);
        }
    }
}