﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Reflection;
using Entities;
using Helpers;
using PatternValidator;
using static Entities.Enums;
using System.Data;

namespace LogParserApp
{
    public partial class FrmMain : Form
    {
        private Parser _parser;
        private ProfileManager _profileMng;        
        private XElement _selectedProfile;
        private string _externalEditorExecutablePath;
        private string _currentDevice = null;
        private string _currentFilterThis = null;
        private string _currentFilterState = null;
        private string _loadedLogFileName = null;
        private string _selectedProfileFileName = null;
        private bool _currentFilterHasDataBuffer = false;        
        private Process _externalEditorProcess = null;

        public FrmMain()
        {           
            InitializeComponent();
            bkgWorkerLoad.WorkerReportsProgress = true;
            bkgWorkerLoad.WorkerSupportsCancellation = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {            
            _profileMng = new ProfileManager();                                                                              
            _externalEditorExecutablePath = ConfigurationManager.AppSettings["ExternalEditorExecutablePath"].ToString();
            if (string.IsNullOrWhiteSpace(_externalEditorExecutablePath))
                _externalEditorExecutablePath = "notepad.exe";

            TryImportProfile();
            
            mnuItemLoad.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);

            btnViewLoadedLog.Enabled = false;
            btnViewAppLog.Enabled = false;
            btnViewAppLog.Enabled = false;
            progressBar.Visible = false;
        }
  

        private void TryImportProfile()
        {
            if (ConfigurationManager.AppSettings["LastUsedProfile"] != null)
                _selectedProfileFileName = ConfigurationManager.AppSettings["LastUsedProfile"].ToString();
            if (string.IsNullOrEmpty(_selectedProfileFileName) || !File.Exists(_selectedProfileFileName))
                _selectedProfileFileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DefaultProfile.xml");
            if (!string.IsNullOrEmpty(_selectedProfileFileName))
            {
                _profileMng.ProfilePath = _selectedProfileFileName;
                _profileMng.LoadXmlFile(_selectedProfileFileName);
                _selectedProfile = _profileMng.CurrentProfile;
            }
        }

        private void UpdateFormTitle()
        {
            if (!string.IsNullOrWhiteSpace(_selectedProfileFileName))
            {                                              
                Text = string.Format("LogParser - Profile: [{0}]", Path.GetFileName(_selectedProfileFileName));
                if (!string.IsNullOrWhiteSpace(_loadedLogFileName))                  
                    Text = Text + "   " +string.Format("Log File: [{0}]", Path.GetFileName(_loadedLogFileName));
                
            }
            else
            { 
                Text = "LogParser";
            }
        }
        
        private void UpdateInfoBox(StateObject stateObj)
        {
            
            var ds = new List<KeyValuePair<string, string>>();


            if (stateObj != null && stateObj.State != State.Empty)
            {
                var baseObj = stateObj.Parent;                
                var time = stateObj.Time != DateTime.MinValue ?
                                    string.Format("{0:dd/MM/yyyy-HH:mm:ss.FFF}", stateObj.Time)
                                    : null;
                
                ds.Add(new KeyValuePair<string, string>("Class", stateObj.ObjectClass.ToString()));
                ds.Add(new KeyValuePair<string, string>("State", stateObj.State.ToString()));
                ds.Add(new KeyValuePair<string, string>("this", baseObj.GetThis()));
                ds.Add(new KeyValuePair<string, string>("Time", time));

                foreach (var prop in baseObj.DynObjectDictionary)
                {
                    if (ParserView.AllowedForDisplayProperties(prop.Key) && !ds.Any(x => x.Key == prop.Key))
                        ds.Add(new KeyValuePair<string, string>(prop.Key, prop.Value.ToString()));
                }

                Type t = typeof(StateObject);
                var propertyInfo = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in propertyInfo)
                {
                    if (!ds.Any(x => x.Key == prop.Name))
                    {
                        if ((prop.PropertyType == typeof(string) ||
                        prop.PropertyType == typeof(int) ||
                        prop.PropertyType == typeof(ObjectClass))
                        && prop.Name.ToLower() != "description")
                        {                           
                            var val = prop.GetValue(stateObj, null).ToString();                            
                            ds.Add(new KeyValuePair<string, string>(prop.Name, val));
                        }
                    }
                }

                foreach (var desc in stateObj.VisualDescription)
                {               
                    if (!ds.Any(x => x.Key == desc.Key))
                        ds.Add(new KeyValuePair<string, string>(desc.Key, desc.Value));
                }                               

                dgvInfo.AutoGenerateColumns = false;
                dgvInfo.DataSource = ds;               
                dgvInfo.ClearSelection();
                dgvInfo.Enabled = true;
                dgvInfo.ReadOnly = false;
                dgvInfo.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;               
         
            }
            else
            {
                dgvInfo.AutoGenerateColumns = true;
                dgvInfo.DataSource = ds;                
            }
        }


