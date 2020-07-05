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
        private void DoActionNew(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectType, string thisValue)
        {
            if (!ValidateProfileDefinition(profilePropDefinition,
                out string name,
                out PropertyDataType dataType,
                out string format)) return;
 
            SetPropertiesByProfile(profilePropDefinition,
                patternIndex,
                parsedValue,
                objectType,
                dataType,
                format);           
        }

        private void DoActionAssignToSelf(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectType, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;             

            _currentObj.SetDynProperty(name, parsedValue, dataType, format);

        }
        private void DoActionLocate(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectType, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            var searchValue = _currentObj.GetDynPropertyValue("this");

            _locatedObj = ObjectCollection.FirstOrDefault(x => x != null && (string)x.GetDynPropertyValue("this") == (string)searchValue);
        }

        private void DoActionAssign(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectType, string thisValue)
        {
            if (_locatedObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            _locatedObj.SetDynProperty(objectType, parsedValue, dataType, format);
            _locatedObj = null;
        }

        private void DoActionAssignData(List<string> list, ParserObject lastVisualObj, XElement filter, XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectType, string thisValue)
        {
            if (!ValidateProfileDefinition(profilePropDefinition,
                out string name,
                out PropertyDataType dataType,
                out string format)) return;

            SetPropertiesByProfile(profilePropDefinition,
                patternIndex,
                parsedValue,
                objectType,
                dataType,
                format);

            XElement displayMember = profilePropDefinition.Element("DisplayMember");
            if (displayMember == null || displayMember.Value == null || !displayMember.Value.ToBoolean()) return;
          
            var key = profilePropDefinition.Element("Name").Value.ToString();
            if (!lastVisualObj.ObjectDescription.ContainsKey(key))
                lastVisualObj.ObjectDescription.Add(new KeyValuePair<string, string>(key, parsedValue.ToString()));  
                
        }

        private void DoActionDrop(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectType, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;         
        }

        private void DoActionDelete(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectType, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                     out string name,
                     out PropertyDataType dataType,
                     out string format)) return;
        }

        private bool ValidateProfileDefinition(XElement profilePropDefinition, out string name, out PropertyDataType dataType, out string format)
        {
            dataType = PropertyDataType.String;
            format = null;
            name = null;

            if (profilePropDefinition.Element("Name") == null) return false;

            name = profilePropDefinition.Element("Name").Value;
            if (string.IsNullOrWhiteSpace(name)) return false;

            if (profilePropDefinition.Element("DataType") == null) return false;
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

        private bool FindOrCreateBaseObject(int lineNumber, XElement filter, List<object> parsedList, out string objectType, out string thisValue, out string objectState)
        {
            thisValue = null;
            objectType = null;
            objectState = null;
            var objState = ObjectState.Unknown;
            var objType = ObjectType.Unknown;
            ParserObject obj = null;
            bool isExistingFound = false;
           
            _currentObj = null;

            foreach (var prop in filter.XPathSelectElements("Properties/Property"))
            {
                var target = prop.Element("Target");
                if (target != null && prop.Element("Target") != null &&
                        prop.Element("Target").Value.ToLower() == "this")
                {

                    if (prop.Element("PatternIndex") == null || prop.Attribute("i") == null) continue;

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
                                objState = Enum.IsDefined(typeof(ObjectState), objectState) ? objectState.ToEnum<ObjectState>() : ObjectState.Unknown;

                            objectType = filter.Element("ObjectType") != null &&
                            !string.IsNullOrWhiteSpace(filter.Element("ObjectType").Value)
                            ? filter.Element("ObjectType").Value : null;
                            if (!string.IsNullOrWhiteSpace(objectType))
                                objType = Enum.IsDefined(typeof(ObjectType), objectType) ? objectType.ToEnum<ObjectType>() :  ObjectType.Unknown;                            

                            var filterKey = filter.Attribute("key").Value;

                            bool isVisible = true;
                            if (filter.Attribute("IsVisible") != null)
                                isVisible = filter.Attribute("IsVisible").Value.ToBoolean();

                            bool isFindable = true;
                            if (filter.Attribute("IsFindable") != null)
                                isFindable = filter.Attribute("IsFindable").Value.ToBoolean();

                            if (Enum.IsDefined(typeof(ObjectType), objType) &&
                                !string.IsNullOrWhiteSpace(thisValue))
                            {
                                var foundExistingObject = ObjectCollection.LastOrDefault(x =>
                                                    x.GetThis() == thisVal &&
                                                    x.ObjectType == objType);

                                isExistingFound = foundExistingObject != null;

                                if (isExistingFound)
                                {
                                    var foundVoCollection = foundExistingObject.VisualObjectCollection;
                                    if (foundVoCollection != null && foundVoCollection.Count > 0
                                        && foundVoCollection[foundVoCollection.Count - 1].ObjectState == ObjectState.Completed)

                                        isExistingFound = false;
                                    else if (foundVoCollection != null && foundVoCollection.Count > 0
                                        && foundVoCollection[foundVoCollection.Count - 1].ObjectState + 1 != objState)
                                    {
                                        int droppedStatesCount = objState - foundVoCollection[foundVoCollection.Count - 1].ObjectState;
                                        for (int i=0; i< droppedStatesCount; i++)
                                        {
                                            var vo = new ParserObject(objType){ LineNum = -1 };
                                            vo.ObjectState = ObjectState.Dropped;
                                            vo.SetDynProperty("Parent", foundExistingObject.GetDynPropertyValue("Parent"));
                                            vo.SetDynProperty("this", thisValue);
                                            //vo.SetDynProperty("FilterKey", filterKey);
                                            //vo.SetDynProperty("IsVisible", isVisible);
                                            //vo.SetDynProperty("IsFindable", isFindable);
                                            foundVoCollection.Add(vo);
                                            i++;
                                        }
                                    }
                                }

                                if (!isExistingFound)
                                {                                   
                                    obj = new ParserObject(objType) { LineNum = lineNumber };
                                    obj.ObjectState = objState;
                                    obj.SetDynProperty("this", thisValue);                                    
                                    obj.SetDynProperty("FilterKey", filterKey);
                                    obj.SetDynProperty("IsVisible", isVisible);
                                    obj.SetDynProperty("IsFindable", isFindable);
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

        private void SetObjectDescription(XElement prop, object parsedValue)
        {
            XElement displayMember = prop.Element("DisplayMember");
            if (displayMember == null || displayMember.Value == null || !displayMember.Value.ToBoolean()) return;

            if (prop.Element("DataType").Value.ToLower() == "time" && prop.Element("DataType").Attribute("Format") != null)
            {                          
                if (DateTime.TryParseExact((string)parsedValue, prop.Element("DataType").Attribute("Format").Value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dt))
                    parsedValue = dt.ToString(_visualDateTimeFormat);
            }             
         
            var key = prop.Element("Name").Value.ToString();               
            if (_currentObj != null && !_currentObj.ObjectDescription.ContainsKey(key))
                _currentObj.ObjectDescription.Add(new KeyValuePair<string, string>(key, parsedValue.ToString()));          
        }     
    }
}
