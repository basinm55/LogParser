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
            this.btnOk = new System.Windows.Forms.Button();
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
            this.rbOr = new System.Windows.Forms.RadioButton();
            this.rbAnd = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilter)).BeginInit();
            this.gbOperator.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(422, 388);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(332, 388);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(75, 23);
            this.btnClearAll.TabIndex = 1;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(236, 388);
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
            this.lsbValues.Location = new System.Drawing.Point(12, 89);
            this.lsbValues.Name = "lsbValues";
            this.lsbValues.Size = new System.Drawing.Size(148, 316);
            this.lsbValues.TabIndex = 3;
            this.lsbValues.SelectedIndexChanged += new System.EventHandler(this.lsbValues_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Value:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Property:";
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(167, 186);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(58, 42);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = ">";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(167, 244);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(58, 42);
            this.btnRemove.TabIndex = 8;
            this.btnRemove.Text = "<";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // cmbProps
            // 
            this.cmbProps.FormattingEnabled = true;
            this.cmbProps.Location = new System.Drawing.Point(12, 31);
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
            this.dgvFilter.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvFilter.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvFilter.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvFilter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFilter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFilter.ColumnHeadersVisible = false;
            this.dgvFilter.Enabled = false;
            this.dgvFilter.Location = new System.Drawing.Point(231, 89);
            this.dgvFilter.MultiSelect = false;
            this.dgvFilter.Name = "dgvFilter";
            this.dgvFilter.RowHeadersVisible = false;
            this.dgvFilter.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvFilter.Size = new System.Drawing.Size(266, 278);
            this.dgvFilter.TabIndex = 21;
            // 
            // gbOperator
            // 
            this.gbOperator.Controls.Add(this.rbOr);
            this.gbOperator.Controls.Add(this.rbAnd);
            this.gbOperator.Enabled = false;
            this.gbOperator.Location = new System.Drawing.Point(167, 85);
            this.gbOperator.Name = "gbOperator";
            this.gbOperator.Size = new System.Drawing.Size(58, 69);
            this.gbOperator.TabIndex = 22;
            this.gbOperator.TabStop = false;
            // 
            // rbOr
            // 
            this.rbOr.AutoSize = true;
            this.rbOr.Location = new System.Drawing.Point(7, 40);
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
            this.rbAnd.Location = new System.Drawing.Point(7, 16);
            this.rbAnd.Name = "rbAnd";
            this.rbAnd.Size = new System.Drawing.Size(44, 17);
            this.rbAnd.TabIndex = 2;
            this.rbAnd.TabStop = true;
            this.rbAnd.Text = "And";
            this.rbAnd.UseVisualStyleBackColor = true;
            // 
            // FrmFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 420);
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
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Custom Filter";
            this.Load += new System.EventHandler(this.FrmFilter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilter)).EndInit();
            this.gbOperator.ResumeLayout(false);
            this.gbOperator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
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
        private System.Windows.Forms.RadioButton rbOr;
        private System.Windows.Forms.RadioButton rbAnd;
    }
}