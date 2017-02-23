using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using Newtonsoft.Json;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Articles;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.ComponentModel;
using CAF.Infrastructure.Core.Domain.Cms.Manufacturers;

namespace CAF.WebSite.Application.Services.Filter
{
    public partial class FilterService : IFilterService
    {
        private const string _defaultType = "String";
        private readonly IArticleService _articleService;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IArticleCategoryService _categoryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<ArticleCategory> _articleCategoryRepository;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ICommonServices _services;

        private IQueryable<Article> _articles;

        public FilterService(IArticleService articleService,
            IRepository<Manufacturer> manufacturerRepository,
            IArticleCategoryService categoryService,
            IProductCategoryService productCategoryService,
            ArticleCatalogSettings catalogSettings,
            IRepository<Article> articleRepository,
            IRepository<ArticleCategory> articleCategoryRepository,
            ILocalizedEntityService localizedEntityService,
            ICommonServices services)
        {
            _articleService = articleService;
            _manufacturerRepository = manufacturerRepository;
            _categoryService = categoryService;
            _productCategoryService = productCategoryService;
            _catalogSettings = catalogSettings;
            _articleRepository = articleRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _localizedEntityService = localizedEntityService;
            _services = services;
        }

        public static string ShortcutPrice { get { return "_Price"; } }
        public static string ShortcutSpecAttribute { get { return "_SpecId"; } }

        // helper
        private string ValidateValue(string value, string alternativeValue)
        {
            if (value.HasValue() && !value.IsCaseInsensitiveEqual("null"))
                return value;

            return alternativeValue;
        }

        private string FormatParameterIndex(ref int index)
        {
            //if (curlyBracketFormatting)
            //	return "{0}{1}{2}".FormatWith('{', index++, '}');

            return "@{0}".FormatWith(index++);
        }

        private object FilterValueToObject(string value, string type)
        {
            if (value == null)
                value = "";

            //if (value == "__UtcNow__")
            //	return DateTime.UtcNow;

            //if (value == "__Now__")
            //	return DateTime.Now;

            //if (curlyBracketFormatting)
            //	return value.FormatWith("\"{0}\"", value.Replace("\"", "\"\""));

            Type t = Type.GetType("System.{0}".FormatInvariant(ValidateValue(type, _defaultType)));

            var result = TypeConverterFactory.GetConverter(t).ConvertFrom(CultureInfo.InvariantCulture, value);

            return result;
        }

        private bool IsShortcut(FilterSql context, FilterCriteria itm, ref int index)
        {
            if (itm.Entity.IsCaseInsensitiveEqual(ShortcutPrice))
            {
                // TODO: where clause of special price not correct. article can appear in price and in special price range.

                if (itm.IsRange)
                {
                    string valueLeft, valueRight;
                    itm.Value.SplitToPair(out valueLeft, out valueRight, "~");

                    context.WhereClause.AppendFormat("((Price >= {0} And Price {1} {2}) Or (SpecialPrice >= {0} And SpecialPrice {1} {2} And SpecialPriceStartDateTimeUtc <= {3} And SpecialPriceEndDateTimeUtc >= {3}))",
                        FormatParameterIndex(ref index),
                        itm.Operator == FilterOperator.RangeGreaterEqualLessEqual ? "<=" : "<",
                        FormatParameterIndex(ref index),
                        FormatParameterIndex(ref index)
                    );

                    context.Values.Add(FilterValueToObject(valueLeft, itm.Type ?? "Decimal"));
                    context.Values.Add(FilterValueToObject(valueRight, itm.Type ?? "Decimal"));
                    context.Values.Add(DateTime.UtcNow);
                }
                else
                {
                    context.WhereClause.AppendFormat("(Price {0} {1} Or (SpecialPrice {0} {1} And SpecialPriceStartDateTimeUtc <= {2} And SpecialPriceEndDateTimeUtc >= {2}))",
                        itm.Operator == null ? "=" : itm.Operator.ToString(),
                        FormatParameterIndex(ref index),
                        FormatParameterIndex(ref index));

                    context.Values.Add(FilterValueToObject(itm.Value, itm.Type ?? "Decimal"));
                    context.Values.Add(DateTime.UtcNow);
                }
            }
            else if (itm.Entity.IsCaseInsensitiveEqual(ShortcutSpecAttribute))
            {
                context.WhereClause.AppendFormat("SpecificationAttributeOptionId {0} {1}", itm.Operator == null ? "=" : itm.Operator.ToString(), FormatParameterIndex(ref index));

                context.Values.Add(itm.ID ?? 0);
            }
            else
            {
                return false;
            }
            return true;
        }

