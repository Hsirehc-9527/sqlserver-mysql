using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class UserControl : System.Windows.Forms.UserControl
    {
        public string TextBoxValue
        {
            get { return tb_Where.Text; }
            set { tb_Where.Text = value; }
        }
        public string LableValue
        {
            get { return lb_TableName.Text; }
            set { lb_TableName.Text = value; }
        }
        public UserControl(string tableName,string comm, FlowLayoutPanel flp_Controls,bool add)
        {
            InitializeComponent();
            LableValue = tableName;
            TextBoxValue = comm;
            if (add)
            {
                if (flp_Controls.Controls.Count > 0)
                {
                    foreach (var item in flp_Controls.Controls)
                    {
                        UserControl control = (UserControl)item;
                        if (!string.Equals(control.LableValue, tableName))
                        {
                            flp_Controls.Controls.Add(this);
                        }
                    }
                }
                else
                {
                    flp_Controls.Controls.Add(this);
                }
            }
            else
            {
                if (flp_Controls.Controls.Count > 0)
                {
                    foreach (var item in flp_Controls.Controls)
                    {
                        UserControl control = (UserControl)item;
                        if (string.Equals(control.LableValue, tableName))
                        {
                            flp_Controls.Controls.Remove(control);
                        }
                    }
                }
            }
        }
    }
}
