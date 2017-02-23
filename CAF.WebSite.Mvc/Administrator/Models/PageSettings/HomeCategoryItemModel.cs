using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Articles;

namespace CAF.WebSite.Mvc.Admin.Models.PageSettings
{
    
    public class HomeCategoryItemModel : EntityModelBase
    {
        public HomeCategoryItemModel()
        {
             
        }
        [LangResourceDisplayName("Admin.ContentManagement.HomeCategoryItem.HomeCategoryId", "名称")]
        public int HomeCategoryId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.HomeCategoryItem.CategoryId", "名称")]
        public int CategoryId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.HomeCategoryItem.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.HomeCategoryItem.DisplayOrder", "排序")]
        public int DisplayOrder {get;set;}

        [LangResourceDisplayName("Admin.ContentManagement.HomeCategoryItem.Depth", "排序")]
        public int Depth { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.HomeCategoryItem.RowNumber", "排序")]
        public int RowNumber { get; set; }

    }


}