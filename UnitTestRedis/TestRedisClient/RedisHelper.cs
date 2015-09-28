using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRedisClient
{
    #region Version Info
    /* ========================================================================
	 * 【本类功能概述】
	 * 
	 * 作者：Alan 时间：2015/9/10 17:27:04
	 * 文件名：RedisHelper
	 * 版本：V1.0.0
	 *
	 * 修改者： 时间： 
	 * 修改说明：
	 * ========================================================================
	 */
    #endregion
    class RedisHelper
    {
        private static string Domain { get; set; }
        private static int Port { get; set; }
        private static string Password { get; set; }
        protected static ConfigurationOptions configOptions = null;
        static ConnectionMultiplexer conn = null;
        static RedisHelper()
        {
            string redisInfo = ConfigurationManager.AppSettings["Redis"];
            int ConnectTimeout = int.Parse(ConfigurationManager.AppSettings["RedisConnectTimeout"]);
            string[] redisinfos = redisInfo.Split('#');
            Domain = string.Empty;
            Port = 0;
            Password = string.Empty;
            if (redisinfos.Length > 0)
            {
                string[] temps = redisinfos[0].Split(':');
                System.Diagnostics.Debug.Assert(temps.Length == 2);
                Domain = temps[0];
                Port = int.Parse(temps[1]);
            }
            if (redisinfos.Length > 1)
            {
                Password = redisinfos[1];
            }

            configOptions = new ConfigurationOptions
            {
                EndPoints =
               {
                    { Domain, Port }
               },
                AllowAdmin = true,
                //Proxy = Proxy.Twemproxy,
                Password = Password,
                ConnectTimeout = ConnectTimeout,
                SyncTimeout = ConnectTimeout,
            };
        }
        public static void ClientTemplate(Action<IDatabase> cb)
        {
            if (conn == null || conn.IsConnected == false)
            {
                conn = ConnectionMultiplexer.Connect(configOptions);
            }
            IDatabase ds=  conn.GetDatabase();
            cb(ds); 
        }
        public static bool KeyExpire(RedisKey key, TimeSpan? expiry)
        {
            bool result = false;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.KeyExpire(key, expiry);
            });
            return result;
        }
        public static bool KeyExists(string key)
        {
            bool result = false;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.KeyExists(key);
            });
            return result;
        }
        public static TimeSpan? KeyTimeToLive(string key)
        {
            TimeSpan? result = null;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.KeyTimeToLive(key);
            });
            return result;
        }
    }
    
    #region key value 封装
    class RedisKvHelper : RedisHelper {
        public bool StringSet(string key, string value, TimeSpan? expiry=null, When when = When.Always)
        {
            bool result = false;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.StringSet(key, value, expiry, when);
            }); 
            return result;
        }
        public string StringGet(string key)
        {
            string result = string.Empty;
            ClientTemplate((redisCli) =>
            {
                var temp=redisCli.StringGet(key);
                if(!temp.IsNullOrEmpty)
                    result = temp.ToString();
            });
            return result;
        }
        public bool Set<T>(string key, T value, When when = When.Always)
        {
            bool result = false;
            
            ClientTemplate((redisCli) =>
            {
                result = redisCli.StringSet(key, JsonConvert.SerializeObject(value),null, when);
            });
            return result;
        }
        public T Get<T>(string key)
        {
            T result = default(T);
            string temp= StringGet(key);
            if (!string.IsNullOrEmpty(temp)) {
                result = JsonConvert.DeserializeObject<T>(key);
            }
            return result;
        } 
    
    } 
    #endregion

    #region list 封装
    class RedisListHelper : RedisHelper {
        #region 输入压入List
        public long ListLeftPush(string key, string value)
        {
            long result = 0;
            ClientTemplate((redisCli) =>
            { 
                result = redisCli.ListLeftPush(key, value);
            });
            return result;
        }
        public long ListLeftPush<T>(string key, T value)
        { 
            return ListLeftPush(key, JsonConvert.SerializeObject(value));
        }
        public long ListLeftPushs(string key, RedisValue[] values)
        {
            long result = 0;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.ListLeftPush(key, values);
            });
            return result;
        }
        public long ListRightPush(string key, string value)
        {
            long result = 0;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.ListRightPush(key, value);
            });
            return result;
        }
        public long ListRightPush<T>(string key, T value)
        {
            return ListRightPush(key, JsonConvert.SerializeObject(value));
        }
        public long ListRightPush(string key, RedisValue[] value)
        {
            long result = 0;
            ClientTemplate((redisCli) =>
            { 
                result = redisCli.ListRightPush(key, value);
            });
            return result;
        }
        #endregion 
        #region 获取数据
        public RedisValue ListLeftPop<T>(string key)
        {
            RedisValue val = default(RedisValue);
            ClientTemplate((redisCli) =>
            {
               val = redisCli.ListLeftPop(key);
            });
            return val;
        }
        public RedisValue ListRightPop(string key)
        {
            RedisValue val=default(RedisValue);
            ClientTemplate((redisCli) =>
            {
                val = redisCli.ListRightPop(key);
            });
            return val;
        }
        public RedisValue ListGetByIndex(string key, int index)
        {
            RedisValue val = default(RedisValue);
            ClientTemplate((redisCli) =>
            { 
                val = redisCli.ListGetByIndex(key,index);
            });
            return val;
        }
        public long ListGetByIndex(string key, RedisValue value)
        {
            long result = 0;
            ClientTemplate((redisCli) =>
            { 
                result = redisCli.ListRemove(key, value);
            });
            return result;
        }
        public void ListSetByIndex(string key, int index, RedisValue value)
        { 
            ClientTemplate((redisCli) =>
            {
                 redisCli.ListSetByIndex(key, index, value);
            }); 
        }
        public long ListLength(string key)
        {
            long length = 0;
            ClientTemplate((redisCli) =>
            {
                length= redisCli.ListLength(key );
            });
            return length;
        }
        public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1)
        {
            RedisValue[] vals =null ;
            ClientTemplate((redisCli) =>
            { 
                vals = redisCli.ListRange(key, start, stop);
            });
            return vals;
        } 
        #endregion
    }
    #endregion

    #region Set 封装
    class RedisSetHelper : RedisHelper {
        /// <summary>
        /// 往Set里添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetAdd(string key, RedisValue value)
        {
            bool result = false;
            ClientTemplate((redisCli) =>
            { 
                result = redisCli.SetAdd(key, value);
            });
            return result;
        }
        /// <summary>
        /// 往Set里添加批量数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long SetAdd(string key, RedisValue[] values)
        {
            long result = 0;
            ClientTemplate((redisCli) =>
            { 
                result = redisCli.SetAdd(key,values);
            });
            return result;
        }
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetContains(RedisKey key, RedisValue value)
        {
            bool result = false;
            ClientTemplate((redisCli) => {
                result = redisCli.SetContains(key, value);
            });
            return result;
        }

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public RedisValue[] SetGetMembers(RedisKey key)
        {
            RedisValue[] result = null;
            ClientTemplate((redisCli) =>
            { 
                result = redisCli.SetMembers(key);
            });
            return result;
        }

        #region 排序的Set
        /// <summary>
        /// 往SortedSet里添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SortedSetAdd(string key, RedisValue value, double score=0)
        {
            bool result = false;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.SortedSetAdd(key, value, score);
            });
            return result;
        }
        /// <summary>
        /// 往SortedSet里添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long SortedSetAdd(string key, SortedSetEntry[] values)
        {
            long result = 0;
            ClientTemplate((redisCli) =>
            { 
                result = redisCli.SortedSetAdd(key, values);
            });
            return result;
        }

        public long SortedSetLength(string key) {
            long result = 0;
            ClientTemplate((redisCli) =>
            {
                result=redisCli.SortedSetLength(key);
            });
            return result;
        } 

        public bool SortedSetRemove(string key, RedisValue member)
        {
            bool result = false;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.SortedSetRemove(key,member);
            });
            return result;
        }

        public long SortedSetRemove(RedisKey key, RedisValue[] members)
        {
            long result = 0;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.SortedSetRemove(key, members);
            });
            return result;
        }
         

        #endregion 排序的Set

        #region hash

        #region 往hash压入数据
        void HashSet(RedisKey key, HashEntry[] hashFields)
        {
            ClientTemplate((redisCli) =>
            {
                redisCli.HashSet(key, hashFields);
            });
        }  
        public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always)
        {
            bool result = false;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.HashSet(key, hashField, value, when);
            });
            return result;
        }
        #endregion

        #region 删除
        public bool HashDelete(RedisKey key, RedisValue hashField)
        {
            bool result = false;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.HashDelete(key, hashField);
            });
            return result;
        }

        public long HashDelete(RedisKey key, RedisValue[] hashFields)
        {
            long result = 0;
            ClientTemplate((redisCli) =>
            {
                result = redisCli.HashDelete(key, hashFields);
            });
            return result;
        }
        #endregion

        #region 获取hash数据
        public RedisValue HashGet(RedisKey key, RedisValue hashField)
        {
            RedisValue val = default(RedisValue);
            ClientTemplate((redisCli) =>
            {
                val = redisCli.HashGet(key, hashField);
            });
            return val;
        }
        public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields)
        {
            RedisValue[] val = null;
            ClientTemplate((redisCli) =>
            {
                val = redisCli.HashGet(key, hashFields);
            });
            return val;
        }

        public HashEntry[] HashGetAll(RedisKey key)
        {
            HashEntry[] val = null;
            ClientTemplate((redisCli) =>
            {
                val = redisCli.HashGetAll(key);
            });
            return val;
        }

        #endregion

        public long HashLength(RedisKey key)
        {
            long length = 0;
            ClientTemplate((redisCli) =>
            {
                length=redisCli.HashLength(key);
            });
            return length;
        }

        public bool HashExists(RedisKey key, RedisValue hashField)
        {
            bool length = false;
            ClientTemplate((redisCli) =>
            {
                length = redisCli.HashExists(key, hashField);
            });
            return length;
        }
     
        #endregion
    }
    #endregion
}
