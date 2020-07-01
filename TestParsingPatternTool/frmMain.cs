using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Helpers;

namespace PatternValidator
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            //txtBoxLogEntry.Text = "[7]0000.0000::05/31/2020-16:43:13.927 [jtucxip]CUsbipRequest::CUsbipRequest: type R dev FFFFC30247334410 -> FFFFC3023C1F74F0";
            //txtBoxPattern.Text = "[% *1d]% *4c.% *4c::% s[% *7s]CUsbipRequest::CUsbipRequest: type % s dev % s-> % s";
            //dGVResults.RowsDefaultCellStyle.SelectionBackColor = dGVResults.RowsDefaultCellStyle.BackColor.IsEmpty ? System.Drawing.Color.White : dGVResults.RowsDefaultCellStyle.BackColor;
            //dGVResults.RowsDefaultCellStyle.SelectionForeColor = dGVResults.RowsDefaultCellStyle.ForeColor.IsEmpty ? System.Drawing.Color.Black : dGVResults.RowsDefaultCellStyle.ForeColor;
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            ScanFormatted sf = new ScanFormatted();
            sf.Parse(txtBoxLogEntry.Text.Trim(), txtBoxPattern.Text.Trim());            
            var isSuccess = IsParsingSuccessful(sf.Results);

            var ds = new List<KeyValuePair<int, string>>();
            int i = 0;
            foreach (var res in sf.Results)
            {
                ds.Add(new KeyValuePair<int, string>(i, res.ToString()));
                i++;
            }
            if (!isSuccess)
            {
                dGVResults.ForeColor = Color.Red;
                lblResult.BackColor = Color.Red;
                lblResult.ForeColor = Color.White;
                lblResult.Text = "Error";
            }
            else
            {
                dGVResults.ForeColor = Color.Green;
                lblResult.BackColor = Color.Green;
                lblResult.ForeColor = Color.White;
                lblResult.Text = "OK";
            }
         
            dGVResults.DataSource = ds;
            dGVResults.ClearSelection();
        }

        private bool IsParsingSuccessful(List<object> results)
        {
            if (results.Count == 0)
                return false;

            var percentCount = txtBoxPattern.Text.Count(c => c == '%');
            var droppedPercentCount = txtBoxPattern.Text.Split(new string[] { "%*" }, StringSplitOptions.None).Length - 1;
            if (results.Count != percentCount - droppedPercentCount)
                return false;

            return true;
        }
    }
}
