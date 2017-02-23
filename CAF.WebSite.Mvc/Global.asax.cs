using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Mvc.Bundles;
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using CAF.WebSite.Application.WebUI;
using FluentValidation.Mvc;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Plugins;
using CAF.WebSite.Application.Services.Tasks;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Theming;

namespace CAF.WebSite.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        #region Methods
   
        protected void Application_Start()
        {

        }
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            string result = string.Empty;

            if (DataSettings.DatabaseIsInstalled())
            {
                custom = custom.ToLower();

                switch (custom)
                {
                    case "theme":
                        result = EngineContext.Current.Resolve<IThemeContext>().CurrentTheme.ThemeName;
                        break;
                    case "site":
                        result = EngineContext.Current.Resolve<ISiteContext>().CurrentSite.Id.ToString();
                        break;
                    case "theme_site":
                        result = "{0}-{1}".FormatInvariant(
                            EngineContext.Current.Resolve<IThemeContext>().CurrentTheme.ThemeName,
                            EngineContext.Current.Resolve<ISiteContext>().CurrentSite.Id.ToString());
                        break;

                }
            }

            if (result.HasValue())
            {
                return result;
            }

            return base.GetVaryByCustomString(context, custom);
        }

        #endregion
    }
}
