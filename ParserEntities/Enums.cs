using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Enums
    {

        public enum Action
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
            Hex = 2,
            DateTime = 3
        }
    }
}