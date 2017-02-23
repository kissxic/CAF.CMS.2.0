using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Authentication.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.ConsoleApplication
{
    class Program
    {


        static void Main(string[] args)
        {
            // initialize engine context
            EngineContext.Initialize(false);
            new AuthorizerTest().GetRandowString();
        }
    }
}
