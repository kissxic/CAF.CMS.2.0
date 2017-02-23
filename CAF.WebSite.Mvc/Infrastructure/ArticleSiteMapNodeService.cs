using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Seo;
using CAF.Mvc.SiteMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CAF.Infrastructure.Core;

namespace CAF.WebSite.Mvc.Infrastructure
{
    public class ArticleSiteMapNodeService : ISiteMapNodeService
    {
        private readonly IRepository<Article> _articleRepository;
        private readonly IWebHelper _webHelper;
        public ArticleSiteMapNodeService(
            IRepository<Article> articleRepository, IWebHelper webHelper
          )
        {
            this._articleRepository = articleRepository;
            this._webHelper = webHelper;

        }


        private string baseUrl = string.Empty;
        private List<string> addedUrls = new List<string>();

        public IEnumerable<ISiteMapNode> GetSiteMapNodes(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var mapNodes = new List<SiteMapNode>();
            var articles = _articleRepository.Table.ToList();
            if (articles == null)
            {
                //  log.LogWarning("post list came back null so returning empty list of sitemapnodes");
                return mapNodes;
            }

            foreach (var article in articles)
            {
                if (article.StatusFormat != StatusFormat.Norma) continue;
                if (article.EndDateUtc.HasValue && article.EndDateUtc.Value < DateTime.UtcNow) continue;
                var url = "{0}/{1}/{2}".FormatCurrent(this._webHelper.GetSiteLocation(), "Article", article.GetSeName());

                if (string.IsNullOrEmpty(url))
                {
                    // log.LogWarning("failed to resolve url for post " + post.Id + ", skipping this post for sitemap");
                    continue;
                }
                if (addedUrls.Contains(url)) continue;
                mapNodes.Add(
                            new SiteMapNode(url)
                            {
                                LastModified = article.ModifiedOnUtc
                            });

                addedUrls.Add(url);
            }
            return mapNodes;
        }


    }
}
