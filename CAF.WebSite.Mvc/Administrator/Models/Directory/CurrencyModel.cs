using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Directory;
using CAF.WebSite.Mvc.Admin.Models.Sites;

namespace CAF.WebSite.Mvc.Admin.Models.Directory
{
    [Validator(typeof(CurrencyValidator))]
    public class CurrencyModel : EntityModelBase, ILocalizedModel<CurrencyLocalizedModel>
    {
        public CurrencyModel()
        {
            Locales = new List<CurrencyLocalizedModel>();

			AvailableDomainEndings = new List<SelectListItem>
			{
				new SelectListItem { Text = ".com", Value = ".com" },
				new SelectListItem { Text = ".uk", Value = ".uk" },
				new SelectListItem { Text = ".de", Value = ".de" },
				new SelectListItem { Text = ".ch", Value = ".ch" }
			};
        }
        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.Name","名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyCode", "编码")]
        [AllowHtml]
        public string CurrencyCode { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayLocale", "显示区域")]
        [AllowHtml]
        public string DisplayLocale { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.Rate", "汇率")]
        public decimal Rate { get; set; }
        /// <summary>
        /// 应用于货币值的自定义格式设置。
        /// </summary>
        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.CustomFormatting", "自定义格式")]
        [AllowHtml]
        public string CustomFormatting { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.Published", "发布")]
        public bool Published { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayOrder", "排序")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.CreatedOn", "创建时间")]
        public DateTime CreatedOn { get; set; }

		public bool IsPrimarySiteCurrency { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Currencies.Fields.PrimarySiteCurrencySites", "默认")]
		public IList<SelectListItem> PrimarySiteCurrencySites { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Currencies.Fields.DomainEndings", "域名后缀")]
		public string DomainEndings { get; set; }
		public IList<SelectListItem> AvailableDomainEndings { get; set; }

        public IList<CurrencyLocalizedModel> Locales { get; set; }

		//Site mapping
		[LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
		public bool LimitedToSites { get; set; }

		[LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
		public List<SiteModel> AvailableSites { get; set; }
		public int[] SelectedSiteIds { get; set; }
    }

    public class CurrencyLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Currencies.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }
    }
}