using System;
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
using Polenter.Serialization;
using System.Windows;

namespace LogParserApp
{
    public partial class FrmMain : Form
    {
        private Parser _parser;
        private ProfileManager _profileMng;        
        private XElement _selectedProfile;
        private string _externalEditorExecutablePath;
        private string _externalEditorArguments;
        private string _currentDevice = null; 
        private string _loadedLogFileName = null;
        private string _selectedProfileFileName = null;           
        private Process _externalEditorProcess = null;
        private string[] _displayInInfoboxProps;
        private FilterObject _currentFilter = null;
        private ToolTip _ttFilterInfo;
        private string _visualTimeFormat, _visualDateTimeFormat;

        private bool _loadCompleted = false;
        private bool _cacheEnabled;

        public FrmMain()
        {            
            InitializeComponent();
            bkgWorkerLoad.WorkerReportsProgress = true;
            bkgWorkerLoad.WorkerSupportsCancellation = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {            
            if (ConfigurationManager.AppSettings["CacheEnabled"] != null)
                _cacheEnabled = ConfigurationManager.AppSettings["CacheEnabled"].ToBoolean();

            _profileMng = new ProfileManager();
            if (ConfigurationManager.AppSettings["ExternalEditorExecutablePath"] != null)
            {
                _externalEditorExecutablePath = ConfigurationManager.AppSettings["ExternalEditorExecutablePath"].ToString();
                if (string.IsNullOrWhiteSpace(_externalEditorExecutablePath))
                    _externalEditorExecutablePath = "notepad.exe";
            }
            else
                _externalEditorExecutablePath = "notepad.exe";

            if (ConfigurationManager.AppSettings["ExternalEditorArguments"] != null)
            {
                _externalEditorArguments = ConfigurationManager.AppSettings["ExternalEditorArguments"].ToString();
                if (string.IsNullOrWhiteSpace(_externalEditorArguments))
                    _externalEditorArguments = null;
            }           


            _visualTimeFormat = (string)Utils.GetConfigValue<string>("VisualTimeFormat");
            _visualTimeFormat = string.IsNullOrWhiteSpace(_visualTimeFormat)
                ? "HH:mm:ss.FFF"
                : _visualTimeFormat;

            _visualDateTimeFormat = (string)Utils.GetConfigValue<string>("_visualDateTimeFormat");
            _visualDateTimeFormat = string.IsNullOrWhiteSpace(_visualDateTimeFormat)
                ? "{0:dd/MM/yyyy-HH:mm:ss.FFF}"
                : _visualDateTimeFormat;

            TryImportProfile();

            mnuItemClearCache.Enabled = _cacheEnabled;
            mnuItemLoad.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);
            mnuItemEditCurrentProfile.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);
            mnuItemGoToLine.Enabled = !string.IsNullOrEmpty(_loadedLogFileName);

            btnViewLoadedLog.Enabled = false;
            btnViewAppLog.Enabled = false;            
            progressBar.Visible = false;            

            txtHeader.Text = string.Empty;

            _ttFilterInfo = new ToolTip()
            {
                AutoPopDelay = 32767,//5000;
                InitialDelay = 1000,
                ReshowDelay = 500,
                ToolTipTitle = "Current Filter:",
                UseFading = true,
                UseAnimation = true,
                IsBalloon = true,
                ShowAlways = true
            };

            _ttFilterInfo.SetToolTip(btnCustomFilter, _currentFilter == null ||
                string.IsNullOrWhiteSpace(_currentFilter.FilterExpression) ? "No filter. Click to set." :
                StringExt.Wrap(_currentFilter.FilterExpression, 70));
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

        private void UpdateFormTitle(bool isFromCache = false)
        {
            if (!string.IsNullOrWhiteSpace(_selectedProfileFileName))
            {                                              
                Text = string.Format("LogParser - Profile: [{0}]", Path.GetFileName(_selectedProfileFileName));
                if (!string.IsNullOrWhiteSpace(_loadedLogFileName))                  
                    Text = Text + "   " +string.Format("Log File: [{0} {1}]", Path.GetFileName(_loadedLogFileName),
                        isFromCache ? "(from cache)" : string.Empty);
                
            }
            else
            { 
                Text = "LogParser";
            }
        }
        
