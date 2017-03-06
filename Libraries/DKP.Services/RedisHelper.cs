
using System;
using System.Collections.Generic;
using System.Linq;

namespace DKP.Services
{
    using ServiceStack.Redis;
    using ServiceStack.Redis.Generic;
    using System.Configuration;
    /// <summary>
    /// Redis公共辅助类库
    /// </summary>
    public class RedisHelper : IDisposable
    {
        //redis服务IP和端口

        public static string redisUrl = ConfigurationManager.AppSettings["RedisUrl"].ToString();
        public static int RedisPort = int.Parse(ConfigurationManager.AppSettings["RedisPort"]);
        public RedisClient Redis = new RedisClient(redisUrl, RedisPort);
        //缓存池
        PooledRedisClientManager prcm = new PooledRedisClientManager();
        //默认缓存过期时间单位秒
        public int secondsTimeOut = 30 * 60;

        /// <summary>
        /// 缓冲池
        /// </summary>
        /// <param name="readWriteHosts"></param>
        /// <param name="readOnlyHosts"></param>
        /// <returns></returns>
        public static PooledRedisClientManager CreateManager(
         string[] readWriteHosts, string[] readOnlyHosts)
        {
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts,
                new RedisClientManagerConfig
                {
                    MaxWritePoolSize = readWriteHosts.Length * 5,
                    MaxReadPoolSize = readOnlyHosts.Length * 5,
                    AutoStart = true,
                });// { RedisClientFactory = (IRedisClientFactory)RedisCacheClientFactory.Instance.CreateRedisClient("127.0.0.1", 6379) };          
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="openPooledRedis">是否开启缓冲池</param>
        public RedisHelper(bool openPooledRedis = false)
        {
            if (openPooledRedis)
            {
                prcm = CreateManager(new string[] { redisUrl+":"+ RedisPort }, new string[] { redisUrl + ":" + RedisPort });
                Redis = prcm.GetClient() as RedisClient;
            }
        }

        #region 对象操作
        public IList<T> GetAll<T>()
        {
            return Redis.As<T>().GetAll();
        }

        public IList<T> GetList<T>(Func<T, bool> func)
        {
            return Redis.As<T>().GetAll().Where(func).ToList();
        }

        public T GetByKey<T>(string key)
        {
            return Redis.As<T>().GetValue(key);
        }

        public T GetById<T>(object id)
        {
            return Redis.As<T>().GetById(id);
        }

        public void GetByIds<T>(List<object> ids)
        {
            Redis.As<T>().GetByIds(ids);
        }

        public void Insert<T>(T t)
        {
            Redis.As<T>().Store(t);
        }

        public void InsertList<T>(List<T> list)
        {
            Redis.As<T>().StoreAll(list);
        }

        public void UpdateByKey<T>(string key,T t)
        {
            Redis.Set(key, t);
        }

        public void Update<T>(object id,T t)
        {
            var obj = GetById<T>(id);
            string key = getKey<T>(obj);
            UpdateByKey<T>(key, t);
        }

        public string getKey<T>(T t)
        {
            return Redis.As<T>().UrnKey(t);
        }

        public List<string> GetAllKey<T>()
        {
            return Redis.As<T>().GetAllKeys();
        }

        public void Delete<T>(T t)
        {
            Redis.As<T>().Delete(t);
        }

        public void DeleteById<T>(object id)
        {
            Redis.As<T>().DeleteById(id);
        }

        public void DeleteByIds<T>(List<object> ids)
        {
            Redis.As<T>().DeleteByIds(ids);
        }

        public void DeleteList<T>(List<T> list)
        {
            foreach (var t in list)
            {
                Redis.As<T>().Delete(t);
            }
        }

        public void DeleteAll<T>()
        {
            Redis.As<T>().DeleteAll();
        }
        #endregion


        #region Key/Value存储
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存建</param>
        /// <param name="t">缓存值</param>
        /// <param name="timeout">过期时间，单位秒,-1：不过期，0：默认过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string key, T t, int timeout = 0)
        {
            if (timeout >= 0)
            {
                if (timeout > 0)
                {
                    secondsTimeOut = timeout;
                }
                Redis.Expire(key, secondsTimeOut);
            }
            return Redis.Add<T>(key, t);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return Redis.Get<T>(key);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return Redis.Remove(key);
        }

        #endregion
        
        #region 链表操作
        /// <summary>
        /// 根据IEnumerable数据添加链表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="values"></param>
        /// <param name="timeout"></param>
        public void AddList<T>(string listId, IEnumerable<T> values, int timeout = 0)
        {
            Redis.Expire(listId, 60);
            IRedisTypedClient<T> iredisClient = Redis.As<T>();
            if (timeout >= 0)
            {
                if (timeout > 0)
                {
                    secondsTimeOut = timeout;
                }
                Redis.Expire(listId, secondsTimeOut);
            }
            var redisList = iredisClient.Lists[listId];
            redisList.AddRange(values);
            iredisClient.Save();
        }

        /// <summary>
        /// 添加单个实体到链表中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="Item"></param>
        /// <param name="timeout"></param>
        public void AddEntityToList<T>(string listId, T Item, int timeout = 0)
        {
            IRedisTypedClient<T> iredisClient = Redis.As<T>();
            if (timeout >= 0)
            {
                if (timeout > 0)
                {
                    secondsTimeOut = timeout;
                }
                Redis.Expire(listId, secondsTimeOut);
            }
            var redisList = iredisClient.Lists[listId];
            redisList.Add(Item);
            iredisClient.Save();
        }

        /// <summary>
        /// 获取链表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList<T>(string listId)
        {
            IRedisTypedClient<T> iredisClient = Redis.As<T>();
            return iredisClient.Lists[listId];
        }
        /// <summary>
        /// 在链表中删除单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="t"></param>
        public void RemoveFromList<T>(string listId, T t)
        {
            IRedisTypedClient<T> iredisClient = Redis.As<T>();
            var redisList = iredisClient.Lists[listId];
            redisList.RemoveValue(t);
            iredisClient.Save();
        }
        /// <summary>
        /// 根据lambada表达式删除符合条件的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="func"></param>
        public void RemoveFromList<T>(string listId, Func<T, bool> func)
        {
            IRedisTypedClient<T> iredisClient = Redis.As<T>();
            var redisList = iredisClient.Lists[listId];
            T value = redisList.Where(func).FirstOrDefault();
            redisList.RemoveValue(value);
            iredisClient.Save();
        }
        /// <summary>
        /// 根据lambada表达式获取符合条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public IEnumerable<T> GetFromList<T>(string listId, Func<T, bool> func)
        {
            IRedisTypedClient<T> iredisClient = Redis.As<T>();
            var redisList = iredisClient.Lists[listId];
            var result = redisList.Where(func);
            return result;
        }
        #endregion

        //释放资源
        public void Dispose()
        {
            if (Redis != null)
            {
                Redis.Dispose();
                Redis = null;
            }
            GC.Collect();
        }
    }
}

