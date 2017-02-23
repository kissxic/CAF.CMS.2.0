using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.ExtendAttributes;


namespace CAF.WebSite.Mvc.Admin.Models.ExtendedAttributes
{
    [Validator(typeof(ExtendedAttributeValidator))]
    public class ExtendedAttributeModel : EntityModelBase, ILocalizedModel<ExtendedAttributeLocalizedModel>
    {
        public ExtendedAttributeModel()
        {
            Locales = new List<ExtendedAttributeLocalizedModel>();
       
        }
        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ChannelId","所属频道")]
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }

        [LangResourceDisplayName("Common.IsActive")]
		public bool IsActive { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ExtendedAttributes.Fields.Name","字段名称")]
        [AllowHtml]
        public string Name { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ExtendedAttributes.Fields.Title","名称")]
        [AllowHtml]
        public string Title { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ExtendedAttributes.Fields.TextPrompt")]
        [AllowHtml]
        public string TextPrompt { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ExtendedAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Attributes.AttributeControlType")]
        [AllowHtml]
        public string AttributeControlTypeName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ExtendedAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        
        public IList<ExtendedAttributeLocalizedModel> Locales { get; set; }
    }

    public class ExtendedAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ExtendedAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ExtendedAttributes.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ExtendedAttributes.Fields.TextPrompt")]
        [AllowHtml]
        public string TextPrompt { get; set; }
    }
}