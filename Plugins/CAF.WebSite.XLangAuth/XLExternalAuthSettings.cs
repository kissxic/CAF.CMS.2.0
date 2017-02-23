

using CAF.Infrastructure.Core.Configuration;
namespace CAF.WebSite.XLAuth
{
    public class XLExternalAuthSettings : ISettings
    {
        public string ClientKeyIdentifier { get; set; }
        public string ClientSecret { get; set; }

        public string AuthorizeURL { get; set; }
    }
}
