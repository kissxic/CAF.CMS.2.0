using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.BaseService.DistributedCache.Storage
{
    public class BaseConfig
    {
        public int MaxPoolSize = 100;

        public string Compress = "gzip";
        public virtual void Parse(string config)
        {

        }
    }
}
