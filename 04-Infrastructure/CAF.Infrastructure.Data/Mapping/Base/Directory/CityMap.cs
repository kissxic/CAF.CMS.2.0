using CAF.Infrastructure.Core.Domain.Directory;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping
{
    public partial class CityMap : EntityTypeConfiguration<City>
    {
        public CityMap()
        {
            this.ToTable("City");
            this.HasKey(sp => sp.Id);
            this.Property(sp => sp.Name).IsRequired().HasMaxLength(100);
            this.Property(sp => sp.Code).IsRequired().HasMaxLength(10);
            this.Property(sp => sp.ProvinceCode).IsRequired().HasMaxLength(10);

            this.HasRequired(sp => sp.StateProvince)
                .WithMany(c => c.Citys)
                .HasForeignKey(sp => sp.ProvinceId);
        }
    }
}