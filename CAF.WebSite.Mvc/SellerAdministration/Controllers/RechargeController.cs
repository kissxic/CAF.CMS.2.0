using CAF.WebSite.Application.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Seller.Controllers
{
    public class RechargeController : SellerAdminControllerBase
    {
        // GET: Recharge
        public ActionResult Index(int? channelId)
        {
            return View();
        }
    }
}