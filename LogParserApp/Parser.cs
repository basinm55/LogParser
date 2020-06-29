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
using System.ComponentModel;

namespace LogParserApp
{
    public partial class Parser : IDisposable
    {
        string _logFileName;
        public List<ParserObject> ObjectCollection { get; private set; }
        ParserObject _currentObj, _locatedObj;

        private ScanFormatted _sf;

        public int TotalLogLines { get; private set; }
        public int CompletedLogLines { get; private set; }

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

        public Parser(string logFileName, bool getDeviceNamesOnly)
        {
            _logFileName = logFileName;
            ObjectCollection = new List<ParserObject>();
        }


        public List<string> GetAllDeviceNames()
        {
            List<string> result  = null;
            return result;
        }

        public void Run(XElement profile, BackgroundWorker worker, DoWorkEventArgs e)
        {
            const int maxLines = 100000;

            _sf = new ScanFormatted();

            ObjectCollection.Clear();
            var list = ReadLogFileToList();
            TotalLogLines = list.Count;
            CompletedLogLines = 0;
            int lineNumber = 0;
            foreach (var line in list)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.                    
                    int percentComplete = (int)(CompletedLogLines / (float)TotalLogLines * 100);

                    worker.ReportProgress(percentComplete);
                }


                CompletedLogLines++;

                if (CompletedLogLines >= maxLines) break;

