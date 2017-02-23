﻿using System;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core;

namespace CAF.Infrastructure.Core.Domain.Cms.Forums
{
    /// <summary>
    /// Represents a forum subscription item
    /// </summary>
    public partial class ForumSubscription : BaseEntity
    {
        /// <summary>
        /// Gets or sets the forum subscription identifier
        /// </summary>
        public Guid SubscriptionGuid { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the forum identifier
        /// </summary>
        public int ForumId { get; set; }

        /// <summary>
        /// Gets or sets the topic identifier
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the user
        /// </summary>
        public virtual User User { get; set; }
    }
}
