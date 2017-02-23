using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using BSF.BaseService.DistributedCache.LoadBalance;
using BSF.BaseService.DistributedCache.SystemRuntime;



namespace BSF.BaseService.DistributedCache.Storage
{
    /*
     * Redis存储
     *序列化方面未来需要重写，并适应不同的存储方式；并提高序列化性能
     */
    public class RedisCache : BaseCache
    {
        private IDatabase redisdb = null;

        public override T GetValue<T>()
        {
            return Catch(() =>
            {
                if (typeof(T) == typeof(string))
                {
                    var stringValue = redisdb.getValueString(_key);
                    stringValue = Compress.CompressFactory.DecompressString(this.Config.Compress, stringValue);
                    return stringValue as T;
                }
                var bs = redisdb.getValueByte(_key);
                if (bs == null)
                    return null;
                string s = Encoding.UTF8.GetString(bs);
                if (s == "")//约定""为null的情况。
                    return null;
                try
                {
                    s = Compress.CompressFactory.DecompressString(this.Config.Compress, s);
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
                if (typeof(T) == typeof(string))
                {
                    string stringValue = value as string;

                    stringValue = Compress.CompressFactory.CompressString(this.Config.Compress, stringValue);

                    return redisdb.SetValue(_key, stringValue, expiretime);
                }

                string s = null;
                try
                {
                    s = (value == null ? "" : JsonConvert.SerializeObject(value));//约定""为null的情况。
                }
                catch (Exception exp)
                {
                    throw new DistributedCacheSerializationException(typeof(T), exp);
                }

                s = Compress.CompressFactory.CompressString(this.Config.Compress, s);
                return redisdb.SetValue(_key, Encoding.UTF8.GetBytes(s), expiretime);
            });

        }

        public override void Delete()
        {
            Catch(() =>
            {
                redisdb.Delete(_key);
                return 1;
            });
        }

        public override void OpenConn(string key)
        {
            Catch(() =>
            {
                _key = key;
                var serverconfig = this.Config as RedisCacheConfig;
                var manager = new BSF.Redis.RedisManager();
                redisdb = manager.GetPoolClient((string.IsNullOrEmpty(serverconfig.Password) ? "" : serverconfig.Password.NullToEmpty() + "@") + serverconfig.Host.NullToEmpty() + ":" + serverconfig.Port, serverconfig.MaxWritePool, serverconfig.MaxReadPool);
                return 1;
            });
        }

        private T Catch<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (ServiceStack.Redis.RedisException e)
            {
                throw new DistributedCacheConnectException(e.Message, e);
            }
            catch (Newtonsoft.Json.JsonException e2)
            {
                throw new DistributedCacheSerializationException(e2);
            }

        }

        public override void Dispose()
        {
            if (redisdb != null)
            {
                redisdb.Dispose();
                redisdb = null;
            }
        }
    }

    public class RedisCacheConfig : BaseConfig
    {
        public string Host { get; set; }
        public int Port = 6379;
        public string Password = "";
        public int MaxWritePool = 100;
        public int MaxReadPool = 100;

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
                    MaxWritePool = MaxPoolSize;
                    MaxReadPool = MaxPoolSize;
                }
                else if (s.StartWithIgnoreCase("maxwritepool"))
                {
                    MaxWritePool = Convert.ToInt32(s.RemoveStart("maxwritepool=".Length));
                }
                else if (s.StartWithIgnoreCase("maxreadpool"))
                {
                    MaxReadPool = Convert.ToInt32(s.RemoveStart("maxreadpool=".Length));
                }
                else if (s.StartWithIgnoreCase("compress"))
                {
                    Compress = Convert.ToString(s.RemoveStart("compress=".Length));
                }
            }
        }
    }
}
