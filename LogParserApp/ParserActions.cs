using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using System.Xml.XPath;
using Entities;
using Helpers;
using static Entities.Enums;

namespace LogParserApp
{
    public partial class Parser
    {        
        private void DoActionNew(XElement filter, List<string> list, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (!ValidateProfileDefinition(profilePropDefinition,
                lineNumber,
                out string name,
                out PropertyDataType dataType,
                out string format))
                return;
 
            SetPropertiesByProfile(profilePropDefinition,
                patternIndex,
                parsedValue,
                objectClass,
                dataType,
                format);           
        }

        private void DoActionAssignToSelf(XElement filter, List<string> list, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      lineNumber,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;             

            _currentObj.SetDynProperty(name, parsedValue, dataType, format);
        }

        private int DoActionAssignDataBuffer(XElement filter, List<string> list, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            int skipLines = 0;
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      lineNumber,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return skipLines;

            _currentObj.SetDynProperty(name, parsedValue, dataType, format);            

            if (name.ToLower() == "buffersize")
            {
                skipLines = skipLines + BuildDataBuffer(name, patternIndex, list, lineNumber, out StringBuilder dataBuff);
                //Update DataBuffer
                var foundStateObjToBufferUpdate = _currentObj.StateCollection.LastOrDefault(
                    x => x.ObjectClass != ObjectClass.Blank && x.ObjectClass != ObjectClass.ViewArrow);

                if (foundStateObjToBufferUpdate != null)
                    foundStateObjToBufferUpdate.DataBuffer = dataBuff;
                else
                {
                    if (filter.Attribute("key") != null)                   
                        AppLogger.LogLine(string.Format("DataBuffer for filter [{0}] cannot be applied.", filter.Attribute("key").Value), lineNumber);
                }
            }
            return skipLines;
        }
        private void DoActionLocate(XElement filter, List<string> list, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      lineNumber,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            var searchValue = _currentObj.GetDynPropertyValue("this");

            _locatedObj = ObjectCollection.FirstOrDefault(x => x != null && (string)x.GetDynPropertyValue("this") == (string)searchValue);
            if (_locatedObj == null && filter.Attribute("key") != null)
                AppLogger.LogLine(string.Format("Object [{0}] for filter [{1}] cannot be located.", searchValue, filter.Attribute("key").Value), lineNumber);
        }

        private void DoActionAssign(XElement filter, List<string> list, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (_locatedObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      lineNumber,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            _locatedObj.SetDynProperty(name, parsedValue, dataType, format);
            _locatedObj = null;
        }       

        private void DoActionDrop(XElement filter, List<string> list, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      lineNumber,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;         
        }

        private void DoActionDelete(XElement filter, List<string> list, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                     lineNumber,
                     out string name,
                     out PropertyDataType dataType,
                     out string format)) return;
        }

        private bool ValidateProfileDefinition(XElement profilePropDefinition, int lineNum, out string name, out PropertyDataType dataType, out string format)
        {
            dataType = PropertyDataType.String;
            format = null;
            name = null;

            if (profilePropDefinition.Element("Name") == null)
            {
                AppLogger.LogLine("Invalid profile definition: missing element 'Name' of the property", lineNum);
                return false;
            }

            name = profilePropDefinition.Element("Name").Value;
            if (string.IsNullOrWhiteSpace(name))
            {
                AppLogger.LogLine("Invalid profile definition: element 'Name' is null or empty", lineNum);
                return false;
            }

            if (profilePropDefinition.Element("DataType") == null)
            {
                AppLogger.LogLine(string.Format("Invalid profile definition: missing element '{0}' of property '{1}'", "DataType", name), lineNum);
                return false;
            }
            dataType = profilePropDefinition.Element("DataType").Value.ToEnum<PropertyDataType>();

            format = dataType == PropertyDataType.Time ?
                profilePropDefinition.Element("DataType").Attribute("Format").Value :
                null;

            return true;
        }
        

