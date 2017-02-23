using CAF.Infrastructure.Core.Domain.Sellers;
using System.Data.Entity.ModelConfiguration;
namespace CAF.Infrastructure.Data.Cms.Sellers
{
    public partial class VendorMap : EntityTypeConfiguration<Vendor>
    {
        public VendorMap()
        {
            this.ToTable("Vendor");
            this.HasKey(v => v.Id);

            this.Property(v => v.Name).IsRequired().HasMaxLength(400);
            this.Property(v => v.Email).HasMaxLength(400);
            this.Property(v => v.MetaKeywords).HasMaxLength(400);
            this.Property(v => v.MetaTitle).HasMaxLength(400);
            this.Property(v => v.MetaTitle).HasMaxLength(400);
            this.Property(v => v.PageSizeOptions).HasMaxLength(200);
            this.Property(v => v.CompanyName).HasMaxLength(100);
            this.Property(v => v.CompanyAddress).HasMaxLength(400);
            this.Property(v => v.CompanyPhone).HasMaxLength(100);
            this.Property(v => v.ContactsFax).HasMaxLength(100);
            this.Property(v => v.CompanyWebSite).HasMaxLength(100);

            this.Property(v => v.LegalPerson).HasMaxLength(100);
            this.Property(v => v.ContactsName).HasMaxLength(100);
            this.Property(v => v.ContactsPhone).HasMaxLength(100);
            this.Property(v => v.ContactsEmail).HasMaxLength(100);
            this.Property(v => v.BankAccountName).HasMaxLength(100);
            this.Property(v => v.BankAccountNumber).HasMaxLength(100);
            this.Property(v => v.BankName).HasMaxLength(100);
            this.Property(v => v.BankCode).HasMaxLength(100);

            this.Ignore(v => v.VendorStage);
            this.Ignore(v => v.CompanyEmployeeCount);

            this.HasOptional(p => p.Picture)
                .WithMany()
                .HasForeignKey(p => p.PictureId)
                .WillCascadeOnDelete(false);

        }
    }
}