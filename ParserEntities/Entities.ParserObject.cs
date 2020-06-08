using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using static Entities.Enums;
using Helpers;

namespace Entities
{ 
    public class ParserObject
    {
        public string ObjectClass { get; private set; }
        public DateTime CreationTimeStamp { get; set; }                
        private dynamic _dynObject { get; }

        private IDictionary<string, object> _dynObjectDictionary;


        //C'tor
        public ParserObject(string objectClass)
        {
            _dynObject = new ExpandoObject();
            _dynObjectDictionary = (IDictionary<string, object>)_dynObject;
            ObjectClass = objectClass;
        }


        private bool DynPropertyExists(string propertyName)
        {
            return _dynObjectDictionary.ContainsKey(propertyName);
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
                case PropertyDataType.DateTime:
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
    

        public void AddDynProperty(string propertyName, object propertyValue, PropertyDataType propertyDataType = PropertyDataType.String, Type enumType = null, string dateTimeFormat = null)
        {

            if (!DynPropertyExists(propertyName))
                _dynObjectDictionary.Add(propertyName, CovertValueToRequiredDataType(propertyValue.ToString(), propertyDataType, enumType, dateTimeFormat));
                
        }

        public void RemoveDynProperty(string propertyName, object propertyValue)
        {
            if (!DynPropertyExists(propertyName))
                throw new Exception(string.Format("Property '{0}' not found." +
                 Environment.NewLine +
                 "Check LogParserProfile definitions."
                 , propertyName));

            _dynObjectDictionary.Remove(propertyName);
        }

        public void ClearDynProperties()
        {
            _dynObjectDictionary.Clear();
        }

        public object GetDynPropertyValue(string propertyName)
        {
            _dynObjectDictionary.TryGetValue(propertyName, out object value);
            return value;
        }

        public void SetDynPropertyValue(string propertyName, object value, PropertyDataType propertyDataType = PropertyDataType.String, Type enumType = null, string dateTimeFormat = null)
        {
            if (!DynPropertyExists(propertyName))
                throw new Exception(string.Format("Property '{0}' not found." +
                    Environment.NewLine +
                    "Check LogParserProfile definitions."
                    , propertyName));

            _dynObjectDictionary[propertyName] = CovertValueToRequiredDataType(value.ToString(), propertyDataType, enumType, dateTimeFormat);
        }


        public class PropertyDefinition
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public PropertyAction Action { get; set; }

        public PropertyDataType Type { get; set; }

        public object Value { get; set; }
    }
    }
}