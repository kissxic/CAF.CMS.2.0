using CAF.Infrastructure.Core.Configuration;
namespace CAF.WebSite.Im
{
    public class IMSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the value indicting whether this SMS provider is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the MongoServerSettings
        /// </summary>
        public string MongoServerSettings { get; set; }

        /// <summary>
        /// Gets or sets the RabbitMqUri
        /// </summary>
        public string RabbitMqUri { get; set; }

        /// <summary>
        /// Gets or sets the RabbitUserName
        /// </summary>
        public string RabbitUserName { get; set; }

        /// <summary>
        /// Gets or sets the RabbitPassword
        /// </summary>
        public string RabbitPassword { get; set; }
    }
}