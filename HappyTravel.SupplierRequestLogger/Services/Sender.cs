using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace HappyTravel.SupplierRequestLogger.Services
{
    public class Sender : ISender
    {
        public Sender(HttpClient client, IOptions<SenderOptions> options)
        {
            _client = client;
            _options = options.Value;
        }
        
        public Task Send(HttpRequestAuditLogEntry logEntry)
        {
            var content = new StringContent(JsonSerializer.Serialize(logEntry), Encoding.UTF8, "application/json");
            return _client.PostAsync(_options.Endpoint, content);
        }
        
        
        private readonly HttpClient _client;
        private readonly SenderOptions _options;
    }
}