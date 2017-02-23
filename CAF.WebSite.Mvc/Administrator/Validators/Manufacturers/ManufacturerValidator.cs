using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Manufacturers;
using FluentValidation;

namespace CAF.WebSite.Mvc.Admin.Validators.Manufacturers
{
	public partial class ManufacturerValidator : AbstractValidator<ManufacturerModel>
    {
        public ManufacturerValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.Manufacturers.Fields.Name.Required"));
        }
    }
}