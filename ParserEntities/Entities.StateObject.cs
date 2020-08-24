using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums;

namespace Entities
{
    [Serializable]
    public class StateObject : IDisposable
    {        
        public State State { get; set; }

        public Shape Shape { get; set; }

        public ObjectClass ObjectClass { get; set; }

        public DateTime Time { get; set; }

        public string Description { get; set; }

        public IDictionary<string, string> VisualDescription { get; set; }

        public string LogEntry { get; set; }

        public int LineNum { get; set; }

        public string FilterKey { get; set; }

        public string Color { get; set; }

        public ParserObject Parent { get; set; }

        public ParserObject ReferenceObj { get; set; }

        public StateObject ReferenceStateObj { get; set; }
                
        public StringBuilder DataBuffer { get; set; }        

        //C'tor
        public StateObject()
        {
            //Parent = parent;
            //ReferenceStateObj = referenceStateObj;            
            VisualDescription = new Dictionary<string, string>();
            DataBuffer = new StringBuilder();
        }        

        public void Dispose()
        {
            VisualDescription.Clear();
            DataBuffer.Clear();
        }

        //public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        //{
        //    var sw = FastSerializer.Writer;
        //    sw.Write(State);
        //    sw.Write(ObjectClass);
        //    sw.Write(Time);
        //    sw.Write(Description);
        //    sw.Write(VisualDescription);
        //    sw.Write(LogEntry);
        //    sw.Write(LineNum);
        //    sw.Write(FilterKey);
        //    sw.Write(Color);
        //    sw.Write(Parent);
        //    sw.Write(ReferenceObj);
        //    sw.Write(DataBuffer);    

        //    sw.AddToInfo(info);
        //} 
    }
}
