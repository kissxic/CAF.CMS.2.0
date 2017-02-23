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
        /// ��������
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.Name", "����")]
        public string Name { get; set; }
        /// <summary>
        /// ����URL
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.Url", "URL")]
        public string Url { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.DisplayOrder", "����")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.RequiredVendorIds", "�Ƽ��̼�")]
        public string RequiredVendorIds { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.PageSettings.HomeCategory.RequiredManufacturerIds", "�Ƽ�Ʒ��")]
        public string RequiredManufacturerIds { get; set; }

        public List<HomeCategoryItemModel> HomeCategoryItemModels { get; set; }

        public string CategoryIds { get; set; }
        /// <summary>
        /// ��¼�Ƿ��޸Ĺ�����
        /// </summary>
        public bool IsChangeCategory { get; set; }
    }

}
