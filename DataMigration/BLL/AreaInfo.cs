using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DataMigration.DAL;
using DataMigration.Model;

namespace DataMigration.BLL
{
    public class AreaInfo
    {
        /// <summary>
        /// 区服配置
        /// </summary>
        public static Dictionary<int, Dictionary<string, Model.AreaInfo>> cAreaDic;
        public static Dictionary<int, Model.AreaKey> areaKey;
        /// <summary>
        /// 初始化区服配置
        /// </summary>
        public static void Init()
        {
            cAreaDic = new Dictionary<int, Dictionary<string, Model.AreaInfo>>();
            areaKey = new Dictionary<int, Model.AreaKey>();
            lock (cAreaDic)
            {
                string connectionString = GetAppSettings.GetAppConfig("Conn_Public");
                //填充区服配置
                cAreaDic = DAL.AreaInfo.GetModelDic(connectionString);
            }
            if (cAreaDic.Count() > 0)
            {
                foreach (KeyValuePair<int, Dictionary<string, Model.AreaInfo>> item in cAreaDic)
                {
                    if (item.Value != null)
                    {
                        AreaKey info = new AreaKey();
                        if (!areaKey.ContainsKey(item.Key))
                        {
                            info.AreaType = item.Key;
                            info.conn_Players = GetEvValue("conn_Players", item.Value);
                            info.conn_Players_GoldLog = GetEvValue("conn_Players_GoldLog", item.Value);
                            info.conn_Players_Log = GetEvValue("conn_Players_Log", item.Value);
                        }
                        areaKey.Add(info.AreaType, info);
                    }
                }
            }
        }
        /// <summary>
        /// 获得配置
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>配置字符串</returns>
        public static string GetEvValue(string key, Dictionary<string, Model.AreaInfo> areaConfig)
        {
            string result = string.Empty;
            if ((areaConfig != null && areaConfig.ContainsKey(key)))
            {
                return areaConfig[key].EvValue;
            }
            return result;
        }
    }
}