        private void FilterParentheses(List<FilterCriteria> criteria)
        {
            // Logical or combine all criteria with same name.
            //
            // "The order of precedence for the logical operators is NOT (highest), followed by AND, followed by OR.
            // The order of evaluation at the same precedence level is from left to right."
            // http://www.databasedev.co.uk/sql-multiple-conditions.html

            if (criteria.Count > 0)
            {
                criteria.Sort();
                criteria.ForEach(c => { c.Open = null; c.Or = false; });

                var data = (
                    from c in criteria
                    group c by c.Entity).Where(g => g.Count() > 1);
                //group c by c.Name).Where(g => g.Count() > 1);

                foreach (var grp in data)
                {
                    grp.ToList().ForEach(f => f.Or = true);
                    grp.First().Or = false;
                    grp.First().Open = true;
                    grp.Last().Open = false;
                }
            }
        }

        private IQueryable<Article> AllArticles(List<int> categoryIds, List<int> productCategoryIds)
        {
            if (_articles == null)
            {
                var searchContext = new ArticleSearchContext
                {
                    Query = _articleRepository.TableUntracked,
                    FeaturedArticles = (_catalogSettings.IncludeFeaturedArticlesInNormalLists ? null : (bool?)false),
                    SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode,
                    VisibleIndividuallyOnly = true
                };
                //类别组装添加
                //if (categoryIds != null && categoryIds.Count > 1)
                //{
                //    _articles = _articleService.PrepareArticleSearchQuery(searchContext);

                //    //var distinctIds = (
                //    //	from p in _articleRepository.TableUntracked
                //    //	join pc in _articleCategoryRepository.TableUntracked on p.Id equals pc.Articles
                //    //	where categoryIds.Contains(pc.CategoryId)
                //    //	select p.Id).Distinct();

                //    _articles = _articles.Where(p => categoryIds.Contains(p.CategoryId));


                //}
                //else
                //{
                searchContext.CategoryIds = categoryIds;
                searchContext.ProductCategoryIds = productCategoryIds;
                _articles = _articleService.PrepareArticleSearchQuery(searchContext);
                // }
            }
            return _articles;
        }



        private List<FilterCriteria> ArticleFilterablePrices(FilterArticleContext context)
        {
            var result = new List<FilterCriteria>();
            FilterCriteria criteria;
            ArticleCategory category;
            ProductCategory productCategory;
            var tmp = new FilterArticleContext
            {
                ParentCategoryID = context.ParentCategoryID,
                CategoryIds = context.CategoryIds,
                ParentProductCategoryID = context.ParentProductCategoryID,
                ProductCategoryIds = context.ProductCategoryIds,
                Criteria = new List<FilterCriteria>()
            };

            if (context.ParentCategoryID != 0 && (category = _categoryService.GetArticleCategoryById(context.ParentCategoryID)) != null && category.PriceRanges.HasValue())
            {
                string[] ranges = category.PriceRanges.SplitSafe(";");

                foreach (string range in ranges)
                {
                    if ((criteria = range.ParsePriceString()) != null)
                    {
                        tmp.Criteria.Clear();
                        tmp.Criteria.AddRange(context.Criteria);
                        tmp.Criteria.Add(criteria);

                        try
                        {
                            criteria.MatchCount = ArticleFilter(tmp).Count();
                        }
                        catch (Exception exc)
                        {
                            exc.Dump();
                        }

                        if (criteria.MatchCount > 0)
                            result.Add(criteria);
                    }
                }
            }

            if (context.ParentProductCategoryID != 0 && (productCategory = _productCategoryService.GetProductCategoryById(context.ParentProductCategoryID)) != null && productCategory.PriceRanges.HasValue())
            {
                string[] ranges = productCategory.PriceRanges.SplitSafe(";");

                foreach (string range in ranges)
                {
                    if ((criteria = range.ParsePriceString()) != null)
                    {
                        tmp.Criteria.Clear();
                        tmp.Criteria.AddRange(context.Criteria);
                        tmp.Criteria.Add(criteria);

                        try
                        {
                            criteria.MatchCount = ArticleFilter(tmp).Count();
                        }
                        catch (Exception exc)
                        {
                            exc.Dump();
                        }

                        if (criteria.MatchCount > 0)
                            result.Add(criteria);
                    }
                }
            }

            result.ForEach(c => c.IsInactive = true);
            return result;
        }

