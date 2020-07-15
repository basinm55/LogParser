using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums;

namespace Entities
{
    public class StateObject : IDisposable
    {
        public State State { get; set; }

        public ObjectClass ObjectClass { get; set; }

        public DateTime Time { get; set; }

        public string Description { get; set; }

        public IDictionary<string, string> VisualDescription { get; set; }

        public string LogEntry { get; set; }

        public int LineNum { get; set; }

        public string FilterKey { get; set; }

        public Color Color { get; set; }

        public ParserObject Parent { get; private set; }

        public StateObject ReferenceStateObj { get; set; }
                
        public StringBuilder DataBuffer { get; set; }        

        //C'tor
        public StateObject(ParserObject parent, StateObject referenceStateObj = null)
        {
            Parent = parent;
            ReferenceStateObj = referenceStateObj;            
            VisualDescription = new Dictionary<string, string>();
            DataBuffer = new StringBuilder();
        }        

        public void Dispose()
        {
            VisualDescription.Clear();
            DataBuffer.Clear();
        }
    }
}
