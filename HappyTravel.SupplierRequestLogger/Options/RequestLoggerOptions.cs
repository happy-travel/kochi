using System;
using System.Net.Http;

namespace HappyTravel.SupplierRequestLogger.Options
{
    public class RequestLoggerOptions
    {
        public string Endpoint { get; set; } = string.Empty;
        public Func<HttpRequestMessage, bool>? LoggingCondition { get; set; }
    }
}