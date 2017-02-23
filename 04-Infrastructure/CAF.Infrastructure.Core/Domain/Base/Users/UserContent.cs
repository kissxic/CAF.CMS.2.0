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
        /// �û�ID
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// IP��ַ
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// �Ƿ�ͨ��
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// �û�
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// �̼�
        /// </summary>
        public int? VendorId { get; set; }
    }
}