        private void SetPropertiesByProfile(XElement profilePropDefinition, int patternIndex, object parsedValue, string name, PropertyDataType dataType, string format)
        {
            if (_currentObj == null) return;

            _currentObj.SetDynProperty("Name", name);
            _currentObj.SetDynProperty("PatternIndex", patternIndex, PropertyDataType.Decimal);
            _currentObj.SetDynProperty("DataType", dataType, PropertyDataType.Enum, format, typeof(PropertyDataType));

            if (string.Equals(name, "Time", StringComparison.InvariantCultureIgnoreCase))
                _currentObj.SetDynProperty("Time", parsedValue, PropertyDataType.Time, format);
        }      

        private bool FindOrCreateParserObject(int lineNumber, string line, XElement filter, List<object> parsedList, out string objectClass, out string thisValue, out string objectState)
        {
            thisValue = null;
            objectClass = null;
            objectState = null;
            var objState = State.Unknown;
            var objClass = ObjectClass.Unknown;
            ParserObject obj = null;
            bool isExistingFound = false;

            _lastCurrentObject = _currentObj;
            //_currentObj = null;            

            foreach (var prop in filter.XPathSelectElements("Properties/Property"))
            {
                var name = prop.Element("Name");
                if (name != null && prop.Element("Name") != null &&
                        prop.Element("Name").Value.ToLower() == "this")
                {

                    if (prop.Element("PatternIndex") == null || prop.Attribute("i") == null)
                    {
                        AppLogger.LogLine(string.Format("Invalid profile definition: missing index {0} of property '{1}'", "'i=' or 'PatternIndex'", "this"), lineNumber);
                        continue;
                    }

                    if (int.TryParse(prop.Element("PatternIndex").Value, out int patternIndex))
                    {
                        if (patternIndex < _sf.Results.Count)
                        {
                            thisValue = (string)parsedList[patternIndex];
                            var thisVal = thisValue;

                            objectState = filter.Element("State") != null &&
                            !string.IsNullOrWhiteSpace(filter.Element("State").Value)
                            ? filter.Element("State").Value : null;
                            if (!string.IsNullOrWhiteSpace(objectState))
                                objState = Enum.IsDefined(typeof(State), objectState) ? objectState.ToEnum<State>() : State.Unknown;

                            objectClass = filter.Element("ObjectClass") != null &&
                            !string.IsNullOrWhiteSpace(filter.Element("ObjectClass").Value)
                            ? filter.Element("ObjectClass").Value : null;
                            if (!string.IsNullOrWhiteSpace(objectClass))
                                objClass = Enum.IsDefined(typeof(ObjectClass), objectClass) ? objectClass.ToEnum<ObjectClass>() : ObjectClass.Unknown;

                            var filterKey = filter.Attribute("key").Value;                           
                        }
                    }
                }
            }

            if (Enum.IsDefined(typeof(ObjectClass), objClass) &&
                !string.IsNullOrWhiteSpace(thisValue))
            {
                var thisVal = thisValue;
                var foundInterruptedLastState = State.Unknown;
                ParserObject foundInterruptedObj = null;
                var foundExistingObjects = ObjectCollection.Where(x =>
                                    x.GetThis() == thisVal &&
                                    x.ObjectClass == objClass &&                                    
                                    objClass != ObjectClass.Device && //TODO ?????????
                                    x.IsFindable == true);

                isExistingFound = foundExistingObjects != null && foundExistingObjects.Count() > 0;
                if (isExistingFound)
                {
                    var isCompletedFound = foundExistingObjects.Any(x => x.StateCollection.Any(y => y.State == State.Completed));
                    if (isCompletedFound)
                    {
                        //Set isFindable = true
                        foreach (var o in foundExistingObjects)
                        {
                            o.IsFindable = false;
                        }
                        isExistingFound = false;
                    }
                    else
                    {
                        foundInterruptedObj = foundExistingObjects.LastOrDefault();
                        if (foundInterruptedObj != null)
                        {
                            var foundStateCollection = foundInterruptedObj.StateCollection;
                            if (foundStateCollection != null && foundStateCollection.Count > 0)
                             {
                                foundInterruptedLastState = foundStateCollection[foundStateCollection.Count - 1].State;
                                if (ObjectCollection[ObjectCollection.Count - 1].GetThis() != thisVal ||
                                                            objState < foundInterruptedLastState)                                  
                                    isExistingFound = false;
                            }
                        }
                    }
                }                                                               

                if (!isExistingFound)
                {
                    if (foundInterruptedObj != null )
                    {
                        obj = foundInterruptedObj.CreateObjectClone();
                        //MB
                        obj.BaseColor = foundInterruptedObj.BaseColor;
                        foundInterruptedObj.NextContinuedObj = obj;
                        obj.PrevInterruptedObj = foundInterruptedObj;                  

                        for (int i = 0; i < foundInterruptedObj.StateCollection.Count; i++)
                        {
                            obj.StateCollection.Add(obj.CreateBlankStateObject());                          
                        }
                    }
                    else
                    {
                        var filterKey = filter.Attribute("key").Value;
                        bool isVisible = true;
                        if (filter.Attribute("IsVisible") != null)
                            isVisible = filter.Attribute("IsVisible").Value.ToBoolean();
                        obj = new ParserObject(objClass);
                        obj.SetDynProperty("this", thisValue);
                        obj.SetDynProperty("FilterKey", filterKey);
                        obj.SetDynProperty("IsVisible", isVisible);
                        obj.LineNum = lineNumber;
                        obj.LogEntry = line;
                        obj.FilterKey = filterKey;                                       
                    }
                }
                else
                {                               
                    obj = foundExistingObjects.LastOrDefault();
                    isExistingFound = true;
                }
                _currentObj = obj;
                if (_currentObj == null)
                {
                    _currentObj = _lastCurrentObject;
                    isExistingFound = true;
                }
            }                            
            return isExistingFound;
        }
            

