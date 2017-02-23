using CAF.Infrastructure.Core.Domain.Directory;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping
{
    public partial class DistrictMap : EntityTypeConfiguration<District>
    {
        public DistrictMap()
        {
            this.ToTable("District");
            this.HasKey(sp => sp.Id);
            this.Property(sp => sp.Name).IsRequired().HasMaxLength(100);
            this.Property(sp => sp.Code).IsRequired().HasMaxLength(100);
            this.Property(sp => sp.CityCode).IsRequired().HasMaxLength(10);

            this.HasRequired(sp => sp.City)
                .WithMany(c => c.Districts)
                .HasForeignKey(sp => sp.CityId);
        }
    }
}