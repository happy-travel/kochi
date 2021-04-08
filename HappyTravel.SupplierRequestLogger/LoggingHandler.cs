using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using HappyTravel.SupplierRequestLogger.Extensions;
using HappyTravel.SupplierRequestLogger.Models;
using HappyTravel.SupplierRequestLogger.Options;
using Microsoft.Extensions.Options;

namespace HappyTravel.SupplierRequestLogger
{
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(ChannelWriter<HttpRequestAuditLogEntry> channel, IOptions<RequestLoggerOptions> options)
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
                
                // write to the channel so as not to lose the message
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
        private readonly RequestLoggerOptions _options;
    }
}