        /// <summary>
        /// 所有供应商筛选条件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        private List<FilterCriteria> ArticleFilterableManufacturer(FilterArticleContext context, bool getAll = false)
        {
            var query = ArticleFilter(context);

            //根据内容查询条件获取供应商
            var manus =
            from p in query
            join pm in _manufacturerRepository.Table on p.ManufacturerId equals pm.Id
            where !pm.Deleted
            select pm;

            var grouped =
                from m in manus
                group m by m.Id into grp
                orderby grp.Key
                select new FilterCriteria
                {
                    MatchCount = grp.Count(),
                    Value = grp.FirstOrDefault().Name,
                    DisplayOrder = grp.FirstOrDefault().DisplayOrder
                };
            //获取所有供应商
            //var manus = _manufacturerRepository.Table.ToList().Select(x =>
            //{
            //    return new FilterCriteria()
            //    {
            //        MatchCount = 1,
            //        Value = x.Name,
            //        DisplayOrder = x.DisplayOrder
            //    };
            //});

            if (_catalogSettings.SortFilterResultsByMatches)
            {
                grouped = grouped.OrderByDescending(m => m.MatchCount);
            }
            else
            {
                grouped = grouped.OrderBy(m => m.DisplayOrder);
            }

            if (!getAll)
            {
                grouped = grouped.Take(_catalogSettings.MaxFilterItemsToDisplay);
            }

            var lst = grouped.ToList();

            lst.ForEach(c =>
            {
                c.Name = "Name";
                c.Entity = "Manufacturer";
                c.IsInactive = true;
            });

            return lst;
        }
        /// <summary>
        /// 根据当前所有内容的属性组装条件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private List<FilterCriteria> ArticleFilterableSpecAttributes(FilterArticleContext context, string attributeName = null)
        {
            List<FilterCriteria> criterias = null;
            var languageId = _services.WorkContext.WorkingLanguage.Id;
            var query = ArticleFilter(context);

            var attributes =
                from p in query
                from sa in p.ArticleSpecificationAttributes
                where sa.AllowFiltering
                select sa.SpecificationAttributeOption;

            if (attributeName.HasValue())
            {
                attributes = attributes.Where(a => a.SpecificationAttribute.Name == attributeName);
            }

            var grouped =
                from a in attributes
                group a by new { a.SpecificationAttributeId, a.Id } into g
                select new FilterCriteria
                {
                    Name = g.FirstOrDefault().SpecificationAttribute.Name,
                    Value = g.FirstOrDefault().Name,
                    ID = g.Key.Id,
                    PId = g.FirstOrDefault().SpecificationAttribute.Id,
                    MatchCount = g.Count(),
                    DisplayOrder = g.FirstOrDefault().SpecificationAttribute.DisplayOrder,
                    DisplayOrderValues = g.FirstOrDefault().DisplayOrder
                };

            if (_catalogSettings.SortFilterResultsByMatches)
            {
                criterias = grouped
                    .OrderBy(a => a.DisplayOrder)
                    .ThenByDescending(a => a.MatchCount)
                    .ThenBy(a => a.DisplayOrderValues)
                    .ToList();
            }
            else
            {
                criterias = grouped
                    .OrderBy(a => a.DisplayOrder)
                    .ThenBy(a => a.DisplayOrderValues)
                    .ToList();
            }

            criterias.ForEach(c =>
            {
                c.Entity = ShortcutSpecAttribute;
                c.IsInactive = true;

                if (c.PId.HasValue)
                    c.NameLocalized = _localizedEntityService.GetLocalizedValue(languageId, c.PId.Value, "SpecificationAttribute", "Name");

                if (c.ID.HasValue)
                    c.ValueLocalized = _localizedEntityService.GetLocalizedValue(languageId, c.ID.Value, "SpecificationAttributeOption", "Name");
            });

            return criterias;
        }
        /// <summary>
        /// 反序列化查询提交
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public virtual List<FilterCriteria> Deserialize(string jsonData)
        {
            if (jsonData.HasValue())
            {
                if (jsonData.StartsWith("["))
                {
                    return JsonConvert.DeserializeObject<List<FilterCriteria>>(jsonData);
                }

                return new List<FilterCriteria> { JsonConvert.DeserializeObject<FilterCriteria>(jsonData) };
            }
            return new List<FilterCriteria>();
        }
        /// <summary>
        /// 序列化查询条件
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual string Serialize(List<FilterCriteria> criteria)
        {
            //criteria.FindAll(c => c.Type.IsNullOrEmpty()).ForEach(c => c.Type = _defaultType);
            if (criteria != null && criteria.Count > 0)
                return JsonConvert.SerializeObject(criteria);

            return "";
        }
        /// <summary>
        /// 创建一个内容筛选条件场景
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="categoryID"></param>
        /// <param name="productCategoryID"></param>
        /// <param name="path"></param>
        /// <param name="pagesize"></param>
        /// <param name="orderby"></param>
        /// <param name="viewmode"></param>
        /// <returns></returns>
        public virtual FilterArticleContext CreateFilterArticleContext(string filter, int categoryID, int productCategoryID, string path, int? pagesize, int? orderby, string viewmode)
        {
            var context = new FilterArticleContext
            {
                Filter = filter,
                ParentCategoryID = categoryID,
                CategoryIds = categoryID == 0 ? new List<int>() : new List<int> { categoryID },
                ParentProductCategoryID = productCategoryID,
                ProductCategoryIds = new List<int> { productCategoryID },
                Path = path,
                PageSize = pagesize ?? 12,
                ViewMode = viewmode,
                OrderBy = orderby,
                Criteria = Deserialize(filter)
            };

   
            int languageId = _services.WorkContext.WorkingLanguage.Id;

            foreach (var criteria in context.Criteria.Where(x => x.Entity.IsCaseInsensitiveEqual(ShortcutSpecAttribute)))
            {
                if (criteria.PId.HasValue)
                    criteria.NameLocalized = _localizedEntityService.GetLocalizedValue(languageId, criteria.PId.Value, "SpecificationAttribute", "Name");

                if (criteria.ID.HasValue)
                    criteria.ValueLocalized = _localizedEntityService.GetLocalizedValue(languageId, criteria.ID.Value, "SpecificationAttributeOption", "Name");
            }

            return context;
        }

