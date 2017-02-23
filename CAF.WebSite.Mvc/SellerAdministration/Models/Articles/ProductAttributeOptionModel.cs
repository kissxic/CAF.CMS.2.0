using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Mvc.Seller.Validators.Articles;
using CAF.WebSite.Application.WebUI;
using System.ComponentModel.DataAnnotations;

namespace CAF.WebSite.Mvc.Seller.Models.Articles
{

    public class ProductAttributeOptionModel : EntityModelBase, ILocalizedModel<ProductAttributeOptionLocalizedModel>
    {
        public ProductAttributeOptionModel()
        {
            Locales = new List<ProductAttributeOptionLocalizedModel>();
        }

        public int ProductAttributeId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Options.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Options.Fields.DisplayOrder", "排序")]
        public int DisplayOrder { get; set; }

        public IList<ProductAttributeOptionLocalizedModel> Locales { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Options..Fields.ColorSquaresRgb","颜色")]
        [AllowHtml, UIHint("Color")]
        public string ColorSquaresRgb { get; set; }
        // codehint: sm-add
        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Options.Fields.Multiple", "多选")]
        public bool Multiple { get; set; }
    }

    public class ProductAttributeOptionLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Attributes.ProductAttributes.Options.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }
    }
}