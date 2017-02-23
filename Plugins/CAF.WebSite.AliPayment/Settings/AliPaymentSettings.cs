
using CAF.Infrastructure.Core.Configuration;
using System.Text;

namespace CAF.WebSite.AliPayment
{
    public class AliPaymentSettings : ISettings
    {
        //private  string _seller = "";//�տ�֧�����ʻ�
        //private  string _partner = "";//���������ID����2088��ͷ��16λ��������ɵ��ַ���
        //private  string _key = ""; //���װ�ȫ�����룬�����ֺ���ĸ��ɵ�32λ�ַ���
        //private  Encoding _code = null;//�ַ������ʽ Ŀǰ֧�� gbk �� utf-8
        //private  string _inputcharset = "";//�ַ������ʽ(�ı�)
        //private  string _signtype = "";//ǩ����ʽ��ѡ���RSA��DSA��MD5
        //private  string _gateway = "";//֧�������ص�ַ���£�
        //private  string _veryfyurl = "";//֧������Ϣ��֤��ַ

        //private  string _privatekey = "";//�̻���˽Կ
        //private  string _publickey = "";//֧�����Ĺ�Կ�������޸ĸ�ֵ
        //private  string _appinputcharset = "";//app�ַ������ʽ Ŀǰ֧�� gbk �� utf-8
        //private  string _appsigntype = "";//appǩ����ʽ��ѡ���RSA��DSA��MD5
        //private  string _appveryfyurl = "";//app֧������Ϣ��֤��ַ

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
        /// �տ�֧�����ʻ�ID
        /// </summary>
        public string SellerEmail { get; set; }

        /// <summary>
        /// ���������ID
        /// </summary>
        public string Partner { get; set; }

        /// <summary>
        /// ���װ�ȫУ����
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// �ַ������ʽ
        /// </summary>
        public Encoding Code { get; set; }

        /// <summary>
        /// �ַ������ʽ(�ı�)
        /// </summary>
        public string InputCharset { get; set; }

        /// <summary>
        /// ǩ����ʽ
        /// </summary>
        public string SignType { get; set; }

        /// <summary>
        /// ֧�������ص�ַ���£�
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// ֧������Ϣ��֤��ַ
        /// </summary>
        public string VeryfyUrl { get; set; }


        /// <summary>
        /// �̻���˽Կ
        /// </summary>
        public string PrivateKey { get; set; }
        /// <summary>
        /// ֧�����Ĺ�Կ�������޸ĸ�ֵ
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// app�ַ������ʽ Ŀǰ֧�� gbk �� utf-8
        /// </summary>
        public string AppInputCharset { get; set; }
        /// <summary>
        /// appǩ����ʽ��ѡ���RSA��DSA��MD5
        /// </summary>
        public string AppSignType { get; set; }
        /// <summary>
        /// app֧������Ϣ��֤��ַ
        /// </summary>
        public string AppVeryfyUrl { get; set; }
    }
}
