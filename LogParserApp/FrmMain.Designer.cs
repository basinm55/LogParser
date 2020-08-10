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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.dlgLoadLog = new System.Windows.Forms.OpenFileDialog();
            this.gridCmStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dataBufferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.btnStopLoading = new System.Windows.Forms.ToolStripSplitButton();
            this.resultLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.calculateLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.gridLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.bkgWorkerLoad = new System.ComponentModel.BackgroundWorker();
            this.lblShowDevice = new System.Windows.Forms.Label();
            this.cmbShowDevice = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtHeader = new System.Windows.Forms.TextBox();
            this.gbFilter = new System.Windows.Forms.GroupBox();
            this.btnCustomFilter = new System.Windows.Forms.Button();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.mnuStripMain = new System.Windows.Forms.MenuStrip();
            this.mnuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemImportProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemEditCurrentProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemPatternValidator = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgProfile = new System.Windows.Forms.OpenFileDialog();
            this.dgvInfo = new System.Windows.Forms.DataGridView();
            this.btnViewAppLog = new System.Windows.Forms.Button();
            this.btnViewLoadedLog = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.gridCmStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.gbFilter.SuspendLayout();
            this.mnuStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).BeginInit();
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
            this.dataGV.Location = new System.Drawing.Point(3, 142);
            this.dataGV.MultiSelect = false;
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGV.RowHeadersVisible = false;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.dataGV.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGV.RowTemplate.Height = 66;
            this.dataGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGV.Size = new System.Drawing.Size(1062, 587);
            this.dataGV.TabIndex = 0;
            this.dataGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGV_CellClick);
            this.dataGV.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGV_CellFormatting);
            this.dataGV.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGV_CellMouseLeave);
            this.dataGV.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGV_CellMouseMove);
            this.dataGV.CellStateChanged += new System.Windows.Forms.DataGridViewCellStateChangedEventHandler(this.dataGV_CellStateChanged);
            this.dataGV.SelectionChanged += new System.EventHandler(this.dataGV_SelectionChanged);
            // 
            // gridCmStrip
            // 
            this.gridCmStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataBufferToolStripMenuItem});
            this.gridCmStrip.Name = "gridCmStrip";
            this.gridCmStrip.Size = new System.Drawing.Size(134, 26);
            // 
            // dataBufferToolStripMenuItem
            // 
            this.dataBufferToolStripMenuItem.Name = "dataBufferToolStripMenuItem";
            this.dataBufferToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.dataBufferToolStripMenuItem.Text = "Data Buffer";
            this.dataBufferToolStripMenuItem.Click += new System.EventHandler(this.dataBufferToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.AllowItemReorder = true;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.btnStopLoading,
            this.resultLabel,
            this.calculateLabel,
            this.gridLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 732);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1359, 22);
            this.statusStrip.TabIndex = 4;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(300, 16);
            // 
            // btnStopLoading
            // 
            this.btnStopLoading.DropDownButtonWidth = 1;
            this.btnStopLoading.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnStopLoading.Image = ((System.Drawing.Image)(resources.GetObject("btnStopLoading.Image")));
            this.btnStopLoading.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStopLoading.Name = "btnStopLoading";
            this.btnStopLoading.Size = new System.Drawing.Size(107, 20);
            this.btnStopLoading.Text = "Stop loading...";
            this.btnStopLoading.ButtonClick += new System.EventHandler(this.btnStopLoading_ButtonClick);
            // 
            // resultLabel
            // 
            this.resultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // calculateLabel
            // 
            this.calculateLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.calculateLabel.Name = "calculateLabel";
            this.calculateLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // gridLabel
            // 
            this.gridLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.gridLabel.Name = "gridLabel";
            this.gridLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // bkgWorkerLoad
            // 
            this.bkgWorkerLoad.WorkerReportsProgress = true;
            this.bkgWorkerLoad.WorkerSupportsCancellation = true;
            this.bkgWorkerLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bkgWorkerLoad_DoWork);
            this.bkgWorkerLoad.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bkgWorkerLoad_ProgressChanged);
            this.bkgWorkerLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bkgWorkerLoad_RunWorkerCompleted);
            // 
            // lblShowDevice
            // 
            this.lblShowDevice.AutoSize = true;
            this.lblShowDevice.Location = new System.Drawing.Point(18, 61);
            this.lblShowDevice.Name = "lblShowDevice";
            this.lblShowDevice.Size = new System.Drawing.Size(44, 13);
            this.lblShowDevice.TabIndex = 8;
            this.lblShowDevice.Text = "Device:";
            // 
            // cmbShowDevice
            // 
            this.cmbShowDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShowDevice.FormattingEnabled = true;
            this.cmbShowDevice.Location = new System.Drawing.Point(64, 58);
            this.cmbShowDevice.Name = "cmbShowDevice";
            this.cmbShowDevice.Size = new System.Drawing.Size(148, 21);
            this.cmbShowDevice.TabIndex = 7;
            this.cmbShowDevice.SelectedIndexChanged += new System.EventHandler(this.cmbShowDevice_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 99);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1053, 36);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selected device details:";
            // 
            // txtHeader
            // 
            this.txtHeader.BackColor = System.Drawing.SystemColors.Control;
            this.txtHeader.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHeader.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHeader.ForeColor = System.Drawing.SystemColors.Window;
            this.txtHeader.Location = new System.Drawing.Point(20, 120);
            this.txtHeader.Name = "txtHeader";
            this.txtHeader.ReadOnly = true;
            this.txtHeader.Size = new System.Drawing.Size(1039, 13);
            this.txtHeader.TabIndex = 7;
            this.txtHeader.Click += new System.EventHandler(this.txtHeader_Click);
            this.txtHeader.TextChanged += new System.EventHandler(this.txtHeader_TextChanged);
            this.txtHeader.Leave += new System.EventHandler(this.txtHeader_Leave);
            this.txtHeader.MouseLeave += new System.EventHandler(this.txtHeader_MouseLeave);
            this.txtHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtHeader_MouseMove);
            // 
            // gbFilter
            // 
            this.gbFilter.Controls.Add(this.btnCustomFilter);
            this.gbFilter.Controls.Add(this.btnClearFilter);
            this.gbFilter.Location = new System.Drawing.Point(761, 27);
            this.gbFilter.Name = "gbFilter";
            this.gbFilter.Size = new System.Drawing.Size(304, 74);
            this.gbFilter.TabIndex = 12;
            this.gbFilter.TabStop = false;
            this.gbFilter.Text = "Filter";
            // 
            // btnCustomFilter
            // 
            this.btnCustomFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCustomFilter.Image = global::LogParserApp.Properties.Resources.filter;
            this.btnCustomFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomFilter.Location = new System.Drawing.Point(30, 25);
            this.btnCustomFilter.Name = "btnCustomFilter";
            this.btnCustomFilter.Size = new System.Drawing.Size(121, 30);
            this.btnCustomFilter.TabIndex = 15;
            this.btnCustomFilter.Text = "Custom filter";
            this.btnCustomFilter.UseVisualStyleBackColor = true;
            this.btnCustomFilter.Click += new System.EventHandler(this.btnCustomFilter_Click);
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearFilter.Enabled = false;
            this.btnClearFilter.Image = global::LogParserApp.Properties.Resources.clearfilter;
            this.btnClearFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClearFilter.Location = new System.Drawing.Point(169, 25);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(121, 30);
            this.btnClearFilter.TabIndex = 14;
            this.btnClearFilter.Text = "Clear filter";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            // 
            // mnuStripMain
            // 
            this.mnuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemFile,
            this.mnuItemTools});
            this.mnuStripMain.Location = new System.Drawing.Point(0, 0);
            this.mnuStripMain.Name = "mnuStripMain";
            this.mnuStripMain.Size = new System.Drawing.Size(1359, 24);
            this.mnuStripMain.TabIndex = 16;
            this.mnuStripMain.Text = "menuStrip1";
            // 
            // mnuItemFile
            // 
            this.mnuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemLoad,
            this.mnuItemExit});
            this.mnuItemFile.Name = "mnuItemFile";
            this.mnuItemFile.Size = new System.Drawing.Size(37, 20);
            this.mnuItemFile.Text = "File";
            // 
            // mnuItemLoad
            // 
            this.mnuItemLoad.Image = global::LogParserApp.Properties.Resources.log;
            this.mnuItemLoad.Name = "mnuItemLoad";
            this.mnuItemLoad.Size = new System.Drawing.Size(147, 22);
            this.mnuItemLoad.Text = "Load LOG File";
            this.mnuItemLoad.Click += new System.EventHandler(this.mnuItemLoad_Click);
            // 
            // mnuItemExit
            // 
            this.mnuItemExit.Name = "mnuItemExit";
            this.mnuItemExit.Size = new System.Drawing.Size(147, 22);
            this.mnuItemExit.Text = "Exit";
            this.mnuItemExit.Click += new System.EventHandler(this.mnuItemExit_Click);
            // 
            // mnuItemTools
            // 
            this.mnuItemTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemImportProfile,
            this.mnuItemEditCurrentProfile,
            this.mnuItemPatternValidator});
            this.mnuItemTools.Name = "mnuItemTools";
            this.mnuItemTools.Size = new System.Drawing.Size(46, 20);
            this.mnuItemTools.Text = "Tools";
            // 
            // mnuItemImportProfile
            // 
            this.mnuItemImportProfile.Name = "mnuItemImportProfile";
            this.mnuItemImportProfile.Size = new System.Drawing.Size(192, 22);
            this.mnuItemImportProfile.Text = "Import profile...";
            this.mnuItemImportProfile.Click += new System.EventHandler(this.mnuItemImportProfile_Click);
            // 
            // mnuItemEditCurrentProfile
            // 
            this.mnuItemEditCurrentProfile.Name = "mnuItemEditCurrentProfile";
            this.mnuItemEditCurrentProfile.Size = new System.Drawing.Size(192, 22);
            this.mnuItemEditCurrentProfile.Text = "Edit current profile";
            this.mnuItemEditCurrentProfile.Click += new System.EventHandler(this.mnuItemEditCurrentProfile_Click);
            // 
            // mnuItemPatternValidator
            // 
            this.mnuItemPatternValidator.Name = "mnuItemPatternValidator";
            this.mnuItemPatternValidator.Size = new System.Drawing.Size(192, 22);
            this.mnuItemPatternValidator.Text = "Pattern Validation Tool";
            this.mnuItemPatternValidator.Click += new System.EventHandler(this.mnuItemPatternValidator_Click);
            // 
            // dgvInfo
            // 
            this.dgvInfo.AllowUserToAddRows = false;
            this.dgvInfo.AllowUserToDeleteRows = false;
            this.dgvInfo.AllowUserToResizeRows = false;
            this.dgvInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvInfo.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvInfo.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInfo.ColumnHeadersVisible = false;
            this.dgvInfo.Enabled = false;
            this.dgvInfo.Location = new System.Drawing.Point(1071, 141);
            this.dgvInfo.MultiSelect = false;
            this.dgvInfo.Name = "dgvInfo";
            this.dgvInfo.RowHeadersVisible = false;
            this.dgvInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvInfo.Size = new System.Drawing.Size(280, 588);
            this.dgvInfo.TabIndex = 20;
            // 
            // btnViewAppLog
            // 
            this.btnViewAppLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewAppLog.Image = global::LogParserApp.Properties.Resources.applog;
            this.btnViewAppLog.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnViewAppLog.Location = new System.Drawing.Point(1189, 71);
            this.btnViewAppLog.Name = "btnViewAppLog";
            this.btnViewAppLog.Size = new System.Drawing.Size(153, 30);
            this.btnViewAppLog.TabIndex = 17;
            this.btnViewAppLog.Text = "Application  log";
            this.btnViewAppLog.UseVisualStyleBackColor = true;
            this.btnViewAppLog.Click += new System.EventHandler(this.btnViewAppLog_Click);
            // 
            // btnViewLoadedLog
            // 
            this.btnViewLoadedLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewLoadedLog.Image = global::LogParserApp.Properties.Resources.log;
            this.btnViewLoadedLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnViewLoadedLog.Location = new System.Drawing.Point(1189, 30);
            this.btnViewLoadedLog.Name = "btnViewLoadedLog";
            this.btnViewLoadedLog.Size = new System.Drawing.Size(153, 30);
            this.btnViewLoadedLog.TabIndex = 15;
            this.btnViewLoadedLog.Text = "View loaded log";
            this.btnViewLoadedLog.UseVisualStyleBackColor = true;
            this.btnViewLoadedLog.Click += new System.EventHandler(this.btnViewLoadedLog_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1359, 754);
            this.Controls.Add(this.txtHeader);
            this.Controls.Add(this.dgvInfo);
            this.Controls.Add(this.btnViewAppLog);
            this.Controls.Add(this.btnViewLoadedLog);
            this.Controls.Add(this.gbFilter);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblShowDevice);
            this.Controls.Add(this.cmbShowDevice);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mnuStripMain);
            this.Controls.Add(this.dataGV);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuStripMain;
            this.Name = "FrmMain";
            this.Text = "LogParser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.gridCmStrip.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.gbFilter.ResumeLayout(false);
            this.mnuStripMain.ResumeLayout(false);
            this.mnuStripMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.OpenFileDialog dlgLoadLog;
        private System.Windows.Forms.ContextMenuStrip gridCmStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel resultLabel;
        public System.ComponentModel.BackgroundWorker bkgWorkerLoad;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel calculateLabel;        
        private System.Windows.Forms.Label lblShowDevice;
        private System.Windows.Forms.ComboBox cmbShowDevice;
        private System.Windows.Forms.ToolStripSplitButton btnStopLoading;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Button btnViewLoadedLog;
        private System.Windows.Forms.ToolStripStatusLabel gridLabel;
        private System.Windows.Forms.MenuStrip mnuStripMain;
        private System.Windows.Forms.ToolStripMenuItem mnuItemFile;
        private System.Windows.Forms.ToolStripMenuItem mnuItemLoad;
        private System.Windows.Forms.ToolStripMenuItem mnuItemExit;
        private System.Windows.Forms.ToolStripMenuItem mnuItemTools;
        private System.Windows.Forms.ToolStripMenuItem mnuItemPatternValidator;
        private System.Windows.Forms.ToolStripMenuItem dataBufferToolStripMenuItem;
        private System.Windows.Forms.Button btnViewAppLog;
        private System.Windows.Forms.OpenFileDialog dlgProfile;
        private System.Windows.Forms.ToolStripMenuItem mnuItemImportProfile;
        private System.Windows.Forms.DataGridView dgvInfo;
        private System.Windows.Forms.TextBox txtHeader;
        private System.Windows.Forms.Button btnCustomFilter;
        private System.Windows.Forms.ToolStripMenuItem mnuItemEditCurrentProfile;
    }
}

