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
	 * 作者：Alan 时间：2015/9/28 11:02:16
	 * 文件名：Strings
	 * 版本：V1.0.0
	 *
	 * 修改者： 时间： 
	 * 修改说明：
	 * ========================================================================
	 */
    #endregion
    public class StringsDemo
    { 
        public static void Run() {
            StringsDemo s = new StringsDemo();
            s.TestStringsFunc();
        }

        /// <summary>
        /// 在没有为键设置生存时间（即永久存在，建一个键之后的默认情况）时返回的是-1
        /// </summary>
        public void TestStringsFunc()
        {
            #region 无条件覆盖
            RedisKvHelper helper = new RedisKvHelper();
            
            helper.StringSet("TestSetFunc01","alan");
            PrintHelper.Info(helper.StringGet("TestSetFunc01"));
            helper.StringSet("TestSetFunc01", "alan01");
            PrintHelper.Info(helper.StringGet("TestSetFunc01"));
            #endregion

            #region 条件覆盖 
            //不存在才有效
            PrintHelper.Info(helper.StringSet("TestSetFunc01", "alan02", null, When.NotExists).ToString());
            PrintHelper.Info(helper.StringGet("TestSetFunc01"));
            PrintHelper.Info(helper.StringSet("TestSetFunc02", "alan02", null, When.NotExists).ToString());
            PrintHelper.Info(helper.StringGet("TestSetFunc02"));

            //存在才有效
            PrintHelper.Info(helper.StringSet("TestSetFunc01", "alan_Exists", null, When.Exists).ToString());
            PrintHelper.Info(helper.StringGet("TestSetFunc01"));
            PrintHelper.Info(helper.StringSet("TestSetFunc03", "alan_Exists", null, When.Exists).ToString());
            PrintHelper.Info(helper.StringGet("TestSetFunc03"));
            #endregion

            #region 失效时间
            helper.StringSet("TestSetFunc04", "alanTimeSpan", new TimeSpan(0, 0, 10));
            TimeSpan? ts=  RedisHelper.KeyTimeToLive("TestSetFunc04");
            if (ts != null) {
                PrintHelper.Info(ts.ToString());
            }
            #endregion 

            #region 获取某个value的长度
            RedisHelper.ClientTemplate((redisCli) =>
            {
                PrintHelper.Info(redisCli.StringLength("TestSetFunc01").ToString());
            });
            #endregion
            
            #region 增加
            RedisHelper.ClientTemplate((redisCli) =>
            {
                redisCli.StringSet("testNO",1);
                PrintHelper.Info(redisCli.StringGet("testNO"));
                PrintHelper.Info(redisCli.StringIncrement("testNO").ToString());     //自增1
                PrintHelper.Info(redisCli.StringIncrement("testNO",5).ToString());   //增加5
            });
            #endregion
        }
    }
}
