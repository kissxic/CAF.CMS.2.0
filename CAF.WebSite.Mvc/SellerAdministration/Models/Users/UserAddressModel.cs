using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Seller.Models.Common;

namespace CAF.WebSite.Mvc.Seller.Models.Users
{
    public class UserAddressModel : ModelBase
    {
        public int UserId { get; set; }

        public AddressModel Address { get; set; }
    }
}