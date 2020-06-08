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
            Locate = 2,
            Assign = 3,
            Drop = 4,
            Delete = 5
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
            Hexadecimal = 2,
            DateTime = 3,
            Enum = 4
        }

        public enum Visibility
        {
            Visible = 0,
            Hidden = 1
        }

        public enum ObjectState
        {
            New = 0,
            Prepared = 1,
            Sent = 2,
            Completed = 3,
            Destroyed = 4
        }
    }
}