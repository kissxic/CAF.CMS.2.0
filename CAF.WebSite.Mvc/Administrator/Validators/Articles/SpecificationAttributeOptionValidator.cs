using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using FluentValidation;

namespace CAF.WebSite.Mvc.Admin.Validators.Articles
{
	public partial class SpecificationAttributeOptionValidator : AbstractValidator<SpecificationAttributeOptionModel>
    {
        public SpecificationAttributeOptionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.Attributes.SpecificationAttributes.Options.Fields.Name.Required"));
        }
    }
}