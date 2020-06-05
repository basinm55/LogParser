using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Windows.Forms;
using Helpers;
using Entities;
using static Entities.Enums;

namespace LogParserApp
{
    public partial class Parser
    {                         
        string _logFileName;
        //List<LogEntry> _logEntries;

        private ScanFormatted _sf;
        public Parser(string logFileName)
        {
            if (string.IsNullOrWhiteSpace(logFileName) || !File.Exists(logFileName))
                MessageBox.Show(string.Format("Log file: '{0}' does not exists!", logFileName));
            else
            {
                _logFileName = logFileName;
                //_logEntries = new List<LogEntry>();
            }

        }

        public void Run(XElement profile)
        {
            //Example:  
            //dynamic person = new ExpandoObject();
            //var dictionary = (IDictionary<string, object>)person;
            //dictionary.Add("Name", "Filip");
            //dictionary.Add("Age", 24);

            _sf = new ScanFormatted();

            //_logEntries.Clear();
            var list = ReadLogFileToList();
            foreach (var line in list)
            {
                try
                {
                    ParseLogLine(line, profile);
                    //if (logEntry != null)
                        //_logEntries.Add(logEntry);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Can't parse this line:"
                        + Environment.NewLine
                        + Environment.NewLine + line
                        + Environment.NewLine
                        + Environment.NewLine + e.ToString());
                }
            }
        }

        private List<string> ReadLogFileToList()
        {
            return File.ReadLines(_logFileName).ToList();
        }

        private void ParseLogLine(string line, XElement profile)
        {
            var linePropertyList = new List<PropertyDefinition>();

            var filters = profile.XPathSelectElements("Filters/Filter");
            foreach (var filter in filters)
            {
                var propDef = ApplyFilter(line, filter);
                if (propDef != null && propDef.Count > 0)
                    linePropertyList.AddRange(propDef);

            }

                //[0]44D8.44D0::05/31/2020-16:46:30.355
                //format "%[*][width][modifiers]type"
                //format example: "[%d]%d.%4s::%s %s %20c"
                //datetime format "MM/dd/yyyy-HH:mm:ss.FFF"

                //[6]0004.0230::05/31/2020-16:43:13.894 [jtucxip]CPort::BindUsbDevice port 4 CUsbDevice FFFFC30247334410

                //Timestamp
                //string parsingFormat = "[%*1d]%*4c.%*4c::%s";//TODO: Read it from profile            
            //Device
            //string parsingFormat = "%*s [%*7c]CUsbDevice::CUsbDevice: %s";
        

            //string parsingFormat = "CUsbipRequest::CUsbipRequest: type %s";

            //var sf = new ScanFormatted();                        
            //var x = sf.Parse(line, parsingFormat/*profile["ParsingFormat"]*/);

            //var regex = Regex.Match(line, "[0-9]{1}[0-9]{1}");

            //var logEntry = new LogEntry();

            //Convert time string to DateTime
            //if (sf.Results.Count > 3) // Because the buffer contents problem TODO
               //logEntry.Time = DateTime.ParseExact(sf.Results[3].ToString(), profile["DateTimeFormat"], null);

            //if (sf.Results.Count > 4)
                //logEntry.EntryType = sf.Results[4].ToString();


            //if (_sf.Results.Count > 5)
            //{
            //    //var descriptionList = sf.Results.GetRange(5, sf.Results.Count - 5);                
            //    //logEntry.Description = string.Join(" ", descriptionList); 
            //}

            //return logEntry;
        }        

        private List<PropertyDefinition> ApplyFilter(string line, XElement filter)
        {
            var result = new List<PropertyDefinition>();
            var filterPattern = filter.XPathSelectElement("Pattern").Value;
            var valCount = _sf.Parse(line, filterPattern);

            if (valCount == 0) return result;   
                                   
            foreach (var prop in filter.XPathSelectElements("Properties/Property"))
            {              
                if (Int32.TryParse(prop.Attribute("i").Value, out int index))
                {                    
                    if (index < valCount)
                    {
                        result.Add(new PropertyDefinition()
                        {
                            Index = index,
                            Name = prop.Element("Name").Value,
                            Action = prop.Element("Action").Value.ToEnum<PropertyAction>(),
                            Type = prop.Element("Type").Value.ToEnum<PropertyDataType>(),
                            Value = _sf.Results[index]

                        });                         
                    }
                }
            }

            return result;

        }
    }
}
