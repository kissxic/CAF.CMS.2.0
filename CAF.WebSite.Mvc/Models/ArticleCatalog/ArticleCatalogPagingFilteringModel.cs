using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Models.ArticleCatalog
{
    // 类别筛选条件（界面）
    public partial class ArticleCatalogPagingFilteringModel : PagingFilteringModel //BasePageableModel
    {
        #region Constructors

        public ArticleCatalogPagingFilteringModel()
        {
            this.PriceRangeFilter = new PriceRangeFilterModel();
            this.SpecificationFilter = new SpecificationFilterModel();
            this.BaseFilter = new BaseFilterModel();
        }

        #endregion

        #region Properties
        /// <summary>
        /// base filter model
        /// </summary>
        public BaseFilterModel BaseFilter { get; set; }
        /// <summary>
        /// Price range filter model
        /// </summary>
        public PriceRangeFilterModel PriceRangeFilter { get; set; }

        /// <summary>
        /// Specification filter model
        /// </summary>
        public SpecificationFilterModel SpecificationFilter { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsTop")]
        public bool? IsTop { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsRed")]
        public bool? IsRed { get; set; }
        /// <summary>
        /// 是否热门
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsHot")]
        public bool? IsHot { get; set; }
        /// <summary>
        /// 是否幻灯片
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsSlide")]
        public bool? IsSlide { get; set; }
        /// <summary>
        /// 是否最新
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.IsNew")]
        public bool? IsNew { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        [LangResourceDisplayName("ArticleCategories.AreaId")]
        public int? AreaId { get; set; }

        /// <summary>
        /// Query string
        /// </summary>
        [LangResourceDisplayName("Search.SearchTerm")]
        [AllowHtml]
        public string Q { get; set; }
        /// <summary>
        #endregion

        #region Nested classes

        public partial class BaseFilterModel : ModelBase
        {
            #region Const

            private const string QUERYSTRINGPARAM = "areaId";
            private const string SPECQUERYSTRINGPARAM = "specs";
            #endregion 

            #region Ctor

            public BaseFilterModel()
            {
                this.Items = new List<BaseFilterItem>();
            }

            #endregion

            #region Utilities



            protected virtual string ExcludeQueryStringParams(string url, IWebHelper webHelper)
            {
                var excludedQueryStringParams = "pagenumber"; //remove page filtering
                if (!String.IsNullOrEmpty(excludedQueryStringParams))
                {
                    string[] excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string exclude in excludedQueryStringParamsSplitted)
                        url = webHelper.RemoveQueryString(url, exclude);
                }

                return url;
            }

            #endregion

            #region Methods


            public virtual void LoadBaseFilters(int areaId, IStateProvinceService stateProvinceService, bool isHasSpe, IWebHelper webHelper)
            {

                this.Enabled = true;
                var global = webHelper.GetThisPageUrl(true);
                if (!isHasSpe)
                    global = webHelper.RemoveQueryString(global, SPECQUERYSTRINGPARAM);
                var selectedPriceRange = areaId;
                var states = stateProvinceService.GetStateProvincesByCountryId(23, true);
                var allArea = new BaseFilterItem()
                {
                    Name = "全国",
                    Value = "0",
                    Selected = 0 == areaId,
                };
                string allUrl = webHelper.ModifyQueryString(global, QUERYSTRINGPARAM + "=" + allArea.Value, null);

                allUrl = ExcludeQueryStringParams(allUrl, webHelper);
                allArea.FilterUrl = allUrl;
                this.Items.Add(allArea);
                this.Items.AddRange(states.ToList().Select(x =>
                {
                    var item = new BaseFilterItem();
                    item.Name = x.Name;
                    item.Value = x.Id.ToString();
                    item.Selected = x.Id == areaId;
                    string url = webHelper.ModifyQueryString(global, QUERYSTRINGPARAM + "=" + x.Id, null);

                    url = ExcludeQueryStringParams(url, webHelper);
                    item.FilterUrl = url;
                    return item;
                }).ToList());


                //remove filter URL
                string removeFilterUrl = webHelper.RemoveQueryString(global, QUERYSTRINGPARAM);

                this.RemoveFilterUrl = removeFilterUrl;

            }

            #endregion

            #region Properties
            public bool Enabled { get; set; }
            public IList<BaseFilterItem> Items { get; set; }
            public string RemoveFilterUrl { get; set; }

            #endregion
        }

        public partial class BaseFilterItem : ModelBase
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string FilterUrl { get; set; }
            public bool Selected { get; set; }
        }

        public partial class PriceRangeFilterModel : ModelBase
        {
            #region Const

            private const string QUERYSTRINGPARAM = "price";

            #endregion

            #region Ctor

            public PriceRangeFilterModel()
            {
                this.Items = new List<PriceRangeFilterItem>();
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Gets parsed price ranges
            /// </summary>
            protected virtual IList<PriceRange> GetPriceRangeList(string priceRangesStr)
            {
                var priceRanges = new List<PriceRange>();
                if (string.IsNullOrWhiteSpace(priceRangesStr))
                    return priceRanges;
                string[] rangeArray = priceRangesStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str1 in rangeArray)
                {
                    string[] fromTo = str1.Trim().Split(new char[] { '-' });

                    if (fromTo.Length > 1)
                    {
                        decimal? from = null;
                        if (!String.IsNullOrEmpty(fromTo[0]) && !String.IsNullOrEmpty(fromTo[0].Trim()))
                            from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("en-US"));

                        decimal? to = null;
                        if (!String.IsNullOrEmpty(fromTo[1]) && !String.IsNullOrEmpty(fromTo[1].Trim()))
                            to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("en-US"));

                        priceRanges.Add(new PriceRange() { From = from, To = to });
                    }
                }
                return priceRanges;
            }

            protected virtual string ExcludeQueryStringParams(string url, IWebHelper webHelper)
            {
                var excludedQueryStringParams = "pagenumber"; //remove page filtering
                if (!String.IsNullOrEmpty(excludedQueryStringParams))
                {
                    string[] excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string exclude in excludedQueryStringParamsSplitted)
                        url = webHelper.RemoveQueryString(url, exclude);
                }

                return url;
            }

            #endregion

            #region Methods

            public virtual PriceRange GetSelectedPriceRange(IWebHelper webHelper, string priceRangesStr)
            {
                string range = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (String.IsNullOrEmpty(range))
                    return null;
                string[] fromTo = range.Trim().Split(new char[] { '-' });
                if (fromTo.Length == 2)
                {
                    decimal? from = null;
                    if (!String.IsNullOrEmpty(fromTo[0]) && !String.IsNullOrEmpty(fromTo[0].Trim()))
                        from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("zh-CN"));
                    decimal? to = null;
                    if (!String.IsNullOrEmpty(fromTo[1]) && !String.IsNullOrEmpty(fromTo[1].Trim()))
                        to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("zh-CN"));

                    var priceRangeList = GetPriceRangeList(priceRangesStr);
                    foreach (var pr in priceRangeList)
                    {
                        if (pr.From == from && pr.To == to)
                            return pr;
                    }
                }
                return null;
            }

            public virtual void LoadPriceRangeFilters(string priceRangeStr, IWebHelper webHelper, IPriceFormatter priceFormatter)
            {
                var priceRangeList = GetPriceRangeList(priceRangeStr);
                if (priceRangeList.Count > 0)
                {
                    this.Enabled = true;

                    var selectedPriceRange = GetSelectedPriceRange(webHelper, priceRangeStr);

                    this.Items = priceRangeList.ToList().Select(x =>
                    {
                        //from&to
                        var item = new PriceRangeFilterItem();
                        if (x.From.HasValue)
                            item.From = priceFormatter.FormatPrice(x.From.Value, true, false);
                        if (x.To.HasValue)
                            item.To = priceFormatter.FormatPrice(x.To.Value, true, false);
                        string fromQuery = string.Empty;
                        if (x.From.HasValue)
                            fromQuery = x.From.Value.ToString(new CultureInfo("zh-CN"));
                        string toQuery = string.Empty;
                        if (x.To.HasValue)
                            toQuery = x.To.Value.ToString(new CultureInfo("zh-CN"));

                        //is selected?
                        if (selectedPriceRange != null
                        && selectedPriceRange.From == x.From
                        && selectedPriceRange.To == x.To)
                            item.Selected = true;

                        //filter URL
                        string url = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM + "=" + fromQuery + "-" + toQuery, null);
                        url = ExcludeQueryStringParams(url, webHelper);
                        item.FilterUrl = url;


                        return item;
                    }).ToList();

                    if (selectedPriceRange != null)
                    {
                        //remove filter URL
                        string url = webHelper.RemoveQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM);
                        url = ExcludeQueryStringParams(url, webHelper);
                        this.RemoveFilterUrl = url;
                    }
                }
                else
                {
                    this.Enabled = false;
                }
            }

            #endregion

            #region Properties
            public bool Enabled { get; set; }
            public IList<PriceRangeFilterItem> Items { get; set; }
            public string RemoveFilterUrl { get; set; }

            #endregion
        }

        public partial class PriceRangeFilterItem : ModelBase
        {
            public string From { get; set; }
            public string To { get; set; }
            public string FilterUrl { get; set; }
            public bool Selected { get; set; }
        }

        public partial class SpecificationFilterModel : ModelBase
        {
            #region Const

            private const string QUERYSTRINGPARAM = "specs";

            #endregion

            #region Ctor

            public SpecificationFilterModel()
            {
                this.AlreadyFilteredItems = new List<SpecificationFilterItem>();
                this.AllFilteredItems = new List<SpecificationFilterItem>();
            }

            #endregion

            #region Utilities

            protected virtual string ExcludeQueryStringParams(string url, IWebHelper webHelper)
            {
                var excludedQueryStringParams = "pagenumber"; //remove page filtering
                if (!String.IsNullOrEmpty(excludedQueryStringParams))
                {
                    string[] excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string exclude in excludedQueryStringParamsSplitted)
                    {
                        url = webHelper.RemoveQueryString(url, exclude);
                    }
                }

                return url;
            }

            protected virtual string GenerateFilteredSpecQueryParam(IList<int> optionIds)
            {
                string result = "";

                if (optionIds == null || optionIds.Count == 0)
                    return result;

                for (int i = 0; i < optionIds.Count; i++)
                {
                    result += optionIds[i];
                    if (i != optionIds.Count - 1)
                        result += ",";
                }
                return result;
            }

            #endregion

            #region Methods

            public virtual List<int> GetAlreadyFilteredSpecOptionIds(IWebHelper webHelper)
            {
                var result = new List<int>();

                string alreadyFilteredSpecsStr = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (String.IsNullOrWhiteSpace(alreadyFilteredSpecsStr))
                    return result;

                foreach (var spec in alreadyFilteredSpecsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int specId = 0;
                    int.TryParse(spec.Trim(), out specId);
                    if (!result.Contains(specId))
                        result.Add(specId);
                }
                return result;
            }

            public virtual void PrepareSpecsFilters(IList<int> alreadyFilteredSpecOptionIds,
                IList<int> filterableSpecificationAttributeOptionIds,
                ISpecificationAttributeService specificationAttributeService,
                IWebHelper webHelper,
                IWorkContext workContext)
            {
                var allFilters = new List<SpecificationAttributeOptionFilter>();
                if (filterableSpecificationAttributeOptionIds != null)
                    foreach (var saoId in filterableSpecificationAttributeOptionIds)
                    {
                        var sao = specificationAttributeService.GetSpecificationAttributeOptionById(saoId);
                        if (sao != null)
                        {
                            var sa = sao.SpecificationAttribute;
                            if (sa != null)
                            {
                                allFilters.Add(new SpecificationAttributeOptionFilter
                                {
                                    SpecificationAttributeId = sa.Id,
                                    SpecificationAttributeName = sa.GetLocalized(x => x.Name, workContext.WorkingLanguage.Id),
                                    SpecificationAttributeDisplayOrder = sa.DisplayOrder,
                                    SpecificationAttributeOptionId = sao.Id,
                                    SpecificationAttributeOptionName = sao.GetLocalized(x => x.Name, workContext.WorkingLanguage.Id),
                                    SpecificationAttributeOptionDisplayOrder = sao.DisplayOrder
                                });
                            }
                        }
                    }

                //sort loaded options
                allFilters = allFilters.OrderBy(saof => saof.SpecificationAttributeDisplayOrder)
                    .ThenBy(saof => saof.SpecificationAttributeOptionDisplayOrder)
                    .ThenBy(saof => saof.SpecificationAttributeName)
                    .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();

                //get already filtered specification options
                var alreadyFilteredOptions = allFilters
                    .Where(x => alreadyFilteredSpecOptionIds.Contains(x.SpecificationAttributeOptionId))
                    .Select(x => x)
                    .ToList();

                //prepare the model properties
                if (alreadyFilteredOptions.Count > 0)
                {
                    this.AlreadyFilteredItems = alreadyFilteredOptions.ToList().Select(x =>
                    {
                        var item = new SpecificationFilterItem();
                        item.SpecificationAttributeName = x.SpecificationAttributeName;
                        item.SpecificationAttributeOptionName = x.SpecificationAttributeOptionName;
                        return item;
                    }).ToList();

                }

                this.AllFilteredItems = allFilters.Select(x =>
                    {
                        var item = new SpecificationFilterItem();
                        item.SpecificationAttributeName = x.SpecificationAttributeName;
                        item.SpecificationAttributeOptionName = x.SpecificationAttributeOptionName;
                        //filter URL
                        var alreadyFilteredOptionIds = GetAlreadyFilteredSpecOptionIds(webHelper);
                        if (!alreadyFilteredOptionIds.Contains(x.SpecificationAttributeOptionId))
                            alreadyFilteredOptionIds.Add(x.SpecificationAttributeOptionId);
                        else
                            item.Selected = true;
                        string newQueryParam = GenerateFilteredSpecQueryParam(alreadyFilteredOptionIds);
                        string filterUrl = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM + "=" + newQueryParam, null);
                        filterUrl = ExcludeQueryStringParams(filterUrl, webHelper);
                        item.FilterUrl = filterUrl;

                        return item;
                    }).ToList();
                //prepare the model properties
                if (AllFilteredItems.Count > 0)
                {
                    this.Enabled = true;
                    //remove filter URL
                    string removeFilterUrl = webHelper.RemoveQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM);
                    removeFilterUrl = ExcludeQueryStringParams(removeFilterUrl, webHelper);
                    this.RemoveFilterUrl = removeFilterUrl;
                }
                else
                {
                    this.Enabled = false;
                }
            }

            #endregion

            #region Properties
            public bool Enabled { get; set; }
            public IList<SpecificationFilterItem> AlreadyFilteredItems { get; set; }
            public IList<SpecificationFilterItem> AllFilteredItems { get; set; }
            public string RemoveFilterUrl { get; set; }

            #endregion
        }

        public partial class SpecificationFilterItem : ModelBase
        {
            public string SpecificationAttributeName { get; set; }
            public string SpecificationAttributeOptionName { get; set; }
            public string FilterUrl { get; set; }
            public bool Selected { get; set; }
        }

        #endregion
    }
}