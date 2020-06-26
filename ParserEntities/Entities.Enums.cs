using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Enums
    {

        public enum PropertyAction
        {
            Unrecognized = 0,
            New = 1,
            AssignToSelf = 2,
            Locate = 3,
            Assign = 4,
            Drop = 5,
            Delete = 6
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
            Boolean = 2,
            Hexadecimal = 3,
            Time = 4,
            Enum = 5,
            Object = 6
        }

        public enum Visibility
        {
            Visible = 0,
            Hidden = 1
        }

        public enum ObjectState
        {
            Unknown = -1,
            Created = 0,                       
            Prepared = 1,
            Queued = 2,
            Sent = 3,
            Responded = 4,
            Completed = 5,
            Deleted = 6
        }

        public enum ObjectType
        {
            Unrecognized = 0,
            Device = 1,
            Request = 2
        }
    }
}