using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Collections.Specialized;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Payments;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Logging;
using CAF.Infrastructure.Core.Domain.Cms.Payments;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services;
using CAF.Infrastructure.Core.Exceptions;
using CAF.WebSite.AliPayment.Models;
using CAF.Infrastructure.Core.Settings;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.AliPayment.Services;

namespace CAF.WebSite.AliPayment.Controllers
{
    public class AliPaymentController : PaymentControllerBase
    {
        private readonly IPaymentService _paymentService;
        //private readonly IOrderService _orderService;
        //private readonly IOrderProcessingService _orderProcessingService;
        private readonly ISiteContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ICommonServices _services;
        private readonly ISiteService _storeService;

        public AliPaymentController(
            IPaymentService paymentService,
            //IOrderService orderService,
            //IOrderProcessingService orderProcessingService,
            ISiteContext storeContext,
            IWorkContext workContext,
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            ILocalizationService localizationService,
            ICommonServices services,
            ISiteService storeService)
        {
            _paymentService = paymentService;
            //_orderService = orderService;
            //_orderProcessingService = orderProcessingService;
            _storeContext = storeContext;
            _workContext = workContext;
            _webHelper = webHelper;
            _paymentSettings = paymentSettings;
            _localizationService = localizationService;
            _services = services;
            _storeService = storeService;
        }

        [AdminAuthorize, ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();

            int storeScope = this.GetActiveSiteScopeConfiguration(_storeService, _services.WorkContext);
            var settings = _services.Settings.LoadSetting<AliPaymentSettings>(storeScope);

            model.Copy(settings, true);

            var storeDependingSettingHelper = new SiteDependingSettingHelper(ViewData);
            storeDependingSettingHelper.GetOverrideKeys(settings, model, storeScope, _services.Settings);

            return View(model);
        }

        [HttpPost, AdminAuthorize, ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model, FormCollection form)
        {
            if (!ModelState.IsValid)
                return Configure();

            ModelState.Clear();

            var storeDependingSettingHelper = new SiteDependingSettingHelper(ViewData);
            int storeScope = this.GetActiveSiteScopeConfiguration(_storeService, _services.WorkContext);
            var settings = _services.Settings.LoadSetting<AliPaymentSettings>(storeScope);

            model.Copy(settings, false);

            storeDependingSettingHelper.UpdateSettings(settings, form, storeScope, _services.Settings);


            _services.Settings.SaveSetting(settings, 0);

            _services.Settings.ClearCache();

            NotifySuccess(_services.Localization.GetResource("Admin.Common.DataSuccessfullySaved"));
            return Configure();

        }

        public ActionResult PaymentInfo()
        {
            return PartialView();
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        [ValidateInput(false)]
        public ActionResult Notify(FormCollection form)
        {
            var provider = _paymentService.LoadPaymentMethodBySystemName("SmartStore.AliPayment", true);
            var processor = provider != null ? provider.Value as AliPaymentProvider : null;

            if (processor == null)
                throw new WorkException("AliPay module cannot be loaded");

            var settings = _services.Settings.LoadSetting<AliPaymentSettings>();


            SortedDictionary<string, string> paras = AlipayCore.GetRequestPost();

            if (paras.Count > 0)//判断是否有带返回参数
            {
                bool verifyResult = AlipayNotify.Verify(paras, Request.Form["notify_id"], Request.Form["sign"], settings.SignType, settings.Key, settings.Code, settings.VeryfyUrl, settings.Partner);
                if (verifyResult && (Request.Form["trade_status"] == "TRADE_FINISHED" || Request.Form["trade_status"] == "TRADE_SUCCESS"))//验证成功
                {
                    string out_trade_no = Request.Form["out_trade_no"];//商户订单号
                    string tradeSN = Request.QueryString["trade_no"];//支付宝交易号
                    decimal tradeMoney = Request.QueryString["total_fee"].Convert<decimal>();//交易金额
                    DateTime tradeTime = Request.QueryString["notify_time"].Convert<DateTime>();//交易时间

                    string orderByNumber = "";
                    if (!out_trade_no.IsEmpty())
                    {
                        orderByNumber = out_trade_no;
                        //var order = _orderService.GetOrderByNumber(orderByNumber);
                        //if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
                        //{
                        //    _orderProcessingService.MarkOrderAsPaid(order);
                        //}
                    }

                    return Content("success");
                }
                else//验证失败
                {
                    //过滤空值、sign与sign_type参数
                    var sPara = AlipayCore.FilterPara(paras);
                    var str = AlipayCore.CreateLinkString(sPara);
                    string logStr = "MD5:mysign=" + str + ",responseText=检验失败，数据可疑或者支付未成功";
                    Logger.Error(logStr);
                    return Content("fail");
                }
            }
            else
            {
                return Content("无通知参数");
            }
        }
        [ValidateInput(false)]
        public ActionResult Return()
        {

            var provider = _paymentService.LoadPaymentMethodBySystemName("SmartStore.AliPayment", true);
            var processor = provider != null ? provider.Value as AliPaymentProvider : null;

            if (processor == null)
                throw new WorkException("AliPay module cannot be loaded");

            var settings = _services.Settings.LoadSetting<AliPaymentSettings>();
            SortedDictionary<string, string> paras = AlipayCore.GetRequestGet();

            if (paras.Count > 0)//判断是否有带返回参数
            {
                bool verifyResult = AlipayNotify.Verify(paras, Request.QueryString["notify_id"], Request.QueryString["sign"], settings.SignType, settings.Key, settings.Code, settings.VeryfyUrl, settings.Partner);
                if (verifyResult && (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS"))//验证成功
                {
                    string out_trade_no = Request.QueryString["out_trade_no"];//商户订单号
                    string tradeSN = Request.QueryString["trade_no"];//支付宝交易号
                    decimal tradeMoney = Request.QueryString["total_fee"].Convert<decimal>();//交易金额
                    DateTime tradeTime = Request.QueryString["notify_time"].Convert<DateTime>();//交易时间

                    string orderByNumber = "";
                    if (!out_trade_no.IsEmpty())
                    {
                        orderByNumber = out_trade_no;
                        //var order = _orderService.GetOrderByNumber(orderByNumber);
                        //if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
                        //{
                        //    _orderProcessingService.MarkOrderAsPaid(order);
                        //}
                    }
                    return RedirectToAction("payresult", "order", new { area = "", oid = orderByNumber });
                }
                else//验证失败
                {
                    //过滤空值、sign与sign_type参数
                    var sPara = AlipayCore.FilterPara(paras);
                    var str = AlipayCore.CreateLinkString(sPara);
                    string logStr = "MD5:mysign=" + str + ",responseText=检验失败，数据可疑或者支付未成功";
                    Logger.Error(logStr);
                    return Content("支付失败");
                }
            }
            else
            {
                return Content("支付失败");
            }

        }

    }
}