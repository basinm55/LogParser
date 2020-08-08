namespace LogParserApp
{
    partial class FrmFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFilter));
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lsbValues = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.cmbProps = new System.Windows.Forms.ComboBox();
            this.dgvFilter = new System.Windows.Forms.DataGridView();
            this.gbOperator = new System.Windows.Forms.GroupBox();
            this.rbNotEqual = new System.Windows.Forms.RadioButton();
            this.rbEqual = new System.Windows.Forms.RadioButton();
            this.rbOr = new System.Windows.Forms.RadioButton();
            this.rbAnd = new System.Windows.Forms.RadioButton();
            this.gbConnect = new System.Windows.Forms.GroupBox();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgLoadFilter = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveFilter = new System.Windows.Forms.SaveFileDialog();
            this.btnApply = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilter)).BeginInit();
            this.gbOperator.SuspendLayout();
            this.gbConnect.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(347, 445);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(75, 23);
            this.btnClearAll.TabIndex = 1;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(260, 445);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lsbValues
            // 
            this.lsbValues.FormattingEnabled = true;
            this.lsbValues.Location = new System.Drawing.Point(12, 111);
            this.lsbValues.Name = "lsbValues";
            this.lsbValues.Size = new System.Drawing.Size(148, 355);
            this.lsbValues.TabIndex = 3;
            this.lsbValues.SelectedIndexChanged += new System.EventHandler(this.lsbValues_SelectedIndexChanged);
            this.lsbValues.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lsbValues_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Value:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Property:";
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAdd.Location = new System.Drawing.Point(167, 212);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(58, 42);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = ">>";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.Location = new System.Drawing.Point(167, 270);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(58, 42);
            this.btnRemove.TabIndex = 8;
            this.btnRemove.Text = "<<";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // cmbProps
            // 
            this.cmbProps.FormattingEnabled = true;
            this.cmbProps.Location = new System.Drawing.Point(12, 53);
            this.cmbProps.Name = "cmbProps";
            this.cmbProps.Size = new System.Drawing.Size(148, 21);
            this.cmbProps.TabIndex = 9;
            this.cmbProps.SelectedIndexChanged += new System.EventHandler(this.cmbProps_SelectedIndexChanged);
            // 
            // dgvFilter
            // 
            this.dgvFilter.AllowUserToAddRows = false;
            this.dgvFilter.AllowUserToDeleteRows = false;
            this.dgvFilter.AllowUserToResizeRows = false;
            this.dgvFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFilter.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFilter.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvFilter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFilter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFilter.ColumnHeadersVisible = false;
            this.dgvFilter.Location = new System.Drawing.Point(259, 111);
            this.dgvFilter.MultiSelect = false;
            this.dgvFilter.Name = "dgvFilter";
            this.dgvFilter.ReadOnly = true;
            this.dgvFilter.RowHeadersVisible = false;
            this.dgvFilter.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFilter.Size = new System.Drawing.Size(238, 316);
            this.dgvFilter.TabIndex = 21;
            this.dgvFilter.SelectionChanged += new System.EventHandler(this.dgvFilter_SelectionChanged);
            // 
            // gbOperator
            // 
            this.gbOperator.Controls.Add(this.rbNotEqual);
            this.gbOperator.Controls.Add(this.rbEqual);
            this.gbOperator.Enabled = false;
            this.gbOperator.Location = new System.Drawing.Point(167, 107);
            this.gbOperator.Name = "gbOperator";
            this.gbOperator.Size = new System.Drawing.Size(86, 70);
            this.gbOperator.TabIndex = 22;
            this.gbOperator.TabStop = false;
            // 
            // rbNotEqual
            // 
            this.rbNotEqual.AutoSize = true;
            this.rbNotEqual.Location = new System.Drawing.Point(8, 41);
            this.rbNotEqual.Name = "rbNotEqual";
            this.rbNotEqual.Size = new System.Drawing.Size(72, 17);
            this.rbNotEqual.TabIndex = 3;
            this.rbNotEqual.Text = "Not Equal";
            this.rbNotEqual.UseVisualStyleBackColor = true;
            // 
            // rbEqual
            // 
            this.rbEqual.AutoSize = true;
            this.rbEqual.Checked = true;
            this.rbEqual.Location = new System.Drawing.Point(8, 17);
            this.rbEqual.Name = "rbEqual";
            this.rbEqual.Size = new System.Drawing.Size(52, 17);
            this.rbEqual.TabIndex = 2;
            this.rbEqual.TabStop = true;
            this.rbEqual.Text = "Equal";
            this.rbEqual.UseVisualStyleBackColor = true;
            // 
            // rbOr
            // 
            this.rbOr.AutoSize = true;
            this.rbOr.Location = new System.Drawing.Point(54, 13);
            this.rbOr.Name = "rbOr";
            this.rbOr.Size = new System.Drawing.Size(36, 17);
            this.rbOr.TabIndex = 3;
            this.rbOr.Text = "Or";
            this.rbOr.UseVisualStyleBackColor = true;
            // 
            // rbAnd
            // 
            this.rbAnd.AutoSize = true;
            this.rbAnd.Checked = true;
            this.rbAnd.Location = new System.Drawing.Point(6, 13);
            this.rbAnd.Name = "rbAnd";
            this.rbAnd.Size = new System.Drawing.Size(44, 17);
            this.rbAnd.TabIndex = 2;
            this.rbAnd.TabStop = true;
            this.rbAnd.Text = "And";
            this.rbAnd.UseVisualStyleBackColor = true;
            // 
            // gbConnect
            // 
            this.gbConnect.Controls.Add(this.rbOr);
            this.gbConnect.Controls.Add(this.rbAnd);
            this.gbConnect.Enabled = false;
            this.gbConnect.Location = new System.Drawing.Point(169, 43);
            this.gbConnect.Name = "gbConnect";
            this.gbConnect.Size = new System.Drawing.Size(96, 34);
            this.gbConnect.TabIndex = 23;
            this.gbConnect.TabStop = false;
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.cancelToolStripMenuItem});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(514, 24);
            this.mnuMain.TabIndex = 24;
            this.mnuMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFilterToolStripMenuItem,
            this.saveFilterToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadFilterToolStripMenuItem
            // 
            this.loadFilterToolStripMenuItem.Name = "loadFilterToolStripMenuItem";
            this.loadFilterToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.loadFilterToolStripMenuItem.Text = "Load filter...";
            this.loadFilterToolStripMenuItem.Click += new System.EventHandler(this.loadFilterToolStripMenuItem_Click);
            // 
            // saveFilterToolStripMenuItem
            // 
            this.saveFilterToolStripMenuItem.Enabled = false;
            this.saveFilterToolStripMenuItem.Name = "saveFilterToolStripMenuItem";
            this.saveFilterToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.saveFilterToolStripMenuItem.Text = "Save filter...";
            this.saveFilterToolStripMenuItem.Click += new System.EventHandler(this.saveFilterToolStripMenuItem_Click);
            // 
            // cancelToolStripMenuItem
            // 
            this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            this.cancelToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.cancelToolStripMenuItem.Text = "Cancel";
            this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(430, 445);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // FrmFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 475);
            this.Controls.Add(this.gbConnect);
            this.Controls.Add(this.gbOperator);
            this.Controls.Add(this.dgvFilter);
            this.Controls.Add(this.cmbProps);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lsbValues);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.mnuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Custom Filter";
            this.Load += new System.EventHandler(this.FrmFilter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilter)).EndInit();
            this.gbOperator.ResumeLayout(false);
            this.gbOperator.PerformLayout();
            this.gbConnect.ResumeLayout(false);
            this.gbConnect.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lsbValues;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ComboBox cmbProps;
        private System.Windows.Forms.DataGridView dgvFilter;
        private System.Windows.Forms.GroupBox gbOperator;
        private System.Windows.Forms.RadioButton rbNotEqual;
        private System.Windows.Forms.RadioButton rbEqual;
        private System.Windows.Forms.RadioButton rbOr;
        private System.Windows.Forms.RadioButton rbAnd;
        private System.Windows.Forms.GroupBox gbConnect;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFilterToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog dlgLoadFilter;
        private System.Windows.Forms.SaveFileDialog dlgSaveFilter;
        private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
    }
}