using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class LogEntryEntity
    {
        public DateTime Time { get; set; }
        public int PortNumber { get; set; }
        public string DeviceName { get; set; }        
    }

    public class GridViewRow
    {
        public DateTime RowStartTime { get; set; }
        public DateTime RowEndTime { get; set; }
        public int PortNumber { get; set; }
        public string DeviceName { get; set; }
        public string Status { get; set; }
        public LogEntryEntity[] ConnectedEntities { get; set; }
    }
}
