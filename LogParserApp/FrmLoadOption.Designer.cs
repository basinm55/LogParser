namespace LogParserApp
{
    partial class FrmLoadOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLoadOption));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbLoadOriginal = new System.Windows.Forms.RadioButton();
            this.rbLoadFromCache = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbLoadOriginal);
            this.groupBox1.Controls.Add(this.rbLoadFromCache);
            this.groupBox1.Location = new System.Drawing.Point(5, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(138, 73);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Load";
            // 
            // rbLoadOriginal
            // 
            this.rbLoadOriginal.AutoSize = true;
            this.rbLoadOriginal.Location = new System.Drawing.Point(6, 42);
            this.rbLoadOriginal.Name = "rbLoadOriginal";
            this.rbLoadOriginal.Size = new System.Drawing.Size(117, 17);
            this.rbLoadOriginal.TabIndex = 1;
            this.rbLoadOriginal.Text = "Load Original (slow)";
            this.rbLoadOriginal.UseVisualStyleBackColor = true;
            // 
            // rbLoadFromCache
            // 
            this.rbLoadFromCache.AutoSize = true;
            this.rbLoadFromCache.Checked = true;
            this.rbLoadFromCache.Location = new System.Drawing.Point(6, 19);
            this.rbLoadFromCache.Name = "rbLoadFromCache";
            this.rbLoadFromCache.Size = new System.Drawing.Size(105, 17);
            this.rbLoadFromCache.TabIndex = 0;
            this.rbLoadFromCache.TabStop = true;
            this.rbLoadFromCache.Text = "Load from cache";
            this.rbLoadFromCache.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(76, 91);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(67, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FrmLoadOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(149, 121);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLoadOption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TopMost = true;            
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbLoadOriginal;
        private System.Windows.Forms.RadioButton rbLoadFromCache;
        private System.Windows.Forms.Button btnOK;
    }
}