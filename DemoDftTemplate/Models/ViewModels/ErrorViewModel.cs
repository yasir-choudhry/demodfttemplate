using System;

namespace DemoDftTemplate.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string StatusCode { get; set; }
        public string Details { get; set; }
    }
}