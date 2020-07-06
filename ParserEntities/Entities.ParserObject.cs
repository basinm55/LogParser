using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using static Entities.Enums;
using Helpers;
using System.Linq;
using System.Drawing;
using System.Text;

namespace Entities
{ 
    public class ParserObject : IDisposable
    {                           
        public Color BaseColor { get; set; }     

        public dynamic DynObject { get; set; }

        public IDictionary<string, object> DynObjectDictionary;

        public List<StateObject> StateCollection { get; set; }
       

        public ObjectType Type { get; private set; }

        public bool IsFindable { get; set; }


        //C'tor
        public ParserObject(ObjectType objType)
        {
            DynObject = new ExpandoObject();
            DynObjectDictionary = (IDictionary<string, object>)DynObject;
            Type = objType;
            IsFindable = true;
            BaseColor = Color.Transparent;
            StateCollection = new List<StateObject>();        
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

        public void Dispose()
        {
            DynObjectDictionary.Clear();
            StateCollection.Clear();
        }
    }

    public static class Extensions
    {      
        public static string GetThis(this ParserObject item)
        {
            return item != null ? (string)item.GetDynPropertyValue("this") : null;
        }

        public static void SetThis(this ParserObject item, string thisValue)
        {
            if (item != null)
                item.SetDynPropertyValue("this", thisValue);
        }

        public static string GetParent(this ParserObject item)
        {
            return item != null ? (string)item.GetDynPropertyValue("Parent") : null;
        }
          

        public static StateObject CreateStateObject(this ParserObject baseObject, State state, int lineNumber, string line)
        {
            var result = new StateObject(baseObject);
            result.LineNum = lineNumber;
            result.Line = line;
            result.State = state;
            result.Color = baseObject.BaseColor;                                  
            return result;
        }      
 

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