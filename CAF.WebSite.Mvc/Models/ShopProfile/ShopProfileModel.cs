
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Users;

namespace CAF.WebSite.Mvc.Models.ShopProfile
{
    public partial class ShopProfileModel : EntityModelBase
    {
        public int Step { get; set; }
        public string MenuStep { get; set; }
        public string Frame { get; set; }
        public string StoreName { get; set; }
        public string SeName { get; set; }
        public VendorModel VendorAgreement { get; set; }

        public UserNavigationModel NavigationModel { get; set; }
    }
}