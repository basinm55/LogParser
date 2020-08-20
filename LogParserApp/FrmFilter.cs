using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entities;

namespace LogParserApp
{
    public partial class FrmFilter : Form
    {
        private bool _isClearAll;

        private string _loadedFilter;

        public List<PropertyFilter> PropertyFilter { get; set; }

        public FilterObject CurrentFilter { get; set; }

        public string CurrentDevice { get; set; }

        public FrmFilter()
        {
            InitializeComponent();
        }

        private void FrmFilter_Load(object sender, EventArgs e)
        {
            CreateGridColumns();

            cmbProps.DataSource = new BindingSource(PropertyFilter, null);
            cmbProps.DisplayMember = "PropertyName";
            //cmbProps.ValueMember = "Values";
            cmbProps.SelectedIndex = -1;

            if (CurrentFilter != null)
                PopulateCurrentFilter();
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

            if (cmbProps.SelectedIndex < 0 || cmbProps.SelectedItem == null || !(cmbProps.SelectedItem is PropertyFilter)) return;            

            var selectedItem = PropertyFilter.FirstOrDefault(x => x.PropertyName == (cmbProps.SelectedValue as PropertyFilter).PropertyName);
            selectedItem.Values.Sort(CompareValues);
            foreach (var val in selectedItem.Values)
            {
                if (val.Value != null)
                {
                    if (CurrentDevice == null && !lsbValues.Items.Contains(val.Value))
                        lsbValues.Items.Add(val.Value);
                    else if (val.Parent == CurrentDevice && !lsbValues.Items.Contains(val.Value))
                        lsbValues.Items.Add(val.Value);
                }
            }                    
            gbConnect.Enabled = true;

        }

        private static int CompareValues(PropertyFilterValue a, PropertyFilterValue b)
        {
            switch (a.Value)
            {
                case string _:
                    return ((string)a.Value).CompareTo((string)b.Value);
                case decimal _:
                    return ((decimal)a.Value).CompareTo((decimal)b.Value);
                case int _:
                    return ((int)a.Value).CompareTo((int)b.Value);
                default:
                    return 0;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void lsbValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = lsbValues.SelectedIndex >= 0;
            gbOperator.Enabled = lsbValues.SelectedIndex >= 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbProps.SelectedIndex < 0 ||
                cmbProps.SelectedItem == null ||
                !(cmbProps.SelectedItem is PropertyFilter)) return;

            var connect = string.Empty;
            if (dgvFilter.Rows.Count > 0)
                connect = rbOr.Checked ? "[or]" : "[and]";

            var name = ((PropertyFilter)cmbProps.SelectedItem).PropertyName;
            var operat = rbNotEqual.Checked ? "Not equal" : "equal";
            string value = lsbValues.SelectedIndex >= 0 ? lsbValues.SelectedItem.ToString() : null;

            if (IsDuplicatedCriteria(name, operat, value))
                return;

            AddGridRow(connect, name, operat, value);
            _loadedFilter = null;
            UpdateFormTitle();

            _isClearAll = dgvFilter.Rows.Count == 0;
            btnApply.Enabled = _isClearAll || (dgvFilter.Enabled && dgvFilter.Rows.Count > 0);
            saveFilterToolStripMenuItem.Enabled = btnApply.Enabled;
        }

        private void lsbValues_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (cmbProps.SelectedIndex < 0 ||
                           cmbProps.SelectedItem == null ||
                           !(cmbProps.SelectedItem is PropertyFilter)) return;

            var connect = string.Empty;
            if (dgvFilter.Rows.Count > 0)
                connect = rbOr.Checked ? "[or]" : "[and]";

            var name = ((PropertyFilter)cmbProps.SelectedItem).PropertyName;
            var operat = rbNotEqual.Checked ? "Not equal" : "equal";
            string value = lsbValues.SelectedIndex >= 0 ? lsbValues.SelectedItem.ToString() : null;

            if (IsDuplicatedCriteria(name, operat, value))
                return;

            AddGridRow(connect, name, operat, value);

            _isClearAll = dgvFilter.Rows.Count == 0;
            btnApply.Enabled = _isClearAll || (dgvFilter.Enabled && dgvFilter.Rows.Count > 0);
            _loadedFilter = null;
            UpdateFormTitle();
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
            //remove first connection if need
            if (dgvFilter.Rows.Count > 0 && dgvFilter.Rows[0] != null)
                dgvFilter.Rows[0].Cells[0].Value = string.Empty;

            _isClearAll = dgvFilter.Rows.Count == 0;
            btnApply.Enabled = _isClearAll || (dgvFilter.Enabled && dgvFilter.Rows.Count > 0);
            saveFilterToolStripMenuItem.Enabled = btnApply.Enabled;
            _loadedFilter = null;
            UpdateFormTitle();
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
            btnAdd.Enabled = false;
            btnRemove.Enabled = false;
            saveFilterToolStripMenuItem.Enabled = false;
            btnApply.Enabled = true;
            _isClearAll = dgvFilter.Rows.Count == 0;
        }

