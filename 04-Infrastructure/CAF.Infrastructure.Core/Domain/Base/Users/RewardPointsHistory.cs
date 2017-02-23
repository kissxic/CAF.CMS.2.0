using System;

namespace CAF.Infrastructure.Core.Domain.Users
{
    /// <summary>
    /// Represents a reward point history entry
    /// </summary>
    public partial class RewardPointsHistory : BaseEntity
    {
        /// <summary>
        /// ����
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ���/���
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the points balance
        /// </summary>
        public int PointsBalance { get; set; }

        /// <summary>
        /// ��ʹ��
        /// </summary>
        public decimal UsedAmount { get; set; }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the order for which points were redeemed as a payment
        /// </summary>
       // public virtual Order UsedWithOrder { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual User User { get; set; }
    }
}
