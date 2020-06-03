using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogParserApp
{
    public class Profile
    {
        public string Font { get; set; }
        public string ForeColor { get; set; }
        public string BackColor { get; set; }
        public string TextFormat { get; set; }
        public string DateTimeFormat { get; set; }

    }

    public class ProfileManager
    {
        Configuration _config;
        public ProfileManager()
        {
            _config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
        }

        public List<string> GetProfileNames()
        {                 
            var groups = _config.SectionGroups["profileSettingsGroup"];
            if (groups == null) return null;

            var result = new List<string>();

            var profileSections = groups.Sections;
            if (profileSections != null && profileSections.Count > 0)
            {                
                foreach (var sect in profileSections)
                    result.Add(((ConfigurationSection)sect).SectionInformation.Name);
            }
 
            return result;
        }

        public NameValueCollection GetProfileByName(string profileName)
        {           
            return (NameValueCollection)ConfigurationManager.GetSection(string.Format("profileSettingsGroup/{0}", profileName));    
        }
    }
}
