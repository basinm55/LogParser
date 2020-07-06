using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LogParserApp
{
    public class ProfileManager
    {
        public string ProfilePath;       

        public XElement CurrentProfile { get; private set; }
        public void LoadXmlFile(string fileName)
        {
            ProfilePath = fileName;
            CurrentProfile = XElement.Load(ProfilePath);
        }       
     
    }
}
