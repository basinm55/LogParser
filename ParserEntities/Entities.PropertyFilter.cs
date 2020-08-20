using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class PropertyFilter
    {
        public string PropertyName{ get; set; }
        public List<PropertyFilterValue> Values { get; set; }

        public void Clear()
        {
            Values.Clear();
        }
    }

    [Serializable]
    public class PropertyFilterValue
    {
        public object Value { get; set; }
        public string Parent { get; set; }
    }
}
