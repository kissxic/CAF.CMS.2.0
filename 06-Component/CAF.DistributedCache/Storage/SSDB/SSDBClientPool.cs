using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BSF.BaseService.DistributedCache.Storage.SSDB.NetSdk;

namespace BSF.BaseService.DistributedCache.Storage.SSDB
{
    /*
     * SSDB 客户端连接池
     * 备注: 代码来自Thrift,并修改集成。
     */
    /// <summary>
    /// SSDB客户端配置
    /// </summary>
    public class SSDBClientConfig 
    {
       /// <summary>
        /// 服务器地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 服务端口
        /// </summary>
        public int Port { get; set; }
        ///// <summary>
        ///// 传输编码
        ///// </summary>
        //public Encoding Encode { get; set; }
        ///// <summary>
        ///// 是否启用压缩
        ///// </summary>
        //public bool Zipped { get; set; }
        ///// <summary>
        ///// 连接超时
        ///// </summary>
        //public int Timeout { get; set; }
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
        ///// <summary>
        ///// 阻塞的最大数量
        ///// </summary>
        //public int MaxWait { get; set; }
        /// <summary>
        /// 从缓存池中分配对象时是否验证对象
        /// </summary>
        public bool ValidateOnBorrow = true;
        /// <summary>
        /// 从缓存池中归还对象时是否验证对象
        /// </summary>
        public bool ValidateOnReturn = false;
        ///// <summary>
        ///// 从缓存池中挂起对象时是否验证对象
        ///// </summary>
        //public bool ValidateWhiledIdle { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetHashCode() == this.GetHashCode())
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (Host + "_" + Port + "_" + MaxActive).GetHashCode();
        }
    }
    /// <summary>
    /// 客户端连接池
    /// </summary>
    public class SSDBClientPool
    {
        #region 属性
        private SSDBClientConfig config;

        /// <summary>
        /// 对象缓存池
        /// </summary>
        //private static Stack<TTransport> objectPool { get; set; }
        private static ConcurrentStack<Client> objectPool { get; set; }
        /// <summary>
        /// 同步对象
        /// </summary>
        private static AutoResetEvent resetEvent;
        /// <summary>
        /// 每取走一例，表示激活对象加1，此属性可控制对象池容量
        /// </summary>
        private static volatile int activedCount = 0;
        /// <summary>
        /// 同步对象锁
        /// </summary>
        private static object locker = new object();

        #endregion

        #region 构造函数

        static SSDBClientPool()
        {
            CreateResetEvent();
            CreateThriftPool();
        }

        public SSDBClientPool(SSDBClientConfig thriftconfig)
        {
            config = thriftconfig;
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 从对象池取出一个对象
        /// </summary>
        /// <returns></returns>
        public Client BorrowInstance()
        {
            lock (locker)
            {
                //Console.WriteLine("借前对象池个数：{0}，工作对象个数：{1}", objectPool.Count(), activedCount);
                Client transport;
                //对象池无空闲对象
                if (objectPool.Count() == 0)
                {
                    //对象池已激活对象数达上限
                    if (activedCount == config.MaxActive)
                    {
                        if (resetEvent.WaitOne(1000) == false)
                        {
                            throw new Exception(string.Format("当前活动连接已经达到上限!当前活动连接数为:{0}",activedCount));
                        }
                    }
                    else
                    {
                        PushObject(CreateInstance());
                    }
                }
                if (!objectPool.TryPop(out transport)) throw new Exception(string.Format("连接池异常,可能是连接数已耗尽。当前活动连接数为:{0}",activedCount));
                //transport = objectPool.Pop();
                activedCount++;
                //检查对象池存量
                //对象池存量小于最小空闲数，并且激活数小于最大激活数，添加一个对象到对象池
                if (objectPool.Count() < config.MinIdle && activedCount < config.MaxActive)
                {
                    PushObject(CreateInstance());
                }
                if (config.ValidateOnBorrow)
                {
                    ValidateOnBorrow(transport);
                }
                return transport;
            }
        }

        /// <summary>
        /// 归还一个对象
        /// </summary>
        /// <param name="instance"></param>
        public void ReturnInstance(Client instance)
        {
            //对象池容量达到上限，不再返回线程池,直接销毁
            if (objectPool.Count() == config.MaxIdle)
            {
                DestoryInstance(instance);
            }
            else
            {
                if (config.ValidateOnReturn)
                {
                    ValidateOnReturn(instance);
                }
                PushObject(instance);
                activedCount--;
                //发通知信号，有对象归还到对象池
                resetEvent.Set();
            }
            //Console.WriteLine("归还后对象池个数：{0}，归还后工作对象个数：{1}", objectPool.Count(), activedCount);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 创建线程同步对象
        /// </summary>
        private static void CreateResetEvent()
        {
            if (resetEvent == null)
            {
                resetEvent = new AutoResetEvent(false);
            }
        }

        /// <summary>
        /// 创建对象池
        /// </summary>
        private static void CreateThriftPool()
        {
            if (objectPool == null)
            {
                objectPool = new ConcurrentStack<Client>();// new Stack<TTransport>();
            }
        }

        /// <summary>
        /// 添加对象到对象池
        /// </summary>
        /// <param name="transport"></param>
        private void PushObject(Client transport)
        {
            objectPool.Push(transport);
        }

        /// <summary>
        /// 创建一个对象
        /// </summary>
        /// <returns></returns>
        private Client CreateInstance()
        {
            Client transport = new Client(config.Host, config.Port);
            //transport.Open();
            return transport;
        }

        /// <summary>
        /// 取出对象时校验对象
        /// </summary>
        private void ValidateOnBorrow(Client instance)
        {
            //if (!instance.IsOpen)
            //{
            //    instance.Open();
            //} 
            if (instance.link==null || instance.link.sock == null)
                instance.link = new Link(config.Host, config.Port);
            if (!instance.link.sock.Connected)
            {
                instance.link.sock.Connect(config.Host,config.Port);
            }
        }

        /// <summary>
        /// 归还对象时校验对象
        /// </summary>
        private void ValidateOnReturn(Client instance)
        {
            //if (instance.IsOpen)
            //{
            //    instance.Close();
            //}
            if (instance.link.sock!=null && instance.link.sock.Connected)
            {
                instance.close();
            }
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="instance"></param>
        private void DestoryInstance(Client instance)
        {
            //instance.Flush();
            //if (instance.IsOpen)
            //{
            //    instance.Close();
            //}
            //instance.Dispose();
            if (instance.link.sock.GetStream() != null)
                instance.link.sock.GetStream().Flush();

            if (instance.link.sock.Connected)
            {
                instance.close();
            }
        }

        

        #endregion

       

    }
    

   
}
