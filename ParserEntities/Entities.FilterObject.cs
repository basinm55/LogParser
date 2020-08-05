using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class FilterObject
    {
        public string FilterExpression { get; set; }

        public List<FilterDefinition> Definitions;


        public FilterObject()
        {
            Definitions = new List<FilterDefinition>();
        }

        public void Clear()
        {
            Definitions.Clear();
            FilterExpression = null;
        }
    }

    public class FilterDefinition
    {
        public string Connector { get; set; }
        public string Property { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }       
    }
}
