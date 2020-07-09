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
        private void DoActionNew(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
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

        private void DoActionAssignToSelf(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      lineNumber,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;             

            _currentObj.SetDynProperty(name, parsedValue, dataType, format);

        }
        private void DoActionLocate(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      lineNumber,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            var searchValue = _currentObj.GetDynPropertyValue("this");

            _locatedObj = ObjectCollection.FirstOrDefault(x => x != null && (string)x.GetDynPropertyValue("this") == (string)searchValue);
        }

        private void DoActionAssign(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (_locatedObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      lineNumber,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            _locatedObj.SetDynProperty(objectClass, parsedValue, dataType, format);
            _locatedObj = null;
        }

        private void DoActionAssignDataBuffer(List<string> list, StateObject lastStatelObj, XElement filter, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (!ValidateProfileDefinition(profilePropDefinition,
                lineNumber,
                out string name,
                out PropertyDataType dataType,
                out string format)) return;

            SetPropertiesByProfile(profilePropDefinition,
                patternIndex,
                parsedValue,
                objectClass,
                dataType,
                format);

            XElement displayMember = profilePropDefinition.Element("DisplayMember");
            if (displayMember == null || displayMember.Value == null || !displayMember.Value.ToBoolean()) return;
          
            var key = profilePropDefinition.Element("Name").Value.ToString();
            if (!lastStatelObj.VisualDescription.ContainsKey(key))
                lastStatelObj.VisualDescription.Add(new KeyValuePair<string, string>(key, parsedValue.ToString()));  
                
        }

        private void DoActionDrop(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      lineNumber,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;         
        }

        private void DoActionDelete(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectClass, string thisValue)
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
                AppLogger.LogLine("Invalid profile definition: missing element 'Name' of property", lineNum);
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
           
            _currentObj = null;

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
                            if  (!string.IsNullOrWhiteSpace(objectState))
                                objState = Enum.IsDefined(typeof(State), objectState) ? objectState.ToEnum<State>() : State.Unknown;

                            objectClass = filter.Element("ObjectClass") != null &&
                            !string.IsNullOrWhiteSpace(filter.Element("ObjectClass").Value)
                            ? filter.Element("ObjectClass").Value : null;
                            if (!string.IsNullOrWhiteSpace(objectClass))
                                objClass = Enum.IsDefined(typeof(ObjectClass), objectClass) ? objectClass.ToEnum<ObjectClass>() : ObjectClass.Unknown;                            

                            var filterKey = filter.Attribute("key").Value;

                            bool isVisible = true;
                            if (filter.Attribute("IsVisible") != null)
                                isVisible = filter.Attribute("IsVisible").Value.ToBoolean();                           

                            if (Enum.IsDefined(typeof(ObjectClass), objClass) &&
                                !string.IsNullOrWhiteSpace(thisValue))
                            {
                                var foundLastState = State.Unknown;
                                var foundExistingObject = ObjectCollection.LastOrDefault(x =>
                                                    x.GetThis() == thisVal &&
                                                    x.ObjectClass == objClass &&
                                                    x.IsFindable == true);

                                isExistingFound = foundExistingObject != null;

                                ParserObject foundInterruptedObj = null; 

                                if (isExistingFound)
                                {
                                    //Check is new row needed                                    
                                    var foundStateCollection = foundExistingObject.StateCollection;
                                    if (foundStateCollection != null && foundStateCollection.Count > 0)
                                    {                                        
                                        if (ObjectCollection[ObjectCollection.Count - 1].GetThis() != thisVal)
                                        {
                                            foundInterruptedObj = foundExistingObject;
                                            foundLastState = foundStateCollection[foundStateCollection.Count - 1].State;                                            
                                            isExistingFound = false;
                                        }

                                        //&& foundStateCollection[foundStateCollection.Count - 1].State != State.Completed)


                                    }                                  
                                }

                                if (!isExistingFound)
                                {
                                    if (foundInterruptedObj != null)
                                    {
                                        obj = foundInterruptedObj.CreateObjectClone();
                                        obj.BaseColor = foundInterruptedObj.BaseColor;
                                        obj.PrevInterruptedObj = foundInterruptedObj;
                                        for (int i = 0; i < foundInterruptedObj.StateCollection.Count; i++)
                                        {
                                            obj.StateCollection.Add(obj.CreateEmptyStateObject());                                            
                                        }
                                    }
                                    else
                                    {
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
                                    obj = foundExistingObject;
                                    isExistingFound = true;
                                }
                                _currentObj = obj;
                            }                            
                            return isExistingFound;
                        }
                    }                    
                }

            }            
            return isExistingFound;
        }

        private void SetObjectDescription(StateObject stateObj, XElement prop, object parsedValue)
        {
            if (stateObj.State == State.Empty) return;

            XElement displayMember = prop.Element("DisplayMember");
            if (displayMember == null || displayMember.Value == null || !displayMember.Value.ToBoolean())
                return;

            if (prop.Element("DataType").Value.ToLower() == "time" && prop.Element("DataType").Attribute("Format") != null)
            {
                if (DateTime.TryParseExact((string)parsedValue, prop.Element("DataType").Attribute("Format").Value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dt))
                {
                    parsedValue = dt.ToString(_visualDescriptionDateTimeFormat);
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
    }
}
