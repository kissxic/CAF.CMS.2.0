using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSF.BaseService.DistributedCache.Storage;

namespace BSF.BaseService.DistributedCache.LoadBalance
{
    public class LoadBalanceFactory
    {
        public static string ChooseServer(List<string> serviceconfiglist, string key) 
        {
            return new HashBalance().ChooseServer(serviceconfiglist, key);
        }
    }
}
