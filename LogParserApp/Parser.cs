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
using Polenter.Serialization;

namespace LogParserApp
{    
    [Serializable]
    public partial class Parser : IDisposable
    {       
        public string LogFileName { get; set; }

        public string CacheFilePath { get; set; }

        public bool IsFromCache { get; set; }

        public ManualResetEvent Locker = new ManualResetEvent(true);

        public List<ParserObject> ObjectCollection { get; private set; }
        ParserObject _currentObj, _locatedObj, _lastCurrentObject;
        StateObject _lastStateObject = null;

        public IDictionary<string, List<KeyValuePair<object, string>>> PropertyFilter { get; set; }

        private ScanFormatted _sf;

        private ParserColorManager _colorMng;

        private string _visualTimeFormat;

        public int TotalLogLines { get; set; }
        public int CompletedLogLines { get; set; }

        [ExcludeFromSerializationAttribute]
        public ParserLogger AppLogger { get; set; }

        [ExcludeFromSerializationAttribute]
        public bool AppLogIsActive { get; set; }

        public Parser(string logFileName)
        {
            if (string.IsNullOrWhiteSpace(logFileName) || !File.Exists(logFileName))
                MessageBox.Show(string.Format("Hi, Youri! Unfortunately, the Log file: '{0}' does not exists!", logFileName));
            else
            {
                LogFileName = logFileName;
                _colorMng = new ParserColorManager();
                ObjectCollection = new List<ParserObject>();
                PropertyFilter = new Dictionary<string, List<KeyValuePair<object, string>>>();
                InitLogger();              
            }
        }

        public Parser()
        {                
            _colorMng = new ParserColorManager();
            ObjectCollection = new List<ParserObject>();
            PropertyFilter = new Dictionary<string, List<KeyValuePair<object, string>>>();
            InitLogger();
        }
       

        public void SaveToCache()
        {
            if (IsFromCache) return;

            string cacheFolder = null;
            if (ConfigurationManager.AppSettings["CachePoolPath"] != null)
                cacheFolder = ConfigurationManager.AppSettings["CachePoolPath"].ToString();

            if (string.IsNullOrWhiteSpace(cacheFolder))
                cacheFolder = Path.GetDirectoryName(Application.ExecutablePath);

            if (!Directory.Exists(cacheFolder))
                Directory.CreateDirectory(cacheFolder);

            CacheFilePath = Path.Combine(cacheFolder, Path.GetFileNameWithoutExtension(LogFileName)) + ".cache";

            var settings = new SharpSerializerBinarySettings(BinarySerializationMode.Burst);
            var serializer = new SharpSerializer(settings);
            

            // serialize
            using (Stream stream = new FileStream(CacheFilePath, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(this, stream);
            }                         
        }

        public static Parser LoadFromCache(string cacheFilePath)
        {
            Parser result = null;
            var settings = new SharpSerializerBinarySettings(BinarySerializationMode.Burst);
            var serializer = new SharpSerializer(settings);            
            if (File.Exists(cacheFilePath))
            { 
                using (var stream = new FileStream(cacheFilePath, FileMode.Open, FileAccess.Read))
                {
                    result = (Parser)serializer.Deserialize(stream);
                }
            }
            return result;
        }

        public void Run(XElement profile, BackgroundWorker worker, DoWorkEventArgs e)
        {
            AppLogger.LogLoadingStarted();

            int maxLoadLines = (int)Utils.GetConfigValue<int>("MaxLoadLines");
            maxLoadLines = maxLoadLines == 0 ? 50000 : maxLoadLines;
            _visualTimeFormat = (string)Utils.GetConfigValue<string>("VisualTimeFormat");
            _visualTimeFormat = string.IsNullOrWhiteSpace(_visualTimeFormat)
                ? "HH:mm:ss.FFF"
                : _visualTimeFormat;

            _sf = new ScanFormatted();

            ObjectCollection.Clear();
            foreach (var prop in PropertyFilter)
            {
                if (prop.Value != null)
                    prop.Value.Clear();
            }
            PropertyFilter.Clear();

            var list = ReadLogFileToList();
            TotalLogLines = list.Count;                       
            int skipLines;
            for (int i=0; i < list.Count; i++)
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
                    skipLines = ParseLogLine(list, i+1, list[i], profile);
                    //Skipping lines if any DataBuffer has been found
                    int j = 0;
                    while (j < skipLines - 1)
                    {
                        i++;
                        j++;
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.LogException(ex, i+1);   
                }
                
            }

            SetColors();
    
            if (!e.Cancel)
                worker.ReportProgress(100);

            AppLogger.LogLoadingCompleted();

            
        }

        private List<string> ReadLogFileToList()
        {
            return File.ReadLines(LogFileName).ToList();
        }