        private void UpdateInfoBox(StateObject stateObj)
        {          
            var ds = new List<KeyValuePair<string, string>>();


            if (stateObj != null && stateObj.ObjectClass != ObjectClass.Blank && stateObj.ObjectClass != ObjectClass.Missing)
            {
                var baseObj = stateObj.Parent;                
                var time = stateObj.Time != DateTime.MinValue ?
                                    string.Format("{0:dd/MM/yyyy-HH:mm:ss.FFF}", stateObj.Time)
                                    : null;

                //Add Dynamic properties
                foreach (var prop in baseObj.DynObjectDictionary)
                {
                    if (prop.Key.ToLower() != "state"
                            && prop.Key.ToLower() != "this"
                            && prop.Key.ToLower() != "time"
                            && !ds.Any(x => x.Key == prop.Key))
                    {
                        var val = prop.Value.ToString() + (prop.Key.ToLower() == "timetocomplete" ? " ms." : string.Empty);
                        ds.Add(new KeyValuePair<string, string>(prop.Key, val));
                    }
                }

                //Add Static properties(reflection)
                Type t = typeof(StateObject);
                var propertyInfo = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in propertyInfo.Where(prop => !ds.Any(x => x.Key == prop.Name)).Where(prop => (prop.PropertyType == typeof(string) ||
                                             prop.PropertyType == typeof(int) ||
                                             prop.PropertyType == typeof(ObjectClass))
                                             && prop.Name.ToLower() != "description"))
                {
                    var val = prop.GetValue(stateObj, null);
                    if (val != null)
                        ds.Add(new KeyValuePair<string, string>(prop.Name, val.ToString()));
                }

                ds.Sort(CompareStringValues);

                if (baseObj != null && baseObj.ColorKeys != null && baseObj.ColorKeys.Count > 0)
                    ds.Add(new KeyValuePair<string, string>("ColorKeys", string.Join(", ", baseObj.ColorKeys)));

                if (stateObj.DataBuffer != null && stateObj.DataBuffer.Length > 0)
                    ds.Add(new KeyValuePair<string, string>("DataBuffer", stateObj.DataBuffer.ToString()));
               

                if (time != null)
                    ds.Insert(0, new KeyValuePair<string, string>("Time", time));
                ds.Insert(0, new KeyValuePair<string, string>("this", baseObj.GetThis()));
                ds.Insert(0, new KeyValuePair<string, string>("State", stateObj.State.ToString()));      

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

        private static int CompareStringValues(KeyValuePair<string, string> a, KeyValuePair<string, string> b)
        {
            return a.Key.CompareTo(b.Key);   
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
                    if (ParserView.AllowedForDisplayProperties(prop.Key, ref _displayInInfoboxProps) && !ds.Any(x => x.Key == prop.Key))
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

        private void SearchInterrupted(DataGridViewCellEventArgs e)
        {
            int rowIndex = 0;

            if (!(dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewImageCell)) return;

            var clickedCell = (DataGridViewImageCell)dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (clickedCell == null || clickedCell.Tag == null || !(clickedCell.Tag is TagArrowInfo) ||
                !(clickedCell.Tag as TagArrowInfo).IsClickable) return;

            var referenceObj = (clickedCell.Tag as TagArrowInfo).refObj;
            if (referenceObj == null) return;

            StateObject referenceStateObj = null;

            if (referenceObj.PrevInterruptedObj != null)
                referenceStateObj = referenceObj.StateCollection.FirstOrDefault(x => x.ObjectClass != ObjectClass.Blank && x.ObjectClass != ObjectClass.ViewArrow);
            else if (referenceObj.NextContinuedObj != null)
                referenceStateObj = referenceObj.StateCollection.LastOrDefault(x => x.ObjectClass != ObjectClass.Blank && x.ObjectClass != ObjectClass.ViewArrow);
            else
                return;

            if (referenceStateObj == null) return;

            var colIndex = referenceObj.StateCollection.IndexOf(referenceStateObj) + 1; //+1 because the Timeline column presents

            bool isFound = false;
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    var stateObj = (cell as DataGridViewCell).Value as StateObject;
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
        }

        private bool SearchLineNumber(int lineNumber)
        {
            bool isFound = false;
            int rowIndex = 0;
            int colIndex = 0;           
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                colIndex = 0;
                foreach (var cell in row.Cells)
                {
                    var stateObj = (cell as DataGridViewCell).Value as StateObject;
                    if (stateObj != null && stateObj.LineNum == lineNumber)
                    {
                        isFound = true;
                        break;
                    }

                    colIndex++;    
                }

                if (isFound)
                    break;

                rowIndex++;
            }

            if (isFound == true)
                dataGV.CurrentCell = dataGV[colIndex, rowIndex];

            return isFound;
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
                _loadCompleted = false;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    progressBar.Value = 20;
                    CreateComboDeviceDataSource();
                    Application.DoEvents();
                    Thread.Sleep(300);

                    progressBar.Value = 60; ;
                    Application.DoEvents();
                    Thread.Sleep(300);

                    if (cmbShowDevice.Items.Count > 1)
                        cmbShowDevice.SelectedIndex = 0;
                    cmbShowDevice.Enabled = true;
                    lblShowDevice.Enabled = true;                   
                    mnuItemImportProfile.Enabled = true;
                    mnuItemLoad.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);
                    mnuItemGoToLine.Enabled = !string.IsNullOrEmpty(_loadedLogFileName);

                    btnViewLoadedLog.Enabled = true;
                    btnViewAppLog.Enabled = true;
                    btnCustomFilter.Enabled = true;
                    UpdateFormTitle();

                    progressBar.Value = 80;                    
                    Application.DoEvents();
                    Thread.Sleep(300);

                    if (_cacheEnabled)
                    {
                        _parser.SaveToCache();
                        mnuItemCacheClearCurrent.Enabled = !string.IsNullOrEmpty(_parser.CacheFilePath);
                        mnuItemCacheClearAll.Enabled = !string.IsNullOrEmpty(_parser.CacheFilePath);
                    }

                    RefreshGridView(_parser.ObjectCollection);
                    UpdateCustomFilterExists(false);
                }
                finally
                {
                    resultLabel.Text = ("Ready");
                    progressBar.Value = 100;
                    Application.DoEvents();                    
                    _parser.AppLogger.LogLoadingCompleted();
                    Thread.Sleep(500);

                    progressBar.Visible = false;
                    progressBar.Value = 0;
                    Cursor.Current = Cursors.Default;
                    _loadCompleted = true;
                    gbFilter.Enabled = true;
                    if (_parser.AppLogIsActive && _parser.AppLogger.ReportedLinesCount > 0)
                    {
                        if (MessageBox.Show(string.Format("Hi, Yuri!"
                            + Environment.NewLine + "Loading completed"
                            + Environment.NewLine + "There are {0} parsing errors/warnings reported while loading."
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
                                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor(_externalEditorExecutablePath, _parser.AppLogger.TargetPath, _externalEditorArguments);
                                }
                                catch
                                {
                                    MessageBox.Show("Hi, Yuri!" + Environment.NewLine + "Unfortunately you can't launch the external editor " +
                                        _externalEditorExecutablePath + Environment.NewLine +
                                        "Current external editor has been reset to the default (Notepad.exe)");
                                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor("notepad.exe", _parser.AppLogger.TargetPath);

                                }
                            }
                            else
                                WindowHelper.BringProcessToFront(_externalEditorProcess);
                        }
                    }
                }
            }
            else if (e.Error != null)
                MessageBox.Show(e.Error.ToString());

