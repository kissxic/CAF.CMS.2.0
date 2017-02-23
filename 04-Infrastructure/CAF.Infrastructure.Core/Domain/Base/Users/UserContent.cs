using CAF.Infrastructure.Core;
using System;
namespace CAF.Infrastructure.Core.Domain.Users
{
    /// <summary>
    /// Represents a user generated content
    /// </summary>
    public partial class UserContent : AuditedBaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// 商家
        /// </summary>
        public int? VendorId { get; set; }
    }
}