        private int ParseLogLine(List<string> list, int lineNumber, string line, XElement profile)
        {
            int skipLines = 0;
            if (string.IsNullOrWhiteSpace(line))
            {
                AppLogger.LogLine("Log entry is empty", lineNumber);
                return skipLines;
            }

            var filters = profile.XPathSelectElements("Profile/Filters/Filter");
            foreach (var filter in filters)
            {
                var filterKey = filter.Attribute("key");
                if (filterKey != null)
                {
                    if (string.IsNullOrWhiteSpace(filterKey.Value) || line.ContainsCaseInsensitive(filterKey.Value))
                        skipLines = skipLines + ApplyFilter(list, lineNumber, line, filter);
                }
            }
            return skipLines;
        }

        private int ApplyFilter(List<string> list, int lineNumber, string line, XElement filter)
        {
            int skipLines = 0;
            bool isExistingFound = false;
            bool isParsingSuccess = false;                                  
            ParserObject lastCurrentObj = _currentObj;
            var filterPatterns = filter.XPathSelectElements("Patterns/Pattern");
            string thisVal = null;
            foreach (var filterPattern in filterPatterns)
            {               

                _sf.Parse(line, filterPattern.Value);
                isParsingSuccess = IsParsingSuccessful(filterPattern);                                   
                if (isParsingSuccess)
                {
                    isExistingFound = FindOrCreateParserObject(lineNumber, line, filter, _sf.Results, out string objectClass, out string thisValue, out string stateStr);
                    thisVal = thisValue;
                    break;
                }
            }

            if (!isParsingSuccess)              
                AppLogger.LogLine("Unrecognized entry: " + line, lineNumber);
 

            bool isVisible = _currentObj != null && ((string)_currentObj.GetDynPropertyValue("IsVisible")).ToBoolean();

  
            var properties = filter.XPathSelectElements("Properties/Property");
            properties.OrderBy(e => e.Attribute("i").Value);               
            var objectState = State.Unknown;
            var objStateStr = filter.Element("State") != null &&
                                        !string.IsNullOrWhiteSpace(filter.Element("State").Value)
                                        ? filter.Element("State").Value : null;
            if (!string.IsNullOrWhiteSpace(objStateStr))
                objectState = Enum.IsDefined(typeof(State), objStateStr) ? objStateStr.ToEnum<State>() : State.Unknown;

            StateObject stateObj = null;
            var filterKey = filter.Attribute("key").Value;

            if (objectState != State.Unknown)
                stateObj = _currentObj.CreateStateObject(objectState, lineNumber, line, filterKey);
            else
                stateObj = _currentObj.CreateStateObject(State.Temporary, lineNumber, line, filterKey);

            _lastStateObject = stateObj;

            foreach (var prop in properties)
            {
                if (prop.Element("PatternIndex") == null || prop.Attribute("i") == null) continue;

                if (int.TryParse(prop.Element("PatternIndex").Value, out int patternIndex))
                {
                    if (patternIndex < _sf.Results.Count)
                    {
                        skipLines = skipLines + DoObjectAction(filter, list, prop, lineNumber, patternIndex, _sf.Results[patternIndex], line, _currentObj.ObjectClass.ToString(), _currentObj.GetThis());
                        SetObjectDescription(stateObj, prop, _sf.Results[patternIndex]);
                        AddFilterValue(_currentObj.GetParent(), prop, _sf.Results[patternIndex]);
                        AddColorKeyValue(_currentObj, prop, _sf.Results[patternIndex]);
                    }
                }
            }               

            if (isVisible)
            {
                if (stateObj.State > State.Created && _currentObj.PrevInterruptedObj != null)
                {                        
                    _currentObj.StateCollection.RemoveAt(_currentObj.StateCollection.Count - 1);
                    _currentObj.StateCollection.Add(_currentObj.CreateArrowStateObject(_currentObj.PrevInterruptedObj));
                }

                if (stateObj.State != State.Temporary)
                {                    
                    if (_currentObj.PrevInterruptedObj != null)
                    {
                        var lastInterruptedState = _currentObj.PrevInterruptedObj.StateCollection
                            .LastOrDefault(x => x.State >= 0).State;

                        State lastCurrentState;
                        var lastCurrentStateObj = _currentObj.StateCollection.LastOrDefault(x => x.State >= 0);
                        lastCurrentState = lastCurrentStateObj == null
                            ? lastInterruptedState
                            : lastCurrentStateObj.State;
                        
                        var diff = (int)stateObj.State - (int)lastCurrentState - 1;

                        //Add missing state objects
                        for (int i = 0; i < diff; i++)
                        {
                            _currentObj.StateCollection.Add(_currentObj.CreateMissingStateObject());
                            _currentObj.StateCollection.Add(_currentObj.CreateArrowStateObject());
                        }
                    }
                                      
                    _currentObj.StateCollection.Add(stateObj);

                    if (stateObj.State < State.Completed)
                    {
                        _currentObj.StateCollection.Add(_currentObj.CreateArrowStateObject(_currentObj.NextContinuedObj));
                    }
                }          
            }

            if (!isExistingFound && stateObj.State != State.Temporary)
                ObjectCollection.Add(_currentObj);

            return skipLines;
        } 
        
