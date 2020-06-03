using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogParserApp
{
    public partial class FrmMain : Form
    {
        private Parser _parser;
        private ProfileManager _profileMng;
        private NameValueCollection _selectedProfile;

        public FrmMain()
        {           
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {            
            _profileMng = new ProfileManager();
            
            cmbProfile.Items.AddRange(_profileMng.GetProfileNames().ToArray());
            cmbProfile.SelectedItem = "default";
            if (cmbProfile.SelectedItem == null)
                cmbProfile.SelectedIndex = -1;
            else
                _selectedProfile = _profileMng.GetProfileByName("default");

            rbPort.Checked = true;
            UpdateControlsState();

            CreateGridView();
        }

        public string loadedLogFileName;

        private void CreateGridView()
        {
            var data = new List<MyStruct>() { new MyStruct("a", "b"), new MyStruct("c", "d") };     

            var source = new BindingSource();
            source.DataSource = data;
            dataGV.AutoGenerateColumns = true;
            dataGV.DataSource = source;

            foreach (DataGridViewColumn col in dataGV.Columns)
            {
                col.DividerWidth = 10;
            }
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                row.DividerHeight = 10;
            }


            dataGV.Rows[1].Cells[1].Style.BackColor = Color.Aquamarine;
            dataGV.Rows[1].Cells[0].Style.BackColor = Color.Aquamarine;
        }

        private void btnLoadLog_Click(object sender, EventArgs e)
        {   
            dlgLoadLog.Filter = "Log files (*.log)|*.log|All files (*.*)|*.*";
            dlgLoadLog.DefaultExt = "*.log";
            if (dlgLoadLog.ShowDialog() != DialogResult.Cancel)
            {
                loadedLogFileName = dlgLoadLog.FileName;
                _parser = new Parser(loadedLogFileName);
                _parser.Run(_selectedProfile);
            }

            UpdateControlsState();

        }


        private void GetProfiles()
        {
            var config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            var x = config.SectionGroups["profileSettingsGroup"];
            //NameValueCollection test = (NameValueCollection)ConfigurationManager.GetSection("profileSettingsGroup/default");
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
    }

    class MyStruct
    {
        public string Name { get; set; }
        public string Address { get; set; }


        public MyStruct(string name, string adress)
        {
            Name = name;
            Address = adress;
        }
    }
}
