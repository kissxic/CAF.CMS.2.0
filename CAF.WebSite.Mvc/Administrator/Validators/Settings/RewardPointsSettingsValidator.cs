
using CAF.Infrastructure.Core.Domain.Orders;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Settings;
using FluentValidation;

namespace CAF.WebSite.Mvc.Admin.Validators.Settings
{
    public partial class RewardPointsSettingsValidator : AbstractValidator<RewardPointsSettingsModel>
    {
        public RewardPointsSettingsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PointsForPurchases_Awarded).NotEqual((int)OrderStatus.Pending)
                .WithMessage(localizationService.GetResource("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Awarded.Pending"));

            RuleFor(x => x.PointsForPurchases_Canceled).NotEqual((int)OrderStatus.Pending)
                .WithMessage(localizationService.GetResource("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Canceled.Pending"));
        }
    }
}