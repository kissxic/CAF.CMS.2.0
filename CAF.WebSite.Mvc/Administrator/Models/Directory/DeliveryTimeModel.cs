using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using  CAF.WebSite.Mvc.Admin.Validators.Directory;

using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;

namespace  CAF.WebSite.Mvc.Admin.Models.Directory
{
    [Validator(typeof(DeliveryTimeValidator))]
    public class DeliveryTimeModel : EntityModelBase, ILocalizedModel<DeliveryTimeLocalizedModel>
    {
        public DeliveryTimeModel()
        {
            Locales = new List<DeliveryTimeLocalizedModel>();
        }

        [LangResourceDisplayName("Admin.Configuration.DeliveryTimes.Fields.Name","名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Configuration.DeliveryTimes.Fields.DisplayLocale")]
        [AllowHtml]
        public string DisplayLocale { get; set; }

        [LangResourceDisplayName("Admin.Configuration.DeliveryTimes.Fields.Color", "颜色标识")]
        [AllowHtml]
        public string ColorHexValue { get; set; }

        [LangResourceDisplayName("Admin.Configuration.DeliveryTimes.Fields.DisplayOrder", "排序")]
        public int DisplayOrder { get; set; }

        public IList<DeliveryTimeLocalizedModel> Locales { get; set; }
    }

    public class DeliveryTimeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.Configuration.DeliveryTimes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}