
using CAF.Infrastructure.Core.Configuration;
using System.Text;

namespace CAF.WebSite.AliPayment
{
    public class AliPaymentSettings : ISettings
    {
        //private  string _seller = "";//收款支付宝帐户
        //private  string _partner = "";//合作身份者ID，以2088开头由16位纯数字组成的字符串
        //private  string _key = ""; //交易安全检验码，由数字和字母组成的32位字符串
        //private  Encoding _code = null;//字符编码格式 目前支持 gbk 或 utf-8
        //private  string _inputcharset = "";//字符编码格式(文本)
        //private  string _signtype = "";//签名方式，选择项：RSA、DSA、MD5
        //private  string _gateway = "";//支付宝网关地址（新）
        //private  string _veryfyurl = "";//支付宝消息验证地址

        //private  string _privatekey = "";//商户的私钥
        //private  string _publickey = "";//支付宝的公钥，无需修改该值
        //private  string _appinputcharset = "";//app字符编码格式 目前支持 gbk 或 utf-8
        //private  string _appsigntype = "";//app签名方式，选择项：RSA、DSA、MD5
        //private  string _appveryfyurl = "";//app支付宝消息验证地址

        public AliPaymentSettings()
        {

            Code = Encoding.GetEncoding("utf-8");
            InputCharset = "utf-8";
            SignType = "MD5";
            Gateway = "https://mapi.alipay.com/gateway.do?";
            VeryfyUrl = "https://mapi.alipay.com/gateway.do?service=notify_verify&";

            PrivateKey = "";
            PublicKey = @"";
            AppInputCharset = "utf-8";
            AppSignType = "RSA";
            AppVeryfyUrl = "https://mapi.alipay.com/gateway.do?service=notify_verify&";
        }
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFeePercentage { get; set; }
        /// <summary>
        /// 收款支付宝帐户ID
        /// </summary>
        public string SellerEmail { get; set; }

        /// <summary>
        /// 合作者身份ID
        /// </summary>
        public string Partner { get; set; }

        /// <summary>
        /// 交易安全校验码
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 字符编码格式
        /// </summary>
        public Encoding Code { get; set; }

        /// <summary>
        /// 字符编码格式(文本)
        /// </summary>
        public string InputCharset { get; set; }

        /// <summary>
        /// 签名方式
        /// </summary>
        public string SignType { get; set; }

        /// <summary>
        /// 支付宝网关地址（新）
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// 支付宝消息验证地址
        /// </summary>
        public string VeryfyUrl { get; set; }


        /// <summary>
        /// 商户的私钥
        /// </summary>
        public string PrivateKey { get; set; }
        /// <summary>
        /// 支付宝的公钥，无需修改该值
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// app字符编码格式 目前支持 gbk 或 utf-8
        /// </summary>
        public string AppInputCharset { get; set; }
        /// <summary>
        /// app签名方式，选择项：RSA、DSA、MD5
        /// </summary>
        public string AppSignType { get; set; }
        /// <summary>
        /// app支付宝消息验证地址
        /// </summary>
        public string AppVeryfyUrl { get; set; }
    }
}
