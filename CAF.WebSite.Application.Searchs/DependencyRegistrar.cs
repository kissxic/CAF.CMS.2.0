using Autofac;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.Infrastructure.SearchModule.Core.Controllers;
using CAF.Infrastructure.SearchModule.Core.Extensions;
using CAF.Infrastructure.SearchModule.Model;
using CAF.Infrastructure.SearchModule.Model.Filters;
using CAF.Infrastructure.SearchModule.Model.Indexing;
using CAF.Infrastructure.SearchModule.Model.Search;
using CAF.Infrastructure.SearchModule.Providers.ElasticSearch.Nest;
using CAF.Infrastructure.SearchModule.Providers.Lucene;
using CAF.WebSite.Application.Searchs.BackgroundJobs;
using CAF.WebSite.Application.Searchs.Infrastructure;
using CAF.WebSite.Application.Searchs.Services;
using CAF.WebSite.Application.Searchs.Services.Providers.ElasticSearch.Nest;
using CAF.WebSite.Application.Searchs.Services.Providers.Lucene;
using System;
using System.Configuration;

namespace CAF.WebSite.Application.Searchs
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {
            if (DataSettings.DatabaseIsInstalled())
            {
                var connectionString = "";
                var configConnectionString = ConfigurationManager.ConnectionStrings["SearchConnectionString"];
                if (configConnectionString != null && !string.IsNullOrEmpty(configConnectionString.ConnectionString))
                {
                    connectionString = configConnectionString.ConnectionString;
                }
                var searchConnection = new SearchConnection(connectionString);
             

                builder.Register<ISearchConnection>(c => searchConnection).InstancePerRequest();

                builder.RegisterType<CatalogItemIndexBuilder>().As<ISearchIndexBuilder>().InstancePerRequest();
                builder.RegisterType<SearchIndexController>().As<ISearchIndexController>().InstancePerRequest();

                if (searchConnection.Provider.Equals(SearchProviders.Elasticsearch.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    builder.RegisterType<ElasticSearchQueryBuilder>().As<ISearchQueryBuilder>().InstancePerRequest();
                    builder.RegisterType<ElasticSearchProvider>().As<ISearchProvider>().InstancePerRequest();

                }
                else if (searchConnection.Provider.Equals(SearchProviders.Lucene.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    builder.RegisterType<LuceneSearchProvider>().As<ISearchProvider>().InstancePerRequest();
                    builder.RegisterType<LuceneSearchQueryBuilder>().As<ISearchQueryBuilder>().InstancePerRequest();

                }

                builder.RegisterType<SearchIndexJobsScheduler>().As<SearchIndexJobsScheduler>().InstancePerRequest();
                builder.RegisterType<ItemBrowsingService>().As<IItemBrowsingService>().InstancePerRequest();
                builder.RegisterType<SearchIndexJobs>().As<SearchIndexJobs>().InstancePerRequest();
                builder.RegisterType<CategoryBrowsingService>().As<ICategoryBrowsingService>().InstancePerRequest();
                builder.RegisterType<FilterService>().As<IBrowseFilterService>().InstancePerRequest();

                builder.RegisterType<CatalogSearchServiceImpl>().As<ICatalogSearchService>().InstancePerRequest();

                if (searchConnection.Provider.Equals(SearchProviders.Elasticsearch.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    builder.RegisterType<CatalogElasticSearchQueryBuilder>().Named<ISearchQueryBuilder>("elastic-search").InstancePerRequest();

                }
                else if (searchConnection.Provider.Equals(SearchProviders.Lucene.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    builder.RegisterType<CatalogLuceneQueryBuilder>().Named<ISearchQueryBuilder>("lucene").InstancePerRequest();

                }
        
                builder.RegisterType<AppWorkContext>().As<AppWorkContext>().InstancePerRequest();
            }
        }

        public int Order
        {
            get { return 100; }
        }
    }
}
