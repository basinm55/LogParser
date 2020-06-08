using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using Entities;
using Helpers;
using static Entities.Enums;

namespace LogParserApp
{
    public class ParserActions
    {
        public static bool DoActionNew(XElement profilePropDefinition, int parsedIndex, object parsedValue, List<ParserObject> objectList)
        {
            bool isSuccess = false;          
            var name = profilePropDefinition.Element("Name").Value;
            var dataType = profilePropDefinition.Element("Type").Value.ToEnum<PropertyDataType>();  

            if (!string.IsNullOrWhiteSpace(name))
            {
                var obj = new ParserObject(name);
                obj.AddDynProperty(name, parsedValue, dataType);
                obj.AddDynProperty("ParsedIndex", parsedIndex, PropertyDataType.Decimal);
                obj.CreationTimeStamp = DateTime.MinValue; //TODO
                IEnumerable<XElement> childElements =  from el in profilePropDefinition.Elements() select el;
                foreach (XElement el in childElements)
                {
                    switch (el.Name.LocalName.ToUpper())
                    {
                        case "ACTION":
                            obj.AddDynProperty(el.Name.LocalName, el.Value, PropertyDataType.Enum, typeof(PropertyAction));
                            break;
                        default:
                            obj.AddDynProperty(el.Name.LocalName, el.Value);
                            break;
                    }
                    objectList.Add(obj);

                }
                isSuccess = true;
            }
                      
            return isSuccess;
        }

        public static bool DoActionLocate(XElement profilePropDefinition, int parsedIndex, object parsedValue, List<ParserObject> objectList)
        {
            var name = profilePropDefinition.Element("Name").Value;
            var dataType = profilePropDefinition.Element("Type").Value.ToEnum<PropertyDataType>();
            
            return true;
        }

        public static bool DoActionAssign(XElement profilePropDefinition, int parsedIndex, object parsedValue, List<ParserObject> objectList)
        {
            var name = profilePropDefinition.Element("Name").Value;
            var dataType = profilePropDefinition.Element("Type").Value.ToEnum<PropertyDataType>();

            return true;
        }

        public static bool DoActionDrop(XElement profilePropDefinition, int parsedIndex, object parsedValue, List<ParserObject> objectList)
        {
            var name = profilePropDefinition.Element("Name").Value;
            var dataType = profilePropDefinition.Element("Type").Value.ToEnum<PropertyDataType>();

            return true;
        }

        public static bool DoActionDelete(XElement profilePropDefinition, int parsedIndex, object parsedValue, List<ParserObject> objectList)
        {
            var name = profilePropDefinition.Element("Name").Value;
            var dataType = profilePropDefinition.Element("Type").Value.ToEnum<PropertyDataType>();

            return true;
        }
    }
}
