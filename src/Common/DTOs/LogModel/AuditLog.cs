using Newtonsoft.Json;
using System;

namespace Common.DTOs.LogModel
{
    public class SystemLog
    {
        public string Description { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }

    public class TransactionLog
    {
        public TransactionLog()
        {
        }

        public TransactionLog(string logEvent)
        {
            LogEvent log = JsonConvert.DeserializeObject<LogEvent>(logEvent);
            var nameOfProperty = "Data";
            var propertyInfo = log.Properties.GetType().GetProperty(nameOfProperty);
            var value = propertyInfo.GetValue(log.Properties, null);
            if (value != null)
            {
                TransactionLog data = JsonConvert.DeserializeObject<TransactionLog>(value.ToString());
                Action = data.Action;
                Username = data.Username;
                EventCode = data.EventCode;
                // TODO Information Transaction Log

                Time = log.TimeStamp;
            }
        }

        public string Action { get; set; }
        public string Username { get; set; }
        public string EventCode { get; set; }
        // TODO The Properties

        public DateTime Time { get; set; }
    }

    public class SynchronizationLog
    {
        public SynchronizationLog()
        {
        }

        public SynchronizationLog(string logEvent)
        {
            LogEvent log = JsonConvert.DeserializeObject<LogEvent>(logEvent);
            var nameOfProperty = "Data";
            var propertyInfo = log.Properties.GetType().GetProperty(nameOfProperty);
            var value = propertyInfo.GetValue(log.Properties, null);
            if (value != null)
            {
                SynchronizationLog data = JsonConvert.DeserializeObject<SynchronizationLog>(value.ToString());
                Username = data.Username;
                // TODO Information Synchronization Log

                Time = log.TimeStamp;
            }
        }

        public string Username { get; set; }
        // TODO The Properties

        public DateTime Time { get; set; }
    }
}