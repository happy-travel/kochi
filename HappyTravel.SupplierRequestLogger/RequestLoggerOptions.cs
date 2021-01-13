using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace HappyTravel.SupplierRequestLogger
{
    public class RequestLoggerOptions
    {
        [Required]
        public string Endpoint { get; set; } = string.Empty;
        public Func<HttpRequestMessage, bool>? LoggingCondition { get; set; }
    }
}