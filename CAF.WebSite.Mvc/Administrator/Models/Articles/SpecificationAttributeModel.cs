using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Articles;

namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    [Validator(typeof(SpecificationAttributeValidator))]
    public class SpecificationAttributeModel : EntityModelBase, ILocalizedModel<SpecificationAttributeLocalizedModel>
    {
        public SpecificationAttributeModel()
        {
            Locales = new List<SpecificationAttributeLocalizedModel>();
        }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.SpecificationAttributes.Fields.Name","名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.SpecificationAttributes.Fields.DisplayOrder", "排序")]
        public int DisplayOrder {get;set;}

        public IList<SpecificationAttributeLocalizedModel> Locales { get; set; }

		[LangResourceDisplayName("Admin.ContentManagement.Attributes.SpecificationAttributes.OptionsCount", "选项值")]
		public int OptionCount { get; set; }
    }

    public class SpecificationAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.SpecificationAttributes.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }
    }
}