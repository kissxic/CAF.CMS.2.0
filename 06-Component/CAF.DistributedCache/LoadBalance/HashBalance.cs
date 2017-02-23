using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSF.BaseService.DistributedCache.Storage;

namespace BSF.BaseService.DistributedCache.LoadBalance
{
    public class HashBalance : IBalance
    {
        public string ChooseServer(List<string> serviceconfiglist, string key) 
        {
            if (serviceconfiglist == null||serviceconfiglist.Count==0)
                return null;
            return serviceconfiglist[Math.Abs(key.GetHashCode()) % serviceconfiglist.Count];
        }
    }
}
