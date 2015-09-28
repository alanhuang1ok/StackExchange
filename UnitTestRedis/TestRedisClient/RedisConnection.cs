using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TestRedisClient
{
    public sealed class RedisConnection
    {
        private static ConfigurationOptions configOptions = new ConfigurationOptions
        {
            EndPoints =
            {
                { "10.20.0.6", 6379 }
            },
            AllowAdmin = true,
            Proxy = Proxy.Twemproxy,
            Password="123456"
        };
        private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configOptions);

        static RedisConnection()
        {

        }

        /// <summary>
        /// Get Redis connection instance
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                return redis;
            }
        }
    }    
}
