using Entities.Entities;

namespace Common.DTOs.SystemConfigModel
{
    public class SystemConfigOutput
    {
        public SystemConfigOutput(SystemConfiguration SystemConfig = null)
        {
            Key = SystemConfig?.KeyStr;
            Value = SystemConfig?.Value;
            ValueUnit = SystemConfig?.ValueUnit;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public string ValueUnit { get; set; }
    }
}