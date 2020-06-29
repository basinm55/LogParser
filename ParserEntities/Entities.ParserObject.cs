using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using static Entities.Enums;
using Helpers;
using System.Linq;
using System.Drawing;

namespace Entities
{ 
    public class ParserObject
    {
        public ObjectType ObjectType { get; set; }

        public ObjectState ObjectState { get; set; }

        public string LogLine { get; set; }

        public int LineNum { get; set; }

        public string VisualDescription { get; set; }

        public Color ObjectColor { get; set; }

        public string Parent { get; set; }
        public dynamic DynObject { get; set; }

        public IDictionary<string, object> DynObjectDictionary;

        public List<ParserObject> VisualObjectCollection { get; private set; }


        //C'tor
        public ParserObject(ObjectType objectType)
        {
            DynObject = new ExpandoObject();
            DynObjectDictionary = (IDictionary<string, object>)DynObject;            
            ObjectType = objectType;            
            VisualObjectCollection = new List<ParserObject>();
        }


        private bool DynPropertyExists(string propertyName)
        {
            return DynObjectDictionary.ContainsKey(propertyName);
        }

        private object CovertValueToRequiredDataType(string value,
                                                     PropertyDataType propertyDataType = PropertyDataType.String,
                                                     Type enumType = null,
                                                     string dateTimeFormat = null)
        {
            if (value == null || string.IsNullOrWhiteSpace(value))
                return null;

            switch (propertyDataType)
            {
                case PropertyDataType.String:
                    return value;
                case PropertyDataType.Decimal:
                    if (Decimal.TryParse(value, out decimal decVal))
                        return decVal;
                    else
                        return null;
                case PropertyDataType.Boolean:
                    if (bool.TryParse(value, out bool boolVal))
                        return boolVal;
                    else
                        return null;
                case PropertyDataType.Time:
                    var format = dateTimeFormat ?? "MM/dd/yyyy-HH:mm:ss.FFF";
                    if (DateTime.TryParseExact(value, format, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime datetimeVal))
                        return datetimeVal;
                    else
                        return null;
                case PropertyDataType.Hexadecimal:
                    return Convert.ToSByte(value, 16);

                case PropertyDataType.Enum:
                    if (enumType != null)
                    {
                        if (NongenericEnumHelper.TryParse(enumType, value, true, out object enumValue))
                            return enumValue;
                        else
                            return null;
                    }                      
                    else
                        return null;

                default:
                    return value;
            }
        }
    

        public void SetDynProperty(string propertyName, object propertyValue, PropertyDataType propertyDataType = PropertyDataType.String, string format = null, Type enumType = null)
        {

            if (!DynPropertyExists(propertyName))
                DynObjectDictionary.Add(propertyName, CovertValueToRequiredDataType(propertyValue != null ? propertyValue.ToString() : null, propertyDataType, enumType, format));
            else
                DynObjectDictionary[propertyName] = CovertValueToRequiredDataType(propertyValue != null ? propertyValue.ToString() : null, propertyDataType, enumType, format);

        }

        public void RemoveDynProperty(string propertyName)
        {
            if (!DynPropertyExists(propertyName))
                throw new Exception(string.Format("Property '{0}' not found." +
                 Environment.NewLine +
                 "Check the LogParserProfile definitions."
                 , propertyName));

            DynObjectDictionary.Remove(propertyName);
        }

        public void ClearDynProperties()
        {
            DynObjectDictionary.Clear();
        }

        public bool GetDynPropertyValue(string propertyName, out object propertyValue)
        {
            return DynObjectDictionary.TryGetValue(propertyName, out propertyValue); 
        }

        public object GetDynPropertyValue(string propertyName)
        {
            GetDynPropertyValue(propertyName, out object propertyValue);
            return propertyValue;
        }

        public void SetDynPropertyValue(string propertyName, object propertyValue, PropertyDataType propertyDataType = PropertyDataType.String, Type enumType = null, string dateTimeFormat = null)
        {
            if (!DynPropertyExists(propertyName))
                throw new Exception(string.Format("Property '{0}' not found." +
                    Environment.NewLine +
                    "Check the LogParserProfile definitions."
                    , propertyName));

            DynObjectDictionary[propertyName] = CovertValueToRequiredDataType(propertyValue.ToString(), propertyDataType, enumType, dateTimeFormat);
        }
        
    }

    public static class Extensions
    {

        public static ParserObject GetPrevtem(this List<ParserObject> list, ParserObject item)
        {
            var i = list.IndexOf(item);
            var j = (i - 1) % list.Count;
            return list[j];
        }

        public static string GetThis(this ParserObject item)
        {
            return (string)item.GetDynPropertyValue("this");
        }

        public static void SetThis(this ParserObject item, string thisValue)
        {
            item.SetDynPropertyValue("this", thisValue);
        }

        public static ObjectState GetState(this ParserObject item)
        {
            return item.GetDynPropertyValue("State").ToString().ToEnum<ObjectState>();
        }

        public static void SetState(this ParserObject item, ObjectState state)
        {
            item.SetDynPropertyValue("State", state);
        }      

        public static ParserObject GetLastVisualObject(this ParserObject item)
        {
            return item.VisualObjectCollection.Count > 0 ? item.VisualObjectCollection.LastOrDefault() : null;
        }

        public static ParserObject GetBeforeLastVisualObject(this ParserObject item)
        {
            return item.VisualObjectCollection.Count > 0 ? item.VisualObjectCollection.Skip(-1).LastOrDefault() : null;
        }

        public static ParserObject CreateVisualObject(this ParserObject baseObject, ObjectState newState, int lineNumber, string line)
        {
            var result = new ParserObject(baseObject.ObjectType);

            result.DynObject = DeepClone(baseObject.DynObject);
            result.DynObjectDictionary = new Dictionary<string, object>((IDictionary<string, object>)baseObject.DynObject);
            result.ObjectType = baseObject.ObjectType;
            result.LogLine = line;
            result.LineNum = lineNumber;
            result.ObjectState = newState;
            
            return result;
        }

        public static ParserObject CreateObjectClone(this ParserObject original)
        {
            var result = new ParserObject(original.ObjectType);

            result.DynObject = DeepClone(original.DynObject);
            result.DynObjectDictionary = (IDictionary<string, object>)result.DynObject;
            result.ObjectType = original.ObjectType;
            return result;
        }


        //public static ParserObject FindLastVisualObjectByLineNumber(this ParserObject obj, int lineNum)
        //{
        //    ParserObject result = null;
        //    if (obj != null && lineNum >= 0)
        //        result = obj.VisualObjectCollection.Count > 0 ? obj.VisualObjectCollection.LastOrDefault(x => x.LineNum == lineNum) : null;               
            
        //    return result;
        //}

        private static ExpandoObject DeepClone(ExpandoObject original)
        {
            var clone = new ExpandoObject();

            var _original = (IDictionary<string, object>)original;
            var _clone = (IDictionary<string, object>)clone;

            foreach (var kvp in _original)
                _clone.Add(kvp.Key, kvp.Value is ExpandoObject ? DeepClone((ExpandoObject)kvp.Value) : kvp.Value);

            return clone;
        }
    }
}