﻿using System;
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
            AssignDataBuffer = 5,
            Drop = 6,
            Delete = 7
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
            Unknown = -3,
            Deleted = -2,
            Dropped = -1,
            Created = 0,                                  
            Queued = 1,
            Sent = 2,
            Responded = 3,
            Completed = 4,                        
        }

        public enum ObjectType
        {
            Unknown = -1,
            Device = 0,
            Request = 1,
            Data = 2
        }
    }
}