        public virtual bool ToWhereClause(FilterSql context)
        {
            if (context.Values == null)
                context.Values = new List<object>();
            else
                context.Values.Clear();

            if (context.WhereClause == null)
                context.WhereClause = new StringBuilder();
            else
                context.WhereClause.Clear();

            int index = 0;

            FilterParentheses(context.Criteria);

            foreach (var itm in context.Criteria)
            {
                if (context.WhereClause.Length > 0)
                    context.WhereClause.AppendFormat(" {0} ", itm.Or ? "Or" : "And");

                if (itm.Open.HasValue && itm.Open.Value)
                    context.WhereClause.Append("(");

                if (IsShortcut(context, itm, ref index))
                {
                }
                else if (itm.IsRange)
                {
                    string valueLeft, valueRight;
                    itm.Value.SplitToPair(out valueLeft, out valueRight, "~");

                    context.WhereClause.AppendFormat("({0} >= {1} And {0} {2} {3})",
                        itm.SqlName,
                        FormatParameterIndex(ref index),
                        itm.Operator == FilterOperator.RangeGreaterEqualLessEqual ? "<=" : "<",
                        FormatParameterIndex(ref index)
                    );

                    context.Values.Add(FilterValueToObject(valueLeft, itm.Type));
                    context.Values.Add(FilterValueToObject(valueRight, itm.Type));
                }
                else if (itm.Value.IsEmpty())
                {
                    context.WhereClause.AppendFormat("ASCII({0}) Is Null", itm.SqlName);        // true if null or empty (string)
                }
                else
                {
                    context.WhereClause.Append(itm.SqlName);

                    if (itm.Operator == FilterOperator.Contains)
                        context.WhereClause.Append(".Contains(");
                    else if (itm.Operator == FilterOperator.StartsWith)
                        context.WhereClause.Append(".StartsWith(");
                    else if (itm.Operator == FilterOperator.EndsWith)
                        context.WhereClause.Append(".EndsWith(");
                    else
                        context.WhereClause.AppendFormat(" {0} ", itm.Operator == null ? "=" : itm.Operator.ToString());

                    context.WhereClause.Append(FormatParameterIndex(ref index));

                    if (itm.Operator == FilterOperator.Contains || itm.Operator == FilterOperator.StartsWith || itm.Operator == FilterOperator.EndsWith)
                        context.WhereClause.Append(")");

                    context.Values.Add(FilterValueToObject(itm.Value, itm.Type));
                }

                if (itm.Open.HasValue && !itm.Open.Value)
                    context.WhereClause.Append(")");
            }
            return (context.WhereClause.Length > 0);
        }

