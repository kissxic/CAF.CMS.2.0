using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using CAF.WebSite.AliPayment.Controllers;
using System.Net;
using System.Security.Cryptography;
using CAF.Infrastructure.Core;
using CAF.WebSite.AliPayment.Services;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Payments;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Application.Services;
using CAF.Infrastructure.Core.Domain.Cms.Payments;
using CAF.Infrastructure.Core.Domain.Orders;

namespace CAF.WebSite.AliPayment
{
    /// <summary>
    /// AliPaymentStandard provider
    /// </summary>
    [SystemName("CAF.WebSite.AliPayment")]
    [FriendlyName("AliPayment Standard")]
    [DisplayOrder(2)]
    public partial class AliPaymentProvider : PaymentPluginBase, IConfigurable
    {
        //  private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICommonServices _services;
        private readonly ILogger _logger;

        public AliPaymentProvider(
            //  IOrderTotalCalculationService orderTotalCalculationService,
            ICommonServices services,
            ILogger logger)
        {
            //  _orderTotalCalculationService = orderTotalCalculationService;
            _services = services;
            _logger = logger;
        }

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public override ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;

            var settings = _services.Settings.LoadSetting<AliPaymentSettings>(processPaymentRequest.SiteId);

            if (settings.SellerEmail.IsEmpty() || settings.Key.IsEmpty())
            {
                result.AddError(T("Plugins.Payments.AliPayment.InvalidCredentials"));
            }

            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public override void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            if (postProcessPaymentRequest.Order.PaymentStatus == PaymentStatus.Paid)
                return;

            var store = _services.SiteService.GetSiteById(postProcessPaymentRequest.Order.SiteId);
            var _AliPaymentSettings = _services.Settings.LoadSetting<AliPaymentSettings>(postProcessPaymentRequest.Order.SiteId);

            string orderNumber = postProcessPaymentRequest.Order.GetOrderNumber();

            //支付类型，必填，不能修改
            string paymentType = "1";

            //服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数
            string notifyUrl = _services.WebHelper.GetSiteLocation(false) + "Plugins/AliPayment/Notify";
            //页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/
            string returnUrl = _services.WebHelper.GetSiteLocation(false) + "Plugins/AliPayment/Return";

            //收款支付宝帐户
            string sellerEmail = _AliPaymentSettings.SellerEmail;
            //合作者身份ID
            string partner = _AliPaymentSettings.Partner;
            //交易安全检验码
            string key = _AliPaymentSettings.Key;

            //商户订单号
            string outTradeNo = orderNumber;
            //订单名称
            string subject = store.Name + "购物";
            //付款金额
            var orderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            string totalFee = orderTotal.ToString("0.00", CultureInfo.InvariantCulture);

            //订单描述
            string body = "订单来自 " + store.Name;

            //防钓鱼时间戳,若要使用请调用类文件submit中的query_timestamp函数
            string antiPhishingKey = "";
            //客户端的IP地址,非局域网的外网IP地址，如：221.0.0.1
            string exterInvokeIP = "";

            //把请求参数打包成数组
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
            parms.Add("partner", partner);
            parms.Add("_input_charset", _AliPaymentSettings.InputCharset);
            parms.Add("service", "create_direct_pay_by_user");
            parms.Add("payment_type", paymentType);
            parms.Add("notify_url", notifyUrl);
            parms.Add("return_url", returnUrl);
            parms.Add("seller_email", sellerEmail);
            parms.Add("out_trade_no", outTradeNo);
            parms.Add("subject", subject);
            parms.Add("total_fee", totalFee);
            parms.Add("body", body);
            parms.Add("show_url", "");
            parms.Add("anti_phishing_key", antiPhishingKey);
            parms.Add("exter_invoke_ip", exterInvokeIP);

            //建立请求
            string sHtmlText = AlipaySubmit.BuildRequest(parms, _AliPaymentSettings.SignType, _AliPaymentSettings.Key, _AliPaymentSettings.Code, _AliPaymentSettings.Gateway, _AliPaymentSettings.InputCharset, "post", "确认");
            var _httpContext = EngineContext.Current.Resolve<HttpContextBase>();
            _httpContext.Response.Clear();
            _httpContext.Response.Write(sHtmlText);
            _httpContext.Response.End();
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public override bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.PaymentStatus == PaymentStatus.Pending && (DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds > 5)
            {
                return true;
            }
            return true;
        }

        public override Type GetControllerType()
        {
            return typeof(AliPaymentController);
        }

        public override decimal GetAdditionalHandlingFee(IList<OrganizedShoppingCartItem> cart)
        {
            var result = decimal.Zero;
            try
            {
                var settings = _services.Settings.LoadSetting<AliPaymentSettings>(_services.SiteContext.CurrentSite.Id);

                // result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart, settings.AdditionalFee, settings.AdditionalFeePercentage);
            }
            catch (Exception)
            {
            }
            return result;
        }




        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public override void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "AliPayment";
            routeValues = new RouteValueDictionary() { { "area", "SmartSite.AliPayment" } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public override void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "AliPayment";
            routeValues = new RouteValueDictionary() { { "area", "SmartSite.AliPayment" } };
        }
        /// <summary>
        /// Gets MD5 hash
        /// </summary>
        /// <param name="Input">Input</param>
        /// <param name="Input_charset">Input charset</param>
        /// <returns>Result</returns>
        public string GetMD5(string Input, string Input_charset)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(Input_charset).GetBytes(Input));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Bubble sort
        /// </summary>
        /// <param name="Input">Input</param>
        /// <returns>Result</returns>
        public string[] BubbleSort(string[] Input)
        {
            int i, j;
            string temp;

            bool exchange;

            for (i = 0; i < Input.Length; i++)
            {
                exchange = false;

                for (j = Input.Length - 2; j >= i; j--)
                {
                    if (System.String.CompareOrdinal(Input[j + 1], Input[j]) < 0)
                    {
                        temp = Input[j + 1];
                        Input[j + 1] = Input[j];
                        Input[j] = temp;

                        exchange = true;
                    }
                }

                if (!exchange)
                {
                    break;
                }
            }
            return Input;
        }

        /// <summary>
        /// Create URL
        /// </summary>
        /// <param name="Para">Para</param>
        /// <param name="InputCharset">Input charset</param>
        /// <param name="Key">Key</param>
        /// <returns>Result</returns>
        public string CreatUrl(string[] Para, string InputCharset, string Key)
        {
            int i;
            string[] Sortedstr = BubbleSort(Para);
            StringBuilder prestr = new StringBuilder();

            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (i == Sortedstr.Length - 1)
                {
                    prestr.Append(Sortedstr[i]);

                }
                else
                {
                    prestr.Append(Sortedstr[i] + "&");
                }

            }

            prestr.Append(Key);
            string sign = GetMD5(prestr.ToString(), InputCharset);
            return sign;
        }

        /// <summary>
        /// Gets HTTP
        /// </summary>
        /// <param name="StrUrl">Url</param>
        /// <param name="Timeout">Timeout</param>
        /// <returns>Result</returns>
        public string Get_Http(string StrUrl, int Timeout)
        {
            string strResult = string.Empty;
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(StrUrl);
                myReq.Timeout = Timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                strResult = strBuilder.ToString();
            }
            catch (Exception exc)
            {
                strResult = "Error: " + exc.Message;
            }
            return strResult;
        }
        #region Properties



        public override PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        #endregion
    }
}
