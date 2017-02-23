using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Settings;
using FluentValidation.Attributes;

namespace CAF.WebSite.Mvc.Admin.Models.Settings
{
    [Validator(typeof(RewardPointsSettingsValidator))]
    public class RewardPointsSettingsModel
    {
        [LangResourceDisplayName("Admin.Configuration.Settings.RewardPoints.Enabled", "启用")]
        public bool Enabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.RewardPoints.ExchangeRate", "比例")]
        public decimal ExchangeRate { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.RewardPoints.RoundDownRewardPoints", "四舍五入")]
        public bool RoundDownRewardPoints { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForRegistration", "注册")]
        public int PointsForRegistration { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForProductReview", "内容浏览")]
        public int PointsForProductReview { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Amount", "商品购买")]
        public decimal PointsForPurchases_Amount { get; set; }
        public int PointsForPurchases_Points { get; set; }
        public bool PointsForPurchases_OverrideForStore { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Awarded", "订单状态")]
        public int PointsForPurchases_Awarded { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Canceled", "订单取消状态")]
        public int PointsForPurchases_Canceled { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
    }
}