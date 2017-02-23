//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace BSF.BaseService.DistributedCache
//{

//    public class DistributedCacheDemo
//    {
//        public void Run()
//        {
//            //获取某业务缓存的业务1
//            new TestCacheHelper().GetYeWu1<TestCache1>(330100, () =>
//            {
//                return new TestCache1() { data="THIS IS TEST ENTITY!!!" };
//            });
//            //删除某业务的业务1的key缓存
//            new TestCacheHelper().DeleteYeWu1Cache(330100);
//        }
//    }

//    /*
//     * 某个业务需要的缓存信息 建议为业务名+Cache
//     */
//    public class TestCache1
//    {
//        public string data;
//        public int data2;
//    }

//    /// <summary>
//    /// 定义一个业务缓存帮助类 
//    /// 建议格式:业务名+CacheHelper
//    /// 这个类只是个例子，具体要根据实际业务情况而定，要思考而定，不要整块拷贝
//    /// </summary>
//    public class TestCacheHelper
//    {
//        private  string Key = "DydTest";//定义该业务的唯一标示，建议为业务名

//        private  List<string> GetConfigs()
//        {
//            /*借助“配置中心”动态设置负载均衡示例*/
//            List<string> configs = new List<string>();
//            for (int i = 0; i < 100; i++)//假设最大支持100个cache负载均衡
//            { 
//                var config = ConfigManager.ConfigManagerHelper.Get<string>(Key+i);//判断第n个负载均衡是否存在
//                if (string.IsNullOrEmpty(config))
//                {
//                    break;
//                }
//                configs.Add(config);//若存在则添加入负载均衡列表
//            }
//            return configs;
//            /*
//            * 分布式缓存连接字符串配置格式说明
//            * 格式:底层存储;指定底层存储的配置连接字符串;
//            * 举例:redis;host=192.168.17.54:6379;password=;maxwritepool=20;maxreadpool=20;
//            * （;分隔信息）
//            * 目前支持的底层存储:redis,ssdb,aliyunmemcached;未来支持:ssdb,memcached,sqlserver
//            * redis连接格式说明:host(表示ip:端口),password(表示密码),maxwritepool(表示最大写连接池大小),maxreadpool(表示最大读连接池大小)
//            * ssdb连接格式说明:host,port,password,maxactive(表示最大激活的连接数),maxidle(表示最大空闲连接数),minidle(表示最小空闲连接数)
//            * aliyunmemcached连接格式说明:networkaddress(表示阿里云内网地址),port,zone(未知参数),username,password,minpoolsize(表示最小连接数),maxpoolsize(表示最大连接池数)
//            */
//            /*return new List<string>() { "redis;host=192.168.17.54:6379;" };*/
//        }

//        private string GetKey(string testkey)
//        {
//            return Key +"-"+ testkey;//缓存到底层存储的key
//        }

//        public T GetYeWu1<T>(int shopid,Func<T> action) where T:class
//        {
//            string isopencache = ConfigManager.ConfigManagerHelper.Get<string>(Key+"_Yewu1");//配置中心集成判断业务是否开启缓存
//            if (isopencache!=null &&isopencache.ToLower()=="true")
//            {
//                string exprietime = ConfigManager.ConfigManagerHelper.Get<string>(Key+"_Yewu1_exprietime");//配置中心集成业务缓存时间
//                int cacheexprieseconds = (string.IsNullOrWhiteSpace(exprietime) == null ? 30 : Convert.ToInt32(exprietime));

//                return DistributedCacheHelper.GetOrSetValue<T>(this.GetConfigs(),
//                    this.GetKey(shopid+""),
//                    TimeSpan.FromSeconds(cacheexprieseconds), 
//                    action);
//            }
//            return null;
//        }

//        public void DeleteYeWu1Cache(int shopid) 
//        {
//            DistributedCacheHelper.Delete(this.GetConfigs(),
//                new string[] { this.GetKey(shopid + "") });
//        }
//    }
//}
