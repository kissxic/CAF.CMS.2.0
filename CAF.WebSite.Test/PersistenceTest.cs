
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Authentication.External;
using NUnit.Framework;


namespace CAF.WebSite.Test
{
    [SetUpFixture]
    public abstract class PersistenceTest
    {
        public IExternalAuthorizer _authorizer;
        [SetUp]
        public void SetUp()
        {
            // initialize engine context
            EngineContext.Initialize(false);
            _authorizer = EngineContext.Current.Resolve<IExternalAuthorizer>();
        }

      
    }
}
