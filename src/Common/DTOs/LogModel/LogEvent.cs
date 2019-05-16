using System;

namespace Common.DTOs.LogModel
{
    public class LogEvent
    {
        public DateTime TimeStamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public EventProperties Properties { get; set; }
    }

    public class EventProperties
    {
        public String Data { get; set; }
    }
}