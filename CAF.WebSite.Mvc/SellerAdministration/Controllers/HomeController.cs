
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Mvc.Seller.Models.Common;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Seller.Controllers
{

    public class HomeController : SellerAdminControllerBase
    {
        private readonly IWorkContext _workContext;
        private readonly IChannelService _channelService;
        private readonly IVisitRecordService _visitRecordService;
        public HomeController(IVisitRecordService visitRecordService,
            IChannelService channelService, IWorkContext workContext)
        {
            this._visitRecordService = visitRecordService;
            this._channelService = channelService;
            this._workContext = workContext;
        }


        [NonAction]
        protected void PrepareUserArticlePublicNumModel(MemberInfo model)
        {
            var list = this._channelService.GetUserArticlePublicNumsByUserId(this._workContext.CurrentUser.Id);
            foreach (var item in list)
            {
                model.UserArticlePublicNumModels.Add(new UserArticlePublicNum
                {
                    ChannelName = item.Channel.Title,
                    PublicedNum = item.PublicedTotal,
                    PublicNum = item.PublicTotal
                });
            }
        }

        // GET: Passport
        public ActionResult Index()
        {
            var model = new MemberInfo();
            PrepareUserArticlePublicNumModel(model);
            return View(model);
        }

        public ActionResult Dashboard()
        {
            return View();
        }


    }
}