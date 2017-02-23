using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.Im
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("CAF.WebSite.Im",
				 "Plugins/CAF.WebSite.Im/{action}",
                 new { controller = "IM", action = "Configure" },
                 new[] { "CAF.WebSite.Im.Controllers" }
            )
			.DataTokens["area"] = "CAF.WebSite.Im";

            routes.MapRoute("CAF.WebSite.Im.Default",
                 "Plugins/Im/{action}",
                 new { controller = "IM", action = "Index" },
                 new[] { "CAF.WebSite.Im.Controllers" }
            )
            .DataTokens["area"] = "CAF.WebSite.Im";

            routes.MapRoute("CAF.WebSite.Chat.Default",
              "Plugins/Chat/{action}",
              new { controller = "Chat", action = "Index" },
              new[] { "CAF.WebSite.Im.Controllers" }
         )
         .DataTokens["area"] = "CAF.WebSite.Im";
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
