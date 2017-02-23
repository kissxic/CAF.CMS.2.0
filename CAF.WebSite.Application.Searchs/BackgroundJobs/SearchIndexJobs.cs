
using CAF.Infrastructure.SearchModule.Model.Indexing;
using Hangfire;


namespace CAF.WebSite.Application.Searchs.BackgroundJobs
{
    public class SearchIndexJobs
    {
        private readonly ISearchIndexController _controller;

        public SearchIndexJobs(ISearchIndexController controller)
        {
            _controller = controller;
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public void Process(string scope, string documentType, bool rebuild)
        {
            _controller.Process(scope, documentType, rebuild);
        }
    }
}
