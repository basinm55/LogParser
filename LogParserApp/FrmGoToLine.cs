using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogParserApp
{
    public partial class FrmGoToLine : Form
    {
        public int SelectedLineNum { get; private set; }
        public FrmGoToLine()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedLineNum = (int)numUpDwnLineNum.Value;
            DialogResult = DialogResult.OK;
            Close();
        }
     

       

        private void FrmGoToLine_Load(object sender, EventArgs e)
        {
            numUpDwnLineNum.Focus();
            numUpDwnLineNum.Select(0, 1);
        }

        private void FrmGoToLine_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)27:
                    Close();
                    break;
                case (char)13:
                    Close();
                    SelectedLineNum = (int)numUpDwnLineNum.Value;
                    DialogResult = DialogResult.OK;
                    break;
            }
        }
    }
}
