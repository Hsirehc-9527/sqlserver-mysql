namespace DataMigration
{
    partial class DataMigration
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tb_SqlConn = new System.Windows.Forms.TextBox();
            this.SelectedLabel = new System.Windows.Forms.Label();
            this.tb_MySqlConn = new System.Windows.Forms.TextBox();
            this.TablesListbox = new System.Windows.Forms.CheckedListBox();
            this.SelectAllTables = new System.Windows.Forms.CheckBox();
            this.bt_GenerateScripts = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.ListLog = new System.Windows.Forms.RichTextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.TableName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Where = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.flp_Controls = new System.Windows.Forms.FlowLayoutPanel();
            this.ElapsedTime = new System.Windows.Forms.Timer(this.components);
            this.TimeTaken_Execute = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bt_CheckData = new System.Windows.Forms.Button();
            this.bt_GetList = new System.Windows.Forms.Button();
            this.lb_Progress = new System.Windows.Forms.Label();
            this.bt_CheckTableDef = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_SqlConn
            // 
            this.tb_SqlConn.Location = new System.Drawing.Point(111, 11);
            this.tb_SqlConn.Name = "tb_SqlConn";
            this.tb_SqlConn.Size = new System.Drawing.Size(885, 21);
            this.tb_SqlConn.TabIndex = 0;
            // 
            // SelectedLabel
            // 
            this.SelectedLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SelectedLabel.Location = new System.Drawing.Point(64, 21);
            this.SelectedLabel.Name = "SelectedLabel";
            this.SelectedLabel.Size = new System.Drawing.Size(131, 12);
            this.SelectedLabel.TabIndex = 7;
            this.SelectedLabel.Text = "0/0";
            this.SelectedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tb_MySqlConn
            // 
            this.tb_MySqlConn.Location = new System.Drawing.Point(111, 38);
            this.tb_MySqlConn.Name = "tb_MySqlConn";
            this.tb_MySqlConn.Size = new System.Drawing.Size(885, 21);
            this.tb_MySqlConn.TabIndex = 8;
            // 
            // TablesListbox
            // 
            this.TablesListbox.FormattingEnabled = true;
            this.TablesListbox.Location = new System.Drawing.Point(5, 44);
            this.TablesListbox.Name = "TablesListbox";
            this.TablesListbox.Size = new System.Drawing.Size(305, 580);
            this.TablesListbox.TabIndex = 9;
            this.TablesListbox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.TablesListbox_ItemCheck);
            // 
            // SelectAllTables
            // 
            this.SelectAllTables.AutoSize = true;
            this.SelectAllTables.Enabled = false;
            this.SelectAllTables.Location = new System.Drawing.Point(8, 19);
            this.SelectAllTables.Name = "SelectAllTables";
            this.SelectAllTables.Size = new System.Drawing.Size(48, 16);
            this.SelectAllTables.TabIndex = 10;
            this.SelectAllTables.Text = "全选";
            this.SelectAllTables.UseVisualStyleBackColor = true;
            this.SelectAllTables.CheckedChanged += new System.EventHandler(this.SelectAllTables_CheckedChanged);
            // 
            // bt_GenerateScripts
            // 
            this.bt_GenerateScripts.Enabled = false;
            this.bt_GenerateScripts.Location = new System.Drawing.Point(532, 677);
            this.bt_GenerateScripts.Name = "bt_GenerateScripts";
            this.bt_GenerateScripts.Size = new System.Drawing.Size(75, 23);
            this.bt_GenerateScripts.TabIndex = 11;
            this.bt_GenerateScripts.Text = "数据迁移";
            this.bt_GenerateScripts.UseVisualStyleBackColor = true;
            this.bt_GenerateScripts.Click += new System.EventHandler(this.GenerateScripts_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 706);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(986, 11);
            this.progressBar.TabIndex = 12;
            // 
            // ListLog
            // 
            this.ListLog.Location = new System.Drawing.Point(1002, 11);
            this.ListLog.Name = "ListLog";
            this.ListLog.Size = new System.Drawing.Size(408, 706);
            this.ListLog.TabIndex = 13;
            this.ListLog.Text = "";
            this.ListLog.LostFocus += new System.EventHandler(this.ListLogLostFocus);
            // 
            // listView1
            // 
            this.listView1.AutoArrange = false;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TableName,
            this.columnHeader1,
            this.Where});
            this.listView1.Location = new System.Drawing.Point(336, 73);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(660, 26);
            this.listView1.TabIndex = 15;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // TableName
            // 
            this.TableName.Text = "表名";
            this.TableName.Width = 260;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 49;
            // 
            // Where
            // 
            this.Where.Text = "条件表达式";
            this.Where.Width = 290;
            // 
            // flp_Controls
            // 
            this.flp_Controls.AutoScroll = true;
            this.flp_Controls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flp_Controls.Location = new System.Drawing.Point(336, 99);
            this.flp_Controls.Name = "flp_Controls";
            this.flp_Controls.Size = new System.Drawing.Size(660, 572);
            this.flp_Controls.TabIndex = 16;
            // 
            // ElapsedTime
            // 
            this.ElapsedTime.Interval = 1000;
            this.ElapsedTime.Tick += new System.EventHandler(this.ElapsedTime_Tick);
            // 
            // TimeTaken_Execute
            // 
            this.TimeTaken_Execute.AutoSize = true;
            this.TimeTaken_Execute.Location = new System.Drawing.Point(651, 682);
            this.TimeTaken_Execute.Name = "TimeTaken_Execute";
            this.TimeTaken_Execute.Size = new System.Drawing.Size(59, 12);
            this.TimeTaken_Execute.TabIndex = 17;
            this.TimeTaken_Execute.Text = "执行时间:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "Sql数据库连接:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "MySql数据库连接:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TablesListbox);
            this.groupBox1.Controls.Add(this.SelectedLabel);
            this.groupBox1.Controls.Add(this.SelectAllTables);
            this.groupBox1.Location = new System.Drawing.Point(14, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(316, 635);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "迁移列表";
            // 
            // bt_CheckData
            // 
            this.bt_CheckData.Location = new System.Drawing.Point(850, 677);
            this.bt_CheckData.Name = "bt_CheckData";
            this.bt_CheckData.Size = new System.Drawing.Size(75, 23);
            this.bt_CheckData.TabIndex = 21;
            this.bt_CheckData.Text = "数据验证";
            this.bt_CheckData.UseVisualStyleBackColor = true;
            this.bt_CheckData.Click += new System.EventHandler(this.bt_CheckData_Click);
            // 
            // bt_GetList
            // 
            this.bt_GetList.Location = new System.Drawing.Point(336, 677);
            this.bt_GetList.Name = "bt_GetList";
            this.bt_GetList.Size = new System.Drawing.Size(109, 23);
            this.bt_GetList.TabIndex = 22;
            this.bt_GetList.Text = "初始化(获取列表)";
            this.bt_GetList.UseVisualStyleBackColor = true;
            this.bt_GetList.Click += new System.EventHandler(this.bt_GetList_Click);
            // 
            // lb_Progress
            // 
            this.lb_Progress.AutoSize = true;
            this.lb_Progress.Location = new System.Drawing.Point(650, 682);
            this.lb_Progress.Name = "lb_Progress";
            this.lb_Progress.Size = new System.Drawing.Size(0, 12);
            this.lb_Progress.TabIndex = 23;
            // 
            // bt_CheckTableDef
            // 
            this.bt_CheckTableDef.Enabled = false;
            this.bt_CheckTableDef.Location = new System.Drawing.Point(451, 677);
            this.bt_CheckTableDef.Name = "bt_CheckTableDef";
            this.bt_CheckTableDef.Size = new System.Drawing.Size(75, 23);
            this.bt_CheckTableDef.TabIndex = 24;
            this.bt_CheckTableDef.Text = "表结构对比";
            this.bt_CheckTableDef.UseVisualStyleBackColor = true;
            this.bt_CheckTableDef.Click += new System.EventHandler(this.bt_CheckTableDef_Click);
            // 
            // DataMigration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1420, 727);
            this.Controls.Add(this.bt_CheckTableDef);
            this.Controls.Add(this.lb_Progress);
            this.Controls.Add(this.bt_GetList);
            this.Controls.Add(this.bt_CheckData);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TimeTaken_Execute);
            this.Controls.Add(this.flp_Controls);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.ListLog);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.bt_GenerateScripts);
            this.Controls.Add(this.tb_MySqlConn);
            this.Controls.Add(this.tb_SqlConn);
            this.Name = "DataMigration";
            this.Text = "数据迁移";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_SqlConn;
        private System.Windows.Forms.Label SelectedLabel;
        private System.Windows.Forms.TextBox tb_MySqlConn;
        private System.Windows.Forms.CheckedListBox TablesListbox;
        private System.Windows.Forms.CheckBox SelectAllTables;
        private System.Windows.Forms.Button bt_GenerateScripts;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.RichTextBox ListLog;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader TableName;
        private System.Windows.Forms.ColumnHeader Where;
        private System.Windows.Forms.FlowLayoutPanel flp_Controls;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Timer ElapsedTime;
        private System.Windows.Forms.Label TimeTaken_Execute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bt_CheckData;
        private System.Windows.Forms.Button bt_GetList;
        private System.Windows.Forms.Label lb_Progress;
        private System.Windows.Forms.Button bt_CheckTableDef;
    }
}

