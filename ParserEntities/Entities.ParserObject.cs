using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using static Entities.Enums;
using Helpers;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Runtime.Serialization;

namespace Entities
{
    [Serializable]
    public class ParserObject : IDisposable
    {        
        public string BaseColor { get; set; }

        public dynamic DynObject { get; set; }

        public IDictionary<string, object> DynObjectDictionary;

        public string LogEntry { get; set; }

        public int LineNum { get; set; }

        public List<StateObject> StateCollection { get; set; }

        public ObjectClass ObjectClass { get; set; }

        public DateTime Time { get; set; }

        public string FilterKey { get; set; }

        public ParserObject PrevInterruptedObj { get; set; }

        public ParserObject NextContinuedObj { get; set; }

        public StringBuilder DataBuffer { get; set; }

        public bool IsFindable { get; set; }

        public List<string> ColorKeys { get; set; }


        //C'tor
       
        public ParserObject()
        {            
            DynObject = new ExpandoObject();
            DynObjectDictionary = (IDictionary<string, object>)DynObject;
            //ObjectClass = objClass;
            IsFindable = true;
            BaseColor = ColorTranslator.ToHtml(Color.Transparent);
            StateCollection = new List<StateObject>();
            DataBuffer = new StringBuilder();
            ColorKeys = new List<string>();
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
                 "Check the LogParserProfile definitions.", propertyName));

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

        public void Dispose()
        {
            DynObjectDictionary.Clear();
            StateCollection.Clear();
            ColorKeys.Clear();
        }

        //public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        //{
        //    var sw = FastSerializer.Writer;
        //    sw.Write(BaseColor);
        //    sw.Write(DynObjectDictionary);
        //    sw.Write(DynObject);
        //    sw.Write(LogEntry);
        //    sw.Write(LineNum);
        //    sw.Write(StateCollection);
        //    sw.Write(ObjectClass);
        //    sw.Write(Time);
        //    sw.Write(FilterKey);
        //    sw.Write(PrevInterruptedObj);
        //    sw.Write(NextContinuedObj);
        //    sw.Write(DataBuffer);
        //    sw.Write(IsFindable);
        //    sw.Write(ColorKeys);

        //    sw.AddToInfo(info);
        //}
    }

        public static class Extensions
    {      
        public static string GetThis(this ParserObject item)
        {
            return item != null ? (string)item.GetDynPropertyValue("this") : null;
        }     

        public static string GetParent(this ParserObject item)
        {
            return item != null ? (string)item.GetDynPropertyValue("Parent") : null;
        }
          

        public static StateObject CreateStateObject(this ParserObject baseObject, State state, int lineNumber, string line, string filterKey)
        {
            var result = new StateObject();
            result.Parent = baseObject;
            result.LineNum = lineNumber;
            result.LogEntry = line;
            result.State = state;
            //result.Color = baseObject.BaseColor;
            result.FilterKey = filterKey;
            result.ObjectClass = baseObject.ObjectClass;                                        
            return result;
        }

        public static StateObject CreateBlankStateObject(this ParserObject baseObject)
        {
            var result = new StateObject();
            result.Parent = baseObject;
            result.ObjectClass = ObjectClass.Blank;
            result.State = State.Blank;
            result.Color = ColorTranslator.ToHtml(Color.White);
            result.Description = string.Empty;
            return result;
        }


        public static StateObject CreateMissingStateObject(this ParserObject baseObject)
        {
            var result = new StateObject();
            result.Parent = baseObject;
            result.ObjectClass = ObjectClass.Missing;
            result.State = State.Missing;
            result.Color = ColorTranslator.ToHtml(Color.Black);                        
            return result;
        }

        public static StateObject CreateArrowStateObject(this ParserObject baseObject, ParserObject referenceObject = null)
        {
            var result = new StateObject();
            result.Parent = baseObject;
            result.ObjectClass = ObjectClass.ViewArrow;
            result.State = State.ViewArrow;
            result.ReferenceObj = referenceObject;
            result.Color = ColorTranslator.ToHtml(Color.White);            
            result.Description = null;
            if (referenceObject != null)
                result.ReferenceStateObj = referenceObject.StateCollection.LastOrDefault(x => x.ObjectClass != ObjectClass.ViewArrow && x.ObjectClass != ObjectClass.Blank);

            return result;
        }
     

        public static ParserObject CreateObjectClone(this ParserObject original)
        {
            var result = new ParserObject();
            result.ObjectClass = original.ObjectClass;

            result.DynObject = DeepClone(original.DynObject);
            result.DynObjectDictionary = (IDictionary<string, object>)result.DynObject;            
            result.ObjectClass = original.ObjectClass;
            result.BaseColor = original.BaseColor;
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