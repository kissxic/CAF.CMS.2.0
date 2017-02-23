using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Seller.Models.Articles;
using FluentValidation;
 

namespace CAF.WebSite.Mvc.Seller.Validators.Articles
{
    public partial class ArticleTagValidator : AbstractValidator<ArticleTagModel>
    {
        public ArticleTagValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Seller.ContentManagement.Articles.Fields.Name.Required"));
        }
    }
}