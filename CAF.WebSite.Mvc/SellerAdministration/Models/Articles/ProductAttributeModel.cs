using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Mvc.Seller.Validators.Articles;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.Mvc.Seller.Models.Articles
{
    public class ProductAttributeModel : EntityModelBase, ILocalizedModel<ProductAttributeLocalizedModel>
    {
        public ProductAttributeModel()
        {
            Locales = new List<ProductAttributeLocalizedModel>();
        }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Fields.Alias", "别名")]
        public string Alias { get; set; }
        
        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Fields.Description", "描述")]
        [AllowHtml]
        public string Description {get;set;}
        


        public IList<ProductAttributeLocalizedModel> Locales { get; set; }

    }

    public class ProductAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Fields.Description", "描述")]
        [AllowHtml]
        public string Description {get;set;}
    }
}