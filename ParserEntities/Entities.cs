using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Entities
{ 
    public class ParserObject
    {
        public string ObjectClass { get; private set; }
        public DateTime CreationTimeStamp { get; set; }
        public bool IsExists { get; set; }
        private dynamic _dynObject { get; }

        private IDictionary<string, object> _dynObjectDictionary;


        //C'tor
        public ParserObject(string objectClass, DateTime timeStamp)
        {
            _dynObject = new ExpandoObject();
            _dynObjectDictionary = (IDictionary<string, object>)_dynObject;
            ObjectClass = objectClass;
            CreationTimeStamp = timeStamp;
        }

        public void AddDynProperty(string propertyName, object propertyValue)
        {
            _dynObjectDictionary.Add(propertyName, propertyValue);
        }

        public void RemoveDynProperty(string propertyName, object propertyValue)
        {
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

        public void SetDynPropertyValue(string propertyName, object value)
        {
            if (_dynObjectDictionary.ContainsKey(propertyName))
                _dynObjectDictionary[propertyName] = value;
            else
                throw new Exception(string.Format("Property '{0}' not found", propertyName));
        }
    }
}