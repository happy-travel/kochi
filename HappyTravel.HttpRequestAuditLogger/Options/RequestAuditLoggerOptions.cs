using System;
using System.Net.Http;

namespace HappyTravel.HttpRequestAuditLogger.Options
{
    public class RequestAuditLoggerOptions
    {
        public string Endpoint { get; set; } = string.Empty;
        public Func<HttpRequestMessage, bool>? LoggingCondition { get; set; }
    }
}