using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.AliPayment
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("CAF.WebSite.AliPayment",
                    "Plugins/CAF.WebSite.AliPayment/{controller}/{action}",
                    new { controller = "AliPayment" },
                    new[] { "CAF.WebSite.AliPayment.Controllers" }
            )
            .DataTokens["area"] = "CAF.WebSite.AliPayment"; 

            routes.MapRoute("CAF.WebSite.AliPayment.Notify",
               "Plugins/AliPayment/Notify",
               new { controller = "AliPayment", action = "Notify" },
             new[] { "CAF.WebSite.AliPayment.Controllers" }
              ).DataTokens["area"] = "CAF.WebSite.AliPayment";

            //Notify
            routes.MapRoute("CAF.WebSite.AliPayment.Return",
                 "Plugins/AliPayment/Return",
                 new { controller = "AliPayment", action = "Return" },
                new[] { "CAF.WebSite.AliPayment.Controllers" }
            ).DataTokens["area"] = "CAF.WebSite.AliPayment"; 
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
