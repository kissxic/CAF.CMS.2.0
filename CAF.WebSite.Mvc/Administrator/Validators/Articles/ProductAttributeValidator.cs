using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using FluentValidation;

namespace CAF.WebSite.Mvc.Admin.Validators.Articles
{
	public partial class ProductAttributeValidator : AbstractValidator<ProductAttributeModel>
    {
        public ProductAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.Attributes.ProductAttributes.Fields.Name.Required"));
        }
    }
}