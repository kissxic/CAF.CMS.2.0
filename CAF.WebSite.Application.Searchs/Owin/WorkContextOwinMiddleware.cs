using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CAF.Infrastructure.Core;
using System.Web.Optimization;
using CAF.WebSite.Application.Searchs.Infrastructure;
using CAF.WebSite.Application.Searchs.Models.Common;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Searchs.Services;
using CAF.WebSite.Application.Searchs.Models.Searchs;

namespace CAF.WebSite.Application.Searchs.Owin
{
    public class WorkContextOwinMiddleware : OwinMiddleware
    {
        protected static readonly RequireHttps RequireHttps = GetRequireHttps();
        protected static readonly PathString[] OwinIgnorePathsStrings = GetOwinIgnorePathStrings();

        public WorkContextOwinMiddleware(OwinMiddleware next)
          : base(next)
        {
            // WARNING! WorkContextOwinMiddleware is created once when application starts.
            // Don't store any instances which depend on WorkContext because it has a per request lifetime.

            // CacheManager = container.Resolve<ILocalCacheManager>();
        }
        public override async Task Invoke(IOwinContext context)
        {
            if (RequireHttps.Enabled && !context.Request.IsSecure)
            {
                RedirectToHttps(context);
            }
            else
            {
                if (IsStorefrontRequest(context.Request) && !IsBundleRequest(context.Request))
                {
                    await HandleStorefrontRequest(context);
                }

                await Next.Invoke(context);
            }
        }

        protected virtual async Task HandleStorefrontRequest(IOwinContext context)
        {
            var workContext = EngineContext.Current.Resolve<AppWorkContext>();
            // Initialize common properties
            var qs = HttpUtility.ParseQueryString(context.Request.Uri.Query);
            workContext.RequestUrl = context.Request.Uri;
            workContext.QueryString = qs;
            //Initialize catalog search criteria
            workContext.CurrentProductSearchCriteria = new ProductSearchCriteria(null,null,qs)
            {
                //CatalogId = workContext.CurrentStore.Catalog
            };
            workContext.PageNumber = qs.Get("page").ToNullableInt();
            workContext.PageSize = qs.Get("count").ToNullableInt() ?? qs.Get("page_size").ToNullableInt();

            var catalogSearchService = EngineContext.Current.Resolve<ICatalogSearchService>();
            //This line make delay products loading initialization (products can be evaluated on view rendering time)
            workContext.Products = new MutablePagedList<Article>((pageNumber, pageSize, sortInfos) =>
            {
                var criteria = workContext.CurrentProductSearchCriteria.Clone();
                criteria.PageNumber = pageNumber;
                criteria.PageSize = pageSize;
                if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos.IsNullOrEmpty())
                {
                    criteria.SortBy = SortInfo.ToString(sortInfos);
                }
                var result = catalogSearchService.SearchProducts(criteria);
                //Prevent double api request for get aggregations
                //Because catalog search products returns also aggregations we can use it to populate workContext using C# closure
                //now workContext.Aggregation will be contains preloaded aggregations for current search criteria
                workContext.Aggregations = new MutablePagedList<Aggregation>(result.Aggregations);
                return result.Products;
            });
            //This line make delay aggregation loading initialization (aggregation can be evaluated on view rendering time)
            workContext.Aggregations = new MutablePagedList<Aggregation>((pageNumber, pageSize, sortInfos) =>
            {
                var criteria = workContext.CurrentProductSearchCriteria.Clone();
                criteria.PageNumber = pageNumber;
                criteria.PageSize = pageSize;
                if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos.IsNullOrEmpty())
                {
                    criteria.SortBy = SortInfo.ToString(sortInfos);
                }
                //Force to load products and its also populate workContext.Aggregations by preloaded values
                workContext.Products.Slice(pageNumber, pageSize, sortInfos);
                return workContext.Aggregations;
            });

            //Do not load shopping cart and other for resource requests
            if (!IsAssetRequest(context.Request))
            {
                await HandleNonAssetRequest(context, workContext);
            }
        }
        protected virtual async Task HandleNonAssetRequest(IOwinContext context, AppWorkContext workContext)
        {
            await Task.Run(() =>
            {

            });
        }


        protected virtual void RedirectToHttps(IOwinContext context)
        {
            var uriBuilder = new UriBuilder(context.Request.Uri)
            {
                Scheme = Uri.UriSchemeHttps,
                Port = RequireHttps.Port
            };
            context.Response.StatusCode = RequireHttps.StatusCode;
            context.Response.ReasonPhrase = RequireHttps.ReasonPhrase;
            context.Response.Headers["Location"] = uriBuilder.Uri.AbsoluteUri;
        }

        protected virtual bool IsStorefrontRequest(IOwinRequest request)
        {
            return !OwinIgnorePathsStrings.Any(p => request.Path.StartsWithSegments(p));
        }

        protected virtual bool IsBundleRequest(IOwinRequest request)
        {
            var path = "~" + request.Path;
            return BundleTable.Bundles.Any(b => b.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        }

        protected virtual bool IsAssetRequest(IOwinRequest request)
        {
            var retVal = string.Equals(request.Method, "GET", StringComparison.OrdinalIgnoreCase);
            if (retVal)
            {
                retVal = request.Uri.IsFile || request.Uri.AbsolutePath.Contains("/assets/");
            }
            return retVal;
        }

        protected static PathString[] GetOwinIgnorePathStrings()
        {
            var result = new List<PathString>();

            var owinIgnore = ConfigurationManager.AppSettings["VirtoCommerce:Storefront:OwinIgnore"];

            if (!string.IsNullOrEmpty(owinIgnore))
            {
                result.AddRange(owinIgnore.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(p => new PathString(p)));
            }

            return result.ToArray();
        }

        protected static RequireHttps GetRequireHttps()
        {
            return new RequireHttps
            {
                Enabled = ConfigurationManager.AppSettings.GetValue("VirtoCommerce:Storefront:RequireHttps:Enabled", false),
                StatusCode = ConfigurationManager.AppSettings.GetValue("VirtoCommerce:Storefront:RequireHttps:StatusCode", 308),
                ReasonPhrase = ConfigurationManager.AppSettings.GetValue("VirtoCommerce:Storefront:RequireHttps:ReasonPhrase", "Permanent Redirect"),
                Port = ConfigurationManager.AppSettings.GetValue("VirtoCommerce:Storefront:RequireHttps:Port", 443),
            };
        }
    }



}