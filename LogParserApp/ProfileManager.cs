using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
     
    }
}
