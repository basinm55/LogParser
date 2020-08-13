namespace LogParserApp
{
    partial class FrmGoToLine
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
            this.btnOK = new System.Windows.Forms.Button();
            this.numUpDwnLineNum = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnLineNum)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(26, 38);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // numUpDwnLineNum
            // 
            this.numUpDwnLineNum.Location = new System.Drawing.Point(4, 12);
            this.numUpDwnLineNum.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numUpDwnLineNum.Name = "numUpDwnLineNum";
            this.numUpDwnLineNum.Size = new System.Drawing.Size(120, 20);
            this.numUpDwnLineNum.TabIndex = 0;
            // 
            // FrmGoToLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(128, 65);
            this.Controls.Add(this.numUpDwnLineNum);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmGoToLine";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Go to line #:";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmGoToLine_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmGoToLine_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnLineNum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.NumericUpDown numUpDwnLineNum;
    }
}