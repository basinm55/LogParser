﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Entities;
using Helpers;

namespace LogParserApp
{
    public partial class FrmMain : Form
    {
        private Parser _parser;
        private ProfileManager _profileMng;
        private XElement _selectedProfile;

        public FrmMain()
        {           
            InitializeComponent();
            bkgWorker.WorkerReportsProgress = true;
            bkgWorker.WorkerSupportsCancellation = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {            
            _profileMng = new ProfileManager();
            _profileMng.LoadXmlFile("LogParserProfile.xml");
            _profileMng.GetProfileByName("Default");                                            
            _selectedProfile = _profileMng.CurrentProfile;

            rbPort.Checked = true;
            btnStopLoading.Visible = false;                       
            cmbShowDevice.SelectedIndex = -1;
            cmbShowDevice.Enabled = false;
            UpdateControlsState();            
        }

        public string loadedLogFileName;   

        private void btnLoadLog_Click(object sender, EventArgs e)
        {   
            dlgLoadLog.Filter = "Log files (*.log)|*.log|All files (*.*)|*.*";
            dlgLoadLog.DefaultExt = "*.log";
            if (dlgLoadLog.ShowDialog() != DialogResult.Cancel)
            {  
                loadedLogFileName = dlgLoadLog.FileName;
                _parser = new Parser(loadedLogFileName);                               
                if (!bkgWorker.IsBusy)
                {
                    dataGV.DataSource = null;
                    dataGV.Rows.Clear();
                    dataGV.Refresh();

                    // Start the asynchronous operation.
                    btnLoadLog.Enabled = false;
                    btnStopLoading.Visible = true;
                    calculateLabel.Text = string.Empty;
                    cmbShowDevice.Enabled = false;
                    cmbShowDevice.SelectedIndex = -1;
                    bkgWorker.RunWorkerAsync();
                }               
            }

        }     

        private void UpdateControlsState()
        {
            if (!string.IsNullOrWhiteSpace(loadedLogFileName) && File.Exists(loadedLogFileName))
            {
                var shortLogFileName = Path.GetFileName(loadedLogFileName);
                gbSearch.Enabled = true;
                gbSearch.Text = string.Format("Search in {0}", shortLogFileName);
                Text = string.Format("LogParser [{0}]", shortLogFileName);
            }
            else
            {
                gbSearch.Enabled = false;
                gbSearch.Text = "Search";
                Text = "LogParser";
            }
        }



        private void btnSearch_Click(object sender, EventArgs e)
        {
            ///TODO
        }
      

        private void dataGV_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            //Set empty cells unselectable
            if (e.Cell == null || e.StateChanged != DataGridViewElementStates.Selected)
                return;
            if (e.Cell.Value != null && (e.Cell.Value is ParserObject)) return;
                e.Cell.Selected = false;
        }

        private void dataGV_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Ignore if a column or row header is clicked
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridViewCell clickedCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];

                    if (!(clickedCell.Value is ParserObject)) return;
                                       
                    dataGV.CurrentCell = clickedCell;  // Select the clicked cell

                    // Get mouse position relative to the grid
                    var relativeMousePosition = dataGV.PointToClient(Cursor.Position);

