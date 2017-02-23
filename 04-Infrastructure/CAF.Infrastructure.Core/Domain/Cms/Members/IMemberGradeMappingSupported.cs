namespace CAF.Infrastructure.Core.Domain.Cms.Members
{
    /// <summary>
    /// Represents an entity which supports MemberGrade mapping
    /// </summary>
    public partial interface IMemberGradeMappingSupported
    {
		/// <summary>
		/// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
		/// </summary>
        bool LimitedToMemberGrades { get; set; }
	}
}
