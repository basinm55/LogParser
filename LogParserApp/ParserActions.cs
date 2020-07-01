using System;
using System.Collections.Generic;
using System.Dynamic;
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
            //_currentObj.SetDynProperty(objectType, parsedValue, dataType, format);
        }
        private void DoActionLocate(XElement profilePropDefinition, int lineNumber, int patternIndex, object parsedValue, string logLine, string objectType, string thisValue)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            var searchValue = _currentObj.GetDynPropertyValue("this");

            _locatedObj = ObjectCollection.FirstOrDefault(x => (string)x.GetDynPropertyValue("this") == (string)searchValue);
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
            _currentObj.SetDynProperty("Name", name);
            _currentObj.SetDynProperty("PatternIndex", patternIndex, PropertyDataType.Decimal);
            _currentObj.SetDynProperty("DataType", dataType, PropertyDataType.Enum, format, typeof(PropertyDataType));

            if (string.Equals(name, "Timestamp", StringComparison.InvariantCultureIgnoreCase))
                _currentObj.SetDynProperty("Timestamp", parsedValue, PropertyDataType.Time, format);
        }      

        private bool FindOrCreateBaseObject(int lineNumber, XElement filter, List<object> parsedList, out string objectType, out string thisValue, out string objectState, out ParserObject foundPrevStateObject)
        {
            thisValue = null;
            objectType = null;
            objectState = null;
            var objState = ObjectState.Unknown;
            var objType = ObjectType.Unknown;
            ParserObject obj, foundPrevStateObj = null;
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
                                                    x.ObjectType == objType);// &&
                                                    //x.ObjectState == ObjectState.Created &&
                                                    //x.LineNum < lineNumber);

                                 foundPrevStateObj = ObjectCollection.LastOrDefault(x =>
                                                    x.GetThis() == thisVal &&
                                                    x.ObjectType == objType &&
                                                    x.ObjectState == objState - 1 &&
                                                    ObjectCollection.IndexOf(x) < ObjectCollection.Count - 1 &&
                                                    x.LineNum < lineNumber);

                                //if (foundPrevStateObj == null)

                                //    foundPrevStateObj = ObjectCollection.LastOrDefault(x =>                                                    
                                //                    x.VisualObjectCollection.Any(y => y != null && y.GetThis() == thisVal) &&
                                //                    x.VisualObjectCollection.Any(y => y != null && y.ObjectType == objType) &&
                                //                    x.VisualObjectCollection.Any(y => y != null && y.ObjectState == objState - 1));                                                                                                     

                                if (foundExistingObject == null || objState == ObjectState.Created)
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
                                    if (foundPrevStateObj != null)
                                    {
                                        obj = new ParserObject(objType) { LineNum = lineNumber };
                                        obj.ObjectState = objState;
                                        obj.SetDynProperty("Parent", foundExistingObject.GetDynPropertyValue("Parent"));
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
                                }
                                _currentObj = obj;
                            }

                            foundPrevStateObject = foundPrevStateObj;
                            return isExistingFound;
                        }
                    }                    
                }

            }
            foundPrevStateObject = foundPrevStateObj;
            return isExistingFound;
        }
    }
}
