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
	 * 作者：Alan 时间：2015/9/28 15:56:45
	 * 文件名：ListTest
	 * 版本：V1.0.0
	 *
	 * 修改者： 时间： 
	 * 修改说明：
	 * ========================================================================
	 */
    #endregion
    public class ListTest
    { 
        public static void Run()
        {
            RedisListHelper lHelper = new RedisListHelper();
            string key="alan_list";
            while (lHelper.ListLength(key)>0)
            {
                lHelper.ListRightPop(key);
            }
            //从头部压入
            lHelper.ListRightPush(key, 1);
            lHelper.ListRightPush(key, 2);
            lHelper.ListRightPush(key, 3);
            RedisValue[] values = lHelper.ListRange(key);
            foreach (var va in values) {
                PrintHelper.Info(va);
            }
            lHelper.ListLeftPush(key, 0);
            PrintHelper.Info("在尾部压入");
            values = lHelper.ListRange(key);
            foreach (var va in values)
            {
                PrintHelper.Info(va);
            }
            RedisHelper.ClientTemplate((client) => {
                client.ListInsertBefore(key, 2, "ListInsertBefore");
                client.ListInsertAfter(key, 2, "ListInsertAfter");
            });
            PrintHelper.Info("在指定位置插入元素，下标为2");
            values = lHelper.ListRange(key);
            foreach (var va in values)
            {
                PrintHelper.Info(va);
            }
        }
    }
}
