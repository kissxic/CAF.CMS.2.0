using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CAF.Mvc.SiteMap
{
    public interface ISiteMapNodeService
    {
        IEnumerable<ISiteMapNode> GetSiteMapNodes(
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