        private int BuildDataBuffer(string key, int patternIndex, List<string> list, int lineNumber, out StringBuilder bufferContainer)
        {
            int skipLines = 0;
            bufferContainer = null;
            if (key == "BufferSize")
            {
                if (int.TryParse(_sf.Results[patternIndex].ToString(), out int bufferLength))
                {
                    var readBufferLinesCount = bufferLength / 8 + 1;
                    bufferContainer = new StringBuilder();
                    for (int i = lineNumber - 1; i < lineNumber + readBufferLinesCount - 1; i++)
                    {
                        bufferContainer.AppendLine(list[i]);
                        skipLines++;
                    }
                    lineNumber = lineNumber + readBufferLinesCount - 1;                    
                }
            }            
            return skipLines;
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

        private int DoObjectAction(XElement filter, List<string> list, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            int skipLines = 0;
            if (profilePropDefinition.Element("Action") == null) return skipLines;
            var action = profilePropDefinition.Element("Action").Value.ToEnum<PropertyAction>();           

            switch (action)
            {
                case PropertyAction.New:
                    DoActionNew(filter, list, profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;
                case PropertyAction.AssignToSelf:
                    DoActionAssignToSelf(filter, list, profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;
                case PropertyAction.AssignDataBuffer:
                    skipLines = skipLines + DoActionAssignDataBuffer(filter, list, profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;
                case PropertyAction.Locate:
                    DoActionLocate(filter, list, profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;
                case PropertyAction.Assign:
                    DoActionAssign(filter, list, profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break; 
                case PropertyAction.Drop:
                    DoActionDrop(filter, list, profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;
                case PropertyAction.Delete:
                    DoActionDelete(filter, list, profilePropDefinition, lineNumber, patternIndex, parsedValue, logLine, objectClass, thisValue);
                    break;

                default:
                    break;
            }
            return skipLines;
        }


        public void InitLogger()
        {
            bool appLogIsActive = true;
            if (ConfigurationManager.AppSettings["AppLogIsActive"] != null)
                appLogIsActive = ConfigurationManager.AppSettings["AppLogIsActive"].ToBoolean();

            AppLogIsActive = appLogIsActive;

            if (AppLogger == null)
                AppLogger = new ParserLogger(appLogIsActive);

            if (ConfigurationManager.AppSettings["AppLog"] != null)
            {
                var appLogFile = ConfigurationManager.AppSettings["AppLog"].ToString();
                appLogFile = string.IsNullOrEmpty(Path.GetDirectoryName(appLogFile)) ?
                    appLogFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), appLogFile) :
                    appLogFile;

                if (File.Exists(appLogFile)) File.Delete(appLogFile);

                AppLogger.TargetPath = appLogFile;
                AppLogger.LoadingFilePath = LogFileName;
            }
        }

        private void SetColors()
        {
            if (ObjectCollection.Count == 0) return;

            //Fill dictionary
            var IndexToValues = new Dictionary<int, List<string>>();
            foreach (var obj in ObjectCollection)
            {
                var colorKeys = string.Join("_", obj.ColorKeys);
                IndexToValues[ObjectCollection.IndexOf(obj)] = new List<string>();
                IndexToValues[ObjectCollection.IndexOf(obj)].Add(colorKeys);
            }
            //Reverse dictionary
            var valueToIndexes = IndexToValues.SelectMany(
                    pair => pair.Value.Select(val => new { Key = val, Value = pair.Key }))
                .GroupBy(item => item.Key)
                .ToDictionary(gr => gr.Key, gr => gr.Select(item => item.Value).ToList());

            //Create Color Table
            var colorTable = new List<KeyValuePair<Color, List<int>>>();
            foreach (var val in valueToIndexes)
            {
                colorTable.Add(new KeyValuePair<Color, List<int>>(_colorMng.GetNextBaseColor(), val.Value));
            }

            //Assign colors to objects
            foreach (var obj in ObjectCollection)
            {
                var idx = ObjectCollection.IndexOf(obj);
                var colorItem = colorTable.FirstOrDefault(x => x.Value != null && x.Value.Contains(idx));
                if (obj.PrevInterruptedObj != null)
                    obj.BaseColor = obj.PrevInterruptedObj.BaseColor;
                else
                    obj.BaseColor = ColorTranslator.ToHtml(colorItem.Key);

                foreach (var stateObj in obj.StateCollection)
                {
                    if (stateObj.ObjectClass != ObjectClass.Blank &&
                            stateObj.ObjectClass != ObjectClass.Missing &&
                            stateObj.ObjectClass != ObjectClass.ViewArrow)                      
                        stateObj.Color = ColorTranslator.ToHtml(_colorMng.GetColorByState(ColorTranslator.FromHtml(obj.BaseColor), stateObj.State));
                }
            }


        }
            
        public void Dispose()
        {
            LogFileName = null;
            ObjectCollection.Clear();         
            TotalLogLines = 0;
        }
    }
}