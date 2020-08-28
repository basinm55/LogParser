using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class Enums
    {
        [Serializable]
        public enum PropertyAction
        {
            Unrecognized = 0,
            New = 1,
            AssignToSelf = 2,
            Locate = 3,
            Assign = 4,
            AssignDataBuffer = 5,
            Drop = 6,
            Delete = 7
        }

        [Serializable]
        public enum Shape
        {
            Rectangle = 0,
            Ellipse = 1
        }

        [Serializable]
        public enum PropertyDataType
        {
            String = 0,
            Decimal = 1,
            Boolean = 2,
            Hexadecimal = 3,
            Time = 4,
            Enum = 5,
            Object = 6,
            Data = 7
        }

        [Serializable]
        public enum Visibility
        {
            Visible = 0,
            Hidden = 1
        }

        [Serializable]
        public enum State
        {
            Unknown = -5,
            Temporary = -4,
            Missing = -3,
            ViewArrow = -2,
            Blank = -1,
            Created = 0,                                  
            Queued = 1,
            Sent = 2,
            Responded = 3,
            Completed = 4,                        
        }

        [Serializable]
        public enum ObjectClass
        {
            Unknown = -4,
            Missing = -3,
            ViewArrow = -2,
            Blank = -1,
            Device = 0,
            Port = 1,
            Request = 2,
            Data = 3
        }

        [Serializable]
        public enum RefDirection
        {            
            None = 0,
            Forward = 1,
            Backward = 2
        }
    }
}