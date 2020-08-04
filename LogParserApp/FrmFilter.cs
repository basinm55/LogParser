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

        public string FilterExpression { get; private set; }

        public FrmFilter()
        {
            InitializeComponent();
        }

        private void FrmFilter_Load(object sender, EventArgs e)
        {
            CreateGridColumns();

            cmbProps.DataSource = new BindingSource(PropertyFilter, null);
            cmbProps.DisplayMember = "Key";
            //cmbProps.ValueMember = "Value";
            cmbProps.SelectedIndex = -1;
        }

        private void CreateGridColumns()
        {
            var colConnect = new DataGridViewTextBoxColumn();
            colConnect.HeaderText = "";
            colConnect.Name = "Connect";

            var colProp = new DataGridViewTextBoxColumn();
            colProp.HeaderText = "Property";
            colProp.Name = "Property";

            var colOperator = new DataGridViewTextBoxColumn();
            colOperator.HeaderText = "Operator";
            colOperator.Name = "Operator";

            var colExpr = new DataGridViewTextBoxColumn();
            colExpr.HeaderText = "Value";
            colExpr.Name = "Value";
            dgvFilter.Columns.AddRange(new DataGridViewColumn[] { colConnect, colProp, colOperator, colExpr });
        }

        private void AddGridRow(string connect, string name, string operat, string value)
        {
            DataGridViewRow row = new DataGridViewRow();
            var index = dgvFilter.Rows.Add();
            dgvFilter.Rows[index].Cells["Connect"].Value = connect;
            dgvFilter.Rows[index].Cells["Property"].Value = name;
            dgvFilter.Rows[index].Cells["Operator"].Value = operat;
            dgvFilter.Rows[index].Cells["Value"].Value = value;
            dgvFilter.ClearSelection();
        }

        private void cmbProps_SelectedIndexChanged(object sender, EventArgs e)
        {
            lsbValues.Items.Clear();
            lsbValues.SelectedIndex = -1;
            btnAdd.Enabled = false;
            gbConnect.Enabled = false;

            if (cmbProps.SelectedIndex < 0 || cmbProps.SelectedItem == null || !(cmbProps.SelectedItem is KeyValuePair<string, List<object>>)) return;

            var values = PropertyFilter[((KeyValuePair<string, List<object>>)cmbProps.SelectedItem).Key];
            foreach (var val in values)
            {
                lsbValues.Items.Add(val);
            }

            gbConnect.Enabled = true;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            FilterExpression = FilterToLambdaExpression();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void lsbValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = lsbValues.SelectedIndex >= 0;
            gbOperator.Enabled = lsbValues.SelectedIndex >= 0;
            //string value = lsbValues.SelectedIndex >= 0 ? lsbValues.SelectedItem.ToString() : null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbProps.SelectedIndex < 0 ||
                cmbProps.SelectedItem == null ||
                !(cmbProps.SelectedItem is KeyValuePair<string, List<object>>)) return;

            var connect = string.Empty;
            if (dgvFilter.Rows.Count > 0)
                connect = rbOr.Checked ? "[or]" : "[and]";

            var name = ((KeyValuePair<string, List<object>>)cmbProps.SelectedItem).Key;
            var operat = rbNotEqual.Checked ? "Not equals" : "Equals";
            string value = lsbValues.SelectedIndex >= 0 ? lsbValues.SelectedItem.ToString() : null;
            AddGridRow(connect, name, operat, value);

            btnApply.Enabled = dgvFilter.Enabled && dgvFilter.Rows.Count > 0;
        }

        private void lsbValues_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (cmbProps.SelectedIndex < 0 ||
                           cmbProps.SelectedItem == null ||
                           !(cmbProps.SelectedItem is KeyValuePair<string, List<object>>)) return;

            var connect = string.Empty;
            if (dgvFilter.Rows.Count > 0)
                connect = rbOr.Checked ? "[or]" : "[and]";

            var name = ((KeyValuePair<string, List<object>>)cmbProps.SelectedItem).Key;
            var operat = rbNotEqual.Checked ? "Not equals" : "Equals";
            string value = lsbValues.SelectedIndex >= 0 ? lsbValues.SelectedItem.ToString() : null;
            AddGridRow(connect, name, operat, value);

            btnApply.Enabled = dgvFilter.Enabled && dgvFilter.Rows.Count > 0;
        }

        private void dgvFilter_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFilter.SelectedRows.Count > 0)
            {
                var selectedRow = dgvFilter.SelectedCells[0];

                btnRemove.Enabled = selectedRow != null;

            }
            else
                btnRemove.Enabled = false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvFilter.SelectedRows)
            {
                dgvFilter.Rows.RemoveAt(row.Index);
            }
            btnApply.Enabled = dgvFilter.Enabled && dgvFilter.Rows.Count > 0;
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            dgvFilter.Rows.Clear();
            lsbValues.Items.Clear();
            cmbProps.SelectedIndex = -1;
            rbAnd.Checked = true;
            rbEqual.Checked = true;
            gbConnect.Enabled = false;
            gbOperator.Enabled = false;
            btnApply.Enabled = false;
            btnAdd.Enabled = false;
            btnRemove.Enabled = false;
        }


        private string FilterToLambdaExpression()
        {
            string result = null;
            var bld = new StringBuilder();
            bld.Append("data.Where(x => x != null && ");
            foreach (DataGridViewRow row in dgvFilter.Rows)
            {                
                var connect = row.Cells["Connect"].Value.ToString();
                var prop = row.Cells["Property"].Value.ToString();               
                var oper = row.Cells["Operator"].Value.ToString();
                var val = row.Cells["Value"].Value.ToString();
                bld.Append(string.Format("{0} {1} {2} {3}",
                    connect == string.Empty ? string.Empty : connect.ToLower() == "[and]" ? " && " : " || ",
                    "(string)(x.GetDynPropertyValue(\"" + prop+ "\"))",
                    oper.ToLower()=="equals" ? "==" : "!=",
                    "\"" + val+ "\""));                              
            }
            bld.Append(")");

            result = bld.ToString();

            return result;
        } 
    }
}