        public virtual bool ToWhereClause(FilterSql context, List<FilterCriteria> findIn, Predicate<FilterCriteria> match)
        {
            if (context.Criteria != null)
                context.Criteria.Clear();   // !

            context.Criteria = findIn.FindAll(match);

            return ToWhereClause(context);
        }

        public virtual IQueryable<Article> ArticleFilter(FilterArticleContext context)
        {
            var sql = new FilterSql();
            var query = AllArticles(context.CategoryIds, context.ProductCategoryIds);

            // 价格条件
            if (ToWhereClause(sql, context.Criteria, c => !c.IsInactive && c.Entity.IsCaseInsensitiveEqual(ShortcutPrice)))
            {
                query = query.Where(sql.WhereClause.ToString(), sql.Values.ToArray());
            }

            // 供应商条件
            if (ToWhereClause(sql, context.Criteria, c => !c.IsInactive && c.Entity.IsCaseInsensitiveEqual("Manufacturer")))
            {
                // var manus = _manufacturerRepository.TableUntracked.Where(sql.WhereClause.ToString(), sql.Values.ToArray())
                //  .Select(pm => pm.Id).ToList();
                //query = query.Where(p => p.ManufacturerId.HasValue && manus.Contains(p.ManufacturerId.Value));
                query =
                  from p in query
                  join mf in _manufacturerRepository.Table on p.ManufacturerId equals mf.Id
                  select p;
                query = query.Where(sql.WhereClause.ToString(), sql.Values.ToArray());
            }

            // 属性条件
            if (ToWhereClause(sql, context.Criteria, c => !c.IsInactive && (c.Entity.IsCaseInsensitiveEqual("SpecificationAttributeOption") || c.Entity.IsCaseInsensitiveEqual(ShortcutSpecAttribute))))
            {
                //var saq = (
                //	from p in query
                //	from sa in p.ArticleSpecificationAttributes
                //	select sa).Where(a => a.AllowFiltering).Where(sql.WhereClause.ToString(), sql.Values.ToArray());

                //query = saq.Select(sa => sa.Article);

                int countSameNameAttributes = sql.Criteria
                    .Where(c => c.Entity.IsCaseInsensitiveEqual(ShortcutSpecAttribute))
                    .GroupBy(c => c.Name)
                    .Count();

                var specRepository = EngineContext.Current.Resolve<IRepository<ArticleSpecificationAttribute>>();

                var saq = specRepository.TableUntracked
                    .Where(a => a.AllowFiltering)
                    .Where(sql.WhereClause.ToString(), sql.Values.ToArray())
                    .GroupBy(a => a.ArticleId)
                    .Where(grp => (grp.Count() >= countSameNameAttributes));

                query =
                    from p in query
                    join sa in saq on p.Id equals sa.Key
                    select p;
            }

            // 排序
            var order = (ArticleSortingEnum)(context.OrderBy ?? 0);
            switch (order)
            {
                case ArticleSortingEnum.NameDesc:
                    query = query.OrderByDescending(p => p.Title);
                    break;
                case ArticleSortingEnum.PriceAsc:
                    query = query.OrderBy(p => p.Price);
                    break;
                case ArticleSortingEnum.PriceDesc:
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case ArticleSortingEnum.CreatedOn:
                    query = query.OrderByDescending(p => p.CreatedOnUtc);
                    break;
                case ArticleSortingEnum.CreatedOnAsc:
                    query = query.OrderBy(p => p.CreatedOnUtc);
                    break;
                default:
                    query = query.OrderBy(p => p.Title);
                    break;
            }

            // distinct cause same articles can be mapped to sub-categories... too slow
            //query =
            //	from p in query
            //	group p by p.Id into grp
            //	orderby grp.Key
            //	select grp.FirstOrDefault();

            //query.ToString().Dump();
            return query;
        }

