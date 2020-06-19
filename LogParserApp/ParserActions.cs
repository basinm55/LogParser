﻿using System;
using System.Collections.Generic;
using System.Dynamic;
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
    public partial class Parser
    {
        private void DoActionNew(XElement filter, XElement profilePropDefinition, int sequenceNum, int patternIndex, object parsedValue, string logLine)
        {
            if (!ValidateProfileDefinition(profilePropDefinition,
                out string name,
                out PropertyDataType dataType,
                out string format)) return;
          
            _currentObj = new ParserObject(name);

            SetFilterProperties(filter, logLine);

            SetPropertiesByProfile(profilePropDefinition,
                sequenceNum,
                patternIndex,
                parsedValue,
                name,
                dataType,
                format);           

            ObjectCollection.Add(_currentObj);
        }

        private void DoActionAssignToSelf(XElement filter, XElement profilePropDefinition, int sequenceNum, int patternIndex, object parsedValue, string logLine)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            SetFilterProperties(filter, logLine);

            _currentObj.SetDynProperty(name, parsedValue, dataType, format);
        }
        private void DoActionLocate(XElement filter, XElement profilePropDefinition, int sequenceNum, int patternIndex, object parsedValue, string logLine)
        {
            if (_currentObj==null || !ValidateProfileDefinition(profilePropDefinition,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            SetFilterProperties(filter, logLine);

            var searchValue = _currentObj.GetDynPropertyValue("this");

            _locatedObj = ObjectCollection.FirstOrDefault(x => x.GetDynPropertyValue("this") == searchValue);            
        }

        private void DoActionAssign(XElement filter, XElement profilePropDefinition, int sequenceNum, int patternIndex, object parsedValue, string logLine)
        {
            if (_locatedObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            SetFilterProperties(filter, logLine);

            _locatedObj.SetDynProperty(name, parsedValue, dataType, format);
            _locatedObj = null;
        }
        private void DoActionDrop(XElement filter, XElement profilePropDefinition, int sequenceNum, int patternIndex, object parsedValue, string logLine)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                      out string name,
                      out PropertyDataType dataType,
                      out string format)) return;

            SetFilterProperties(filter, logLine);
        }

        private void DoActionDelete(XElement filter, XElement profilePropDefinition, int sequenceNum, int patternIndex, object parsedValue, string logLine)
        {
            if (_currentObj == null || !ValidateProfileDefinition(profilePropDefinition,
                     out string name,
                     out PropertyDataType dataType,
                     out string format)) return;

            SetFilterProperties(filter, logLine);


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



        private void SetFilterProperties(XElement filter, string logLine)
        {
            var filterKey = filter.Attribute("key").Value;

            bool isVisible = true;
            if (filter.Attribute("IsVisible") != null)
                isVisible = filter.Attribute("IsVisible").Value.ToBoolean();

            bool isFindable = true;
            if (filter.Attribute("IsFindable") != null)
                isFindable = filter.Attribute("IsFindable").Value.ToBoolean();

            string state = filter.Element("State") != null &&
                !string.IsNullOrWhiteSpace(filter.Element("State").Value)
                ? filter.Element("State").Value : null;
            
            _currentObj.SetDynProperty("FilterKey", filterKey);
            _currentObj.SetDynProperty("IsVisible", isVisible, PropertyDataType.String);
            _currentObj.SetDynProperty("IsFindable", isFindable, PropertyDataType.String);            
            if (_currentObj != null)
             _currentObj.SetDynProperty("State", state, PropertyDataType.String);

        }

        private void SetPropertiesByProfile(XElement profilePropDefinition, int sequenceNum, int patternIndex, object parsedValue, string name, PropertyDataType dataType, string format)
        {           
            _currentObj.SetDynProperty("Name", name);
            _currentObj.SetDynProperty("SequenceNum", sequenceNum, PropertyDataType.Decimal);
            _currentObj.SetDynProperty("PatternIndex", patternIndex, PropertyDataType.Decimal);
            _currentObj.SetDynProperty("DataType", dataType, PropertyDataType.Enum, format, typeof(PropertyDataType));

            //_currentObj.SetDynProperty("Action",
            //    profilePropDefinition.Element("Action").Value.ToEnum<PropertyAction>(),
            //        PropertyDataType.Enum, null, typeof(PropertyAction));

            if (string.Equals(name, "Timestamp", StringComparison.InvariantCultureIgnoreCase))
                _currentObj.SetDynProperty("Timestamp", parsedValue, PropertyDataType.Time, format);

            var target = profilePropDefinition.Element("Target");
            if (target != null)
                _currentObj.SetDynProperty(profilePropDefinition.Element("Target").Value, parsedValue);
        }
    }
}
