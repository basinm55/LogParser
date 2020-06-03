using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LogParserApp
{
    public class ProfileManager
    {

        public XElement ProfileCollection { get; private set; }

        public XElement CurrentProfile { get; private set; }
        public void LoadXmlFile(string fileName)
        {            
            ProfileCollection = XElement.Load(fileName);           
        }

        public XElement GetProfileByName(string profileName)
        {
            IEnumerable<XElement> profiles =
                    from elm in ProfileCollection.Elements("Profile")
                    where (string)elm.Attribute("Name") == profileName
                    select elm;

            CurrentProfile = profiles.FirstOrDefault();

            return CurrentProfile;
        }

        public object[] GetProfileNames()
        {          
            return ProfileCollection.Descendants().Attributes()
            .Where(attr => attr.Name.LocalName=="Name")
            .Select(attr => attr.Value).ToArray();
        }
    }
}
