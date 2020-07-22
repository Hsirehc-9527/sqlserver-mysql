using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.Model
{
    public class AreaKey
    {
        public AreaKey() { }
        private int _AreaType;
        /// <summary>
        /// 区服
        /// </summary>
        public int AreaType
        {
            set { _AreaType = value; }
            get { return _AreaType; }
        }
        private string _conn_Players;
        /// <summary>
        /// 玩家库连接串
        /// </summary>
        public string conn_Players
        {
            set { _conn_Players = value; }
            get { return _conn_Players; }
        }
        private string _conn_Players_Log;
        /// <summary>
        /// 玩家日志库连接串
        /// </summary>
        public string conn_Players_Log
        {
            set { _conn_Players_Log = value; }
            get { return _conn_Players_Log; }
        }
        private string _conn_Players_GoldLog;
        /// <summary>
        /// 玩家元宝日志库连接串
        /// </summary>
        public string conn_Players_GoldLog
        {
            set { _conn_Players_GoldLog = value; }
            get { return _conn_Players_GoldLog; }
        }
    }
}
