namespace LogParserApp
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.btnLoadLog = new System.Windows.Forms.Button();
            this.dlgLoadLog = new System.Windows.Forms.OpenFileDialog();
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.rbSession = new System.Windows.Forms.RadioButton();
            this.rbPort = new System.Windows.Forms.RadioButton();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.cmbProfile = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gridCmStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showRelatedLogEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.gbSearch.SuspendLayout();
            this.gridCmStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.AllowUserToResizeColumns = false;
            this.dataGV.AllowUserToResizeRows = false;
            this.dataGV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGV.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGV.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.ColumnHeadersVisible = false;
            this.dataGV.GridColor = System.Drawing.SystemColors.Window;
            this.dataGV.Location = new System.Drawing.Point(3, 93);
            this.dataGV.MultiSelect = false;
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGV.RowHeadersVisible = false;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.dataGV.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGV.RowTemplate.Height = 66;
            this.dataGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGV.Size = new System.Drawing.Size(907, 374);
            this.dataGV.TabIndex = 0;
            this.dataGV.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGV_CellFormatting);
            this.dataGV.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGV_CellMouseDown);
            this.dataGV.CellStateChanged += new System.Windows.Forms.DataGridViewCellStateChangedEventHandler(this.dataGV_CellStateChanged);
            // 
            // btnLoadLog
            // 
            this.btnLoadLog.Location = new System.Drawing.Point(12, 32);
            this.btnLoadLog.Name = "btnLoadLog";
            this.btnLoadLog.Size = new System.Drawing.Size(131, 44);
            this.btnLoadLog.TabIndex = 1;
            this.btnLoadLog.Text = "Load LOG File";
            this.btnLoadLog.UseVisualStyleBackColor = true;
            this.btnLoadLog.Click += new System.EventHandler(this.btnLoadLog_Click);
            // 
            // gbSearch
            // 
            this.gbSearch.Controls.Add(this.btnSearch);
            this.gbSearch.Controls.Add(this.rbSession);
            this.gbSearch.Controls.Add(this.rbPort);
            this.gbSearch.Controls.Add(this.txtSearch);
            this.gbSearch.Location = new System.Drawing.Point(328, 12);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(338, 75);
            this.gbSearch.TabIndex = 3;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Search";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(252, 15);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // rbSession
            // 
            this.rbSession.AutoSize = true;
            this.rbSession.Location = new System.Drawing.Point(82, 19);
            this.rbSession.Name = "rbSession";
            this.rbSession.Size = new System.Drawing.Size(79, 17);
            this.rbSession.TabIndex = 7;
            this.rbSession.TabStop = true;
            this.rbSession.Text = "by Session:";
            this.rbSession.UseVisualStyleBackColor = true;
            // 
            // rbPort
            // 
            this.rbPort.AutoSize = true;
            this.rbPort.Location = new System.Drawing.Point(15, 20);
            this.rbPort.Name = "rbPort";
            this.rbPort.Size = new System.Drawing.Size(61, 17);
            this.rbPort.TabIndex = 6;
            this.rbPort.TabStop = true;
            this.rbPort.Text = "by Port:";
            this.rbPort.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(15, 43);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(312, 20);
            this.txtSearch.TabIndex = 5;
            // 
            // cmbProfile
            // 
            this.cmbProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProfile.FormattingEnabled = true;
            this.cmbProfile.Location = new System.Drawing.Point(172, 55);
            this.cmbProfile.Name = "cmbProfile";
            this.cmbProfile.Size = new System.Drawing.Size(121, 21);
            this.cmbProfile.TabIndex = 4;
            this.cmbProfile.SelectedIndexChanged += new System.EventHandler(this.cmbProfile_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(172, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select Profile:";
            // 
            // gridCmStrip
            // 
            this.gridCmStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showRelatedLogEntryToolStripMenuItem});
            this.gridCmStrip.Name = "gridCmStrip";
            this.gridCmStrip.Size = new System.Drawing.Size(196, 26);
            // 
            // showRelatedLogEntryToolStripMenuItem
            // 
            this.showRelatedLogEntryToolStripMenuItem.Name = "showRelatedLogEntryToolStripMenuItem";
            this.showRelatedLogEntryToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.showRelatedLogEntryToolStripMenuItem.Text = "Show related Log Entry";
            this.showRelatedLogEntryToolStripMenuItem.Click += new System.EventHandler(this.showRelatedLogEntryToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 466);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbProfile);
            this.Controls.Add(this.gbSearch);
            this.Controls.Add(this.btnLoadLog);
            this.Controls.Add(this.dataGV);
            this.Name = "FrmMain";
            this.Text = "LogParser";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            this.gridCmStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Button btnLoadLog;
        private System.Windows.Forms.OpenFileDialog dlgLoadLog;
        private System.Windows.Forms.GroupBox gbSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.RadioButton rbSession;
        private System.Windows.Forms.RadioButton rbPort;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbProfile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip gridCmStrip;
        private System.Windows.Forms.ToolStripMenuItem showRelatedLogEntryToolStripMenuItem;
    }
}

