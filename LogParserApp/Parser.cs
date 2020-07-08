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
using System.Configuration;
using System.Text;
using System.Threading;

namespace LogParserApp
{
    public partial class Parser : IDisposable
    {
        private string _logFileName;

        public ManualResetEvent Locker = new ManualResetEvent(true);

        public List<ParserObject> ObjectCollection { get; private set; }
        ParserObject _currentObj, _locatedObj;

        private ScanFormatted _sf;

        private ParserColorManager _colorMng;

        private string _visualDescriptionDateTimeFormat;

        public int TotalLogLines { get; private set; }
        public int CompletedLogLines { get; private set; }

        public ParserLogger AppLogger;

        public bool AppLogIsActive;

        public Parser(string logFileName)
        {
            if (string.IsNullOrWhiteSpace(logFileName) || !File.Exists(logFileName))
                MessageBox.Show(string.Format("Hi, Youri! Unfortunately, the Log file: '{0}' does not exists!", logFileName));
            else
            {
                _logFileName = logFileName;
                _colorMng = new ParserColorManager();
                ObjectCollection = new List<ParserObject>();
                InitLogger();              
            }

        }

        public void Run(XElement profile, BackgroundWorker worker, DoWorkEventArgs e)
        {
            AppLogger.LogStartLoadingStarted();

            int maxLoadLines = (int)Utils.GetConfigValue<int>("MaxLoadLines");
            maxLoadLines = maxLoadLines == 0 ? 50000 : maxLoadLines;
            _visualDescriptionDateTimeFormat = (string)Utils.GetConfigValue<string>("VisualDateTimeFormat");
            _visualDescriptionDateTimeFormat = string.IsNullOrWhiteSpace(_visualDescriptionDateTimeFormat)
                ? "HH:mm:ss.FFF"
                : _visualDescriptionDateTimeFormat;

            _sf = new ScanFormatted();

            ObjectCollection.Clear();
            var list = ReadLogFileToList();
            TotalLogLines = list.Count;
            CompletedLogLines = 0;
            int lineNumber = 1;
            foreach (var line in list)
            {
                Locker.WaitOne();

                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //Report progress.                    
                    int percentComplete = (int)(CompletedLogLines / (float)Math.Min(TotalLogLines, maxLoadLines) * 100);

                    worker.ReportProgress(percentComplete);
                }


                CompletedLogLines++;

                if (CompletedLogLines >= maxLoadLines) break;

                try
                {
                    ParseLogLine(list, lineNumber, line, profile);
                }
                catch (Exception ex)
                {
                    AppLogger.LogException(ex, lineNumber);   
                }

                lineNumber++;
            }
            if (!e.Cancel)
                worker.ReportProgress(100);

            AppLogger.LogLoadingCompleted();
        }

        private List<string> ReadLogFileToList()
        {
            return File.ReadLines(_logFileName).ToList();
        }

