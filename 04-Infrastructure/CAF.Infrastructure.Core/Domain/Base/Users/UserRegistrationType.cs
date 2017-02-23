namespace CAF.Infrastructure.Core.Domain.Users
{
    /// <summary>
    /// Represents the user registration type fortatting enumeration
    /// </summary>
    public enum UserRegistrationType : int
    {
        /// <summary>
        /// Standard account creation
        /// </summary>
        Standard = 1,
        /// <summary>
        /// Email validation is required after registration
        /// </summary>
        EmailValidation = 2,
        /// <summary>
        /// Mobile validation is required after registration
        /// </summary>
        MobileValidation =3,
        /// <summary>
        /// A user should be approved by administrator
        /// </summary>
        AdminApproval = 4,
        /// <summary>
        /// Registration is disabled
        /// </summary>
        Disabled = 5,
    }
}
