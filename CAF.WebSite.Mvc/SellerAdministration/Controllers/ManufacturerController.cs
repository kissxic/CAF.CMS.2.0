
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Manufacturers;
using CAF.Infrastructure.Core.Domain.Cms.Manufacturers;

namespace CAF.WebSite.Mvc.Seller.Controllers
{

    public class ManufacturerController : SellerAdminControllerBase
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
