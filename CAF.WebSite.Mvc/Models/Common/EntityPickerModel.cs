using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Models.Common
{
	public class EntityPickerModel : ModelBase
	{
		public string AllString { get; set; }
		public string PublishedString { get; set; }
		public string UnpublishedString { get; set; }

		public string Entity { get; set; }
		public bool HighligtSearchTerm { get; set; }
		public string DisableIf { get; set; }
		public string SearchTerm { get; set; }
		public string ReturnField { get; set; }
		public int MaxReturnValues { get; set; }
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
        public string SelectedIds { get; set; }
        
        public List<SearchResultModel> SearchResult { get; set; }

		#region Articles

		[LangResourceDisplayName("Admin.ContentManagement.Dialog.List.SearchArticleName","名称")]
		public string ArticleName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Dialog.List.SearchChannel", "频道")]
        public int ChannelId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Dialog.List.SearchCategory","分类")]
		public int CategoryId { get; set; }

		[LangResourceDisplayName("Admin.ContentManagement.Dialog.List.SearchManufacturer", "供应商")]
		public int ManufacturerId { get; set; }

		[LangResourceDisplayName("Admin.ContentManagement.Dialog.List.SearchSite", "网站")]
		public int SiteId { get; set; }

		[LangResourceDisplayName("Admin.ContentManagement.Dialog.List.SearchArticleType", "类型")]
		public int ArticleTypeId { get; set; }

        public IList<SelectListItem> AvailableChannels { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
		public IList<SelectListItem> AvailableManufacturers { get; set; }
		public IList<SelectListItem> AvailableSites { get; set; }
		public IList<SelectListItem> AvailableArticleTypes { get; set; }

		#endregion

		public class SearchResultModel : EntityModelBase
		{
			public string ReturnValue { get; set; }
			public string Title { get; set; }
			public string Summary { get; set; }
			public string SummaryTitle { get; set; }
			public bool? Published { get; set; }
			public bool Disable { get; set; }
			public string ImageUrl { get; set; }
			public string LabelText { get; set; }
			public string LabelClassName { get; set; }
		}
	}
}