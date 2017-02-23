using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching;
using Newtonsoft.Json;
using BSF.BaseService.DistributedCache.Storage.AliyunMemcached;
using BSF.BaseService.DistributedCache.SystemRuntime;
 


namespace BSF.BaseService.DistributedCache.Storage
{
    public class AliyunMemcachedCache : BaseCache
    {
        private MemcachedClient client = null;
        public override T GetValue<T>()
        {
            return Catch(() =>
            {
                string json = client.Get<string>(_key);
                if (json == null)//null 表示未添加,""表示为null 这个未来要区分，目前暂时不区分
                    return null;
                if (json == "")//约定""为null的情况。
                    return null;
                try
                {
                    json = Compress.CompressFactory.DecompressString(this.Config.Compress, json);
                   
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (Exception exp)
                {
                    throw new DistributedCacheSerializationException(json.NullToEmpty(), exp);
                }
            });
        }

        public override bool SetValue<T>(T value, TimeSpan expiretime)
        {
            return Catch(() =>
            {
                string json = null;
                try
                {
                    json = (value == null ? "" : JsonConvert.SerializeObject(value));//约定""为null的情况。
                    json = Compress.CompressFactory.CompressString(this.Config.Compress, json);
                }
                catch (Exception exp)
                {
                    throw new DistributedCacheSerializationException(typeof(T), exp);
                }
                var r = client.Store(Enyim.Caching.Memcached.StoreMode.Set, _key, json, expiretime);
                if (r == false)
                    throw new DistributedCacheConnectException("执行SetValue(Store)函数失败");
                return r;
            });

        }

        public override void Delete()
        {
            Catch(() =>
            {
                client.Remove(_key);
                return 1;
            });
        }

        public override void OpenConn(string key)
        {
            Catch(() =>
            {
                _key = key;
                var serverconfig = this.Config as AliyunMemcachedCacheConfig;
                client = new AliyunMemcachedClientPoolProvider().GetPool(serverconfig);
                return 1;
            });
        }

        private T Catch<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (System.Net.Sockets.SocketException e)
            {
                throw new DistributedCacheConnectException(e.Message, e);
            }
            catch (Newtonsoft.Json.JsonException e2)
            {
                throw new DistributedCacheSerializationException(e2);
            }
            catch (Enyim.Caching.Memcached.MemcachedClientException e3)
            {
                throw new DistributedCacheConnectException(e3.Message, e3);
            }
        }

        public override void Dispose()
        {
            if (client != null)
            {

            }
        }
    }

    public class AliyunMemcachedCacheConfig : BaseConfig
    {
        /// <summary>
        /// 阿里云实例ID
        /// </summary>
        public string NetworkAddress { get; set; }
        public int Port { get; set; }
        public string Zone = "";
        public string UserName = "";
        public string Password = "";

        /// <summary>
        /// 超时（毫秒）
        /// </summary>
        public Int64 ConnectionTimeout = 1000;
        /// <summary>
        /// 最小缓存池大小
        /// </summary>
        public int MinPoolSize = 5;
        /// <summary>
        /// 最大缓存池大小
        /// </summary>
        public int MaxPoolSize = 200;

        public override void Parse(string config)
        {
            //config = config.ToLower();
            string[] ss = config.Split(';');
            foreach (var s in ss)
            {
                if (string.IsNullOrWhiteSpace(s))
                    continue;
                if (s.StartWithIgnoreCase("networkaddress"))
                {
                    NetworkAddress = s.RemoveStart("networkaddress=".Length);
                }
                else if (s.StartWithIgnoreCase("port"))
                {
                    Port = Convert.ToInt32(s.RemoveStart("port=".Length));
                }
                else if (s.StartWithIgnoreCase("zone"))
                {
                    Zone = s.RemoveStart("zone=".Length);
                }
                else if (s.StartWithIgnoreCase("username"))
                {
                    UserName = s.RemoveStart("username=".Length);
                }
                else if (s.StartWithIgnoreCase("password"))
                {
                    Password = s.RemoveStart("password=".Length);
                }
                else if (s.StartWithIgnoreCase("minpoolsize"))
                {
                    MinPoolSize = Convert.ToInt32(s.RemoveStart("minpoolsize=".Length));
                }
                else if (s.StartWithIgnoreCase("maxpoolsize"))
                {
                    MaxPoolSize = Convert.ToInt32(s.RemoveStart("maxpoolsize=".Length));
                }
                else if (s.StartWithIgnoreCase("connectiontimeout"))
                {
                    ConnectionTimeout = Convert.ToInt32(s.RemoveStart("connectiontimeout=".Length));
                }
                else if(s.StartWithIgnoreCase("compress"))
                {
                    Compress = Convert.ToString(s.RemoveStart("compress=".Length));
                }

            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetHashCode() == this.GetHashCode())
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (NetworkAddress + "_" + Port).GetHashCode();
        }

    }
}
