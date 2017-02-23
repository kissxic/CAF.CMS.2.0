using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Filter
{
	public partial interface IFilterService
	{
		List<FilterCriteria> Deserialize(string jsonData);
		string Serialize(List<FilterCriteria> criteria);

		FilterArticleContext CreateFilterArticleContext(string filter, int categoryID, int productCategoryID, string path, int? pagesize, int? orderby, string viewmode);

		bool ToWhereClause(FilterSql context);
		bool ToWhereClause(FilterSql context, List<FilterCriteria> findIn, Predicate<FilterCriteria> match);

		IQueryable<Article> ArticleFilter(FilterArticleContext context);

		void ArticleFilterable(FilterArticleContext context);
		void ArticleFilterableMultiSelect(FilterArticleContext context, string filterMultiSelect);
	}
}
