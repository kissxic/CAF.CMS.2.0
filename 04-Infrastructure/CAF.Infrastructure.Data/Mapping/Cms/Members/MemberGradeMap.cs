
using CAF.Infrastructure.Core.Domain.Cms.Members;
using CAF.Infrastructure.Core.Domain.Users;
using System.Data.Entity.ModelConfiguration;


namespace CAF.Infrastructure.Data.Mapping.Members
{
    public partial class MemberGradeMap : EntityTypeConfiguration<MemberGrade>
    {
        public MemberGradeMap()
        {
            this.ToTable("MemberGrade");
            this.HasKey(cr => cr.Id);
            this.Property(cr => cr.SystemName).IsRequired().HasMaxLength(255);
            this.Property(cr => cr.GradeName).HasMaxLength(255);
            this.Property(cr => cr.Comment).HasMaxLength(500);
        }
    }
}