        public virtual void ArticleFilterable(FilterArticleContext context)
        {
            if (context.Criteria.FirstOrDefault(c => c.Entity.IsCaseInsensitiveEqual(FilterService.ShortcutPrice)) == null)
                context.Criteria.AddRange(ArticleFilterablePrices(context));

            if (context.Criteria.FirstOrDefault(c => c.Name.IsCaseInsensitiveEqual("Name") && c.Entity.IsCaseInsensitiveEqual("Manufacturer")) == null)
                context.Criteria.AddRange(ArticleFilterableManufacturer(context));

            context.Criteria.AddRange(ArticleFilterableSpecAttributes(context));
        }

        public virtual void ArticleFilterableMultiSelect(FilterArticleContext context, string filterMultiSelect)
        {
            var criteriaMultiSelect = Deserialize(filterMultiSelect).FirstOrDefault();
            List<FilterCriteria> inactive = null;

            if (criteriaMultiSelect != null)
            {
                context.Criteria.RemoveAll(c => c.Name.IsCaseInsensitiveEqual(criteriaMultiSelect.Name) && c.Entity.IsCaseInsensitiveEqual(criteriaMultiSelect.Entity));

                if (criteriaMultiSelect.Name.IsCaseInsensitiveEqual("Name") && criteriaMultiSelect.Entity.IsCaseInsensitiveEqual("Manufacturer"))
                    inactive = ArticleFilterableManufacturer(context, true);
                else if (criteriaMultiSelect.Entity.IsCaseInsensitiveEqual(FilterService.ShortcutPrice))
                    inactive = ArticleFilterablePrices(context);
                else if (criteriaMultiSelect.Entity.IsCaseInsensitiveEqual(FilterService.ShortcutSpecAttribute))
                    inactive = ArticleFilterableSpecAttributes(context, criteriaMultiSelect.Name);
            }

            // filters WITHOUT the multiple selectable filters
            var excludedFilter = Serialize(context.Criteria);

            // filters WITH the multiple selectable filters (required for highlighting selected values)
            context.Criteria = Deserialize(context.Filter);

            context.Filter = excludedFilter;

            if (inactive != null)
            {
                inactive.ForEach(c => c.IsInactive = true);
                context.Criteria.AddRange(inactive);
            }
        }
    }
}

