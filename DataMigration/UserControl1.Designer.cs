namespace DataMigration
{
    partial class UserControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_TableName = new System.Windows.Forms.Label();
            this.tb_Where = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lb_TableName
            // 
            this.lb_TableName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_TableName.Location = new System.Drawing.Point(3, 3);
            this.lb_TableName.Name = "lb_TableName";
            this.lb_TableName.Size = new System.Drawing.Size(243, 23);
            this.lb_TableName.TabIndex = 0;
            this.lb_TableName.Text = "User_BattleGrowth_TeamRequests";
            this.lb_TableName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tb_Where
            // 
            this.tb_Where.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.tb_Where.Location = new System.Drawing.Point(301, 3);
            this.tb_Where.Name = "tb_Where";
            this.tb_Where.Size = new System.Drawing.Size(332, 23);
            this.tb_Where.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label1.Location = new System.Drawing.Point(254, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "where";
            // 
            // UserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_Where);
            this.Controls.Add(this.lb_TableName);
            this.Name = "UserControl";
            this.Size = new System.Drawing.Size(635, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_TableName;
        private System.Windows.Forms.TextBox tb_Where;
        private System.Windows.Forms.Label label1;
    }
}
