

using CAF.WebSite.Application.Services.Filter;
using CAF.WebSite.Application.WebUI.Mvc;

namespace CAF.WebSite.Mvc.Models.Filter
{
	public partial class ArticleFilterModel : ModelBase
	{
		public FilterArticleContext Context { get; set; }

        public bool IsShowAllText { get; set; }

        public int MaxFilterItemsToDisplay { get; set; }

        public bool ExpandAllFilterGroups { get; set; }
	}
}