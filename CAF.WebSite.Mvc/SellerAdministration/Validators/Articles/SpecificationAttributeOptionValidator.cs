﻿using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Seller.Models.Articles;
using FluentValidation;

namespace CAF.WebSite.Mvc.Seller.Validators.Articles
{
	public partial class SpecificationAttributeOptionValidator : AbstractValidator<SpecificationAttributeOptionModel>
    {
        public SpecificationAttributeOptionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Seller.ContentManagement.Attributes.SpecificationAttributes.Options.Fields.Name.Required"));
        }
    }
}