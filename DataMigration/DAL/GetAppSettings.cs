using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.DAL
{
    public class GetAppSettings
    {
        ///<summary> 
        ///返回***.exe.config文件中appSettings配置节的value项  
        ///</summary> 
        ///<param name="strKey"></param> 
        ///<returns></returns> 
        public static string GetAppConfig(string strKey)
        {
            string result;
            try
            {
                result = ConfigurationManager.AppSettings[strKey];
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }
        /// <summary>
        /// 读取配置的不用检查的表
        /// </summary>
        /// <returns></returns>
        public static string NoCheckData()
        {
            string names = string.Empty;
            string config = GetAppSettings.GetAppConfig("NoCheckData");
            if (!string.IsNullOrEmpty(config))
            {
                foreach (string table_name in GetAppSettings.GetAppConfig("NoCheckData").Split(','))
                {
                    names += string.IsNullOrEmpty(names) ? "'" + table_name + "'" : ",'" + table_name + "'";
                }
            }
            return names;
        }
    }
}
