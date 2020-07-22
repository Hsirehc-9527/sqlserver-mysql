using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using DataMigration.BLL;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DataMigration.DAL;

namespace DataMigration
{
    public partial class DataMigration : Form
    {
        //各个表的bool类型列语句
        Dictionary<string, List<string>> tables_bit_field;
        //SQL数据库连接串
        public string connectionString_MSSQL_Export = string.Empty;
        //MySql数据库连接串
        public static string connectionString_MYSQL_Export = string.Empty;
        //各个表的INSERT语句
        Dictionary<string, string> tables_insert;
        //各个表的字段名称
        Dictionary<string, string> sql_tables_fields;
        Dictionary<string, string> mysql_tables_fields;
        //完成数据加锁
        private static object countLock = new object();
        //返回信息字典(key:表名，value:当前表迁移完成行数百分比)
        private static ConcurrentDictionary<string, string> msgDic = new ConcurrentDictionary<string, string>();
        //完成数量(包含成功和失败的)
        private static int completedCount = 0;
        //进度条最大进度
        private int maxProgress;
        //MySql数据库表
        List<string> tables_mysql_db;
        //计时
        private int totalSeconds = 0;
        public DataMigration()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 获取数据库表
        /// </summary>
        public void GetTable()
        {
            connectionString_MSSQL_Export = tb_SqlConn.Text;
            string mysqlConn = tb_MySqlConn.Text.EndsWith(";") ? tb_MySqlConn.Text : tb_MySqlConn.Text + ";";
            connectionString_MYSQL_Export = mysqlConn.Contains("Variables") ? mysqlConn : mysqlConn + "Allow User Variables=True;";
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerAsync();
        }
        /// <summary>
        /// 使用后台进程加载列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetAllTables();
        }
        private void GetAllTables()
        {
            //获取Sql数据库表列表，加载字段，生成插入语句脚本
            GetAllMSSQLTables();
            GetAllMSSQLFieldType();
            //获取mysql数据库列表
            GetAllMYSQLTables();
            //加载控件
            ListAllTables();
        }
        /// <summary>
        /// 获取Sql数据库表列表，加载字段，生成插入语句脚本
        /// </summary>
        private void GetAllMSSQLTables()
        {
            try
            {
                SqlDataReader rdr = GetData.GetSqlDataReader(connectionString_MSSQL_Export, GetData.EXPORT_SELECT_ALL_TABLES);
                if (rdr != null)
                {
                    tables_insert = new Dictionary<string, string>();
                    while (rdr.Read())
                    {
                        tables_insert.Add(rdr["tablename"].ToString(), rdr["insertscript"].ToString());
                    }
                    rdr.Close();
                }
                rdr = GetData.GetSqlDataReader(connectionString_MSSQL_Export, GetData.EXPORT_SELECT_ALL_TABLES_FIELDS);
                if (rdr != null)
                {
                    sql_tables_fields = new Dictionary<string, string>();
                    while (rdr.Read())
                    {
                        sql_tables_fields.Add(rdr["tablename"].ToString(), rdr["insertscript"].ToString());
                    }
                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                WirterLogs.WriteFile(ex);
            }
        }
        /// <summary>
        /// 获取Sql数据库为bool的字段列表
        /// </summary>
        private bool GetAllMSSQLFieldType()
        {
            try
            {
                tables_bit_field = new Dictionary<string, List<string>>();
                SqlDataReader rdr = GetData.GetSqlDataReader(connectionString_MSSQL_Export, GetData.EXPORT_SELECT_TABLE_TYPE);
                if (rdr != null)
                {
                    tables_bit_field = new Dictionary<string, List<string>>();
                    while (rdr.Read())
                    {
                        if (!tables_bit_field.ContainsKey(rdr["TABLE_NAME"].ToString()))
                        {
                            tables_bit_field.Add(rdr["TABLE_NAME"].ToString(), new List<string>() { rdr["COLUMN_NAME"].ToString() });
                        }
                        else
                        {
                            tables_bit_field[rdr["TABLE_NAME"].ToString()].Add(rdr["COLUMN_NAME"].ToString());
                        }
                    }
                    rdr.Close();
                }
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                WirterLogs.WriteFile(ex);
                return false;
            }
        }
        /// <summary>
        /// 获取MySql数据库表列表
        /// </summary>
        private void GetAllMYSQLTables()
        {
            MySqlConnection conn = new MySqlConnection(connectionString_MYSQL_Export);
            try
            {
                MySqlDataReader rdr = GetData.GetMySqlDataReader(connectionString_MYSQL_Export, GetData.EXPORT_SELECT_ALL_TABLES_MYSQL + "'" + conn.Database + "'");
                if (rdr != null)
                {
                    tables_mysql_db = new List<string>();
                    while (rdr.Read())
                    {
                        tables_mysql_db.Add(rdr["TABLE_NAME"].ToString());
                    }
                    rdr.Close();
                }
                rdr = GetData.GetMySqlDataReader(connectionString_MYSQL_Export, GetData.EXPORT_SELECT_ALL_TABLES_MYSQL_FIELDS + "'" + conn.Database + "'");
                if (rdr != null)
                {
                    mysql_tables_fields = new Dictionary<string, string>();
                    while (rdr.Read())
                    {
                        if (!mysql_tables_fields.ContainsKey(rdr["TABLE_NAME"].ToString()))
                        {
                            mysql_tables_fields.Add(rdr["TABLE_NAME"].ToString(), "[" + rdr["COLUMN_NAME"].ToString() + "]");
                        }
                        else
                        {
                            mysql_tables_fields[rdr["TABLE_NAME"].ToString()] = mysql_tables_fields[rdr["TABLE_NAME"].ToString()] + ", [" + rdr["COLUMN_NAME"].ToString() + "]";
                        }
                    }
                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                WirterLogs.WriteFile(ex);
            }
        }
        /// <summary>
        /// 加载列表控件
        /// </summary>
        private void ListAllTables()
        {
            try
            {
                if (tables_mysql_db != null && tables_mysql_db.Count > 0)
                {
                    if (tables_insert != null && tables_insert.Count > 0)
                    {
                        TablesListbox.Invoke((Action)(() => TablesListbox.Items.Clear()));
                        foreach (string item in tables_insert.Keys)
                        {
                            if (tables_mysql_db.Contains(item.ToLower()))
                            {
                                TablesListbox.Invoke((Action)(() => TablesListbox.Items.Add(item)));
                            }
                        }
                        SelectedLabel.Invoke((Action)(() => SelectedLabel.Text = "0" + "/" + TablesListbox.Items.Count));
                    }
                }
                SelectAllTables.Invoke((Action)(()=> SelectAllTables.Enabled = true));
                bt_CheckTableDef.Invoke((Action)(() => bt_CheckTableDef.Enabled = true));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                WirterLogs.WriteFile(e);
            }
        }
        /// <summary>
        /// 全选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllTables_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectAllTables.Checked)
            {
                for (int i = 0; i < TablesListbox.Items.Count; i++)
                {
                    TablesListbox.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < TablesListbox.Items.Count; i++)
                {
                    TablesListbox.SetItemChecked(i, false);
                }
            }

            SelectedLabel.Text = TablesListbox.CheckedItems.Count + "/" + TablesListbox.Items.Count;
        }
        /// <summary>
        /// 迁移数据按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateScripts_Click(object sender, EventArgs e)
        {
            var lob = (IList)Invoke(new Func<IList>(() => TablesListbox.CheckedItems.Cast<object>().ToList()));
            if (lob != null && lob.Count > 0)
            {
                if (MessageBox.Show("确定要将MSSQL数据加载到MYSQL数据库吗", "确认操作", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (MessageBox.Show("MySQL数据库中的所有现有数据都将被清除，您确定吗?", "确认操作", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        bt_CheckData.Enabled = false;
                        bt_GetList.Enabled = false;
                        bt_GenerateScripts.Enabled = false;
                        bt_CheckTableDef.Enabled = false;
                        totalSeconds = 0;
                        ElapsedTime.Start();
                        progressBar.Invoke((Action)(() => progressBar.Value = 0));
                        BackgroundWorker backgroundWorker = new BackgroundWorker();
                        backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(DataMigration_DoWork);
                        backgroundWorker.RunWorkerAsync();

                    }
                }
            }
            else
            {
                MessageBox.Show("未选择表...");
            }
        }
        /// <summary>
        /// 使用后台进程迁移数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataMigration_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, string> tablesList = null;
            string val;
            if (tables_insert != null && tables_insert.Count > 0)
            {
                tablesList = new Dictionary<string, string>();
                var lob = (IList)this.Invoke(new Func<IList>(() => TablesListbox.CheckedItems.Cast<object>().ToList()));
                foreach (string item in lob)
                {
                    tables_insert.TryGetValue(item, out val);
                    tablesList.Add(item, val);
                }
            }
            GetTableData(tablesList, sql_tables_fields, connectionString_MSSQL_Export);
        }
        /// <summary>
        /// 获取Sql数据库中的数据
        /// </summary>
        /// <param name="tablesList"></param>
        /// <param name="tables_fields"></param>
        /// <param name="connectionString"></param>
        private void GetTableData(Dictionary<string, string> tablesList, Dictionary<string, string> tables_fields, string connectionString)
        {
            try
            {
                if (tablesList != null && tablesList.Count > 0 && tables_fields != null && tables_fields.Count > 0)
                {
                    ListLog.Invoke((Action)(() => this.ListLog.Clear()));
                    Dictionary<string, int> SqlTableRowsCount = new Dictionary<string, int>();
                    string tableName = "(";
                    for (int i = 0; i < tablesList.Count; i++)
                    {
                        if (i != tablesList.Count - 1)
                        {
                            tableName += "'" + tablesList.ElementAt(i).Key + "'" + ",";
                        }
                        else
                        {
                            tableName += "'" + tablesList.ElementAt(i).Key + "'" + ")";
                        }
                    }
                    SqlDataReader sqlRdr = GetData.GetSqlDataReader(connectionString_MSSQL_Export, GetData.EXPORT_SELECT_ALL_SQLROW + tableName);
                    if (sqlRdr != null)
                    {
                        while (sqlRdr.Read())
                        {
                            SqlTableRowsCount.Add(sqlRdr["TABLE_NAME"].ToString(), int.Parse(sqlRdr["ROWS"].ToString()));
                            if (int.Parse(sqlRdr["ROWS"].ToString()) == 0)
                            {
                                FontColor(ListLog, Color.Red, sqlRdr["TABLE_NAME"].ToString() + " 表为空，不迁移");
                            }
                        }
                        sqlRdr.Close();
                    }
                    maxProgress = SqlTableRowsCount.Where(a => a.Value > 0).ToDictionary(a => a.Key, b => b.Value).Keys.Count;
                    if (maxProgress > 0)
                    {
                        FontColor(ListLog, Color.Green, "开始迁移数据...");
                        FontColor(ListLog, Color.Green, "总迁移表数" + maxProgress);
                        progressBar.Invoke((Action)(() => progressBar.Maximum = maxProgress));
                        Dictionary<string, string> commDic = GetComm();
                        FontColor(ListLog, Color.Green, "开始清空表...");
                        foreach (string table in tablesList.Keys)
                        {
                            StringBuilder sb = new StringBuilder();
                            // 设置foreign_key_checks = 0;
                            sb.Append("SET foreign_key_checks = 0;" + Environment.NewLine);
                            Dictionary<string, List<StringBuilder>> queryToExecute = new Dictionary<string, List<StringBuilder>>();
                            queryToExecute.Add("FK_Disable", new List<StringBuilder> { sb });
                            // 添加删除脚本
                            sb = new StringBuilder();
                            sb.Append("SET SQL_SAFE_UPDATES = 0;" + Environment.NewLine);
                            sb.Append("TRUNCATE `" + table + "`;" + Environment.NewLine);
                            queryToExecute.Add(table, new List<StringBuilder> { sb });
                            GetData.GetMysqlDBConnection_Execute(connectionString_MYSQL_Export, queryToExecute, progressBar, ListLog);
                            FontColor(ListLog, Color.Green, table + " 表清空完成...");
                        }
                        completedCount = 0;
                        foreach (string table in SqlTableRowsCount.Where(a => a.Value > 0).ToDictionary(a => a.Key, b => b.Value).Keys)
                        {
                            Thread thread = new Thread(() =>
                            {
                                try
                                {
                                    msgDic.TryAdd(table, "(" + decimal.Parse((((decimal)0 / (decimal)SqlTableRowsCount[table]) * 100).ToString("0.00")) + "%" + ")");
                                    string fields, initialQuery;
                                    StringBuilder sb = new StringBuilder();
                                    Dictionary<string, List<StringBuilder>> queryToExecute = new Dictionary<string, List<StringBuilder>>();
                                    int progress = 0;
                                    // 打印日志
                                    FontColor(ListLog, Color.Green, table + "表数据开始迁移...");
                                    tables_fields.TryGetValue(table, out fields);
                                    string sqlComm = string.Empty;
                                    List<int> field_index = new List<int>();
                                    if (tables_bit_field.ContainsKey(table))
                                    {
                                        foreach (string field in fields.Split(','))
                                        {
                                            if (tables_bit_field[table].Contains(field.Replace("[", "").Replace("]", "").Trim()))
                                            {
                                                field_index.Add(fields.Split(',').ToList().IndexOf(field));
                                            }
                                        }
                                    }
                                    if (commDic.ContainsKey(table))
                                    {
                                        sqlComm = "SELECT " + fields + " FROM [" + table + "]" + "WHERE " + commDic[table];
                                    }
                                    else
                                    {
                                        sqlComm = "SELECT " + fields + " FROM [" + table + "]";
                                    }
                                    SqlDataReader rdr = GetData.GetSqlDataReader(connectionString, sqlComm);
                                    if (rdr != null)
                                    {
                                        tablesList.TryGetValue(table, out initialQuery);
                                        // 追加插入脚本
                                        sb.Append(initialQuery);
                                        if (rdr.HasRows)
                                        {
                                            List<StringBuilder> builders = new List<StringBuilder>();
                                            int count = 0;
                                            while (rdr.Read())
                                            {
                                                if (count > 10000)
                                                {
                                                    builders.Add(sb);
                                                    queryToExecute.Add(table, builders);
                                                    GetData.GetMysqlDBConnection_Execute(connectionString_MYSQL_Export, queryToExecute, progressBar, ListLog);
                                                    msgDic[table] = "(" + decimal.Parse((((decimal)progress / (decimal)SqlTableRowsCount[table]) * 100).ToString("0.00")) + "%" + ")";
                                                    queryToExecute = new Dictionary<string, List<StringBuilder>>();
                                                    builders = new List<StringBuilder>();
                                                    count = 0;
                                                    sb = new StringBuilder();
                                                    sb.Append(initialQuery);
                                                }
                                                // 追加插入脚本
                                                sb.Append("(");
                                                for (int i = 0; i < rdr.FieldCount; i++)
                                                {
                                                    // 格式化引号的值
                                                    if (rdr[i].ToString().Contains("'"))
                                                    {
                                                        string[] s = rdr[i].ToString().Split('\'');
                                                        sb.Append("'");
                                                        if (i + 1 == rdr.FieldCount)
                                                        {
                                                            sb.Append("" + s[0].ToString() + "''" + s[1].ToString() + "'");
                                                        }
                                                        else
                                                        {
                                                            sb.Append("" + s[0].ToString() + "''" + s[1].ToString() + "', ");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // 根据数据类型格式化数据
                                                        if (field_index.Contains(i) && rdr[i] == (object)System.DBNull.Value)
                                                        {
                                                            if (i + 1 == rdr.FieldCount)
                                                            {
                                                                sb.Append(0);
                                                            }
                                                            else
                                                            {
                                                                sb.Append(0 + ",");
                                                            }
                                                        }
                                                        else if (rdr[i] == (object)System.DBNull.Value)
                                                        {
                                                            if (i + 1 == rdr.FieldCount)
                                                            {
                                                                sb.Append("NULL");
                                                            }
                                                            else
                                                            {
                                                                sb.Append("NULL, ");
                                                            }
                                                        }
                                                        else if (rdr[i] is long || rdr[i] is short)
                                                        {
                                                            if (i + 1 == rdr.FieldCount)
                                                            {
                                                                sb.Append(" " + rdr[i].ToString());
                                                            }
                                                            else
                                                            {
                                                                sb.Append(" " + rdr[i].ToString() + ", ");
                                                            }
                                                        }
                                                        else if (rdr[i] is bool)
                                                        {
                                                            if (i + 1 == rdr.FieldCount)
                                                            {
                                                                sb.Append(rdr[i].ToString().ToLower().Equals("false") ? 0 : 1);
                                                            }
                                                            else
                                                            {
                                                                sb.Append(rdr[i].ToString().ToLower().Equals("false") ? 0 + "," : 1 + ",");
                                                            }
                                                        }
                                                        else if (rdr[i] is DateTime)
                                                        {
                                                            sb.Append("'");
                                                            if (i + 1 == rdr.FieldCount)
                                                            {
                                                                sb.Append("" + Convert.ToDateTime(rdr[i]).ToString("yyyy/MM/dd HH:mm:ss.fff") + "'");
                                                            }
                                                            else
                                                            {
                                                                sb.Append("" + Convert.ToDateTime(rdr[i]).ToString("yyyy/MM/dd HH:mm:ss.fff") + "', ");
                                                            }
                                                        }
                                                        else if (rdr[i] is byte[])
                                                        {
                                                            if (((byte[])rdr[i]).Length != 0)
                                                            {
                                                                string HexStr = byteToHexStr((byte[])rdr[i]);
                                                                if (i + 1 == rdr.FieldCount)
                                                                {
                                                                    sb.Append("" + HexStr);
                                                                }
                                                                else
                                                                {
                                                                    sb.Append("" + HexStr + ", ");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (i + 1 == rdr.FieldCount)
                                                                {
                                                                    sb.Append("NULL");
                                                                }
                                                                else
                                                                {
                                                                    sb.Append("NULL, ");
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            sb.Append("'");
                                                            if (i + 1 == rdr.FieldCount)
                                                            {
                                                                sb.Append("" + rdr[i].ToString() + "'");
                                                            }
                                                            else
                                                            {
                                                                sb.Append("" + rdr[i].ToString() + "', ");
                                                            }
                                                        }
                                                    }
                                                }
                                                sb.Append(")," + Environment.NewLine);
                                                count++;
                                                progress++;
                                            }
                                            builders.Add(sb);
                                            queryToExecute.Add(table, builders);
                                        }
                                        rdr.Close();
                                    }
                                    GetData.GetMysqlDBConnection_Execute(connectionString_MYSQL_Export, queryToExecute, progressBar, ListLog);
                                    msgDic[table] = "(" + decimal.Parse((((decimal)SqlTableRowsCount[table] / (decimal)SqlTableRowsCount[table]) * 100).ToString("0.00")) + "%" + ")";
                                    lock (countLock)
                                    {
                                        completedCount++;
                                    }
                                    // 进度条更新
                                    progressBar.Invoke((Action)(() => this.progressBar.Value = completedCount));
                                    FontColor(ListLog, Color.Blue, table + " 表复制成功");
                                }
                                catch (Exception e)
                                {
                                    FontColor(ListLog, Color.Red, "线程方法执行错误:" + e.Message);
                                    WirterLogs.WriteFile(e);
                                    lock (countLock)
                                    {
                                        completedCount++;
                                    }
                                }
                            });
                            thread.IsBackground = true;
                            thread.Start();
                        }
                        while (true)
                        {
                            Thread.Sleep(5000);
                            if (completedCount == SqlTableRowsCount.Where(a => a.Value > 0).ToDictionary(a => a.Key, b => b.Value).Keys.Count)
                            {
                                break;
                            }
                            else
                            {
                                foreach (var msg in msgDic)
                                {
                                    if (msg.Value != "(100.00%)")
                                    {
                                        FontColor(ListLog, Color.Green, msg.Key + ":" + msg.Value);
                                    }
                                }
                                ListLog.Invoke((Action)(() => ListLog.SelectionColor = Color.Red));
                                ListLog.Invoke((Action)(() => ListLog.AppendText("-------------------------------------------------------" + Environment.NewLine)));
                                ListLog.Invoke((Action)(() => ListLog.Focus()));
                            }
                        }
                        Dictionary<string, List<StringBuilder>> ToExecute = new Dictionary<string, List<StringBuilder>>();
                        ToExecute.Add("FK_Enabled", new List<StringBuilder> { new StringBuilder("SET foreign_key_checks = 1;" + Environment.NewLine) });
                        GetData.GetMysqlDBConnection_Execute(connectionString_MYSQL_Export, ToExecute, progressBar, ListLog);
                        this.ElapsedTime.Enabled = true;
                        this.ElapsedTime.Stop();
                        bt_CheckData.Invoke((Action)(() => bt_CheckData.Enabled = true));
                        bt_GetList.Invoke((Action)(() => bt_GetList.Enabled = true));
                        bt_GenerateScripts.Invoke((Action)(() => bt_GenerateScripts.Enabled = true));
                        FontColor(ListLog, Color.Blue, "迁移完成！！！");
                        bt_GenerateScripts.Invoke((Action)(() => bt_GenerateScripts.Enabled = false));
                        bt_CheckTableDef.Invoke((Action)(() => bt_CheckTableDef.Enabled = false));
                    }
                    else
                    {
                        this.ElapsedTime.Enabled = true;
                        this.ElapsedTime.Stop();
                        bt_CheckData.Invoke((Action)(() => bt_CheckData.Enabled = true));
                        bt_GetList.Invoke((Action)(() => bt_GetList.Enabled = true));
                        bt_GenerateScripts.Invoke((Action)(() => bt_GenerateScripts.Enabled = true));
                    }
                }
            }
            catch (Exception ex)
            {
                FontColor(ListLog, Color.Red, "迁移语句出错，查看日志");
                WirterLogs.WriteFile(ex);
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            try
            {
                string returnStr = "";
                if (bytes.Length != 0)
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        returnStr += bytes[i].ToString("X2");
                    }
                }
                return "0x" + returnStr;
            }
            catch (Exception e)
            {
                WirterLogs.WriteFile(e);
                return "";
            }
        }
        private void TablesListbox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                int c = TablesListbox.CheckedItems.Count + 1;
                this.SelectedLabel.Text = c + "/" + TablesListbox.Items.Count;
                UserControl control = new UserControl(TablesListbox.Items[e.Index].ToString(), "", flp_Controls, true);
            }
            else
            {
                int c = TablesListbox.CheckedItems.Count - 1;
                this.SelectedLabel.Text = c + "/" + TablesListbox.Items.Count;
                UserControl control = new UserControl(TablesListbox.Items[e.Index].ToString(), "", flp_Controls, false);
            }
        }
        /// <summary>
        /// 获取条件表达式填写的Sql条件
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetComm()
        {
            Dictionary<string, string> CommDic = new Dictionary<string, string>();
            try
            {
                foreach (var item in flp_Controls.Controls)
                {
                    UserControl control = (UserControl)item;
                    if (!string.IsNullOrEmpty(control.TextBoxValue))
                    {
                        CommDic.Add(control.LableValue, control.TextBoxValue);
                    }
                }
                return CommDic;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return CommDic;
            }
        }
        /// <summary>
        /// 执行时间更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElapsedTime_Tick(object sender, EventArgs e)
        {
            totalSeconds++;
            if (totalSeconds > 0)
            {
                int tSec = totalSeconds;
                int sSec = tSec % 60;
                int minute = tSec / 60;
                int tMinute = minute % 60;
                int hour = minute / 60;
                int tHour = hour % 12;
                this.TimeTaken_Execute.Text = "执行时间:" + tHour.ToString() + ":" + tMinute.ToString() + ":" + sSec.ToString();
            }
        }
        /// <summary>
        /// 数据一致性检查点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_CheckData_Click(object sender, EventArgs e)
        {
            totalSeconds = 0;
            ElapsedTime.Start();
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(CheckData_DoWork);
            backgroundWorker.RunWorkerAsync();
            bt_CheckData.Enabled = false;
        }
        /// <summary>
        /// 数据一致性检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckData_DoWork(object sender, EventArgs e)
        {
            try
            {
                var lob = (IList)this.Invoke(new Func<IList>(() => TablesListbox.CheckedItems.Cast<object>().ToList()));
                if (lob.Count > 0)
                {
                    FontColor(ListLog, Color.Green, "开始验证表行数...");
                    string tableName = "(";
                    for (int i = 0; i < lob.Count; i++)
                    {
                        if (i != lob.Count - 1)
                        {
                            tableName += "'" + lob[i] + "'" + ",";
                        }
                        else
                        {
                            tableName += "'" + lob[i] + "'" + ")";
                        }
                    }
                    Dictionary<string, int> mySqlTableRowsCount = new Dictionary<string, int>();
                    Dictionary<string, int> SqlTableRowsCount = new Dictionary<string, int>();
                    SqlDataReader sqlRdr = GetData.GetSqlDataReader(connectionString_MSSQL_Export, GetData.EXPORT_SELECT_ALL_SQLROW + tableName);
                    if (sqlRdr != null)
                    {
                        while (sqlRdr.Read())
                        {
                            SqlTableRowsCount.Add(sqlRdr["TABLE_NAME"].ToString().ToLower(), int.Parse(sqlRdr["ROWS"].ToString()));
                        }
                        sqlRdr.Close();
                    }
                    foreach (string item in lob)
                    {
                        MySqlDataReader mySqlRdr = GetData.GetMySqlDataReader(connectionString_MYSQL_Export, string.Format("SELECT '{0}' AS TABLE_NAME,COUNT(*) AS ROWS FROM {0}", item));
                        if (mySqlRdr != null)
                        {
                            while (mySqlRdr.Read())
                            {
                                mySqlTableRowsCount.Add(mySqlRdr["TABLE_NAME"].ToString().ToLower(), int.Parse(mySqlRdr["ROWS"].ToString()));
                            }
                            mySqlRdr.Close();
                        }
                    }
                    bool stat = true;
                    foreach (var mySqlRows in mySqlTableRowsCount)
                    {
                        if (SqlTableRowsCount[mySqlRows.Key] != mySqlRows.Value)
                        {
                            stat = false;
                            FontColor(ListLog, Color.Red, mySqlRows.Key + "表行不一致，请检查");
                        }
                    }
                    if (stat)
                    {
                        FontColor(ListLog, Color.Green, "行数检查完成，各表行一致");
                    }
                    FontColor(ListLog, Color.Green, "开始导出数值进行验证...");
                    Dictionary<string, long> mySqlTableRowsSum = new Dictionary<string, long>();
                    Dictionary<string, long> SqlTableRowsSum = new Dictionary<string, long>();
                    string noCheckTbale = GetAppSettings.NoCheckData();
                    SqlDataReader rdr = GetData.GetSqlDataReader(connectionString_MSSQL_Export, string.Format(GetData.EXPORT_SELECT_ALL_TABLES_SQL_SUM, tableName, string.IsNullOrEmpty(noCheckTbale) ? null : string.Format("AND TABLE_NAME NOT IN ({0})", noCheckTbale)));
                    if (rdr != null)
                    {
                        while (rdr.Read())
                        {
                            SqlTableRowsSum.Add(rdr["FIELD"].ToString(), rdr["RESULT"] == DBNull.Value ? 0 : long.Parse(rdr["RESULT"].ToString()));
                        }
                        rdr.Close();
                    }
                    MySqlConnection conn = new MySqlConnection(connectionString_MYSQL_Export);
                    MySqlDataReader myRdr = GetData.GetMySqlDataReader(connectionString_MYSQL_Export, string.Format(GetData.EXPORT_SELECT_ALL_TABLES_MYSQL_SQLS, tableName, string.IsNullOrEmpty(noCheckTbale) ? null : string.Format("AND TABLE_NAME NOT IN({0})", noCheckTbale), conn.Database));
                    List<string> sqlList = new List<string>();
                    if (myRdr != null)
                    {
                        while (myRdr.Read())
                        {
                            sqlList.Add(myRdr["sqls"].ToString());
                        }
                        myRdr.Close();
                    }
                    string mySqlComm = "DROP TABLE IF EXISTS temp;\n CREATE TABLE temp (FIELD VARCHAR(500),RESULT VARCHAR(500));";
                    foreach (string sql in sqlList)
                    {
                        mySqlComm += "\n" + sql;
                    }
                    mySqlComm += "\n" + "SELECT * FROM temp";
                    myRdr = GetData.GetMySqlDataReader(connectionString_MYSQL_Export, mySqlComm);
                    if (myRdr != null)
                    {
                        while (myRdr.Read())
                        {
                            mySqlTableRowsSum.Add(myRdr["FIELD"].ToString(), myRdr["RESULT"] == DBNull.Value ? 0 : long.Parse(myRdr["RESULT"].ToString()));
                        }
                        myRdr.Close();
                    }
                    if (mySqlTableRowsSum.Count() > 0 || SqlTableRowsSum.Count() > 0)
                    {
                        FontColor(ListLog, Color.Green, "开始对比数据值...");
                        string check_msg = string.Empty;
                        bool check_result = CheckDate.CheckData(SqlTableRowsSum, mySqlTableRowsSum, out check_msg);
                        FontColor(ListLog, check_result ? Color.Blue : Color.Red, check_msg.Substring(0, check_msg.Length - 2));
                    }
                    bt_CheckData.Invoke((Action)(() => bt_CheckData.Enabled = true));
                    FontColor(ListLog, Color.Blue, "数据验证结束！！！");
                    this.ElapsedTime.Enabled = true;
                    this.ElapsedTime.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                WirterLogs.WriteFile(ex);
            }
        }
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="color"></param>
        /// <param name="msg"></param>
        public void FontColor(RichTextBox textBox, Color color, string msg)
        {
            textBox.Invoke((Action)(() => textBox.SelectionColor = color));
            textBox.Invoke((Action)(() => textBox.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + msg + Environment.NewLine)));
            textBox.Invoke((Action)(() => textBox.Focus()));
        }
        #region RichTextBox失去焦点事件
        private void ListLogLostFocus(object sender, EventArgs e)
        {
            ListLog.SelectionStart = ListLog.Text.Length;
        }
        #endregion
        /// <summary>
        /// 获取数据库表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_GetList_Click(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(tb_MySqlConn.Text)) && (!string.IsNullOrEmpty(tb_SqlConn.Text)))
            {
                SelectAllTables.Checked = false;
                SelectAllTables.Enabled = false;
                GetTable();
            }
            else
            {
                MessageBox.Show("请填写连接");
            }
        }
        /// <summary>
        /// 表结构对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_CheckTableDef_Click(object sender, EventArgs e)
        {
            string check_msg = string.Empty;
            bool check_result = CheckDate.CheckTableDef(sql_tables_fields, mysql_tables_fields, out check_msg);
            bt_GenerateScripts.Enabled = check_result;
            FontColor(ListLog, check_result ? Color.Blue : Color.Red, check_msg);
        }
    }
}
