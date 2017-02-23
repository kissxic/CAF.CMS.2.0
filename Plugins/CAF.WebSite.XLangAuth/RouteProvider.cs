using CAF.WebSite.Application.WebUI.Mvc.Routes;
using CAF.WebSite.XLAuth.Core;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.XLAuth
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("CAF.WebSite.XLAuth",
                 "Plugins/CAF.WebSite.XLAuth/{action}",
                 new { controller = "ExternalAuthXL" },
                 new[] { "CAF.WebSite.XLAuth.Controllers" }
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
