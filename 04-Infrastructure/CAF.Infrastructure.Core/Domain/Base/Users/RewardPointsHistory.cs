using System;

namespace CAF.Infrastructure.Core.Domain.Users
{
    /// <summary>
    /// Represents a reward point history entry
    /// </summary>
    public partial class RewardPointsHistory : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 赎回/添加
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the points balance
        /// </summary>
        public int PointsBalance { get; set; }

        /// <summary>
        /// 已使用
        /// </summary>
        public decimal UsedAmount { get; set; }

        /// <summary>
        /// 积分信息
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
