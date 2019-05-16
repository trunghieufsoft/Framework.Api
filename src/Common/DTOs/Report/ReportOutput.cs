using System.Collections.Generic;

namespace Common.DTOs.Report
{
    public class ReportOutput
    {
        // TODO
    }

    public class ReportExeptionOutput
    {
        public string Export { get; set; }
        public IEnumerable<ExceptionObj> Exception { get; set; }
    }

    public class ExceptionObj
    {
        public string Export { get; set; }
        public string ExceptionCode { get; set; }
        public string ExceptionProp { get; set; }
        public int Quantity { get; set; }
    }
}