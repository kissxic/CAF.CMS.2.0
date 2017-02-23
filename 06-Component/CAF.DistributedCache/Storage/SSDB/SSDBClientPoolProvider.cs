using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.BaseService.DistributedCache.Storage.SSDB
{
    /// <summary>
    /// 客户端连接池
    /// </summary>
    public class SSDBClientPoolProvider
    {
        private static Dictionary<SSDBClientConfig, SSDBClientPool> _pools = new Dictionary<SSDBClientConfig, SSDBClientPool>();
        private static object _lockpool = new object();

        public SSDBClientPool GetPool(SSDBClientConfig config)
        {
            if (_pools.ContainsKey(config))
            {
                return _pools[config];
            }
            lock (_lockpool)
            {
                if (_pools.ContainsKey(config))
                {
                    return _pools[config];
                }

                SSDBClientPool pool = new SSDBClientPool(config);
                _pools.Add(config, pool);
                return _pools[config];

            }
        }

        public void Dispose()
        { }
    }
}
