using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.PageSettings
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("CAF.WebSite.PageSettings",
                 "Plugins/CAF.WebSite.PageSettings/{controller}/{action}",
                 new { controller = "PageSetting", action = "Index" },
                 new[] { "CAF.WebSite.PageSettings.Controllers" }
            )
            .DataTokens["area"] = "CAF.WebSite.PageSettings";

            routes.MapRoute("CAF.WebSite.PageSettingsNavigation",
                          "Plugins/CAF.WebSite.PageSettings/{controller}/{action}",
                          new { controller = "Navigation", action = "Index" },
                          new[] { "CAF.WebSite.PageSettings.Controllers" }
                      )
                      .DataTokens["area"] = "CAF.WebSite.PageSettings";
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
