using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HappyTravel.SupplierRequestLogger.Extensions;
using HappyTravel.SupplierRequestLogger.Services;

namespace HappyTravel.SupplierRequestLogger
{
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(ISender sender)
        {
            _sender = sender;
        }
        
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            
            var auditLog = new HttpRequestAuditLogEntry
            {
                Url = request.RequestUri?.AbsoluteUri ?? string.Empty,
                RequestHeaders = request.Headers.ToDictionary(),
                RequestBody = request.Content?.ToString(),
                ResponseHeaders = response.Headers.ToDictionary(),
                ResponseBody = await response.Content.ReadAsStringAsync(cancellationToken),
                StatusCode = (int) response.StatusCode
            };

            await _sender.Send(auditLog);
            return response;
        }


        private readonly ISender _sender;
    }
}