            btnStopLoading.Visible = false;
            closePending = false;
        }

        #endregion Background Worker


        private void RefreshGridViewAfterCacheLoaded()
        {
            if (!_cacheEnabled) return;

            CreateComboDeviceDataSource();
            Application.DoEvents();
            Thread.Sleep(300);

            progressBar.Value = 60; ;
            Application.DoEvents();
            Thread.Sleep(300);

            if (cmbShowDevice.Items.Count > 1)
                cmbShowDevice.SelectedIndex = 0;
            cmbShowDevice.Enabled = true;
            lblShowDevice.Enabled = true;
            mnuItemImportProfile.Enabled = true;
            mnuItemLoad.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);
            mnuItemGoToLine.Enabled = !string.IsNullOrEmpty(_loadedLogFileName);
            mnuItemCacheClearCurrent.Enabled = !string.IsNullOrEmpty(_parser.CacheFilePath);
            mnuItemCacheClearAll.Enabled = !string.IsNullOrEmpty(_parser.CacheFilePath);

            btnViewLoadedLog.Enabled = true;
            btnViewAppLog.Enabled = true;
            btnCustomFilter.Enabled = true;

            UpdateFormTitle(true);

            progressBar.Value = 80;
            Application.DoEvents();
            Thread.Sleep(300);

            resultLabel.Text = "Prepare data for view...";
            RefreshGridView(_parser.ObjectCollection);
            UpdateCustomFilterExists(false);
        }

        private void CloseView()
        {
            if (_parser != null)
            {
                _parser.CacheFilePath = null;
                _parser.LogFileName = null;
            }
            _loadedLogFileName = null;
            _currentDevice = null;
            if (_currentFilter != null)
                _currentFilter.Clear();

            txtHeader.Text = string.Empty;
            resultLabel.Text = string.Empty;
            dataGV.DataSource = null;
            dataGV.Rows.Clear();
            dataGV.Columns.Clear();
            dataGV.Refresh();
            mnuItemLoad.Enabled = false;
            mnuItemImportProfile.Enabled = false;
            mnuItemGoToLine.Enabled = false;
            mnuItemGoToLine.Enabled = false;
            mnuItemCacheClearCurrent.Enabled = false;
            mnuItemCacheClearAll.Enabled = false;
            calculateLabel.Text = string.Empty;
            gridLabel.Text = string.Empty;
            cmbShowDevice.Enabled = false;
            lblShowDevice.Enabled = false;
            cmbShowDevice.SelectedIndex = -1;

            btnViewLoadedLog.Enabled = false;
            btnViewAppLog.Enabled = false;
            btnClearFilter.Enabled = false;
            btnCustomFilter.Enabled = false;
            btnStopLoading.Visible = false;
            txtHeader.Text = string.Empty;
            progressBar.Value = 0;
            progressBar.Visible = false;
            UpdateFormTitle();

            mnuItemLoad.Enabled = true;
        }

        private void CreateComboDeviceDataSource()
        {
            var ds = _parser.ObjectCollection.
                                Where( o => o != null && o.ObjectClass == ObjectClass.Device).Distinct().
                                ToList();

            var comboSource = new Dictionary<string, ParserObject>();
            comboSource.Add("All devices...", null);
            foreach (var itm in ds)
            {
                var thisVal = itm.GetThis();
                if (!comboSource.ContainsKey(thisVal))
                    comboSource.Add(thisVal, itm);
            }

            if (comboSource.Count > 1)
            {
                cmbShowDevice.DataSource = new BindingSource(comboSource, null);
                cmbShowDevice.DisplayMember = "Key";
                cmbShowDevice.ValueMember = "Value";
            }
        }
                   

        private void btnStopLoading_ButtonClick(object sender, EventArgs e)
        {
            if (bkgWorkerLoad.WorkerSupportsCancellation == true)
                bkgWorkerLoad.CancelAsync();
        }

        private void cmbShowDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loadCompleted) return;

            if (cmbShowDevice.SelectedValue != null || _currentDevice != null)
            {
                if (cmbShowDevice.SelectedItem.GetType() != typeof(KeyValuePair<string, ParserObject>)) return;
                UpdateDeviceDetails();

                //Filter by selected device
                if (cmbShowDevice.SelectedIndex == 0)
                    _currentDevice = null;
                else
                    _currentDevice = ((KeyValuePair<string, ParserObject>)cmbShowDevice.SelectedItem).Key;

                gbFilter.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;                              
                try
                {
                    resultLabel.Text = "Prepare data for view...";
                    progressBar.Value = 60;
                    progressBar.Visible = true;
                    Application.DoEvents();
                    if (_currentFilter != null)
                        _currentFilter.Clear();
                    RefreshGridView(_parser.ObjectCollection);
                    UpdateCustomFilterExists(false);
                }
                catch (Exception ex)
                {
                    _parser.AppLogger.LogException(ex);
                    throw ex;
                }
                finally
                { 
                    resultLabel.Text = ("Ready");
                    progressBar.Value = 100;
                    progressBar.Visible = false;
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                    progressBar.Value = 0;
                    gridLabel.Text = string.Format("  Total view rows: {0}", dataGV.Rows.Count.ToString());
                    gbFilter.Enabled = true;        
                }
            }                       
        }

        private void UpdateDeviceDetails()
        {                                        
            if (cmbShowDevice.SelectedItem.GetType() != typeof(KeyValuePair<string, ParserObject>)) return;
            
            var obj = ((KeyValuePair<string, ParserObject>)cmbShowDevice.SelectedItem).Value;                                               
            if (obj != null)
            {
                var time = string.Format("{0:MM/dd/yyyy-HH:mm:ss.FFF}", obj.GetDynPropertyValue("Time"));
                txtHeader.Tag = obj;
                txtHeader.Text = string.Format("{0}: {1}   ID: {2} on the port {3}. Created at {4}",
                    obj.ObjectClass.ToString(),
                    obj.GetThis(),
                    (string)obj.GetDynPropertyValue("ID"),
                    (string)obj.GetDynPropertyValue("Port"),
                    time);
                
                txtHeader.BackColor = Color.Black;
            }
            else
            {
                txtHeader.Text = string.Empty;
                txtHeader.BackColor = SystemColors.Control;
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
            if (_currentFilter != null)
                _currentFilter.Clear();            
            RefreshGridView(_parser.ObjectCollection);
            UpdateCustomFilterExists(false);
        }
                     
        private void RefreshGridView(List<ParserObject> data)
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            gbFilter.Enabled = false;
            try
            {
                ParserView.CreateGridView(data, dataGV, _currentDevice);
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
            lblShowDevice.Enabled = false; 
            gbFilter.Enabled = false;

            UpdateFormTitle();
        }

        private void btnViewLoadedLog_Click(object sender, EventArgs e)
        {         
            if (_externalEditorProcess == null || _externalEditorProcess.HasExited)
            {
                string arguments = null;
                if (dataGV.SelectedCells.Count > 0)
                {
                    var selectedCell = dataGV.SelectedCells[0];                    
                    if (selectedCell != null || selectedCell.Value != null && selectedCell.Value is StateObject)
                    {
                        var selectedStateObj = selectedCell.Value as StateObject;
                        if (!string.IsNullOrEmpty(_externalEditorArguments) && selectedStateObj.LineNum > 0)
                            arguments = _externalEditorArguments + " -n" + selectedStateObj.LineNum.ToString();

                    }
                    else
                        arguments = _externalEditorArguments;

                }
                else
                       arguments = _externalEditorArguments;
                try
                {
                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor(_externalEditorExecutablePath, _loadedLogFileName, arguments);
                }
                catch
                {
                    MessageBox.Show("Hi, Youri!" + Environment.NewLine +
                        "Unfortunately you can't launch the external editor " + _externalEditorExecutablePath + Environment.NewLine +
                                        "Current external editor has been reset to the default (Notepad.exe)");
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
                MessageBox.Show("Hi, Youri!" + Environment.NewLine +
                    "The application log is not active!" +
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
                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor(_externalEditorExecutablePath, _parser.AppLogger.TargetPath, _externalEditorArguments);                   
                }
                catch
                {
                    MessageBox.Show("Hi, Youri!" + Environment.NewLine +  
                        "Unfortunately you can't launch the external editor " + _externalEditorExecutablePath + Environment.NewLine +
                                        "Current external editor has been reset to the default (Notepad.exe)");
                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor("notepad.exe", _parser.AppLogger.TargetPath);

                }
            }
            else
                WindowHelper.BringProcessToFront(_externalEditorProcess);
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


        #region DataGridView Events

        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                if ((e.Value is StateObject) && (e.Value as StateObject).ObjectClass != ObjectClass.ViewArrow)
                    e.Value = ((StateObject)e.Value).Description;

                if (e.ColumnIndex == dataGV.Columns["Timeline"].Index)
                {
                    var cell = dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    cell.ToolTipText = string.Format(_visualDateTimeFormat, cell.Tag);
                }
            }
        }

        private void dataGV_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1 || e.Value == null || !(e.Value is StateObject)) return;

            var stateObj = e.Value as StateObject;

            if (stateObj == null || stateObj.Shape != Shape.Ellipse) return;

            var currentCell = dataGV[e.ColumnIndex, e.RowIndex];

            //Darw ellipse and text
            using (Brush brush = new SolidBrush(Color.White))
            {
                using (Pen pen = new Pen(brush, 2))
                {
                    Rectangle rectDimensions = e.CellBounds;
                    rectDimensions.Width -= 12;
                    rectDimensions.Height -= 21;
                    rectDimensions.X = rectDimensions.Left + 1;
                    rectDimensions.Y = rectDimensions.Top + 1;
                    e.Graphics.DrawRectangle(pen, rectDimensions);
                    var backColor = currentCell.Selected
                        ? currentCell.Style.SelectionBackColor
                        : ColorTranslator.FromHtml(stateObj.Color);
                    using (SolidBrush fillBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillPie(fillBrush, rectDimensions, 0, 360);

                        TextFormatFlags flags = TextFormatFlags.HorizontalCenter |
                                                    TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;

                        Rectangle textDimensions = new Rectangle(rectDimensions.X + 5,
                            rectDimensions.Y + 5,
                            rectDimensions.Width - 6,
                            rectDimensions.Height - 6);

                        var desc = stateObj.Description.Split((char)13);
                        if (desc.Length > 4)
                        {//Limit number of description rows to 4 ???
                            Array.Resize(ref desc, 4);
                            var strBld = new StringBuilder();
                            foreach (var s in desc)
                                strBld.Append(s);
                            stateObj.Description = strBld.ToString();
                        }




                        TextRenderer.DrawText(e.Graphics, stateObj.Description, currentCell.Style.Font, textDimensions, Color.Black, flags);
                    }

                    e.Handled = true;
                }
            }
        }

        private void dataGV_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            //Set empty cells unselectable
            if (!_loadCompleted) return;

            if (e.Cell == null || e.StateChanged != DataGridViewElementStates.Selected)
                return;

            if (e.Cell.Value != null && (e.Cell.Value is StateObject)) return;

            e.Cell.Selected = false;
        }

        private void dataGV_SelectionChanged(object sender, EventArgs e)
        {
            if (!_loadCompleted) return;

            if (dataGV.SelectedCells.Count > 0)
            {
                var selectedCell = dataGV.SelectedCells[0];

                if (selectedCell == null || selectedCell.Value == null) return;

                if (selectedCell.Value is StateObject)
                {
                    var selectedStateObj = selectedCell.Value as StateObject;
                    UpdateInfoBox(selectedStateObj);

                }
            }
            else
            {
                UpdateInfoBox(null);
            }
        }

        private void dataGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!_loadCompleted) return;

            //Handle arrow click
            SearchInterrupted(e);
        }

        private void dataGV_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewImageCell)
            {
                var tag = dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                if (tag != null && tag is TagArrowInfo)
                { 
                    var ti = tag as TagArrowInfo;
                    if (ti.IsClickable)
                    {
                        (sender as DataGridView).Cursor = Cursors.Hand;                        
                        dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = ti.ToolTipText;                       
                    }          
                }
                return;
            }
            
            var stateObj = dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as StateObject;            

            if (stateObj != null)
                (sender as DataGridView).Cursor = Cursors.Hand;
            else
                (sender as DataGridView).Cursor = Cursors.Default;
           
        }

        private void dataGV_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            (sender as DataGridView).Cursor = Cursors.Default;
        }

        #endregion DataGridView Events

        #region txtHeader Events
        private void txtHeader_Click(object sender, EventArgs e)
        {
            if (_currentDevice == null) return;

            var txtBox = sender as TextBox;            
            dataGV.ClearSelection();                        
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
            txtHeader.Cursor = _currentDevice != null ? Cursors.Hand : Cursors.Default;
        }

        private void txtHeader_Leave(object sender, EventArgs e)
        {            
            var txtBox = sender as TextBox;            
            txtBox.BackColor = _currentDevice != null ? Color.Black : SystemColors.Control;
        }

        private void txtHeader_TextChanged(object sender, EventArgs e)
        {
            //if (_currentDevice == null) return;

            //var txtBox = sender as TextBox;
            //if (string.IsNullOrEmpty(txtBox.Text)) return;
            //Size size = TextRenderer.MeasureText(txtBox.Text, txtBox.Font);
            //txtBox.Width = size.Width;
            //txtBox.Height = size.Height;
        }

        #endregion txtHeader Events       

        #region TopMenu Events

        private void mnuItemLoad_Click(object sender, EventArgs e)
        {
            dlgLoadLog.Filter = "Log files (*.log)|*.log|All files (*.*)|*.*";
            dlgLoadLog.DefaultExt = "*.log";
            if (dlgLoadLog.ShowDialog() != DialogResult.Cancel)
            {
                _loadedLogFileName = dlgLoadLog.FileName;
                if (string.IsNullOrWhiteSpace(_loadedLogFileName) || !File.Exists(_loadedLogFileName))
                {
                    MessageBox.Show(string.Format("Hi, Youri! Unfortunately, the Log file: '{0}' does not exists!", _loadedLogFileName));
                    return;
                }

                //Get cache file path if exists
                string cacheFolder = null;
                if (ConfigurationManager.AppSettings["CachePath"] != null)
                    cacheFolder = ConfigurationManager.AppSettings["CachePath"].ToString();
                if (string.IsNullOrWhiteSpace(cacheFolder))
                    cacheFolder = Path.GetDirectoryName(Application.ExecutablePath);
                var cacheFilePath = Path.Combine(Path.GetFullPath(cacheFolder), Path.GetFileNameWithoutExtension(_loadedLogFileName)) + ".cache";

                _currentDevice = null;

                //Check for cache
                if (_cacheEnabled && File.Exists(cacheFilePath))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    btnStopLoading.Visible = false;
                    resultLabel.Text = "Loading data from cache...";
                    closePending = true;
                    _loadCompleted = false;
                    progressBar.Visible = true;
                    progressBar.Value = 10;
                    btnViewLoadedLog.Enabled = false;
                    btnViewAppLog.Enabled = false;
                    Application.DoEvents();
                    Thread.Sleep(300);
                    try
                    {
                        _parser = Parser.LoadFromCache(cacheFilePath);
                        if (_parser != null)
                        {
                            _parser.CacheFilePath = cacheFilePath;
                            _parser.InitLogger();
                            _parser.AppLogger.LogLoadingStarted(true);

                            progressBar.Value = 20;
                            Application.DoEvents();
                            Thread.Sleep(300);

                            _parser.LogFileName = _loadedLogFileName;
                            _parser.IsFromCache = true;

                            RefreshGridViewAfterCacheLoaded();
                        }

                    }
                    catch (Exception ex)
                    {
                        if (_parser != null)
                            _parser.AppLogger.LogException(ex);
                        throw ex;
                    }
                    finally
                    {
                        if (_parser != null)
                            _parser.AppLogger.LogLoadingCompleted(true);
                        resultLabel.Text = ("Ready");

                        progressBar.Value = 100;
                        Application.DoEvents();
                        Thread.Sleep(500);

                        progressBar.Visible = false;
                        progressBar.Value = 0;
                        Cursor.Current = Cursors.Default;
                        _loadCompleted = true;
                        gbFilter.Enabled = true;
                        btnViewLoadedLog.Enabled = true;
                        btnViewAppLog.Enabled = true;
                    }
                }
                else
                {
                    _parser = new Parser(_loadedLogFileName);

                    if (!bkgWorkerLoad.IsBusy)
                    {
                        txtHeader.Text = string.Empty;
                        dataGV.DataSource = null;
                        dataGV.Rows.Clear();
                        dataGV.Refresh();

                        // Start the asynchronous operation.
                        mnuItemLoad.Enabled = false;
                        mnuItemImportProfile.Enabled = false;
                        mnuItemGoToLine.Enabled = false;
                        btnStopLoading.Visible = true;
                        calculateLabel.Text = string.Empty;
                        gridLabel.Text = string.Empty;
                        cmbShowDevice.Enabled = false;
                        lblShowDevice.Enabled = false;
                        cmbShowDevice.SelectedIndex = -1;
                        bkgWorkerLoad.RunWorkerAsync();
                    }
                }
            }
        }

        private void mnuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuItemImportProfile_Click(object sender, EventArgs e)
        {
            dlgProfile.Filter = "Xml files (*.xml)|*.xml";
            dlgProfile.DefaultExt = "*.xml";
            if (dlgProfile.ShowDialog() != DialogResult.Cancel)
            {
                _selectedProfileFileName = dlgProfile.FileName;
                mnuItemLoad.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);
                mnuItemGoToLine.Enabled = !string.IsNullOrEmpty(_loadedLogFileName);
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
            mnuItemEditCurrentProfile.Enabled = !string.IsNullOrEmpty(_selectedProfileFileName);
            mnuItemGoToLine.Enabled = !string.IsNullOrEmpty(_loadedLogFileName);
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

        private void btnCustomFilter_Click(object sender, EventArgs e)
        {
            var frmFilter = new FrmFilter();           
            frmFilter.PropertyFilter = _parser.PropertyFilter;
            frmFilter.CurrentFilter = _currentFilter;
            frmFilter.CurrentDevice = _currentDevice;
            frmFilter.ShowDialog(this);
            //TestFilter();
            if (frmFilter.DialogResult == DialogResult.OK)                
            {
                resultLabel.Text = "Prepare data for view...";
                progressBar.Value = 60;
                progressBar.Visible = true;
                Application.DoEvents();
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    if (frmFilter.CurrentFilter != null &&
                        frmFilter.CurrentFilter.Definitions.Count > 0)                       
                    {                                        
                        var filteredData = DataFilterHelper.GetFilteredData(_parser.ObjectCollection, frmFilter.CurrentFilter.FilterExpression);
                        _currentFilter = frmFilter.CurrentFilter;
                        RefreshGridView(filteredData.Cast<ParserObject>().ToList());
                        UpdateCustomFilterExists(_currentFilter != null && _currentFilter.FilterExpression != null);                    
                    }
                    else
                    {
                        RefreshGridView(_parser.ObjectCollection);
                        UpdateCustomFilterExists(false);
                    }
                }
                finally
                {                    
                    resultLabel.Text = ("Ready");
                    progressBar.Value = 100;
                    progressBar.Visible = false;
                    progressBar.Value = 0;
                    Cursor.Current = Cursors.Default;
                    Application.DoEvents();
                }
            }
        }

        private void UpdateCustomFilterExists(bool isFilterExists = false)
        {
            if (isFilterExists)
            {
                btnCustomFilter.ForeColor = Color.Red;
                btnClearFilter.Enabled = true;
            }
            else
            {
                btnCustomFilter.ForeColor = Color.Black;
                btnClearFilter.Enabled = false;
            }
            _ttFilterInfo.SetToolTip(btnCustomFilter, _currentFilter == null ||
                string.IsNullOrWhiteSpace(_currentFilter.FilterExpression) ? "No filter. Click to set." :
                StringExt.Wrap(_currentFilter.FilterExpression, 70));

        }
      
        private void mnuItemEditCurrentProfile_Click(object sender, EventArgs e)
        {
            if (_externalEditorProcess == null || _externalEditorProcess.HasExited)
            {
                try
                {
                    string arguments = null;
                    if (Path.GetFileName(_externalEditorExecutablePath) == "notepad++.exe")
                        arguments = "-ro -nosession -notabbar";
                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor(_externalEditorExecutablePath, _selectedProfileFileName, arguments);
                }
                catch
                {
                    MessageBox.Show("Hi, Yuri!" + Environment.NewLine + "Unfortunately you can't launch the external editor " +
                        _externalEditorExecutablePath + Environment.NewLine +
                        "Current external editor has been reset to the default (Notepad.exe)");
                    _externalEditorProcess = WindowHelper.ViewFileInExternalEditor("notepad.exe", _selectedProfileFileName);

                }
            }
            else
                WindowHelper.BringProcessToFront(_externalEditorProcess);
        }

        private void mnuItemCacheClearCurrent_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_parser.CacheFilePath)) return;

            if (MessageBox.Show("Hi, Yuri!"
                            + Environment.NewLine                            
                            + "Are you shure you want to delete active cache of this log?",                            
                            "Clear active cache",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                
                if (!_parser.IsFromCache)
                    File.Delete(_parser.CacheFilePath);
                else
                {
                    if (MessageBox.Show("Warning!"
                            + Environment.NewLine
                            + "Data source of the current view is the same"
                            + Environment.NewLine 
                            + "cache file, you're going to delete."
                            + Environment.NewLine +"So, the view should be closed! Continue?",
                            "Clear active cache",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2) == DialogResult.Yes)

                    File.Delete(_parser.CacheFilePath);
                    CloseView();
                }                          
            }
        }

        private void mnuItemCacheClearAll_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(_parser.CacheFilePath));
            var files = di.GetFiles();

            if (files.Length == 0)
            {
                MessageBox.Show("There are no cache files found.", "Nothing to delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }        

            if (MessageBox.Show("Hi, Yuri!"
                                        + Environment.NewLine
                                        + string.Format("Are you shure you want to delete {0} cache file(s)?", files.Length),
                                        "Clear all cache files",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning,
                                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
               

                if (MessageBox.Show("Warning!"
                        + Environment.NewLine
                        + "All cache files in the pool will be deleted!"
                        + Environment.NewLine
                        + "Continue?",
                        "Clear all cache files",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                    CloseView();
                }
            }
        }

        private void mnuItemGoToLine_Click(object sender, EventArgs e)
        {
            var frmGoToLine = new FrmGoToLine();
            frmGoToLine.ShowDialog(this);
            if (frmGoToLine.DialogResult == DialogResult.OK)
            {
                if (!SearchLineNumber(frmGoToLine.SelectedLineNum))
                    MessageBox.Show(string.Format("Line number {0} is not found", frmGoToLine.SelectedLineNum), "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion TopMenu Events      
    }
}