        private void ParseLogLine(List<string> list, int lineNumber, string line, XElement profile)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                AppLogger.LogLine("Log entry is empty", lineNumber);
                return;
            }

            var filters = profile.XPathSelectElements("Profile/Filters/Filter");
            foreach (var filter in filters)
            {
                var filterKey = filter.Attribute("key");
                if (filterKey != null)
                {
                    if (string.IsNullOrWhiteSpace(filterKey.Value) || line.ContainsCaseInsensitive(filterKey.Value))
                        ApplyFilter(list, lineNumber, line, filter);
                }
            }
        }

        private void ApplyFilter(List<string> list, int lineNumber, string line, XElement filter)
        {
            bool isExistingFound = false;
            bool isParsingSuccess = false;
            var invalisPatterns = new List<string>();
            var invalidPatterns = new List<string>();
            StringBuilder bufferContainer = null;
            var lastCurrentObj = _currentObj;
            var filterPatterns = filter.XPathSelectElements("Patterns/Pattern");
            string thisVal = null;
            foreach (var filterPattern in filterPatterns)
            {               

                _sf.Parse(line, filterPattern.Value);
                isParsingSuccess = IsParsingSuccessful(filterPattern);
                if (!isParsingSuccess)
                {
                    invalidPatterns.Add(filterPattern.Value);
                    //continue;
                }
                                    

                if (isParsingSuccess)
                {
                    isExistingFound = FindOrCreateParserObject(lineNumber, line, filter, _sf.Results, out string objectClass, out string thisValue, out string objectState);
                    thisVal = thisValue;
                    break;
                }
            }

            if (!isParsingSuccess)
            {
                foreach (var pattern in invalidPatterns)
                {
                    AppLogger.LogLine("Invalid pattern(s) detected: " + pattern, lineNumber);
                }               
            }
            invalidPatterns.Clear();

            if (_currentObj == null && filter.Attribute("key") != null)
            {
                AppLogger.LogLine(string.Format("Profile filter [{0}] cannot be applied.", filter.Attribute("key").Value), lineNumber);
                return;
            }



            bool isVisible = _currentObj != null && ((string)_currentObj.GetDynPropertyValue("IsVisible")).ToBoolean();
            if (true)
            {
                var properties = filter.XPathSelectElements("Properties/Property");
                properties.OrderBy(e => e.Attribute("i").Value);               
                var objectState = State.Unknown;
                var objStateStr = filter.Element("State") != null &&
                                            !string.IsNullOrWhiteSpace(filter.Element("State").Value)
                                            ? filter.Element("State").Value : null;
                if (!string.IsNullOrWhiteSpace(objStateStr))
                    objectState = Enum.IsDefined(typeof(State), objStateStr) ? objStateStr.ToEnum<State>() : State.Unknown;

                var filterKey = filter.Attribute("key").Value;
                var stateObj = _currentObj.CreateStateObject(objectState, lineNumber, line, filterKey);
                foreach (var prop in properties)
                {
                    if (prop.Element("PatternIndex") == null || prop.Attribute("i") == null) continue;

                    if (int.TryParse(prop.Element("PatternIndex").Value, out int patternIndex))
                    {
                        if (patternIndex < _sf.Results.Count)
                        {
                            DoObjectAction(filter, prop, lineNumber, patternIndex, _sf.Results[patternIndex], line, _currentObj.ObjectClass.ToString(), _currentObj.GetThis());
                            var key = prop.Element("Name").Value.ToString();

                            //SetDataBuffer(key, patternIndex, bufferContainer, list, lineNumber, lastCurrentObj);

                            SetObjectDescription(stateObj, prop, _sf.Results[patternIndex]);
                        }
                    }
                }
                if (lastCurrentObj != null)
                {
                    //stateObj.DataBuffer = lastCurrentObj.GetDynPropertyValue("DataBuffer"));
                    lastCurrentObj = null;
                }

                stateObj.Color = _colorMng.GetColorByState(_currentObj.BaseColor, stateObj.State);                             

                if (isVisible)
                    _currentObj.StateCollection.Add(stateObj);                     
            }

            if (!isExistingFound)
            {
                if (isVisible)
                {
                    _currentObj.BaseColor = _colorMng.GetNextBaseColor();
                    foreach (var stateObj in _currentObj.StateCollection)
                    {
                        if (stateObj != null)
                            stateObj.Color = _colorMng.GetColorByState(_currentObj.BaseColor, stateObj.State);
                    }
                }

                ObjectCollection.Add(_currentObj);
            }            

        } 
        
        private void SetDataBuffer(string key, int patternIndex, StringBuilder bufferContainer, List<string> list, int lineNumber, ParserObject lastCurrentObj)
        {
            if (key == "BufferSize")
            {
                if (int.TryParse(_sf.Results[patternIndex].ToString(), out int bufferLength))
                {
                    var readBufferLinesCount = bufferLength / 8 + 1;
                    bufferContainer = new StringBuilder();
                    for (int i = lineNumber - 1; i < lineNumber + readBufferLinesCount - 1; i++)
                    {
                        bufferContainer.AppendLine(list[i]);
                    }
                    lineNumber = lineNumber + readBufferLinesCount - 1;
                    if (lastCurrentObj != null && bufferContainer != null && !string.IsNullOrEmpty(bufferContainer.ToString()))
                        lastCurrentObj.SetDynProperty("DataBuffer", bufferContainer.ToString());
                }
            }
            else if (lastCurrentObj != null)
                lastCurrentObj.SetDynProperty("DataBuffer", null);
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

        private void DoObjectAction(XElement filter, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (profilePropDefinition.Element("Action") == null) return;
            var action = profilePropDefinition.Element("Action").Value.ToEnum<PropertyAction>();           

            switch (action)
            {
                case PropertyAction.New:
                    DoActionNew(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;
                case PropertyAction.AssignToSelf:
                    DoActionAssignToSelf(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;
                case PropertyAction.Locate:
                    DoActionLocate(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;
                case PropertyAction.Assign:
                    DoActionAssign(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break; 
                case PropertyAction.Drop:
                    DoActionDrop(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;
                case PropertyAction.Delete:
                    DoActionDelete(profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;

                default:
                    break;
            }
        }


        private void InitLogger()
        {
            bool appLogIsActive = true;
            if (ConfigurationManager.AppSettings["AppLogIsActive"] != null)
                appLogIsActive = ConfigurationManager.AppSettings["AppLogIsActive"].ToBoolean();

            AppLogIsActive = appLogIsActive;

            AppLogger = new ParserLogger(appLogIsActive);
            var appLogFile = ConfigurationManager.AppSettings["AppLog"].ToString();
            appLogFile = string.IsNullOrEmpty(Path.GetDirectoryName(appLogFile)) ?
                appLogFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), appLogFile) :
                appLogFile;

            if (File.Exists(appLogFile)) File.Delete(appLogFile);

            AppLogger.TargetPath = appLogFile;
            AppLogger.LoadingFilePath = _logFileName;
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