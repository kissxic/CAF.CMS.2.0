using CAF.Infrastructure.Core.Domain.Common;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Common
{
    public partial class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            this.ToTable("Address");
            this.HasKey(a => a.Id);

            this.HasOptional(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId).WillCascadeOnDelete(false);

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
