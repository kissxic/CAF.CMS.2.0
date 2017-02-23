
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;


namespace CAF.WebSite.Mvc.Seller.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var idConstraint = new MinRouteConstraint(1);

            var route = routes.MapRoute(
                "Seller_default",
                "Seller/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "Seller", id = "" },
                new[] { "CAF.WebSite.Mvc.Seller.Controllers" }
            );
            route.DataTokens["area"] = "Seller";

            var routeCategory = routes.MapLocalizedRoute("SellerProfile",
              "Seller/Category/{id}",
              new { controller = "Category", action = "Index", area = "Seller" },
              new { id = idConstraint },
              new[] { "CAF.WebSite.Mvc.Seller.Controllers" }
              );
            routeCategory.DataTokens["area"] = "Seller";
        }

        public int Priority
        {
            get { return 1000; }
        }
    }
}