        private void PopulateCurrentFilter()
        {
            foreach (var fltDef in CurrentFilter.Definitions)
            {
                AddGridRow(fltDef.Connector, fltDef.Property, fltDef.Operator, fltDef.Value);
            }
            dgvFilter.Enabled = true;
            btnApply.Enabled = dgvFilter.Enabled && dgvFilter.Rows.Count > 0;
            saveFilterToolStripMenuItem.Enabled = btnApply.Enabled;
        }

        private void ApplyCurrentFilter()
        {
            if (_isClearAll && CurrentFilter != null)
            {
                CurrentFilter.Clear();
                return;
            }

            if (CurrentFilter == null)
                CurrentFilter = new FilterObject();
            else
                CurrentFilter.Clear();

            var bld = new StringBuilder();
            bld.Append("data.Where(x => x != null && ");
            foreach (DataGridViewRow row in dgvFilter.Rows)
            {
                var connect = row.Cells["Connect"].Value.ToString();
                var prop = row.Cells["Property"].Value.ToString();
                var oper = row.Cells["Operator"].Value.ToString();
                var val = row.Cells["Value"].Value.ToString();

                CurrentFilter.Definitions.Add(new FilterDefinition()
                {
                    Connector = connect,
                    Property = prop,
                    Operator = oper,
                    Value = val
                });


                bld.Append(string.Format("{0} {1} {2} {3}",
                    connect == string.Empty ? string.Empty : connect.ToLower() == "[and]" ? " && " : " || ",
                    "(string)(x.GetDynPropertyValue(\"" + prop + "\"))",
                    oper.ToLower() == "equal" ? "==" : "!=",
                    "\"" + val + "\""));
            }
            bld.Append(")");

            CurrentFilter.FilterExpression = bld.ToString();
        }

        private bool IsDuplicatedCriteria(string name, string operat, string value)
        {
            var result = dgvFilter.Rows
                                    .Cast<DataGridViewRow>()
                                    .Any(r => r.Cells["Property"].Value.ToString().Equals(name) &&
                                                r.Cells["Operator"].Value.ToString().Equals(operat) &&
                                                r.Cells["Value"].Value.ToString().Equals(value));
            if (result)
            {
                MessageBox.Show("Hi, Yuri!" + Environment.NewLine +
                    "The same criteria is already exists in the filter!" + Environment.NewLine +
                    "Please select another criteria.",
                    "Duplicated filter criteria", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return result;
        }

        private void loadFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgLoadFilter.Filter = "Flt files (*.flt)|*.flt";
            dlgLoadFilter.DefaultExt = "*.flt";
            if (dlgLoadFilter.ShowDialog() != DialogResult.Cancel)
            {
                var filterFileName = dlgLoadFilter.FileName;
                var fltList = File.ReadLines(filterFileName).ToList();
                if (CurrentFilter == null)
                    CurrentFilter = new FilterObject();
                else
                    CurrentFilter.Clear();

                dgvFilter.Rows.Clear();

                foreach (var line in fltList)
                {
                    var split = line.Split(',');
                    var connect = split[0];
                    var prop = split[1];
                    var oper = split[2];
                    var val = split[3];
                    CurrentFilter.Definitions.Add(new FilterDefinition()
                    {
                        Connector = connect,
                        Property = prop,
                        Operator = oper,
                        Value = val
                    });

                }
                PopulateCurrentFilter();
                _loadedFilter = dlgLoadFilter.FileName;
                UpdateFormTitle();
            }
            else
            {
                _loadedFilter = null;
                UpdateFormTitle();
            }

        }

        private void saveFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgSaveFilter.Filter = "Flt files (*.flt)|*.flt";
            dlgSaveFilter.DefaultExt = "*.flt";
            if (dlgSaveFilter.ShowDialog() != DialogResult.Cancel)
            {

                using (StreamWriter file = new StreamWriter(dlgSaveFilter.FileName))
                {                 
                    foreach (DataGridViewRow row in dgvFilter.Rows)
                    {
                        var connect = row.Cells["Connect"].Value.ToString();
                        var prop = row.Cells["Property"].Value.ToString();
                        var oper = row.Cells["Operator"].Value.ToString();
                        var val = row.Cells["Value"].Value.ToString();

                        file.WriteLine(connect + "," +
                            prop + "," +
                            oper + "," +
                            val);
                    }
                }

            }
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void UpdateFormTitle()
        {
            if (!string.IsNullOrWhiteSpace(_loadedFilter))
            {
                Text = string.Format("Custom Filter: [{0}]", Path.GetFileName(_loadedFilter));             
            }
            else
            {
                Text = "Custom Filter";
            }
        }
    }
}
