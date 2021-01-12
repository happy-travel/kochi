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
        
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var auditLog = new HttpRequestAuditLogEntry
            {
                Url = request.RequestUri?.AbsoluteUri ?? string.Empty,
                RequestHeaders = request.Headers.ToDictionary(),
                RequestBody = request.Content?.ToString()
            };

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                auditLog = auditLog with
                {
                    ResponseHeaders = response.Headers.ToDictionary(),
                    ResponseBody = await response.Content.ReadAsStringAsync(cancellationToken),
                    StatusCode = (int) response.StatusCode
                };
                
                await Send(auditLog);
                return response;
            }
            catch (Exception e)
            {
                auditLog = auditLog with
                {
                    Error = e.Message
                };
                
                await Send(auditLog);
                throw;
            }
        }


        private async Task Send(HttpRequestAuditLogEntry logEntry)
        {
            using var client = _clientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(logEntry), Encoding.UTF8, "application/json");
            await client.PostAsync(_options.Endpoint, content);
        }


        private readonly IHttpClientFactory _clientFactory;
        private readonly RequestLoggerOptions _options;
    }
}