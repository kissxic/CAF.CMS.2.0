using System;
using System.Text;
using Newtonsoft.Json;

using StackExchange.Redis;
using CAF.Infrastructure.Core;
using System.Threading.Tasks;
using CAF.Infrastructure.Core.Async;

namespace CAF.Infrastructure.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching in Redis store (http://redis.io/).
    /// Mostly it'll be used when running in a web farm or Azure.
    /// But of course it can be also used on any server or environment
    /// </summary>
    public partial class RedisCacheManager : DisposableObject, ICacheManager
    {
        #region Fields
        private readonly IRedisConnectionWrapper _connectionWrapper;
        private readonly IDatabase _db;

        public bool IsDistributedCache
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Ctor

        public RedisCacheManager(IRedisConnectionWrapper connectionWrapper)
        {

            // ConnectionMultiplexer.Connect should only be called once and shared between callers
            this._connectionWrapper = connectionWrapper;

            this._db = _connectionWrapper.Database();

        }

        #endregion

        #region Utilities

        protected virtual byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }
        protected virtual T Deserialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null)
                return default(T);

            var jsonString = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        private bool TryGet<T>(string key, out T value)
        {
            value = default(T);

            var obj = _db.StringGet(key);
            if (obj.HasValue)
            {
                var result = Deserialize<T>(obj);
                value = (T)result;
                return true;
            }

            return false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual T Get<T>(string key)
        {

            T value;
            TryGet(key, out value);
            return value;
        }

        public T Get<T>(string key, Func<T> acquirer, TimeSpan? duration = default(TimeSpan?))
        {
            T value;

            if (TryGet(key, out value))
            {
                return value;
            }
            lock (String.Intern(key))
            {
                var rValue = _db.StringGet(key);
                if (!rValue.HasValue)
                {
                    value = acquirer();
                  
                    Set(key, value, duration);
                }
            }
            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquirer, TimeSpan? duration = default(TimeSpan?))
        {

            T value;

            if (TryGet(key, out value))
            {
                return value;
            }

            var keyLock = AsyncLock.Acquire(key);
            using (await keyLock.LockAsync())
            {
                var rValue = _db.StringGet(key);
                if (!rValue.HasValue)
                {
                    value = await acquirer().ConfigureAwait(false);
                   Set(key, value, duration);
                    return value;
                }
                value = Deserialize<T>(rValue);
            }
            return value;
        }
        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        public void Set(string key, object value, TimeSpan? duration = default(TimeSpan?))
        {
            if (value == null)
                return;

            var entryBytes = Serialize(value);

            _db.StringSet(key, entryBytes, duration);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public virtual bool Contains(string key)
        {
            //little performance workaround here:
            //we use "PerRequestCacheManager" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server 500 times per HTTP request (e.g. each time to load a locale or setting)
          
            return _db.KeyExists(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            _db.KeyDelete(key);
            
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            foreach (var ep in _connectionWrapper.GetEndpoints())
            {
                var server = _connectionWrapper.Server(ep);
                var keys = server.Keys(pattern: "*" + pattern + "*");
                foreach (var key in keys)
                    _db.KeyDelete(key);
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            foreach (var ep in _connectionWrapper.GetEndpoints())
            {
                var server = _connectionWrapper.Server(ep);
                //we can use the code below (commented)
                //but it requires administration permission - ",allowAdmin=true"
                //server.FlushDatabase();

                //that's why we simply interate through all elements now
                var keys = server.Keys();
                foreach (var key in keys)
                    _db.KeyDelete(key);
            }
        }

        public string[] Keys(string pattern)
        {
            throw new NotImplementedException();
        }

        protected override void OnDispose(bool disposing)
        {

        }
        #endregion

    }
}
