using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Articles;

namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    public class ArticleSpecificationAttributeModel : EntityModelBase
    {
        public ArticleSpecificationAttributeModel()
        {
            this.SpecificationAttributeOptions = new List<SpecificationAttributeOption>();
        }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.SpecificationAttributes.Fields.SpecificationAttribute", "名称")]
        [AllowHtml]
        public string SpecificationAttributeName { get; set; }

        public int SpecificationAttributeOptionAttributeId { get; set; }

        public int SpecificationAttributeOptionId { get; set; }

        public string SpecificationAttributeOptionsJsonString { get; set; }

        public List<SpecificationAttributeOption> SpecificationAttributeOptions { get; set; }
        

        [LangResourceDisplayName("Admin.ContentManagement.Articles.SpecificationAttributes.Fields.SpecificationAttributeOption", "选项值")]
        public string SpecificationAttributeOptionName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.SpecificationAttributes.Fields.AllowFiltering", "允许筛选")]
        public bool AllowFiltering { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.SpecificationAttributes.Fields.ShowOnArticlePage", "显示在内容页")]
        public bool ShowOnArticlePage { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.SpecificationAttributes.Fields.DisplayOrder", "排序")]
        public int DisplayOrder { get; set; }

        #region Nested classes

        public partial class SpecificationAttributeOption : EntityModelBase
        {
            public int id { get; set; }

            public string name { get; set; }

            public string text { get; set; }

            public bool select { get; set; }
        }

        #endregion

    }
}