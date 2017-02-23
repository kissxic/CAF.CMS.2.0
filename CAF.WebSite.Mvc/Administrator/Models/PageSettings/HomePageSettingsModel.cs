
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Models.PageSettings
{
    public class HomePageSettingsModel : ModelBase
    {
        public HomePageSettingsModel()
        {



        }
        /// <summary>
        /// 关联品牌IDs
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.DisplayOrder", "关联品牌")]
        public string RelateManufacturerIds { get; set; }
        /// <summary>
        /// 关联栏目分类IDs
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.DisplayOrder", "关联栏目分类")]
        public string RelateCategoryIds { get; set; }
        /// <summary>
        /// 关联商品IDs
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.DisplayOrder", "关联商品")]
        public string RelateProductIds { get; set; }
        /// <summary>
        /// 关联商家IDs
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.DisplayOrder", "关联商家")]
        public string RelateVendorIds { get; set; }

    }
}