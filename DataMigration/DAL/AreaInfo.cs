using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.DAL
{
    public class AreaInfo
    {
        private static string selectListSqlStr = "select AreaType,EngName,EvValue from AreaInfo with(nolock)";
        /// <summary>
        /// 取得区服配置信息
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <returns>区服</returns>
        public static Dictionary<int, Dictionary<string, Model.AreaInfo>> GetModelDic(string connStr)
        {
            Dictionary<int, Dictionary<string, Model.AreaInfo>> Temp = new Dictionary<int, Dictionary<string, Model.AreaInfo>>();
            SqlConnection sqlConn = new SqlConnection(connStr);
            SqlCommand command = sqlConn.CreateCommand();
            sqlConn.Open();
            command.CommandType = CommandType.Text;
            command.CommandText = selectListSqlStr;
            SqlDataReader eReader = command.ExecuteReader();
            int areaType = 0;
            string engName = String.Empty;
            while (eReader.Read())
            {
                areaType = (int)eReader["AreaType"];
                engName = eReader["EngName"].ToString();
                if (!Temp.ContainsKey(areaType))
                {
                    Temp.Add(areaType, new Dictionary<string, Model.AreaInfo>());
                }
                if (!Temp[areaType].ContainsKey(engName))
                {
                    Temp[areaType].Add(engName, DataRowToModel(eReader));
                }
            }
            return Temp;
        }
        public static Model.AreaInfo DataRowToModel(SqlDataReader oRow)
        {
            Model.AreaInfo model = new Model.AreaInfo();
            if (oRow != null)
            {
                object AreaType = oRow["AreaType"];
                if (AreaType != null && AreaType != System.DBNull.Value)
                {
                    model.AreaType = (int)AreaType;
                }
                object EngName = oRow["EngName"];
                if (EngName != null)
                {
                    model.EngName = EngName.ToString();
                }
                object EvValue = oRow["EvValue"];
                if (EvValue != null)
                {
                    model.EvValue = EvValue.ToString();
                }
            }
            return model;
        }
    }
}
