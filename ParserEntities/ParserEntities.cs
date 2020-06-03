using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ParserEntities
{
    public class Enums
    {

        public enum Action
        {
            New = 0,
            Locate = 1,
            Assign = 2,
            Drop = 3,
            Delete = 4
        }

        public enum Shape
        {
            Rectangle = 0,
            Circle = 1
        }

        public enum PropertyDataType
        {
            String = 0,
            Decimal = 1,
            Hex = 2,
            DateTime = 3
        }      
    }


    public class ParserObject
    {
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }

        public bool IsExists { get; set; }

        public ExpandoObject DynObject { get; }

        private IDictionary<string, object> _dynObjectDictionary;



        //C'tor
        public ParserObject()
        {
            //dynamic DynObject = new ExpandoObject();
            //var dictionary = (IDictionary<string, object>)DynObject;
            //var dictionary = (IDictionary<string, object>)person;
            //_dynObjectDictionary = (IDictionary<string, object>)DynObject;
            //dictionary.Add("Name", "Filip");
            //dictionary.Add("Age", 24);

            dynamic person = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)person;
        }

    }
}
