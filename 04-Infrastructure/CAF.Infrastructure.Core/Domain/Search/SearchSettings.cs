
using CAF.Infrastructure.Core.Configuration;
using System;

namespace CAF.Infrastructure.Core.Domain.Search
{
    public class SearchSettings : ISettings
    {
		public SearchSettings()
		{
            LastBuildTime = DateTime.UtcNow; ;
          
		}

        /// <summary>
        /// Gets or sets a value indicating whether usernames are used instead of emails
        /// </summary>
        public DateTime LastBuildTime { get; set; }

   
    }
}