        private void UpdateInfoBoxForDevice(ParserObject parserObj)
        {

            var ds = new List<KeyValuePair<string, string>>();


            if (parserObj != null)
            {
                var time = parserObj.Time != DateTime.MinValue ?
                    string.Format("{0:dd/MM/yyyy-HH:mm:ss.FFF}", parserObj.Time)
                    : null;
                ds.Add(new KeyValuePair<string, string>("Class", parserObj.ObjectClass.ToString()));
                ds.Add(new KeyValuePair<string, string>("this", parserObj.GetThis()));
                if (time != null)
                    ds.Add(new KeyValuePair<string, string>("Time", time));

                foreach (var prop in parserObj.DynObjectDictionary)
                {
                    if (ParserView.AllowedForDisplayProperties(prop.Key) && !ds.Any(x => x.Key == prop.Key))
                        ds.Add(new KeyValuePair<string, string>(prop.Key, prop.Value.ToString()));
                }

                Type t = typeof(ParserObject);
                var propertyInfo = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in propertyInfo)
                {
                    if (!ds.Any(x => x.Key == prop.Name))
                    {
                        if ((prop.PropertyType == typeof(string) ||
                            prop.PropertyType == typeof(int)) ||
                            prop.PropertyType == typeof(ObjectClass)
                            && prop.Name.ToLower() != "description")
                        {                            
                            var val = prop.GetValue(parserObj, null).ToString();
                            ds.Add(new KeyValuePair<string, string>(prop.Name, val));
                        }
                    }
                }                

                dgvInfo.AutoGenerateColumns = false;
                dgvInfo.DataSource = ds;
                dgvInfo.ClearSelection();
                dgvInfo.Enabled = true;
                dgvInfo.ReadOnly = false;
                dgvInfo.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
     
            }
            else
            {
                dgvInfo.AutoGenerateColumns = true;
                dgvInfo.DataSource = ds;
            }
        }



        private void dataGV_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            //Set empty cells unselectable
            if (e.Cell == null || e.StateChanged != DataGridViewElementStates.Selected)
                return;
            if (e.Cell.Value != null && (e.Cell.Value is StateObject)) return;
                e.Cell.Selected = false;
        }

        private void dataGV_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGV.SelectedCells.Count > 0)
            {
                var selectedCell = dataGV.SelectedCells[0];

                if (selectedCell == null || selectedCell.Value == null) return;

                if (selectedCell.Value is StateObject)
                {
                    var selctedStateObj = selectedCell.Value as StateObject;                    
                    UpdateInfoBox(selctedStateObj);

                }
            }
            else
            {
                UpdateInfoBox(null);
            }
        }

        private void dataGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SearchInterrupted(e);

            //Handle arrow click
            //if (!(dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewImageCell)) return;

            //var clickedCell = (DataGridViewImageCell)dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex];

            //if (clickedCell == null || clickedCell.Tag == null) return;

            //if (clickedCell.Tag is ParserObject)
            //{

            //    var referenceObj = clickedCell.Tag;
            //    //Find reference                                
            //    for (int i = 0; i < dataGV.Rows.Count; i++)
            //    {

            //        foreach (var cell in dataGV.Rows[i].Cells)
            //        {
            //            if ((cell as DataGridViewCell).Value != null &&
            //                (cell as DataGridViewCell).Value is StateObject &&
            //                ((cell as DataGridViewCell).Value as StateObject).Parent != null)
            //            {
            //                var parentObject = ((cell as DataGridViewCell).Value as StateObject).Parent;

            //                if (parentObject.PrevInterruptedObj == referenceObj ||
            //                    parentObject.NextContinuedObj == referenceObj)
            //                {
            //                    if (dataGV.Rows[i] != dataGV.Rows[(cell as DataGridViewCell).RowIndex])
            //                    {
            //                        dataGV.CurrentCell = (cell as DataGridViewCell);//dataGV[(cell as DataGridViewCell).ColumnIndex, iRowIndex];
            //                        break;
            //                    }
            //                }
            //            }
            //        }


            //        //foreach (var cell in dataGV.Rows[j].Cells)
            //        //{
            //        //    if ((cell as DataGridViewCell).Value != null &&
            //        //        (cell as DataGridViewCell).Value is StateObject &&
            //        //        ((cell as DataGridViewCell).Value as StateObject).Parent != null)
            //        //    {
            //        //        var parentObject = ((cell as DataGridViewCell).Value as StateObject).Parent;

            //        //        if (parentObject.PrevInterruptedObj == referenceObj ||
            //        //            parentObject.NextContinuedObj == referenceObj)
            //        //        {
            //        //            iRowIndex = j;
            //        //            break;
            //        //        }
            //        //    }
            //        //}                   
            //        //j--;
            //        //dataGV.CurrentCell = dataGV[0, 0];               
            //    }
            //}

        }

        private void SearchInterrupted(DataGridViewCellEventArgs e)
        {            
            int rowIndex = 0;

            if (!(dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewImageCell)) return;

            var clickedCell = (DataGridViewImageCell)dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (clickedCell == null || clickedCell.Tag == null || !(clickedCell.Tag is ParserObject)) return;

            var referenceObj = clickedCell.Tag;
            if (!(referenceObj is ParserObject)) return;

            StateObject referenceStateObj = null;

            if ((referenceObj as ParserObject).PrevInterruptedObj != null)
                referenceStateObj = (referenceObj as ParserObject).StateCollection.FirstOrDefault(x => x.ObjectClass != ObjectClass.Empty && x.ObjectClass != ObjectClass.ViewArrow);
            else if ((referenceObj as ParserObject).NextContinuedObj != null)
                referenceStateObj = (referenceObj as ParserObject).StateCollection.LastOrDefault(x => x.ObjectClass != ObjectClass.Empty && x.ObjectClass != ObjectClass.ViewArrow);
            else
                return;

            if (referenceStateObj == null) return;

            var colIndex = (referenceObj as ParserObject).StateCollection.IndexOf(referenceStateObj);

            bool isFound = false;
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    var stateObj = ((cell as DataGridViewCell).Value) as StateObject;
                    if (stateObj == referenceStateObj)
                    {
                        isFound = true;
                        dataGV.CurrentCell = dataGV[colIndex, rowIndex];
                        break;
                    }
                }
                
                if (isFound)
                    break;
               
                rowIndex++;
            }


            //foreach (DataGridViewRow row in dataGV.Rows)
            //{
            //    if (row.Cells[rowIndex].Value == null) continue;

            //    var stateObj = row.Cells[rowIndex].Value as StateObject;

            //    if (stateObj == null ||
            //        stateObj.ObjectClass == ObjectClass.ViewArrow ||
            //        stateObj.ObjectClass == ObjectClass.Empty) continue;

            //    var parentObject = stateObj.Parent;

            //    if (parentObject == referenceObj)
            //    {

            //    }

            //    List<StateObject> stateCollection = null;
            //    if (parentObject.NextContinuedObj != null)
            //        stateCollection = parentObject.NextContinuedObj.StateCollection;
            //    else if (parentObject.PrevInterruptedObj != null)
            //        stateCollection = parentObject.PrevInterruptedObj.StateCollection;

            //    if (stateCollection != null)
            //    {
            //        foreach (var st in stateCollection)
            //        {
            //            if (st.ObjectClass != ObjectClass.ViewArrow && st.ObjectClass != ObjectClass.Empty)
            //            {
            //                if (st == referenceStateObj)//stateObj.LineNum)
            //                {
            //                    rowIndex = row.Index;
            //                    dataGV.Rows[rowIndex].Selected = true;
            //                    break;
            //                }
            //            }
            //        }

            //    }
            //    //else
            //    rowIndex++;
            //}                              
        }

        //private void dataGV_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    // Ignore if a column or row header is clicked
        //    if (e.RowIndex != -1 && e.ColumnIndex != -1)
        //    {
        //        if (e.Button == MouseButtons.Right)
        //        {
        //            DataGridViewCell clickedCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];

        //            if (!(clickedCell.Value is StateObject)) return;

        //            dataGV.CurrentCell = clickedCell;  // Select the clicked cell

        //            // Get mouse position relative to the grid
        //            var relativeMousePosition = dataGV.PointToClient(Cursor.Position);

        //            // Show the context menu
        //            showRelatedLogEntryToolStripMenuItem.Enabled =
        //                (clickedCell.Value as StateObject).State >= 0;

        //            dataBufferToolStripMenuItem.Enabled =
        //                (clickedCell.Value as StateObject).DataBuffer != null;

        //            gridCmStrip.Show(dataGV, relativeMousePosition);
        //        }
        //        else
        //        {

        //        }
        //    }
        //}     

        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {          
            if (e.Value != null && e.Value is StateObject)
                e.Value = ((StateObject)e.Value).Description;
        }


        private void showRelatedLogEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGV.CurrentCell != null && dataGV.CurrentCell.Value != null && dataGV.CurrentCell.Value is ParserObject)
            {
                var relatedStateObject = (StateObject)dataGV.CurrentCell.Value;
                if (relatedStateObject != null)
                    FlexibleMessageBox.Show(relatedStateObject.LogEntry, "Related Log Entry", MessageBoxButtons.OK);
            }

        }

        private void showPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGV.CurrentCell != null && dataGV.CurrentCell.Value != null && dataGV.CurrentCell.Value is ParserObject)
            {
                var relatedStateObject = (StateObject)dataGV.CurrentCell.Value;
                if (relatedStateObject != null && relatedStateObject.Parent != null)
                {
                    var properties = new StringBuilder();
                    foreach (var prop in relatedStateObject.Parent.DynObjectDictionary)
                    {  
                        if (prop.Value is DateTime)
                        {
                            var dateValue = (DateTime)prop.Value;
                            properties.AppendLine(prop.Key + " = " + dateValue.ToString("dd/MM/yy HH:mm:ss.fff"));
                        }
                        else if (prop.Key != "DataBuffer")
                            properties.AppendLine(prop.Key + " = " + prop.Value);
                    }

                    //properties.AppendLine("State = " + relatedParserObject.ObjectState.ToString());
                    //if (relatedParserObject.LineNum >= 0)
                    //    properties.AppendLine("LineNum = " + relatedParserObject.LineNum.ToString());                   

                    //FlexibleMessageBox.Show(properties.ToString(),
                    //    string.Format("Properties of {0}: {1}",
                    //        relatedParserObject.ObjectClass.ToString(),
                    //        relatedParserObject.GetDynPropertyValue("this")),
                    //    MessageBoxButtons.OK);
                }
            }
        }


        #region Background Worker

        private bool closePending;

        private void bkgWorkerLoad_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {           
            _parser.Run(_selectedProfile, sender as BackgroundWorker, e);       
        }

        private void bkgWorkerLoad_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Visible = true;
            resultLabel.Text = (e.ProgressPercentage.ToString() + "%");
            if (!closePending)
                progressBar.Value = e.ProgressPercentage;
        }

        private void bkgWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {  
            if (e.Error != null)
                resultLabel.Text = "Load Error: " + e.Error.Message;
            else if (e.Cancelled == true)
                resultLabel.Text = "Load interrupted by user";
            else
                resultLabel.Text = "Load Done";

            calculateLabel.Text = string.Format("({0} of {1} log entries loaded)", _parser.CompletedLogLines, _parser.TotalLogLines);

            if (e.Error == null && _parser.ObjectCollection != null && _parser.ObjectCollection.Count > 0)
            {
                btnStopLoading.Visible = false;
                resultLabel.Text = "Prepare data for view...";
                closePending = true;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    progressBar.Value = 20;
                    CreateComboDeviceDataSource();
                    Application.DoEvents();

                    progressBar.Value = 40;
                    CreateFilterThisComboDataSource();
                    Application.DoEvents();

                    progressBar.Value = 60;
                    CreateFilterStateComboDataSource();
                    Application.DoEvents();

                    if (cmbShowDevice.Items.Count > 0)
                        cmbShowDevice.SelectedIndex = 0;
                    cmbShowDevice.Enabled = true;
                    chkShowAll.Enabled = true;
                    mnuItemProfile.Enabled = true;
                    mnuItemLoad.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);

                    btnViewLoadedLog.Enabled = true;
                    btnViewAppLog.Enabled = true;
                    UpdateFormTitle();

                    progressBar.Value = 80;                    
                    Application.DoEvents();
                    
                }
                finally
                {
                    Thread.Sleep(500);
                    resultLabel.Text = ("Ready");
                    progressBar.Value = 100;
                    progressBar.Visible = false;
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                    gbFilter.Enabled = true;
                    if (_parser.AppLogIsActive && _parser.AppLogger.ReportedLinesCount > 0)
                    {
                        if (MessageBox.Show(string.Format("Hi, Yuri!"
                            + Environment.NewLine + " There are {0} parsing errors reported while loading."
                            + Environment.NewLine
                            + "Do you want to open Application Log now?",
                            _parser.AppLogger.ReportedLinesCount),
                            "Warning: Errors reported",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            if (_externalEditorProcess == null || _externalEditorProcess.HasExited)
                            {
                                try
                                {
                                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor(_externalEditorExecutablePath, _parser.AppLogger.TargetPath);
                                }
                                catch
                                {
                                    MessageBox.Show("Hi, Yuri! Unfortunately you can't launch the external editor " + _externalEditorExecutablePath);
                                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor("notepad.exe", _parser.AppLogger.TargetPath);

                                }
                            }
                            else
                                WindowHelper.BringProcessToFront(_externalEditorProcess);
                        }
                    }
                }
            }
            else
                MessageBox.Show(e.Error.ToString());

            btnStopLoading.Visible = false;
            closePending = false;
        }

        #endregion Background Worker


        private void CreateComboDeviceDataSource()
        {
            var ds = _parser.ObjectCollection.
                                Where( o => o != null && o.ObjectClass == ObjectClass.Device).Distinct().
                                ToList();

            var comboSource = new Dictionary<string, ParserObject>();
            foreach (var itm in ds)
            {
                var thisVal = itm.GetThis();
                if (!comboSource.ContainsKey(thisVal))
                    comboSource.Add(thisVal, itm);
            }

            if (comboSource.Count > 0)
            {
                cmbShowDevice.DataSource = new BindingSource(comboSource, null);
                cmbShowDevice.DisplayMember = "Key";
                cmbShowDevice.ValueMember = "Value";
            }
        }

        private void CreateFilterThisComboDataSource()
        {
            var ds = _parser.ObjectCollection.Where(o => o.GetParent() != null).
                                Select(o => o.GetThis()).Distinct().
                                ToList();

            ds.Insert(0, "All...");
            cmbThis.DataSource = ds;                     
        }

        private void CreateFilterStateComboDataSource()
        {
            var ds = Enum.GetNames(typeof(State)).Where(x => x != "Unknown").ToList();

            ds.Insert(0, "All...");
            cmbState.DataSource = ds;
        }

        private void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {           
            cmbShowDevice.Enabled = !chkShowAll.Checked && cmbShowDevice.Items.Count > 0;
            if (chkShowAll.Checked)
            {
                txtHeader.Text = string.Empty;
                _currentDevice = null;
            }

            else
            {               
                UpdateDeviceDetails();
                if (cmbShowDevice.SelectedIndex >= 0 && cmbShowDevice.SelectedItem != null)
                    _currentDevice = ((KeyValuePair<string, ParserObject>)cmbShowDevice.SelectedItem).Key;
            }

            Cursor.Current = Cursors.WaitCursor;
            gbFilter.Enabled = false;
            Application.DoEvents();
            try
            {
                ParserView.CreateGridView(_parser.ObjectCollection, dataGV, _currentDevice);
            }
            catch (Exception ex)
            {
                _parser.AppLogger.LogException(ex);
                throw ex;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                gridLabel.Text = string.Format("  Total view rows: {0}", dataGV.Rows.Count.ToString());
                gbFilter.Enabled = true;
            }
        }    

        private void btnStopLoading_ButtonClick(object sender, EventArgs e)
        {
            if (bkgWorkerLoad.WorkerSupportsCancellation == true)
                bkgWorkerLoad.CancelAsync();
        }

        private void cmbShowDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbShowDevice.SelectedValue != null)
            {
                if (cmbShowDevice.SelectedItem.GetType() != typeof(KeyValuePair<string, ParserObject>)) return;
                UpdateDeviceDetails();               

                //Filter by selected device
                _currentDevice = ((KeyValuePair<string, ParserObject>)cmbShowDevice.SelectedItem).Key;
                gbFilter.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;               
                Application.DoEvents();
                try
                {
                    ParserView.CreateGridView(_parser.ObjectCollection, dataGV, _currentDevice);
                }
                catch (Exception ex)
                {
                    _parser.AppLogger.LogException(ex);
                    throw ex;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                    gridLabel.Text = string.Format("  Total view rows: {0}", dataGV.Rows.Count.ToString());
                    gbFilter.Enabled = true;
                    txtHeader.BackColor = Color.Red;
                }
            }                       
        }

        private void UpdateDeviceDetails()
        {         
            if (cmbShowDevice.SelectedItem.GetType() != typeof(KeyValuePair<string, ParserObject>)) return;

            var device = ((KeyValuePair<string, ParserObject>)cmbShowDevice.SelectedItem).Key;
            var obj = ((KeyValuePair<string, ParserObject>)cmbShowDevice.SelectedItem).Value;
            var time = string.Format("{0:MM/dd/yyyy-HH:mm:ss.FFF}", obj.GetDynPropertyValue("Time"));                        
            
            if (obj != null)
            {
                txtHeader.Tag = obj;
                txtHeader.Text = string.Format("{0}: {1}   ID: {2} on the port {3}. Created at {4}",
                    obj.ObjectClass.ToString(),
                    obj.GetThis(),
                    (string)obj.GetDynPropertyValue("ID"),
                    (string)obj.GetDynPropertyValue("Port"),
                    time);
            }
        }
            

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bkgWorkerLoad.IsBusy)
            {
                _parser.Locker.Reset();
                if (MessageBox.Show("Hi, Youri! Do you realy want to cancel loading?", "Log Loading in progress...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    closePending = true;
                    bkgWorkerLoad.CancelAsync();
                }
                else
                {
                    _parser.Locker.Set();
                    e.Cancel = true;
                }

                return;
            }   

        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            cmbThis.SelectedIndex = 0;
            cmbState.SelectedIndex = 0;
            chkHasDataBuffer.Checked = false;
            _currentFilterThis = null;
            _currentFilterState = null;
            _currentFilterHasDataBuffer = false;
            SetFilters();
        }

        private void cmbThis_SelectedIndexChanged(object sender, EventArgs e)
        {    
            SetFilters();
        }

        private void cmbState_SelectedIndexChanged(object sender, EventArgs e)
        {            
            SetFilters();
        }

        private void SetFilters()
        {           
            gbFilter.Enabled = false;
            _currentFilterThis = cmbThis.SelectedIndex > 0 ? (string)cmbThis.SelectedItem : null;
            _currentFilterState = cmbState.SelectedIndex > 0 ? (string)cmbState.SelectedItem : null;
            _currentFilterHasDataBuffer = chkHasDataBuffer.Checked;
            
            var filteredCollection = _currentFilterThis != null ?
                     _parser.ObjectCollection.Where(x => x.GetThis() == _currentFilterThis).ToList() :
                    _parser.ObjectCollection;

            filteredCollection = _currentFilterState != null ?
                     filteredCollection.Where(x => x != null && x.StateCollection.Any(y => y != null && y.State.ToString() == _currentFilterState)).ToList() :
                    filteredCollection;

            filteredCollection = _currentFilterHasDataBuffer == true ?
                    filteredCollection.Where(x => x != null && x.StateCollection.Any(y => y != null && !string.IsNullOrWhiteSpace(y.DataBuffer.ToString()))).ToList() :
                   filteredCollection;


            Cursor.Current = Cursors.WaitCursor;            
            Application.DoEvents();
            gbFilter.Enabled = false;
            try
            {
                ParserView.CreateGridView(filteredCollection, dataGV, _currentDevice);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                gridLabel.Text = string.Format("  Total view rows: {0}", dataGV.Rows.Count.ToString());
                gbFilter.Enabled = true;               
            }

        }
       

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            btnStopLoading.Visible = false;
            cmbShowDevice.SelectedIndex = -1;
            cmbShowDevice.Enabled = false;
            chkShowAll.Checked = false;
            chkShowAll.Enabled = false;
            gbFilter.Enabled = false;

            UpdateFormTitle();
        }

        private void btnViewLoadedLog_Click(object sender, EventArgs e)
        {         
            if (_externalEditorProcess == null || _externalEditorProcess.HasExited)
            {
                try
                {
                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor(_externalEditorExecutablePath, _loadedLogFileName);
                }
                catch
                {
                    MessageBox.Show("Hi, Youri! Unfortunately you can't launch the external editor " + _externalEditorExecutablePath);
                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor("notepad.exe", _loadedLogFileName);
                }
            }
            else
                WindowHelper.BringProcessToFront(_externalEditorProcess);
        }

        private void btnViewAppLog_Click(object sender, EventArgs e)
        {
            if (!_parser.AppLogIsActive)
            {
                MessageBox.Show("Hi, Youri! Unfortunately, the application log is not active!" +
                                Environment.NewLine +
                                "Please activate it via configuration file.",
                                "Application log",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }


            if (_externalEditorProcess == null || _externalEditorProcess.HasExited)
            {
                try
                {
                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor(_externalEditorExecutablePath, _parser.AppLogger.TargetPath);
                }
                catch
                {
                    MessageBox.Show("Hi, Youri! Unfortunately you can't launch the external editor " + _externalEditorExecutablePath);
                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor("notepad.exe", _parser.AppLogger.TargetPath);

                }
            }
            else
                WindowHelper.BringProcessToFront(_externalEditorProcess);
        }

        #region TopMenu
     

        private void mnuItemLoad_Click(object sender, EventArgs e)
        {
            dlgLoadLog.Filter = "Log files (*.log)|*.log|All files (*.*)|*.*";
            dlgLoadLog.DefaultExt = "*.log";
            if (dlgLoadLog.ShowDialog() != DialogResult.Cancel)
            {
                _loadedLogFileName = dlgLoadLog.FileName;
                _parser = new Parser(_loadedLogFileName);
                if (!bkgWorkerLoad.IsBusy)
                {
                    txtHeader.Text = string.Empty;
                    dataGV.DataSource = null;
                    dataGV.Rows.Clear();
                    dataGV.Refresh();

                    // Start the asynchronous operation.
                    mnuItemLoad.Enabled = false;
                    mnuItemProfile.Enabled = false;
                    btnStopLoading.Visible = true;
                    calculateLabel.Text = string.Empty;
                    gridLabel.Text = string.Empty;
                    cmbShowDevice.Enabled = false;
                    chkShowAll.Checked = false;
                    chkShowAll.Enabled = false;
                    cmbShowDevice.SelectedIndex = -1;
                    bkgWorkerLoad.RunWorkerAsync();
                }
            }
        }

        private void mnuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void mnuItemProfile_Click(object sender, EventArgs e)
        {
            dlgProfile.Filter = "Xml files (*.xml)|*.xml";
            dlgProfile.DefaultExt = "*.xml";
            if (dlgProfile.ShowDialog() != DialogResult.Cancel)
            {
                _selectedProfileFileName = dlgProfile.FileName;
                mnuItemLoad.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);
                _profileMng.ProfilePath = _selectedProfileFileName;
                _profileMng.LoadXmlFile(_selectedProfileFileName);
                _selectedProfile = _profileMng.CurrentProfile;
                if (!bkgWorkerLoad.IsBusy)
                {
                    txtHeader.Text = string.Empty;
                    dataGV.DataSource = null;
                    dataGV.Rows.Clear();
                    dataGV.Refresh();
                    UpdateFormTitle();
                    Utils.UpdateConfig("LastUsedProfile", _selectedProfileFileName);
                }
            }
            mnuItemLoad.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);
        }
        private void mnuItemPatternValidator_Click(object sender, EventArgs e)
        {
            var patternValidatorForm = Application.OpenForms["frmPatternValidator"];
            if (patternValidatorForm == null)
                patternValidatorForm = new frmPatternValidator();
            else
            {
                patternValidatorForm.WindowState = FormWindowState.Normal;
                patternValidatorForm.BringToFront();
            }
            patternValidatorForm.Show();
        }

        #endregion TopMenu

        private void chkHasDataBuffer_CheckedChanged(object sender, EventArgs e)
        {
            _currentFilterHasDataBuffer = chkHasDataBuffer.Checked;
            SetFilters();
        }

        private void dataBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGV.CurrentCell != null && dataGV.CurrentCell.Value != null && dataGV.CurrentCell.Value is ParserObject)
            {
                var relatedParserObject = (ParserObject)dataGV.CurrentCell.Value;
                if (relatedParserObject != null)
                {
                    var properties = new StringBuilder();
                    foreach (var prop in relatedParserObject.DynObjectDictionary)
                    {    
                        if (prop.Key == "DataBuffer" && prop.Value != null)
                        {
                            FlexibleMessageBox.Show(prop.Value.ToString(),
                                    string.Format("Data Buffer of {0}: {1}",
                                        relatedParserObject.ObjectClass.ToString(),
                                        relatedParserObject.GetDynPropertyValue("this")),
                                    MessageBoxButtons.OK);
                            return;

                        }
                    }               
                }
            }
        }

        private void dataGV_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewImageCell)
            {
                var tag = dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                if (tag is ParserObject &&
                    ((tag as ParserObject).NextContinuedObj != null || (tag as ParserObject).PrevInterruptedObj != null))
                    (sender as DataGridView).Cursor = Cursors.Hand;
                return;
            }
            

            var stateObj = (dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as StateObject);            

            if (stateObj != null)
                (sender as DataGridView).Cursor = Cursors.Hand;
            else
                (sender as DataGridView).Cursor = Cursors.Default;
           
        }

        private void dataGV_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            (sender as DataGridView).Cursor = Cursors.Default;
        }

        private void txtHeader_Click(object sender, EventArgs e)
        {
            dataGV.ClearSelection();
            var txtBox = sender as TextBox;
            var relatedParserObj = txtBox.Tag as ParserObject;
            if (relatedParserObj != null)
            {
                txtBox.BackColor = Color.DarkOrange;
                UpdateInfoBoxForDevice(relatedParserObj);
            }

        }

        private void txtHeader_MouseLeave(object sender, EventArgs e)
        {
            txtHeader.Cursor = Cursors.Default;
        }

        private void txtHeader_MouseMove(object sender, MouseEventArgs e)
        {
            txtHeader.Cursor = Cursors.Hand;
        }

        private void txtHeader_Leave(object sender, EventArgs e)
        {
            var txtBox = sender as TextBox;
            txtBox.BackColor = Color.Red;
        }

        private void txtHeader_TextChanged(object sender, EventArgs e)
        {
            var txtBox = sender as TextBox;
            Size size = TextRenderer.MeasureText(txtBox.Text, txtBox.Font);
            txtBox.Width = size.Width;
            txtBox.Height = size.Height;
        }       
    }
}
