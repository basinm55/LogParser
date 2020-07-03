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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.dlgLoadLog = new System.Windows.Forms.OpenFileDialog();
            this.gridCmStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showRelatedLogEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.btnStopLoading = new System.Windows.Forms.ToolStripSplitButton();
            this.resultLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.calculateLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.gridLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.bkgWorkerLoad = new System.ComponentModel.BackgroundWorker();
            this.lblHeader = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbShowDevice = new System.Windows.Forms.ComboBox();
            this.chkShowAll = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gbFilter = new System.Windows.Forms.GroupBox();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbState = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbThis = new System.Windows.Forms.ComboBox();
            this.btnViewLog = new System.Windows.Forms.Button();
            this.mnuStripMain = new System.Windows.Forms.MenuStrip();
            this.mnuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemPatternValidator = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.gridCmStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbFilter.SuspendLayout();
            this.mnuStripMain.SuspendLayout();
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
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGV.RowHeadersVisible = false;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.dataGV.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGV.RowTemplate.Height = 66;
            this.dataGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGV.Size = new System.Drawing.Size(1128, 512);
            this.dataGV.TabIndex = 0;
            this.dataGV.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGV_CellFormatting);
            this.dataGV.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGV_CellMouseDown);
            this.dataGV.CellStateChanged += new System.Windows.Forms.DataGridViewCellStateChangedEventHandler(this.dataGV_CellStateChanged);
            // 
            // gridCmStrip
            // 
            this.gridCmStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showRelatedLogEntryToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.gridCmStrip.Name = "gridCmStrip";
            this.gridCmStrip.Size = new System.Drawing.Size(196, 48);
            // 
            // showRelatedLogEntryToolStripMenuItem
            // 
            this.showRelatedLogEntryToolStripMenuItem.Name = "showRelatedLogEntryToolStripMenuItem";
            this.showRelatedLogEntryToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.showRelatedLogEntryToolStripMenuItem.Text = "Show related Log Entry";
            this.showRelatedLogEntryToolStripMenuItem.Click += new System.EventHandler(this.showRelatedLogEntryToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.propertiesToolStripMenuItem.Text = "Properties...";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.showPropertiesToolStripMenuItem_Click);
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
            this.statusStrip.Location = new System.Drawing.Point(0, 657);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1131, 22);
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
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.BackColor = System.Drawing.Color.Crimson;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblHeader.Location = new System.Drawing.Point(27, 16);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(0, 15);
            this.lblHeader.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Device:";
            // 
            // cmbShowDevice
            // 
            this.cmbShowDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShowDevice.FormattingEnabled = true;
            this.cmbShowDevice.Location = new System.Drawing.Point(63, 42);
            this.cmbShowDevice.Name = "cmbShowDevice";
            this.cmbShowDevice.Size = new System.Drawing.Size(148, 21);
            this.cmbShowDevice.TabIndex = 7;
            this.cmbShowDevice.SelectedIndexChanged += new System.EventHandler(this.cmbShowDevice_SelectedIndexChanged);
            // 
            // chkShowAll
            // 
            this.chkShowAll.AutoSize = true;
            this.chkShowAll.Location = new System.Drawing.Point(20, 72);
            this.chkShowAll.Name = "chkShowAll";
            this.chkShowAll.Size = new System.Drawing.Size(106, 17);
            this.chkShowAll.TabIndex = 9;
            this.chkShowAll.Text = "Show all devices";
            this.chkShowAll.UseVisualStyleBackColor = true;
            this.chkShowAll.CheckedChanged += new System.EventHandler(this.chkShowAll_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblHeader);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 99);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1107, 36);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device details:";
            // 
            // gbFilter
            // 
            this.gbFilter.Controls.Add(this.btnClearFilter);
            this.gbFilter.Controls.Add(this.label3);
            this.gbFilter.Controls.Add(this.cmbState);
            this.gbFilter.Controls.Add(this.label2);
            this.gbFilter.Controls.Add(this.cmbThis);
            this.gbFilter.Location = new System.Drawing.Point(230, 21);
            this.gbFilter.Name = "gbFilter";
            this.gbFilter.Size = new System.Drawing.Size(758, 62);
            this.gbFilter.TabIndex = 12;
            this.gbFilter.TabStop = false;
            this.gbFilter.Text = "Filter";
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.Location = new System.Drawing.Point(671, 19);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(81, 30);
            this.btnClearFilter.TabIndex = 14;
            this.btnClearFilter.Text = "Clear filter";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(207, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "State:";
            // 
            // cmbState
            // 
            this.cmbState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbState.FormattingEnabled = true;
            this.cmbState.Location = new System.Drawing.Point(245, 23);
            this.cmbState.Name = "cmbState";
            this.cmbState.Size = new System.Drawing.Size(114, 21);
            this.cmbState.TabIndex = 11;
            this.cmbState.SelectedIndexChanged += new System.EventHandler(this.cmbState_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "This:";
            // 
            // cmbThis
            // 
            this.cmbThis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbThis.FormattingEnabled = true;
            this.cmbThis.Location = new System.Drawing.Point(42, 22);
            this.cmbThis.Name = "cmbThis";
            this.cmbThis.Size = new System.Drawing.Size(148, 21);
            this.cmbThis.TabIndex = 9;
            this.cmbThis.SelectedIndexChanged += new System.EventHandler(this.cmbThis_SelectedIndexChanged);
            // 
            // btnViewLog
            // 
            this.btnViewLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewLog.Location = new System.Drawing.Point(1041, 41);
            this.btnViewLog.Name = "btnViewLog";
            this.btnViewLog.Size = new System.Drawing.Size(78, 30);
            this.btnViewLog.TabIndex = 15;
            this.btnViewLog.Text = "View log";
            this.btnViewLog.UseVisualStyleBackColor = true;
            this.btnViewLog.Click += new System.EventHandler(this.btnViewLog_Click);
            // 
            // mnuStripMain
            // 
            this.mnuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemFile,
            this.mnuItemTools});
            this.mnuStripMain.Location = new System.Drawing.Point(0, 0);
            this.mnuStripMain.Name = "mnuStripMain";
            this.mnuStripMain.Size = new System.Drawing.Size(1131, 24);
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
            this.mnuItemPatternValidator});
            this.mnuItemTools.Name = "mnuItemTools";
            this.mnuItemTools.Size = new System.Drawing.Size(46, 20);
            this.mnuItemTools.Text = "Tools";
            // 
            // mnuItemPatternValidator
            // 
            this.mnuItemPatternValidator.Name = "mnuItemPatternValidator";
            this.mnuItemPatternValidator.Size = new System.Drawing.Size(186, 22);
            this.mnuItemPatternValidator.Text = "Pattern Validator Tool";
            this.mnuItemPatternValidator.Click += new System.EventHandler(this.mnuItemPatternValidator_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1131, 679);
            this.Controls.Add(this.btnViewLog);
            this.Controls.Add(this.gbFilter);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkShowAll);
            this.Controls.Add(this.label1);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbFilter.ResumeLayout(false);
            this.gbFilter.PerformLayout();
            this.mnuStripMain.ResumeLayout(false);
            this.mnuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.OpenFileDialog dlgLoadLog;
        private System.Windows.Forms.ContextMenuStrip gridCmStrip;
        private System.Windows.Forms.ToolStripMenuItem showRelatedLogEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel resultLabel;
        public System.ComponentModel.BackgroundWorker bkgWorkerLoad;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel calculateLabel;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbShowDevice;
        private System.Windows.Forms.CheckBox chkShowAll;
        private System.Windows.Forms.ToolStripSplitButton btnStopLoading;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbState;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbThis;
        private System.Windows.Forms.Button btnViewLog;
        private System.Windows.Forms.ToolStripStatusLabel gridLabel;
        private System.Windows.Forms.MenuStrip mnuStripMain;
        private System.Windows.Forms.ToolStripMenuItem mnuItemFile;
        private System.Windows.Forms.ToolStripMenuItem mnuItemLoad;
        private System.Windows.Forms.ToolStripMenuItem mnuItemExit;
        private System.Windows.Forms.ToolStripMenuItem mnuItemTools;
        private System.Windows.Forms.ToolStripMenuItem mnuItemPatternValidator;
    }
}

