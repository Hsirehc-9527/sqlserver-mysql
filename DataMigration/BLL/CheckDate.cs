using DataMigration.DAL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.BLL
{
    /// <summary>
    /// 数据对比
    /// </summary>
    public class CheckDate
    {
        /// <summary>
        /// 表结构对比
        /// </summary>
        /// <param name="sql_tables_fields"></param>
        /// <param name="mysql_tables_fields"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CheckTableDef(Dictionary<string, string> sql_tables_fields, Dictionary<string, string> mysql_tables_fields, out string msg)
        {
            bool check_result = true;
            try
            {
                string result_msg = string.Empty;
                foreach (KeyValuePair<string, string> table_info in sql_tables_fields)
                {
                    if (mysql_tables_fields.ContainsKey(table_info.Key.ToLower()))
                    {
                        List<string> sqlFieldList = table_info.Value.Split(',').ToList();
                        List<string> mysqlFieldList = mysql_tables_fields[table_info.Key.ToLower()].Split(',').ToList();
                        if (!mysqlFieldList.All(sqlFieldList.Contains)|| mysqlFieldList.Count != sqlFieldList.Count)
                        {
                            result_msg += table_info.Key + "表结构不一致，无法执行迁移，请检查" + Environment.NewLine;
                            check_result = false;
                        }
                    }
                }
                if (check_result)
                {
                    result_msg = "表结构一致，可执行数据迁移";
                }
                msg = result_msg;
                return check_result;
            }
            catch (Exception e)
            {
                WirterLogs.WriteFile(e);
                msg = "检查表结构出错，请查看日志";
                check_result = false;
                return check_result;
            }
        }
        /// <summary>
        /// 数据求和对比
        /// </summary>
        /// <param name="sql_data"></param>
        /// <param name="mysql_data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CheckData(Dictionary<string, long> sql_data, Dictionary<string, long> mysql_data, out string msg)
        {
            bool check_result = true;
            string result_msg = string.Empty;
            foreach (KeyValuePair<string, long> data_info in sql_data)
            {
                if (mysql_data.Where(a => a.Key.ToLower() == data_info.Key.ToLower()).FirstOrDefault().Value != data_info.Value)
                {
                    result_msg += data_info.Key + "求和不一致，请检查" + Environment.NewLine;
                    check_result = false;
                }
            }
            if (check_result)
            {
                result_msg = "数据验证一致" + Environment.NewLine;
            }
            msg = result_msg;
            return check_result;
        }
        // <summary>
        /// 判断两个字典是否是相等的(所有的字典项对应的值都相等)
        /// </summary>
        /// <typeparam name="TKey">字典项类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="sourceDictionary">源字典</param>
        /// <param name="targetDictionary">目标字典</param>
        /// <returns>两个字典相等则返回True,否则返回False</returns>
        public static bool EqualDictionary(ConcurrentDictionary<int, Model.MsgModel> sourceDictionary, ConcurrentDictionary<int, Model.MsgModel> targetDictionary)
        {
            //空字典直接返回False,即使是两个都是空字典,也返回False
            if (sourceDictionary == null || targetDictionary == null)
            {
                return false;
            }

            if (sourceDictionary.Count != targetDictionary.Count)
            {
                return false;
            }

            //比较两个字典的Key与Value
            foreach (var item in sourceDictionary)
            {
                //如果目标字典不包含源字典任意一项,则不相等
                if (!targetDictionary.ContainsKey(item.Key))
                {
                    return false;
                }

                //如果同一个字典项的值不相等,则不相等
                if (!targetDictionary[item.Key].Equals(item.Value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
