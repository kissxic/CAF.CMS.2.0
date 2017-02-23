using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using System;
using System.Collections.Generic;

namespace CAF.WebSite.Mvc.Admin.Models.PageSettings
{
    public class HomeCategoryModel : EntityModelBase
    {
        public HomeCategoryModel()
        {
            HomeCategoryItemModels = new List<PageSettings.HomeCategoryItemModel>();
           
        }

        /// <summary>
        /// 分类名称
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.Name", "名称")]
        public string Name { get; set; }
        /// <summary>
        /// 分类URL
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.Url", "URL")]
        public string Url { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.DisplayOrder", "排序")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.RequiredVendorIds", "推荐商家")]
        public string RequiredVendorIds { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.RequiredManufacturerIds", "推荐品牌")]
        public string RequiredManufacturerIds { get; set; }

        public List<HomeCategoryItemModel> HomeCategoryItemModels { get; set; }

        public string CategoryIds { get; set; }
        /// <summary>
        /// 记录是否修改过分类
        /// </summary>
        public bool IsChangeCategory { get; set; }
    }

}
