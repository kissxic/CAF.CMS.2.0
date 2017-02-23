using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using BSF.BaseService.DistributedCache.LoadBalance;
using BSF.BaseService.DistributedCache.Storage;
using BSF.BaseService.DistributedCache.SystemRuntime;

namespace BSF.BaseService.DistributedCache.Storage
{
    /*
     * 缓存代理实现工厂
     */
    public class CacheFactory
    {
        private static BaseCache GetCache(string serverconfig)
        {
            if (serverconfig.StartsWith(EnumCacheType.Redis.ToString(), true, System.Globalization.CultureInfo.CurrentCulture))
            {
                var c = new RedisCache();
                c.Config = new RedisCacheConfig();
                c.Config.Parse(serverconfig);
                return c;
            }
            else if (serverconfig.StartsWith(EnumCacheType.SSDB.ToString(), true, System.Globalization.CultureInfo.CurrentCulture))
            {
                var c = new SSDBCache();
                c.Config = new SSDBCacheConfig();
                c.Config.Parse(serverconfig);
                return c;
            }
            else if (serverconfig.StartsWith(EnumCacheType.AliyunMemcached.ToString(), true, System.Globalization.CultureInfo.CurrentCulture))
            {
                var c = new AliyunMemcachedCache();
                c.Config = new AliyunMemcachedCacheConfig();
                c.Config.Parse(serverconfig);
                return c;
            }
            else if (serverconfig.StartsWith(EnumCacheType.SqlServer.ToString(), true, System.Globalization.CultureInfo.CurrentCulture))
            {
                throw new DistributedCacheException(string.Format("暂不支持{0}缓存存储实现", EnumCacheType.SqlServer.ToString()));
            }
            else if (serverconfig.StartsWith(EnumCacheType.Memcached.ToString(), true, System.Globalization.CultureInfo.CurrentCulture))
            {
                throw new DistributedCacheException(string.Format("暂不支持{0}缓存存储实现", EnumCacheType.Memcached.ToString()));
            }
            throw new DistributedCacheException("未识别的服务器配置信息");
        }

        public static T GetOrSetValue<T>(List<string> serverconfigs, string key, TimeSpan expiretime, Func<T> action) where T : class
        {
            //if (typeof(T) == typeof(string))
            //    throw new DistributedCacheException("不支持string等类型,仅支持class的实体类型");
            if(expiretime<TimeSpan.FromSeconds(1))
                throw new DistributedCacheException("过期时间不得少于1秒");

            var serverconfig = LoadBalanceFactory.ChooseServer(serverconfigs, key);
            using (var cache = GetCache(serverconfig))
            {
                try
                {
                    cache.OpenConn(key);
                    T r = null;
                    try
                    {
                        r = cache.GetValue<T>();
                        if (r != null) { return r; }//假如key未过期
                    }
                    catch (DistributedCacheSerializationException exp)
                    {
                        //假如内存的序列化内容和实际的序列化结果不一致的情况,则重新序列化覆盖之,并检查反序列情况
                        T r3 = action();
                        var success = cache.SetValue(r3, expiretime);
                        if (success == true)
                        {
                            try
                            {
                                var v2 = cache.GetValue<T>();
                                return v2;
                            }
                            catch { }
                        }
                        BSF.Log.ErrorLog.Write("DistributedCache序列化出错", exp);
                        throw exp;
                    }

                    if (r == null)
                    {
                        //假如key已经过期
                        T v4 = action();
                        var success4 = cache.SetValue(v4, expiretime);
                        if (success4 == true)
                        {
                            return v4;
                        }
                    }
                }
                catch (DistributedCacheConnectException exp)
                {
                    BSF.Log.ErrorLog.Write("DistributedCache连接出错", exp);
                    //假如缓存无法连接或连接失败
                    T v4 = action();
                    return v4;
                }
                BSF.Log.ErrorLog.Write("DistributedCache未知严重错误",new Exception());
                throw new DistributedCache.SystemRuntime.DistributedCacheException("DistributedCache未知严重错误");
            }
        }

        public static T GetValue<T>(List<string> serverconfigs, string key) where T : class
        {
            if (typeof(T) == typeof(string))
                throw new DistributedCacheException("不支持string等类型,仅支持class的实体类型");

            var serverconfig = LoadBalanceFactory.ChooseServer(serverconfigs, key);
            using (var cache = GetCache(serverconfig))
            {
                try
                {
                    cache.OpenConn(key);
                    T r = null;
                    try
                    {
                        r = cache.GetValue<T>();
                        if (r != null) { return r; }//假如key未过期
                    }
                    catch (DistributedCacheSerializationException exp)
                    {
                        BSF.Log.ErrorLog.Write("DistributedCache序列化出错", exp);
                        throw exp;
                    }
                    return r;
                }
                catch (DistributedCacheConnectException exp)
                {
                    BSF.Log.ErrorLog.Write("DistributedCache连接出错", exp);
                    throw new DistributedCache.SystemRuntime.DistributedCacheException("DistributedCache连接出错");
                }
            }
        }

        public static void Delete(List<string> serverconfigs, string[] keys)
        {
            foreach (var key in keys)
            {
                var serverconfig = LoadBalanceFactory.ChooseServer(serverconfigs, key);
                using (var cache = GetCache(serverconfig))
                {
                    cache.OpenConn(key);
                    cache.Delete();
                }
            }
        }
    }
}
