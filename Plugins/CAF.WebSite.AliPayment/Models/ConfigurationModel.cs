using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.AliPayment.Validatiors;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.AliPayment.Models
{
    [Validator(typeof(ConfigurationValidator))]
    public class ConfigurationModel : ModelBase
    {
        public string[] ConfigGroups { get; set; }

        [LangResourceDisplayName("Plugins.Payments.AliPayment.SellerEmail")]
        public string SellerEmail { get; set; }

        [LangResourceDisplayName("Plugins.Payments.AliPayment.Key")]
        public string Key { get; set; }

        [LangResourceDisplayName("Plugins.Payments.AliPayment.Partner")]
        public string Partner { get; set; }

        [LangResourceDisplayName("Plugins.Payments.AliPayment.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        [LangResourceDisplayName("Plugins.Payments.AliPayment.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public void Copy(AliPaymentSettings settings, bool fromSettings)
        {
            if (fromSettings)
            {
                SellerEmail = settings.SellerEmail;
                Key = settings.Key;
                Partner = settings.Partner;
                AdditionalFee = settings.AdditionalFee;
                AdditionalFeePercentage = settings.AdditionalFeePercentage;
            }
            else
            {
                settings.SellerEmail = SellerEmail;
                settings.Key = Key;
                settings.Partner = Partner;
                settings.AdditionalFee = AdditionalFee;
                settings.AdditionalFeePercentage = AdditionalFeePercentage;
            }

        }
    }


}