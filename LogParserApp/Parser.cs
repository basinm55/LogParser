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
using static Entities.ParserObject;
using System.Drawing;

namespace LogParserApp
{
    public partial class Parser
    {                         
        string _logFileName;
        public List<ParserObject> ObjectCollection { get; private set; }
        ParserObject _currentObj, _locatedObj;

        private ScanFormatted _sf;
        public Parser(string logFileName)
        {
            if (string.IsNullOrWhiteSpace(logFileName) || !File.Exists(logFileName))
                MessageBox.Show(string.Format("Log file: '{0}' does not exists!", logFileName));
            else
            {
                _logFileName = logFileName;
                ObjectCollection = new List<ParserObject>();
            }

        }

        public void Run(XElement profile, ToolStripStatusLabel toolStripStatusLabel)
        {
            int maxLines = 100000;
            _sf = new ScanFormatted();

            ObjectCollection.Clear();
            var list = ReadLogFileToList();
            int i = 0;
            foreach (var line in list)
            {
                try
                {
                    i++;
                    toolStripStatusLabel.Text = i.ToString();
                    //Application.DoEvents();
                    if (i > maxLines) break;

                    ParseLogLine(line, profile);      
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
            var filters = profile.XPathSelectElements("Filters/Filter");
            foreach (var filter in filters)
            {
                var filterKey = filter.Attribute("key");               
                if (filterKey != null)
                {
                    if (string.IsNullOrWhiteSpace(filterKey.Value) || line.ContainsCaseInsensitive(filterKey.Value))
                        ApplyFilter(line, filter);
                }
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

        private void ApplyFilter(string line, XElement filter)
        {
            var state = _currentObj != null ? (string)_currentObj.GetDynPropertyValue("State") : null;

            var filterPatterns = filter.XPathSelectElements("Patterns/Pattern");
            foreach (var filterPattern in filterPatterns)
            {
                _sf.Parse(line, filterPattern.Value);
                if (!IsParsingSuccessful(filterPattern))
                    continue;           

                foreach (var prop in filter.XPathSelectElements("Properties/Property"))
                {
                    if (prop.Element("PatternIndex") == null || prop.Attribute("i") == null) continue;

                    if (Int32.TryParse(prop.Attribute("i").Value, out int sequenceNum) &&
                        Int32.TryParse(prop.Element("PatternIndex").Value, out int patternIndex))
                    {
                        if (patternIndex < _sf.Results.Count)
                            DoObjectAction(prop, sequenceNum, patternIndex, _sf.Results[patternIndex], filter, line);
                    }
                }
            }

            if (_currentObj != null)
            {               
                var newState = (string)_currentObj.GetDynPropertyValue("State");

                if (newState != null &&
                _currentObj.GetDynPropertyValue("IsVisible").ToString().ToBoolean() &&
                !newState.Equals(state, StringComparison.InvariantCultureIgnoreCase))
                {
                    var visualObject = _currentObj.CreateVisualObject();
                    visualObject.LogLine = line;
                    _currentObj.VisualObjectCollection.Add(visualObject);
                }
            }

        }

        private bool IsParsingSuccessful(XElement filterPattern)
        {
            if (_sf.Results.Count == 0)
              return false;

            var percentCount = filterPattern.Value.Count(c => c == '%');
            var droppedPercentCount = filterPattern.Value.Split(new string[] {"%*"}, StringSplitOptions.None).Length - 1;
            if (_sf.Results.Count != percentCount - droppedPercentCount)
                return false;

            return true;
        }

        private void DoObjectAction(XElement profilePropDefinition, int sequenceNum, int patternIndex, object parsedValue, XElement filter, string logLine)
        {
            if (profilePropDefinition.Element("Action") == null) return;
            var action = profilePropDefinition.Element("Action").Value.ToEnum<PropertyAction>();   

            switch (action)
            {
                case PropertyAction.New:
                    DoActionNew(filter, profilePropDefinition, sequenceNum, patternIndex, parsedValue, logLine);
                    break;
                case PropertyAction.AssignToSelf:
                    DoActionAssignToSelf(filter, profilePropDefinition, sequenceNum, patternIndex, parsedValue, logLine);
                    break;
                case PropertyAction.Locate:
                    DoActionLocate(filter, profilePropDefinition, sequenceNum, patternIndex, parsedValue, logLine);
                    break;
                case PropertyAction.Assign:
                    DoActionAssign(filter, profilePropDefinition, sequenceNum, patternIndex, parsedValue, logLine);
                    break;
                case PropertyAction.Drop:
                    DoActionDrop(filter, profilePropDefinition, sequenceNum, patternIndex, parsedValue, logLine);
                    break;
                case PropertyAction.Delete:
                    DoActionDelete(filter, profilePropDefinition, sequenceNum, patternIndex, parsedValue, logLine);
                    break;

                default:                   
                    break;
            }      
        }
    }
}