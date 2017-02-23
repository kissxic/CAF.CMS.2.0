using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Linq;
using CAF.WebSite.Application.WebUI.Localization;
using System.Collections.Generic;
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System.Web.Mvc.Routing.Constraints;
using CAF.WebSite.Application.WebUI.Seo;

namespace CAF.WebSite.Mvc.Infrastructure.Routes
{
    public partial class SeoRoutes : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var idConstraint = new MinRouteConstraint(1);
            //generic URLs
            routes.MapGenericPathRoute("GenericUrl",
                "{*generic_se_name}",
                new { controller = "Common", action = "GenericUrl" },
                new[] { "CAF.WebSite.Mvc.Controllers" });

            // Routes solely needed for URL creation, NOT for route matching.
            routes.MapLocalizedRoute("Article",
                "{SeName}",
                new { controller = "Article", action = "Article" },
              new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("HelpeArticle",
             "help/{SeName}",
             new { controller = "Article", action = "Article" },
           new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ArticleCategory",
                "{SeName}",
                new { controller = "ArticleCatalog", action = "Category" },
                new[] { "CAF.WebSite.Mvc.Controllers" });

            //routes.MapLocalizedRoute("ProductCategory",
            //  "list/{ChannelId}/{SeName}",
            //  new { controller = "ProductCategory", action = "Category" },
            //  new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ClassIfication",
               "Info/{SeName}",
               new { controller = "ArticleCatalog", action = "Category" },
               new[] { "CAF.WebSite.Mvc.Controllers" });

            // at the top handles this.
            routes.MapLocalizedRoute("PageNotFound",
                "{*path}",
                new { controller = "Error", action = "NotFound" },
                 new[] { "CAF.WebSite.Mvc.Controllers" });


        }

        public int Priority
        {
            get
            {
                return int.MinValue;
            }
        }
    }
}