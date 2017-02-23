

using CAF.Infrastructure.Core.Configuration;
namespace CAF.WebSite.WeiXinAuth
{
    public class WXExternalAuthSettings : ISettings
    {
        public string ClientKeyIdentifier { get; set; }
        public string ClientSecret { get; set; }

        public string AuthorizeURL { get; set; }
    }
}
