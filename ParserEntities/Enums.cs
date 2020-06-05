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
            Hexadecimal = 2,
            DateTime = 3
        }

        public enum Visibility
        {
            Visible = 0,
            Hidden = 1
        }

        public enum ObjectState
        {
            New = 0,
            Prepared,
            Sent,
            Completed,
            Destroyed
        }
    }
}