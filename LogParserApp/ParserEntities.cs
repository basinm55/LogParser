using System;
using System.Collections.Generic;
using System.Text;

namespace ParserEntities
{

    public enum LogEntryStatus
    {
        OK = 0,
        Error = 1
    }

    public class LogEntry
    {        
        public string EntryType { get; set; }
        public DateTime Time { get; set; }
        public int PortNumber { get; set; }
        public string Address { get; set; }
        public string Device { get; set; }
        public string Description { get; set; }
        public string DataBuffer{ get; set; }

    public LogEntryStatus Status;
    }

    public class GridViewRow
    {
        public int RowOrder { get; set; }
        public DateTime RowStartTime { get; set; }
        public DateTime RowEndTime { get; set; }
        public int PortNumber { get; set; }
        public string DeviceName { get; set; }
        public LogEntryStatus RowStatus { get; set; }
        public LogEntry[] ConnectedEntities { get; set; }
    }
}