        private void SetObjectDescription(StateObject stateObj, XElement prop, object parsedValue)
        {
            if (stateObj == null) return;

            if (stateObj.State == State.Blank) return;

            XElement displayMember = prop.Element("DisplayMember");
            if (displayMember == null || displayMember.Value == null || !displayMember.Value.ToBoolean())
                return;

            if (prop.Element("DataType").Value.ToLower() == "time" && prop.Element("DataType").Attribute("Format") != null)
            {
                if (DateTime.TryParseExact((string)parsedValue, prop.Element("DataType").Attribute("Format").Value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dt))
                {
                    parsedValue = dt.ToString(_visualTimeFormat);
                    stateObj.Time = dt;
                }
            }

            var key = prop.Element("Name").Value.ToString();
            if (stateObj != null && !stateObj.VisualDescription.ContainsKey(key))
            {
                if (key.ToLower() == "timetocomplete")
                    stateObj.VisualDescription.Add(new KeyValuePair<string, string>(key, parsedValue.ToString()+ " ms."));
                else
                    stateObj.VisualDescription.Add(new KeyValuePair<string, string>(key, parsedValue.ToString()));
            }
        }

        private void AddFilterValue(string parent, XElement prop, object parsedValue)
        {              
            XElement filterMember = prop.Element("FilterMember");
            if (parent == null || filterMember == null || filterMember.Value == null || !filterMember.Value.ToBoolean())
                return;          
           
            var key = prop.Element("Name").Value.ToString();
            if (PropertyFilter.ContainsKey(key))
            {
                var values = PropertyFilter[key];
                var searchPair = new KeyValuePair<object, string>(parsedValue, parent);
                if (!values.Contains(searchPair))
                    values.Add(searchPair);                
            }
            else
            {
                var listOfValues = new List<KeyValuePair<object, string>>();
                listOfValues.Add(new KeyValuePair<object, string>(parsedValue, parent));
                PropertyFilter.Add(new KeyValuePair<string, List<KeyValuePair<object, string>>>(key, listOfValues));           
            }
        }

        private void AddColorKeyValue(ParserObject obj, XElement prop, object parsedValue)
        {
            XElement colorKeysMember = prop.Element("ColorKeysMember");
            if (obj == null ||
                colorKeysMember == null || colorKeysMember.Value == null ||
                !colorKeysMember.Value.ToBoolean())
                return;
           
            if (!obj.ColorKeys.Contains(parsedValue))       
                obj.ColorKeys.Add((string)parsedValue);
        }
    }
}
