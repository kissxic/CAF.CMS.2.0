﻿namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents a price range
    /// </summary>
    public partial class PriceRange
    {
        /// <summary>
        /// From
        /// </summary>
        public decimal? From { get; set; }
        /// <summary>
        /// To
        /// </summary>
        public decimal? To { get; set; }
    }
}
