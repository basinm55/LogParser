using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
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
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {            
            _profileMng = new ProfileManager();
            _profileMng.LoadXmlFile("LogParserProfile.xml");
            _profileMng.GetProfileByName("Default");
            cmbProfile.Items.AddRange(_profileMng.GetProfileNames());

            cmbProfile.SelectedItem = "Default";
            if (cmbProfile.SelectedItem == null)
                cmbProfile.SelectedIndex = -1;
            else
                _selectedProfile = _profileMng.CurrentProfile;

            rbPort.Checked = true;
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
                
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    dataGV.DataSource = null;
                    dataGV.Rows.Clear();
                    dataGV.Refresh();

                    _parser.Run(_selectedProfile);
                    if (_parser.ObjectCollection != null && _parser.ObjectCollection.Count > 0)
                        ParserView.CreateGridView(_parser.ObjectCollection, dataGV);
                    else
                    {
                        Cursor.Current = Cursors.Default;
                        MessageBox.Show(string.Format("There is no parsing results for the file: '{0}'", loadedLogFileName));
                    }
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }

                UpdateControlsState();
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

        private void cmbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedProfile = _profileMng.GetProfileByName(cmbProfile.SelectedItem.ToString());
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
                   
                    // Here you can do whatever you want with the cell
                    dataGV.CurrentCell = clickedCell;  // Select the clicked cell, for instance

                    // Get mouse position relative to the vehicles grid
                    var relativeMousePosition = dataGV.PointToClient(Cursor.Position);

                    // Show the context menu
                    gridCmStrip.Show(dataGV, relativeMousePosition);
                }
            }
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

        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {          
            if (e.Value != null && e.Value is ParserObject)
                e.Value = ((ParserObject)e.Value).VisualDescription;
        }
    }  
}
