using System;

namespace DataMigration.Model
{
    public class AreaInfo
    {
        public AreaInfo(){ }

        private int _ID;
        /// <summary>
        /// 主键自增长
        /// </summary>
        public int ID
        {
            set { _ID = value; }
            get { return _ID; }
        }

        private int _AreaType;
        /// <summary>
        /// 区服唯一标识
        /// </summary>
        public int AreaType
        {
            set { _AreaType = value; }
            get { return _AreaType; }
        }

        private string _ChnName;
        /// <summary>
        /// 中文注释
        /// </summary>
        public string ChnName
        {
            set { _ChnName = value; }
            get { return _ChnName; }
        }

        private string _EngName;
        /// <summary>
        /// 字段英文名
        /// </summary>
        public string EngName
        {
            set { _EngName = value; }
            get { return _EngName; }
        }

        private string _ValueType;
        /// <summary>
        /// 类型
        /// </summary>
        public string ValueType
        {
            set { _ValueType = value; }
            get { return _ValueType; }
        }

        private string _EvValue;
        /// <summary>
        /// 值
        /// </summary>
        public string EvValue
        {
            set { _EvValue = value; }
            get { return _EvValue; }
        }
    }
}

