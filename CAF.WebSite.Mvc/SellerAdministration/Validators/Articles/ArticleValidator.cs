
using FluentValidation;
using CAF.WebSite.Mvc.Seller.Models.Articles;
using CAF.WebSite.Application.Services.Localization;

namespace CAF.WebSite.Mvc.Seller.Validators.Articles
{

    public class ArticleValidator : AbstractValidator<ArticleModel>
    {
        public ArticleValidator(ILocalizationService localizationService)
        {

            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Seller.ContentManagement.Articles.Fields.Title.Required"));
            RuleFor(x => x.FullContent)
            .NotNull()
            .WithMessage(localizationService.GetResource("Seller.ContentManagement.Articles.Fields.FullContent.Required"));

        }
    }
}