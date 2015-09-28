using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRedisClient.Demos
{
    #region Version Info
    /* ========================================================================
	 * 【本类功能概述】
	 * 
	 * 作者：Alan 时间：2015/9/28 11:51:26
	 * 文件名：HashSetDemo
	 * 版本：V1.0.0
	 *
	 * 修改者： 时间： 
	 * 修改说明：
	 * ========================================================================
	 */
    #endregion
    public class HashSetDemo
    { 
        public static void Run()
        {
            RedisSetHelper sHelper = new RedisSetHelper();
            
            #region haseSet
            //设置key为"alanhash01" field为"name"的值
            sHelper.HashSet("alanhash01", "name", "alan");
            PrintHelper.Info(sHelper.HashGet("alanhash01", "name"));
            sHelper.HashSet("alanhash01", "age", 1);
            PrintHelper.Info(sHelper.HashGet("alanhash01", "age"));
            RedisHelper.ClientTemplate((client) => {
                //key为"alanhash01" field为"age" 自增1
                client.HashIncrement("alanhash01", "age");
                PrintHelper.Info(sHelper.HashGet("alanhash01", "age"));
            });
            PrintHelper.Info("key为alanhash01的长度" + sHelper.HashLength("alanhash01").ToString());
            PrintHelper.Info("判断key=alanhash01,field=age是否存在" + sHelper.HashExists("alanhash01", "age").ToString());
            //删除
            if (sHelper.HashDelete("alanhash01", "name")) {
                PrintHelper.Info("key=alanhash01, field=name 删除成功");
                PrintHelper.Info("判断key=alanhash01, field=name是否存在" + sHelper.HashExists("alanhash01", "name").ToString());
            }
            //返回所有field,value,所以数据量大
            HashEntry[]  entrys= sHelper.HashGetAll("alanhash01");
            foreach (var en in entrys) {
                PrintHelper.Info(en.ToString());
            }
            #endregion

            #region SortedSet
            sHelper.SortedSetAdd("alan_SortedSet","name");
            sHelper.SortedSetAdd("alan_SortedSet", "name1");
            sHelper.SortedSetAdd("alan_SortedSet", "name2");
            sHelper.SortedSetAdd("alan_SortedSet", "name3");
            sHelper.SortedSetRemove("alan_SortedSet", "name2");
            PrintHelper.Info("alan_SortedSet的长度: "+ sHelper.SortedSetLength("alan_SortedSet").ToString());
            PrintHelper.Info("遍历 alan_SortedSet ");
            RedisHelper.ClientTemplate((client) => {
                IEnumerable<SortedSetEntry>  entrys1=  client.SortedSetScan("alan_SortedSet");
                foreach (var entry in entrys1)
                {
                    PrintHelper.Info(entry.Element);
                }
            }); 
            #endregion
        }
    }
}
