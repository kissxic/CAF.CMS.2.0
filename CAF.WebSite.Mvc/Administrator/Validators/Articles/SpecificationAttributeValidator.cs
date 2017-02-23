using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.Articles
{
	public partial class SpecificationAttributeValidator : AbstractValidator<SpecificationAttributeModel>
    {
        public SpecificationAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.Attributes.SpecificationAttributes.Fields.Name.Required"));
        }
    }
}