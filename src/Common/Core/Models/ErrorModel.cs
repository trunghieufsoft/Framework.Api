namespace Common.Core.Models
{
    public class ErrorModel
    {
        public int ErrorCode { get; set; }

        public string Message { get; set; }

        public dynamic Data { get; set; }
    }
}