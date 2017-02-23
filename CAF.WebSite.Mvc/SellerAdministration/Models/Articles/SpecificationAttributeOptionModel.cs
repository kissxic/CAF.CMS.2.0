using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Seller.Validators.Articles;

namespace CAF.WebSite.Mvc.Seller.Models.Articles
{
    [Validator(typeof(SpecificationAttributeOptionValidator))]
    public class SpecificationAttributeOptionModel : EntityModelBase, ILocalizedModel<SpecificationAttributeOptionLocalizedModel>
    {
        public SpecificationAttributeOptionModel()
        {
            Locales = new List<SpecificationAttributeOptionLocalizedModel>();
        }

        public int SpecificationAttributeId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.SpecificationAttributes.Options.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.SpecificationAttributes.Options.Fields.DisplayOrder", "排序")]
        public int DisplayOrder {get;set;}
        
        public IList<SpecificationAttributeOptionLocalizedModel> Locales { get; set; }

		// codehint: sm-add
		[LangResourceDisplayName("Admin.ContentManagement.Attributes.SpecificationAttributes.Options.Fields.Multiple", "多选")]
		public bool Multiple { get; set; }
    }

    public class SpecificationAttributeOptionLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.SpecificationAttributes.Options.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }
    }
}