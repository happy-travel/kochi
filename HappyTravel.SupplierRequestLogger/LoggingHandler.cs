using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HappyTravel.SupplierRequestLogger.Extensions;
using Microsoft.Extensions.Options;

namespace HappyTravel.SupplierRequestLogger
{
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(IHttpClientFactory clientFactory, IOptions<RequestLoggerOptions> options)
        {
            _clientFactory = clientFactory;
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
            var logEntry = new HttpRequestAuditLogEntry
            {
                Url = request.RequestUri?.AbsoluteUri ?? string.Empty,
                RequestHeaders = request.Headers.ToDictionary(),
                RequestBody = request.Content?.ToString()
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
                
                await Send(logEntry);
                return response;
            }
            catch (Exception e)
            {
                logEntry = logEntry with
                {
                    Error = e.Message
                };
                
                await Send(logEntry);
                throw;
            }
        }


        private Task Send(HttpRequestAuditLogEntry logEntry)
        {
            using var client = _clientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(logEntry), Encoding.UTF8, "application/json");
            try
            {
                return client.PostAsync(_options.Endpoint, content);
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }


        private readonly IHttpClientFactory _clientFactory;
        private readonly RequestLoggerOptions _options;
    }
}