                try
                {
                    ParseLogLine(lineNumber, line, profile);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Parsing error on the line number: {0}" +
                                                       Environment.NewLine +
                                                       "{1}" + Environment.NewLine +
                                                       "Exception Details:" + Environment.NewLine + "{2}",
                                                    lineNumber.ToString(),
                                                    line,
                                                    ex.ToString()));
                }

                lineNumber++;
            }
            if (!e.Cancel)
                worker.ReportProgress(100);
        }

        private List<string> ReadLogFileToList()
        {
            return File.ReadLines(_logFileName).ToList();
        }

        private void ParseLogLine(int lineNumber, string line, XElement profile)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            var filters = profile.XPathSelectElements("Filters/Filter");
            foreach (var filter in filters)
            {
                var filterKey = filter.Attribute("key");
                if (filterKey != null)
                {
                    if (string.IsNullOrWhiteSpace(filterKey.Value) || line.ContainsCaseInsensitive(filterKey.Value))
                        ApplyFilter(lineNumber, line, filter);
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

        private void ApplyFilter(int lineNumber, string line, XElement filter)
        {
            bool isExistingFound = false;
            ParserObject foundPrevStateObj = null;
            var filterPatterns = filter.XPathSelectElements("Patterns/Pattern");
            foreach (var filterPattern in filterPatterns)
            {
                _sf.Parse(line, filterPattern.Value);
                if (!IsParsingSuccessful(filterPattern))
                    continue;         

                isExistingFound = FindOrCreateBaseObject(lineNumber, filter, _sf.Results, out string objectType, out string thisValue, out string objectState, out ParserObject foundPrevStateObject);
                foundPrevStateObj = foundPrevStateObject;

                var properties = filter.XPathSelectElements("Properties/Property");
                properties.OrderBy(e => e.Attribute("i").Value);

                foreach (var prop in properties)
                {
                    if (prop.Element("PatternIndex") == null || prop.Attribute("i") == null) continue;

                    if (int.TryParse(prop.Element("PatternIndex").Value, out int patternIndex))
                    {
                        if (patternIndex < _sf.Results.Count)
                        {                           
                            DoObjectAction(prop, lineNumber, patternIndex, _sf.Results[patternIndex], line, objectType, thisValue);
                        }
                    }
                }
            }
            if (((string)_currentObj.GetDynPropertyValue("IsVisible")).ToBoolean())
            {
                var objectState = ObjectState.Unknown;
                var objStateStr = filter.Element("State") != null &&
                                            !string.IsNullOrWhiteSpace(filter.Element("State").Value)
                                            ? filter.Element("State").Value : null;
                if (!string.IsNullOrWhiteSpace(objStateStr))
                    objectState = Enum.IsDefined(typeof(ObjectState), objStateStr) ? objStateStr.ToEnum<ObjectState>() : ObjectState.Unknown;

                var visualObj = _currentObj.CreateVisualObject(objectState, lineNumber, line);

                if (foundPrevStateObj != null)
                {
                    for (int i = 0; i < foundPrevStateObj.VisualObjectCollection.Count; i++)
                        _currentObj.VisualObjectCollection.Add(null);
                }
                _currentObj.VisualObjectCollection.Add(visualObj);
                _currentObj.ObjectState = visualObj.ObjectState;
            }

            if (!isExistingFound)
                ObjectCollection.Add(_currentObj);

            //if (_currentObj != null)
            //{                
            //    if (_currentObj.GetDynPropertyValue("IsVisible").ToString().ToBoolean()) 
            //    {
            //        //var prevObjectsFound = ObjectCollection.Where(x => (string)x.GetDynPropertyValue("this") == (string)_currentObj.GetDynPropertyValue("this").ToString());                    
            //        if (true)
            //        {
            //            var visualObject = _currentObj.CreateVisualObject();                        

            //            _currentObj.VisualObjectCollection.Add(visualObject);
            //            //_currentObj.SetState(visualObject.GetState());
            //            var found = ObjectCollection.LastOrDefault(x =>
            //                                        x.GetThis() == _currentObj.GetThis() &&
            //                                        x.ObjectType == _currentObj.ObjectType &&
            //                                        x.LineNum < lineNumber &&
            //                                        x.GetState() == _currentObj.GetState() - 1);



            //            var prevItm = _currentObj.VisualObjectCollection.GetPrevtem(visualObject);

            //            var prevState = prevItm.GetState();
            //            if (lineNumber == 8) //(itmState > prevState + 1)
            //            {
            //                var newObj = visualObject.CreateObjectClone();
            //                var newVo = newObj.CreateVisualObject();                            
            //                var foundInterrupted = ObjectCollection.LastOrDefault(x =>
            //                    x.GetThis() == newObj.GetThis() && x.LineNum < lineNumber);

            //                if (foundInterrupted != null)
            //                {
            //                    for (int i = 0; i < foundInterrupted.VisualObjectCollection.Count; i++)
            //                        newObj.VisualObjectCollection.Add(null);                              
            //                }                                                        
            //                newObj.VisualObjectCollection.Add(newVo);

            //                ObjectCollection.Add(newObj);
            //                _currentObj.VisualObjectCollection.Remove(visualObject);

            //            }

            //        }
            //        else
            //        {
            //            var obj = _currentObj.CreateObjectClone();
            //            var visualObject = _currentObj.CreateVisualObject();                       
            //            _currentObj.VisualObjectCollection.Add(visualObject);
            //        }
            //    }
            //}

        }   

        private bool IsParsingSuccessful(XElement filterPattern)
        {
            if (_sf.Results.Count == 0)
                return false;

            var percentCount = filterPattern.Value.Count(c => c == '%');
            var droppedPercentCount = filterPattern.Value.Split(new string[] { "%*" }, StringSplitOptions.None).Length - 1;
            if (_sf.Results.Count != percentCount - droppedPercentCount)
                return false;

            return true;
        }

        private void DoObjectAction(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectType, string thisValue)
        {
            if (profilePropDefinition.Element("Action") == null) return;
            var action = profilePropDefinition.Element("Action").Value.ToEnum<PropertyAction>();           

            switch (action)
            {
                case PropertyAction.New:
                    DoActionNew(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectType, thisValue);
                    break;
                case PropertyAction.AssignToSelf:
                    DoActionAssignToSelf(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectType, thisValue);
                    break;
                case PropertyAction.Locate:
                    DoActionLocate(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectType, thisValue);
                    break;
                case PropertyAction.Assign:
                    DoActionAssign(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectType, thisValue);
                    break;
                case PropertyAction.Drop:
                    DoActionDrop(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectType, thisValue);
                    break;
                case PropertyAction.Delete:
                    DoActionDelete(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectType, thisValue);
                    break;

                default:
                    break;
            }
        }

        public void Dispose()
        {
            _logFileName = null;
            ObjectCollection.Clear();
            //ParserObject _currentObj = null, _locatedObj = null;
            //ScanFormatted _sf = null;
            TotalLogLines = 0;
        }
    }
}