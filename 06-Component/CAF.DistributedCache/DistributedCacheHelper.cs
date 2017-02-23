using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSF.BaseService.DistributedCache.Storage;

namespace BSF.BaseService.DistributedCache
{
    /// <summary>
    /// 分布式缓存帮助类
    /// 用于兼容不同版本及简化使用
    /// </summary>
    public class DistributedCacheHelper
    {
        /*
         * 分布式缓存连接字符串配置格式说明
         * 格式:底层存储;指定底层存储的配置连接字符串;
         * 举例:redis;host=192.168.17.54:6379;password=;maxwritepool=20;maxreadpool=20;
         * （;分隔信息）
         * 目前支持的底层存储:redis;未来支持:ssdb,memcached,sqlserver,阿里云缓存服务
         * redis连接格式说明:host(表示ip:端口),password(表示密码),maxwritepool(表示最大写连接池大小),maxreadpool(表示最大读连接池大小)
         * 
         */

        /// <summary>
        /// 获取或设置key的缓存,并在制定时间后失效
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="serverconfigs">分布式缓存连接字符串配置（负载均衡需要配置多个）</param>
        /// <param name="key"></param>
        /// <param name="expiretime">过期时间</param>
        /// <param name="action">缓存的数据集回调 （当缓存失效的时候或者第一次初始化将回调此方法获取最新数据）</param>
        /// <returns>返回数据集</returns>
        public static T GetOrSetValue<T>(List<string> serverconfigs, string key, TimeSpan expiretime, Func<T> action) where T : class
        {
            return CacheFactory.GetOrSetValue(serverconfigs, key, expiretime, action);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="serverconfigs">分布式缓存连接字符串配置（负载均衡需要配置多个）</param>
        /// <param name="key"></param>
        /// <returns>返回数据集</returns>
        public static T GetValue<T>(List<string> serverconfigs, string key) where T : class
        {
            return CacheFactory.GetValue<T>(serverconfigs, key);
        }

        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="serverconfigs"></param>
        /// <param name="keys"></param>
        public static void Delete(List<string> serverconfigs, string[] keys)
        {
            CacheFactory.Delete(serverconfigs, keys);
        }
    }
}
