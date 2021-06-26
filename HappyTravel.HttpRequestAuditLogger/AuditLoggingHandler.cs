using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using HappyTravel.HttpRequestAuditLogger.Models;
using HappyTravel.HttpRequestAuditLogger.Options;
using HappyTravel.HttpRequestAuditLogger.Extensions;
using Microsoft.Extensions.Options;

namespace HappyTravel.HttpRequestAuditLogger
{
    public class AuditLoggingHandler : DelegatingHandler
    {
        public AuditLoggingHandler(ChannelWriter<HttpRequestAuditLogEntry> channel, IOptions<RequestAuditLoggerOptions> options)
        {
            _channel = channel;
            _options = options.Value;
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _options.LoggingCondition == null || _options.LoggingCondition(request)
                ? SendWithLog(request, cancellationToken)
                : base.SendAsync(request, cancellationToken);
        }


        private async Task<HttpResponseMessage> SendWithLog(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestBody = request.Content is not null
                ? await request.Content.ReadAsStringAsync(cancellationToken)
                : null;
            
            var logEntry = new HttpRequestAuditLogEntry
            {
                Method = request.Method.Method,
                Url = request.RequestUri?.AbsoluteUri ?? string.Empty,
                RequestHeaders = request.Headers.ToDictionary(),
                RequestBody = requestBody
            };

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                logEntry = logEntry with
                {
                    ResponseHeaders = response.Headers.ToDictionary(),
                    ResponseBody = await response.Content.ReadAsStringAsync(cancellationToken),
                    StatusCode = (int) response.StatusCode
                };
                
                // Write to the channel so as not to lose the message
                await _channel.WriteAsync(logEntry, cancellationToken);
                return response;
            }
            catch (Exception e)
            {
                logEntry = logEntry with
                {
                    Error = e.Message
                };
                
                await _channel.WriteAsync(logEntry, cancellationToken);
                throw;
            }
        }


        private readonly ChannelWriter<HttpRequestAuditLogEntry> _channel;
        private readonly RequestAuditLoggerOptions _options;
    }
}