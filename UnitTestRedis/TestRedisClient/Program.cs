using System; 
using StackExchange.Redis;
using System.Collections.Generic; 
using System.IO;
using System.Linq; 
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using TestRedisClient.Demos;

namespace TestRedisClient
{
    class Program
    { 
        static void Main(string[] args)
        {
            //StringsDemo.Run();
            //HashSetDemo.Run();
            ListTest.Run();
            Console.Read();
        } 
    }
}

#region 测试代码
/*测试代码*/
/*
  // private IDatabase db = RedisConnection.Instance.GetDatabase();
        //public void test() {
        //    string value = "abcdefg";
        //    db.StringSet("mykey", value);
        //    var val = db.StringGet("mykey");
        //    Console.WriteLine(val);
        //}
        public void TestSet(ConfigurationOptions config)
        {
            using (var conn = ConnectionMultiplexer.Connect(config))
            {
                var db = conn.GetDatabase();
                string value = "abcdefg";
                Console.WriteLine(db.SetContains(value, "10000001"));
               
                int length = 10000000 + 100000;
                for (int i = 10000000; i < length; i++)
                {
                    db.SetAdd(value, i.ToString()); 
                }
                Console.WriteLine(db.SetLength(value));
                Console.WriteLine(db.SetMembers(value));
            }
        }
        public void TestString(ConfigurationOptions config)
        { 
            using (var conn = ConnectionMultiplexer.Connect(config))
            {
                var db = conn.GetDatabase(); 
                for (int i = 0; i < 100000; i++)
                {
                    string value = "abcdefg";
                    db.StringSet("mykey", value);
                    var val = db.StringGet("mykey");
                }
            }
        }
        public void test1() {
            var config = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                EndPoints =
                {
                     { "10.20.0.6", 6379 }
                },
                ConnectTimeout = 200,
                Password ="123456"
            };
            TestSet(config); 
        }
        static void TestSet() {
            RedisSetHelper sHelper = new RedisSetHelper();
            Stopwatch watch = new Stopwatch();
            long begin = 100000000000;
            long end = begin + 100000; //10W
            watch.Start();
            List<RedisValue> mylist = new List<RedisValue>();
            long allend = (100000 * 100);
            for (long i = begin; i < begin+allend; i++)
            {
                mylist.Add(i);
                if (mylist.Count >= 100000) {
                    Console.WriteLine("Set = " + i);
                    sHelper.SetAdd("alanset",mylist.ToArray());
                    mylist.Clear();
                 } 
            }
            watch.Stop();
            Console.WriteLine("Set =" + watch.ElapsedMilliseconds.ToString());
        }
        static void TestSet2()
        {
            RedisSetHelper sHelper = new RedisSetHelper();
            Stopwatch watch = new Stopwatch();
            long begin = 200000000000;
            long end = begin + 100000; //10W
            watch.Start();
            List<RedisValue> mylist = new List<RedisValue>();
            long allend = (100000 * 100);
            for (long i = begin; i < begin + allend; i++)
            {
                mylist.Add(i);
                if (mylist.Count >= 100000)
                {
                    Console.WriteLine("Set = " + i);
                    sHelper.SetAdd("alanset", mylist.ToArray());
                    mylist.Clear();
                }
            }
            watch.Stop();
            Console.WriteLine("Set =" + watch.ElapsedMilliseconds.ToString());
        }
        static void TestHashSet() {
            RedisSetHelper sHelper = new RedisSetHelper();
            sHelper.HashSet("alanhash01","name","alan");
            sHelper.HashSet("alanhash01", "age", 18);
            HashEntry[]  he = sHelper.HashGetAll("alanhash01");
            foreach (var h in he) {
                Console.WriteLine(h.Name+"   "+h.Value);
            }
        }
        static void TestExpiry()
        {
            RedisSetHelper sHelper = new RedisSetHelper();
            string str = string.Empty;
            bool isfirst = true;
            for (long index = 0; index < 10000000; index++)
            {
                sHelper.HashSet("alanhash01", index.ToString(), "alan");
                if (isfirst == true)
                {
                    RedisHelper.KeyExpire("alanhash01", new TimeSpan(0, 0, 1));
                    isfirst = false;
                }
                str = sHelper.HashGet(index.ToString(), "name");
                Console.WriteLine(str + "  " + index);
                bool eResult = RedisHelper.KeyExists("alanhash01");
                if (str == null)
                {
                    isfirst = true;
                    Console.WriteLine("=========================失效了哇=====================");
                }
            }
            Console.WriteLine(str);
        }
        static void TestExpiry2()
        {
            RedisSetHelper sHelper = new RedisSetHelper();
            string str = string.Empty;
            bool isfirst = true;
            for (long index = 0; index < 10000000; index++)
            {
                sHelper.HashSet("alanhash02", index.ToString(), "alan");
                if (isfirst == true)
                {
                    RedisHelper.KeyExpire("alanhash02", new TimeSpan(0, 0, 1));
                    isfirst = false;
                }
                str = sHelper.HashGet(index.ToString(), "name");
                Console.WriteLine(str + "  " + index);
                bool eResult = RedisHelper.KeyExists("alanhash02");
                if (str == null)
                {
                    isfirst = true;
                    Console.WriteLine("=========================失效了哇1=====================");
                }
            }
            Console.WriteLine(str);
        }
 */
/*  static void Main(string[] args){
            Thread childThread = new Thread(TestExpiry);
            childThread.Start();
            Thread childThread2 = new Thread(TestExpiry2);
            childThread2.Start();
            Console.Read();
            //TestHashSet();
            //new Program().test1();
            RedisKvHelper helper = new RedisKvHelper();
            helper.StringSet("alan111", "alan");
            helper.StringSet("alan111", "alan1");
            string str=helper.StringGet("alan111");
            //Thread childThread = new Thread(TestSet);
            //childThread.Start();
            //Thread childThread2 = new Thread(TestSet2);
            //childThread2.Start();
            List<RedisValue> mylist = new List<RedisValue>() { "alan", "hello" }; 
            long begin=100000000000;
            long end = begin + 100000; //10W
            RedisListHelper lHelper = new RedisListHelper();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine("index = " + i );
                mylist.Clear();
                for (begin = 100000000000; begin < end; ++begin)
                {
                    mylist.Add(begin.ToString());
                }
                lHelper.ListLeftPushs("alan123", mylist.ToArray());
            }
            watch.Stop();
            Console.WriteLine( "!!!!!!!="+ watch.ElapsedMilliseconds.ToString());
            Console.WriteLine("ListLength=" + lHelper.ListLength("alan123"));
            lHelper.ListLeftPush("alan123", "你不懂");

            RedisValue[] values = lHelper.ListRange("alan123", 0, 300);
            values = lHelper.ListRange("alan123", 0, 3);
         
            Console.WriteLine(str);
            Console.WriteLine("----------");
            Console.Read();
        } 
   
       */

#endregion