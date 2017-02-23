
using CAF.WebSite.Application.WebUI;
using System.ComponentModel.DataAnnotations;

namespace CAF.WebSite.Im.Models
{
    public class IMModel
    {
        [LangResourceDisplayName("Plugins.IM.Fields.Enabled", "开启")]
        public bool Enabled { get; set; }



        /// <summary>
        /// Gets or sets the MongoServerSettings
        /// </summary>
        [LangResourceDisplayName("Plugins.IM.Fields.MongoServerSettings", "Mongo连接URL")]
        public string MongoServerSettings { get; set; }

        /// <summary>
        /// Gets or sets the RabbitMqUri
        /// </summary>
        [LangResourceDisplayName("Plugins.IM.Fields.RabbitMqUri", "Rabbit连接URL")]
        public string RabbitMqUri { get; set; }

        /// <summary>
        /// Gets or sets the RabbitUserName
        /// </summary>
        [LangResourceDisplayName("Plugins.IM.Fields.RabbitUserName", "Rabbit账号")]
        public string RabbitUserName { get; set; }

        /// <summary>
        /// Gets or sets the RabbitPassword
        /// </summary>
        [LangResourceDisplayName("Plugins.IM.Fields.RabbitPassword", "Rabbit账号密码")]
        public string RabbitPassword { get; set; }
    }
}