                    // Show the context menu
                    gridCmStrip.Show(dataGV, relativeMousePosition);
                }
            }
        }     

        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {          
            if (e.Value != null && e.Value is ParserObject)
                e.Value = ((ParserObject)e.Value).VisualDescription;
        }


        private void showRelatedLogEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGV.CurrentCell != null && dataGV.CurrentCell.Value != null && dataGV.CurrentCell.Value is ParserObject)
            {
                var relatedParserObject = (ParserObject)dataGV.CurrentCell.Value;
                if (relatedParserObject != null)
                    FlexibleMessageBox.Show(relatedParserObject.LogLine, "Related Log Entry", MessageBoxButtons.OK);
            }

        }

        private void showPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGV.CurrentCell != null && dataGV.CurrentCell.Value != null && dataGV.CurrentCell.Value is ParserObject)
            {
                var relatedParserObject = (ParserObject)dataGV.CurrentCell.Value;
                if (relatedParserObject != null)
                {
                    var properties = new StringBuilder();
                    foreach (var prop in relatedParserObject.DynObjectDictionary)
                        properties.AppendLine(prop.Key + " = " + prop.Value.ToString());
                   
                    FlexibleMessageBox.Show(properties.ToString(),
                        string.Format("Properties of {0}: {1}",
                            relatedParserObject.ObjectClass,
                            relatedParserObject.GetDynPropertyValue("this")),
                        MessageBoxButtons.OK);
                }
            }
        }


        #region Background Worker

        private bool closePending;

        private void bkgWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {            
            _parser.Run(_selectedProfile, sender as BackgroundWorker, e);       
        }

        private void bkgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            resultLabel.Text = (e.ProgressPercentage.ToString() + "%");
            if (!closePending)
                progressBar.Value = e.ProgressPercentage;
        }

        private void bkgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {  
            if (e.Error != null)
                resultLabel.Text = "Load Error: " + e.Error.Message;
            else if (e.Cancelled == true)
                resultLabel.Text = "Load interrupted by user";
            else
                resultLabel.Text = "Load Done";

            calculateLabel.Text = string.Format("({0} of {1} log entries completed)", _parser.CompletedLogLines, _parser.TotalLogLines);

            if (e.Error == null && _parser.ObjectCollection != null && _parser.ObjectCollection.Count > 0)
            {
                //Fill devices                
                //cmbShowDevice.DataSource = _parser.ObjectCollection.
                //    Where(o => (string)o.GetDynPropertyValue("Name") == "Device").
                //    Select(o => o.GetDynPropertyValue("this")).ToArray();

                CreateComboDeviceDataSource();

                if (cmbShowDevice.Items.Count > 0)
                    cmbShowDevice.SelectedIndex = 0;
                cmbShowDevice.Enabled = true;
                btnLoadLog.Enabled = true; 
                UpdateControlsState();
            }
            else
                MessageBox.Show(string.Format("There is no parsing results for the file: '{0}'", loadedLogFileName));

            btnStopLoading.Visible = false;
            closePending = false;
        }

        #endregion Background Worker


        private void CreateComboDeviceDataSource()
        {
            var ds = _parser.ObjectCollection.
                                Where(o => (string)o.GetDynPropertyValue("Name") == "Device").Distinct().
                                ToList();

            var comboSource = new Dictionary<string, ParserObject>();
            foreach (var itm in ds)
                comboSource.Add((string)itm.GetDynPropertyValue("this"), itm);

            cmbShowDevice.DataSource = new BindingSource(comboSource, null);
            cmbShowDevice.DisplayMember = "Key";
            cmbShowDevice.ValueMember = "Value";
        }

        private void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {
            cmbShowDevice.Enabled = !chkShowAll.Checked && cmbShowDevice.Items.Count > 0;
            if (chkShowAll.Checked)
                cmbShowDevice.SelectedIndex = -1;

            ParserView.CreateGridView(_parser.ObjectCollection, dataGV, null);
        }    

        private void btnStopLoading_ButtonClick(object sender, EventArgs e)
        {
            if (bkgWorker.WorkerSupportsCancellation == true)
                bkgWorker.CancelAsync();
        }

        private void cmbShowDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbShowDevice.SelectedValue != null)
            {           
                UpdateDeviceDetails();

                //Filter by selected device
                var deviceFilter = ((KeyValuePair<string, ParserObject>)cmbShowDevice.SelectedItem).Key; 
                ParserView.CreateGridView(_parser.ObjectCollection, dataGV, deviceFilter);
            }                       
        }

        private void UpdateDeviceDetails()
        {
            var deviceAddress = ((KeyValuePair<string, ParserObject>)cmbShowDevice.SelectedItem).Key;
            var obj = ((KeyValuePair<string, ParserObject>)cmbShowDevice.SelectedItem).Value;
            var timestamp = obj.GetDynPropertyValue("Timestamp");

            lblHeader.Text = string.Format("{0}   Time: {1}", deviceAddress, timestamp);
        }
            

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bkgWorker.IsBusy)
            {
                if (MessageBox.Show("Do you want to cancel loading?", "Log Loading in progress...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    closePending = true;
                    bkgWorker.CancelAsync();
                }
                else                
                    e.Cancel = true;

                return;
            }   

        }
    }
}
