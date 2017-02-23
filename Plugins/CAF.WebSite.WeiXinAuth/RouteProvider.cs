using CAF.WebSite.Application.WebUI.Mvc.Routes;
using CAF.WebSite.WeiXinAuth.Core;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.WeiXinAuth
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("CAF.WebSite.WeiXinAuth",
                 "Plugins/CAF.WebSite.WeiXinAuth/{action}",
                 new { controller = "ExternalAuthWX" },
                 new[] { "CAF.WebSite.WeiXinAuth.Controllers" }
            )
            .DataTokens["area"] = Provider.SystemName;
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
