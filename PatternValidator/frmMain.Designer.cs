namespace PatternValidator
{
    partial class frmPatternValidator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPatternValidator));
            this.txtBoxLogEntry = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxPattern = new System.Windows.Forms.TextBox();
            this.btnValidate = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.dGVResults = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dGVResults)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBoxLogEntry
            // 
            this.txtBoxLogEntry.Location = new System.Drawing.Point(29, 33);
            this.txtBoxLogEntry.Multiline = true;
            this.txtBoxLogEntry.Name = "txtBoxLogEntry";
            this.txtBoxLogEntry.Size = new System.Drawing.Size(878, 89);
            this.txtBoxLogEntry.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Single Log Entry:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Parsing pattern string:";
            // 
            // txtBoxPattern
            // 
            this.txtBoxPattern.Location = new System.Drawing.Point(29, 174);
            this.txtBoxPattern.Multiline = true;
            this.txtBoxPattern.Name = "txtBoxPattern";
            this.txtBoxPattern.Size = new System.Drawing.Size(878, 89);
            this.txtBoxPattern.TabIndex = 2;
            // 
            // btnValidate
            // 
            this.btnValidate.Location = new System.Drawing.Point(29, 274);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(84, 66);
            this.btnValidate.TabIndex = 4;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(128, 302);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(0, 13);
            this.lblResult.TabIndex = 7;
            // 
            // dGVResults
            // 
            this.dGVResults.AllowUserToAddRows = false;
            this.dGVResults.AllowUserToDeleteRows = false;
            this.dGVResults.AllowUserToResizeRows = false;
            this.dGVResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dGVResults.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dGVResults.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dGVResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGVResults.ColumnHeadersVisible = false;
            this.dGVResults.Enabled = false;
            this.dGVResults.Location = new System.Drawing.Point(29, 354);
            this.dGVResults.MultiSelect = false;
            this.dGVResults.Name = "dGVResults";
            this.dGVResults.ReadOnly = true;
            this.dGVResults.RowHeadersVisible = false;
            this.dGVResults.Size = new System.Drawing.Size(878, 137);
            this.dGVResults.TabIndex = 5;
            // 
            // frmPatternValidator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 506);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.dGVResults);
            this.Controls.Add(this.btnValidate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBoxPattern);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBoxLogEntry);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPatternValidator";
            this.Text = "Pattern Validator";
            ((System.ComponentModel.ISupportInitialize)(this.dGVResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBoxLogEntry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxPattern;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.DataGridView dGVResults;
    }
}

