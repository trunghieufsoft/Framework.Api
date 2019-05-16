using System.Collections.Generic;

namespace Common.DTOs.Common
{
    public class SearchOutput
    {
        public IEnumerable<object> DataResult { get; set; }
        public int TotalData { get; set; }
        public int? PageIndex { get; set; }
        public int? LimitData { get; set; }
    }
}