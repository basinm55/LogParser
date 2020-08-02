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
    public partial class FrmFilter : Form
    {
        public IDictionary<string, List<object>> PropertyFilter { get; set; }

        public FrmFilter()
        {
            InitializeComponent();
        }

        private void FrmFilter_Load(object sender, EventArgs e)
        {
            cmbProps.DataSource = new BindingSource(PropertyFilter, null);
            cmbProps.DisplayMember = "Key";
            //cmbProps.ValueMember = "Value";
            cmbProps.SelectedIndex = -1;
        }

        private void cmbProps_SelectedIndexChanged(object sender, EventArgs e)
        {            
            lsbValues.Items.Clear();
            lsbValues.SelectedIndex = -1;
            btnAdd.Enabled = false;

            if (cmbProps.SelectedIndex < 0 || cmbProps.SelectedItem == null || !(cmbProps.SelectedItem is KeyValuePair<string, List<object>>)) return;

            var values = PropertyFilter[((KeyValuePair<string, List<object>>)cmbProps.SelectedItem).Key];            
            foreach (var val in values)
            {
                lsbValues.Items.Add(val);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //TODO:
            Close();
        }

        private void lsbValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = lsbValues.SelectedIndex >= 0;            
            string value = lsbValues.SelectedIndex >= 0 ? lsbValues.SelectedItem.ToString() : null;
        }
    }
}
