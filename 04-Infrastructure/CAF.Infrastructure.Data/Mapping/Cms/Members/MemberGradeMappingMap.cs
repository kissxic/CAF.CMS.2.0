using CAF.Infrastructure.Core.Domain.Cms.Members;
using System.Data.Entity.ModelConfiguration;


namespace CAF.Infrastructure.Data.Mapping.Members
{
	public partial class MemberGradeMappingMap : EntityTypeConfiguration<MemberGradeMapping>
	{
        public MemberGradeMappingMap()
		{
            this.ToTable("MemberGradeMapping");
            this.HasKey(cm => cm.Id);

            this.Property(cm => cm.EntityName).IsRequired().HasMaxLength(400);
		}
	}
}