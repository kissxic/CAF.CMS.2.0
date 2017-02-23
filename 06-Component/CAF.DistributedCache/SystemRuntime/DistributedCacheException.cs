using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.BaseService.DistributedCache.SystemRuntime
{
    public class DistributedCacheException:Exception
    {
        public DistributedCacheException(string message)
            : base(message)
        {
 
        }

        public DistributedCacheException(string message,Exception exp)
            : base(message,exp)
        {

        }
    }

    public class DistributedCacheConnectException : DistributedCacheException
    {
        public DistributedCacheConnectException(string message):base(string.Format("分布式缓存连接失败,请检查连接是否可用;错误信息:{0}",message.NullToEmpty()))
        {
            
        }

        public DistributedCacheConnectException(string message, Exception exp)
            : base(string.Format("分布式缓存连接失败,请检查连接是否可用;错误信息:{0}", message.NullToEmpty()),exp)
        {

        }
    }

    public class DistributedCacheSerializationException : DistributedCacheException
    {
        public DistributedCacheSerializationException(Type type, Exception exp)
            : base(string.Format("分布式缓存序列化/反序列化失败,类型为:{0}",type.Name),exp)
        {
            
        }

        public DistributedCacheSerializationException(string json, Exception exp)
            : base(string.Format("分布式缓存序列化/反序列化失败,序列化内容为:{0}", json),exp)
        {

        }

        public DistributedCacheSerializationException( Exception exp)
            : base("分布式缓存序列化/反序列化失败", exp)
        {

        }
    }
}
