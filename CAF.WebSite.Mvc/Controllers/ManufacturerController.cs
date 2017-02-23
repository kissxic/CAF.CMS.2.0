using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Seo;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Manufacturers;
using CAF.Infrastructure.Core.Domain.Cms.Manufacturers;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Application.Services.Helpers;
using CAF.Infrastructure.Core.Logging;

namespace CAF.WebSite.Mvc.Controllers
{

    public class ManufacturerController : PublicControllerBase
    {
        #region Fields
        private readonly IManufacturerService _manufacturerService;
        #endregion Fields

        #region Constructors

        public ManufacturerController(IManufacturerService manufacturerService)
        {

            this._manufacturerService = manufacturerService;

        }

        #endregion

    
        #region List

        //ajax
        // codehint: sm-edit
        public ActionResult AllManufacturers(string label, int selectedId)
        {
            var categories = _manufacturerService.GetAllManufacturers(showHidden: true);
            var cquery = categories.AsQueryable();
            if (label.HasValue())
            {
                categories.Insert(0, new Manufacturer { Name = label, Id = 0 });
            }

            var query =
                from c in cquery
                select new
                {
                    id = c.Id.ToString(),
                    text = c.Name,
                    selected = c.Id == selectedId
                };

            var data = query.ToList();

            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

      
    }
}
