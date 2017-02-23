using CAF.Infrastructure.Core.Configuration;
namespace CAF.Infrastructure.Core.Domain.Cms.Members
{
    public class MemberGradeSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a site default eMemberGrade identifier
        /// </summary>
        public int DefaultMemberGradeId { get; set; }

    }

}
