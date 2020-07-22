using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.Model
{
    public class MsgModel
    {
        public MsgModel() { }
        private string _msg;
        /// <summary>
        /// 消息
        /// </summary>
        public string msg
        {
            set { _msg = value; }
            get { return _msg; }
        }
        private Color _color;
        /// <summary>
        /// 消息
        /// </summary>
        public Color color
        {
            set { _color = value; }
            get { return _color; }
        }
    }
}
