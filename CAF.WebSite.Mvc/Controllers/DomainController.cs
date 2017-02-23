
using CAF.Infrastructure.SearchModule.Core.Model;
using CAF.Infrastructure.SearchModule.Data.Model;
using CAF.Infrastructure.SearchModule.Model;
using CAF.Infrastructure.SearchModule.Model.Filters;
using CAF.WebSite.Application.Searchs.BackgroundJobs;
using CAF.WebSite.Application.Searchs.Infrastructure;
using CAF.WebSite.Application.Searchs.Models.Common;
using CAF.WebSite.Application.Searchs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Controllers
{
    public class DomainController : Controller
    {
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchConnection _searchConnection;
        private readonly SearchIndexJobsScheduler _scheduler;
        private readonly SearchIndexJobs _searchIndexJobs;
        private readonly IBrowseFilterService _browseFilterService;
        private readonly IItemBrowsingService _browseService;
        protected AppWorkContext _workContext { get; private set; }
        public DomainController(AppWorkContext context,
            ISearchProvider searchProvider, ISearchConnection searchConnection,
           IBrowseFilterService browseFilterService, IItemBrowsingService browseService,
           SearchIndexJobsScheduler scheduler,
            SearchIndexJobs searchIndexJobs)
        {
            _workContext = context;
            _browseFilterService = browseFilterService;
            _browseService = browseService;
            _searchProvider = searchProvider;
            _searchConnection = searchConnection;
            _scheduler = scheduler;
            _searchIndexJobs = searchIndexJobs;
        }
        // GET: Domain
        public ActionResult Index()
        {
            _searchIndexJobs.Process(_searchConnection.Scope, string.Empty, true);
            //var jobId = _scheduler.ScheduleRebuildIndex();
            //var result = new { Id = jobId };
            return View();
        }

        public ActionResult SearchProducts(ProductSearch criteria)
        {
            //var result = SearchProducts(_searchConnection.Scope, criteria, ItemResponseGroup.ItemLarge);

            if (_workContext.Aggregations != null)
            {
                
               /// var tags = _workContext.Aggregations.Where(a => a.Items != null);
                foreach (var item in _workContext.Aggregations)
                {

                }
            }

            return View();
        }

        private ProductSearchResult SearchProducts(string scope, ProductSearch criteria, ItemResponseGroup responseGroup)
        {


            var context = new Dictionary<string, object>
            {
                { "Store", 1 },
            };

            var filters = _browseFilterService.GetFilters(context);

            var serviceCriteria = criteria.AsCriteria<CatalogItemSearchCriteria>("0", filters);

            var searchResults = _browseService.SearchItems(scope, serviceCriteria, responseGroup);
            return searchResults;
        }

    }
}