

using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.WeiXinAuth.Models
{
    public class ConfigurationModel : ModelBase
    {
        [LangResourceDisplayName("Plugins.ExternalAuth.WX.ClientKeyIdentifier")]
        public string ClientKeyIdentifier { get; set; }

        [LangResourceDisplayName("Plugins.ExternalAuth.WX.ClientSecret")]
        public string ClientSecret { get; set; }
    }
}