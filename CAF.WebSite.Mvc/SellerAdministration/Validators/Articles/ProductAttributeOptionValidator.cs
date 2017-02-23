using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Seller.Models.Articles;
using FluentValidation;

namespace CAF.WebSite.Mvc.Seller.Validators.Articles
{
	public partial class ProductAttributeOptionValidator : AbstractValidator<ProductAttributeOptionModel>
    {
        public ProductAttributeOptionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Seller.ContentManagement.Attributes.ProductAttributes.Options.Fields.Name.Required"));
        }
    }
}