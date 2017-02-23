using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using FluentValidation;

namespace CAF.WebSite.Mvc.Admin.Validators.Articles
{
	public partial class ProductAttributeOptionValidator : AbstractValidator<ProductAttributeOptionModel>
    {
        public ProductAttributeOptionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.Attributes.ProductAttributes.Options.Fields.Name.Required"));
        }
    }
}