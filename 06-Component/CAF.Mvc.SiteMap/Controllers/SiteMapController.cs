using CAF.Infrastructure.Core.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CAF.Mvc.SiteMap.Controllers
{
    /// <summary>
    /// TODO: this is not implemented to handle huge sites, if the total of all nodes added will be greater 
    /// than 25000 then a sitemap index should be provided that links to multiple sitemaps with each site map
    /// being limted to 25000 urls or less
    /// </summary>
    public class SiteMapController : Controller
    {

        public SiteMapController(
            ILogger logger,
            IEnumerable<ISiteMapNodeService> nodeProviders = null)
        {
            log = logger;
            this.nodeProviders = nodeProviders;
        }

        private ILogger log;
        private IEnumerable<ISiteMapNodeService> nodeProviders;

        [HttpGet]
        public ActionResult Index()
        {
            if (nodeProviders == null)
            {
                log.Information("no ISiteMapNodeService were injected so returning 404");
                Response.StatusCode = 404;
                return new EmptyResult();
            }

            CancellationToken cancellationToken = HttpContext?.Request.TimedOutToken ?? CancellationToken.None;

            XNamespace xmlns = SiteMapConstants.Namespace;
            var root = new XElement(xmlns + SiteMapConstants.UrlSetTag);

            foreach (var nodeService in nodeProviders)
            {
                var nodeList = nodeService.GetSiteMapNodes(cancellationToken);
                foreach (var node in nodeList)
                {
                    var sitemapElement = new XElement(
                    xmlns + SiteMapConstants.UrlTag,
                    new XElement(xmlns + SiteMapConstants.LocTag, node.Url),

                    node.LastModified == null ? null : new XElement(
                        xmlns + SiteMapConstants.LastModTag,
                        node.LastModified.Value.ToLocalTime().ToString(SiteMapConstants.DateFormat)),

                    node.ChangeFrequency == null ? null : new XElement(
                        xmlns + SiteMapConstants.ChangeFreqTag,
                        node.ChangeFrequency.Value.ToString().ToLowerInvariant()),

                    node.Priority == null ? null : new XElement(
                        xmlns + SiteMapConstants.PriorityTag,
                        node.Priority.Value.ToString(SiteMapConstants.PriorityFormat, CultureInfo.InvariantCulture)));

                    ;

                    root.Add(sitemapElement);
                }
            }

            var xml = new XDocument(root);
            return new XmlResult(xml);

        }

    }
}
