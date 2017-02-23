

using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.XLAuth.Models
{
    public class ConfigurationModel : ModelBase
    {
        [LangResourceDisplayName("Plugins.ExternalAuth.XL.ClientKeyIdentifier")]
        public string ClientKeyIdentifier { get; set; }

        [LangResourceDisplayName("Plugins.ExternalAuth.XL.ClientSecret")]
        public string ClientSecret { get; set; }
    }
}