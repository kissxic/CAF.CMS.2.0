using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Members;
using FluentValidation;




namespace CAF.WebSite.Mvc.Admin.Validators.Members
{
    public partial class MemberGradeValidator : AbstractValidator<MemberGradeModel>
    {
        public MemberGradeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SystemName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Members.Fields.SystemName.Required"));
            RuleFor(x => x.GradeName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Members.Fields.GradeName.Required"));


        }
    }
}