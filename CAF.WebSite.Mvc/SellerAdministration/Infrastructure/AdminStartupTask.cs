

using CAF.Infrastructure.Core;
namespace CAF.WebSite.Mvc.Seller.Infrastructure
{
    public class SellerStartupTask : IStartupTask
    {
        public void Execute()
        {
            // codehint: sm-delete (Telerik internal localization works better for whatever reason)
            ////set localization service for telerik
            //Telerik.Web.Mvc.Infrastructure.DI.Current.Register(
            //    () => EngineContext.Current.Resolve<Telerik.Web.Mvc.Infrastructure.ILocalizationServiceFactory>());
        }

        public int Order
        {
            get { return 100; }
        }
    }
}