using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using BSF.BaseService.DistributedCache.Storage.SSDB;
using BSF.BaseService.DistributedCache.SystemRuntime;


namespace BSF.BaseService.DistributedCache.Storage
{
    /*
    * SSDB存储
    *序列化方面未来需要重写，并适应不同的存储方式；并提高序列化性能
    */
    public class SSDBCache : BaseCache
    {
        private SSDB.NetSdk.Client client = null;
        private SSDBClientPool pool = null;
        public override T GetValue<T>()
        {
            return Catch(() =>
            {
                byte[] bs = null;
                bool success = client.get(_key,out bs);
                if (bs == null)
                    return null;
                string s = Encoding.UTF8.GetString(bs);
                if (s == "")//约定""为null的情况。
                    return null;
                try
                {
                    return JsonConvert.DeserializeObject<T>(s);
                }
                catch (Exception exp)
                {
                    throw new DistributedCacheSerializationException(s.NullToEmpty(), exp);
                }
            });
        }

        public override bool SetValue<T>(T value, TimeSpan expiretime)
        {
            return Catch(() =>
            {
                //if (expiretime < TimeSpan.FromMinutes(30))
                //{
                //    throw new DistributedCacheException(string.Format("ssdb 不适合频繁缓存失效的应用,建议使用redis 存储缓存,目前限制{0}分钟内失效的数据不能使用ssdb",30));
                //}
                string s = null;
                try
                {
                    s = (value == null ? "" : JsonConvert.SerializeObject(value));//约定""为null的情况。
                }
                catch (Exception exp)
                {
                    throw new DistributedCacheSerializationException(typeof(T), exp);
                }
                 client.setx(_key, Encoding.UTF8.GetBytes(s), (int)(expiretime.TotalSeconds));
                 return true;
            });

        }

        public override void Delete()
        {
            Catch(() =>
            {
                client.del(_key);
                return 1;
            });
        }

        public override void OpenConn(string key)
        {
            Catch(() =>
            {
                _key = key;
                var serverconfig = this.Config as SSDBCacheConfig;
                SSDB.SSDBClientPoolProvider poolprovider = new SSDB.SSDBClientPoolProvider();
                pool = poolprovider.GetPool(new SSDB.SSDBClientConfig()
                {
                    Host = serverconfig.Host,
                    MaxActive = serverconfig.MaxActive,
                    MaxIdle = serverconfig.MaxIdle,
                    MinIdle = serverconfig.MinIdle,
                    Port = serverconfig.Port,
                    ValidateOnBorrow = serverconfig.ValidateOnBorrow,
                    ValidateOnReturn = serverconfig.ValidateOnReturn
                });
                client = pool.BorrowInstance();
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
            catch (Exception e3)
            {
                if (e3.Message == "Bad response!" || (e3.InnerException != null && e3.InnerException is System.Net.Sockets.SocketException))
                    throw new DistributedCacheConnectException("请求失败!", e3);
                throw e3;
            }
        }

        public override void Dispose()
        {
            if (client != null)
            {
                if (pool != null)
                { pool.ReturnInstance(client); }
                client = null;
            }
        }
    }

    public class SSDBCacheConfig : BaseConfig
    {
        public string Host { get; set; }
        public int Port = 8888;
        public string Password = "";
        /// <summary>
        /// 可以从缓存池中分配对象的最大数量
        /// </summary>
        public int MaxActive = 50;
        /// <summary>
        /// 缓存池中最大空闲对象数量
        /// </summary>
        public int MaxIdle = 20;
        /// <summary>
        /// 缓存池中最小空闲对象数量
        /// </summary>
        public int MinIdle = 5;
        /// <summary>
        /// 从缓存池中分配对象时是否验证对象
        /// </summary>
        public bool ValidateOnBorrow = true;
        /// <summary>
        /// 从缓存池中归还对象时是否验证对象
        /// </summary>
        public bool ValidateOnReturn = false;

        public override void Parse(string config)
        {
            //config = config.ToLower();
            string[] ss = config.Split(';');
            foreach (var s in ss)
            {
                if (string.IsNullOrWhiteSpace(s))
                    continue;
                if (s.StartWithIgnoreCase("host"))
                {
                    Host = s.RemoveStart("host=".Length);
                }
                else if (s.StartWithIgnoreCase("port"))
                {
                    Port = Convert.ToInt32(s.RemoveStart("port=".Length));
                }
                else if (s.StartWithIgnoreCase("password"))
                {
                    Password = s.RemoveStart("password=".Length);
                }
                else if (s.StartWithIgnoreCase("maxpoolsize"))
                {
                    MaxPoolSize = Convert.ToInt32(s.RemoveStart("maxpoolsize=".Length));
                    MaxActive = MaxPoolSize;
                }
                else if (s.StartWithIgnoreCase("maxactive"))
                {
                    MaxActive = Convert.ToInt32(s.RemoveStart("maxactive=".Length));
                }
                else if (s.StartWithIgnoreCase("maxidle"))
                {
                    MaxIdle = Convert.ToInt32(s.RemoveStart("maxidle=".Length));
                }
                else if (s.StartWithIgnoreCase("minidle"))
                {
                    MinIdle = Convert.ToInt32(s.RemoveStart("minidle=".Length));
                }
                else if (s.StartWithIgnoreCase("validateonborrow"))
                {
                    ValidateOnBorrow = Convert.ToBoolean(s.RemoveStart("validateonborrow=".Length));
                }
                else if (s.StartWithIgnoreCase("validateonreturn"))
                {
                    ValidateOnReturn = Convert.ToBoolean(s.RemoveStart("validateonreturn=".Length));
                }
                else if (s.StartWithIgnoreCase("compress"))
                {
                    Compress = Convert.ToString(s.RemoveStart("compress=".Length));
                }
            }
        }
    }
}
