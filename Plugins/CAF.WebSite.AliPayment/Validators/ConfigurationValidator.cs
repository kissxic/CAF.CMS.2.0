using FluentValidation;
using CAF.WebSite.AliPayment.Models;
using CAF.WebSite.Application.Services.Localization;

namespace CAF.WebSite.AliPayment.Validatiors
{
    public class ConfigurationValidator : AbstractValidator<ConfigurationModel>
    {
        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SellerEmail).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.AliPayment.SellerEmailRequired"));
            RuleFor(x => x.Key).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.AliPayment.KeyRequired"));
            RuleFor(x => x.Partner).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.AliPayment.PartnerRequired"));
            RuleFor(x => x.AdditionalFee).GreaterThanOrEqualTo(0).WithMessage(localizationService.GetResource("Plugins.Payments.AliPayment.AdditionalFeeRequired"));
        }
    }
}
