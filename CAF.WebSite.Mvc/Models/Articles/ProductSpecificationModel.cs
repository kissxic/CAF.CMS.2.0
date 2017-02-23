
using CAF.WebSite.Application.WebUI.Mvc;

namespace CAF.WebSite.Mvc.Models.Articles
{
    public partial class ProductSpecificationModel : ModelBase
    {
        public int SpecificationAttributeId { get; set; }

        public string SpecificationAttributeName { get; set; }

        public string SpecificationAttributeOption { get; set; }
    }
}