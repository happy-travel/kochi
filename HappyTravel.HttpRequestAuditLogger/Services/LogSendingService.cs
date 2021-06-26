using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using HappyTravel.HttpRequestAuditLogger.Models;
using HappyTravel.HttpRequestAuditLogger.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace HappyTravel.HttpRequestAuditLogger.Services
{
    public class LogSendingService : BackgroundService
    {
        public LogSendingService(ChannelReader<HttpRequestAuditLogEntry> channel, IHttpClientFactory httpClientFactory, 
            IOptions<RequestAuditLoggerOptions> options, ILogger<LogSendingService> logger)
        {
            _channel = channel;
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _logger = logger;
        }
        
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Use the channel as a buffer so as not to lose messages
            while (await _channel.WaitToReadAsync(cancellationToken))
            {
                while (_channel.TryRead(out var logEntry))
                {
                    using var client = _httpClientFactory.CreateClient();
                    var content = new StringContent(JsonSerializer.Serialize(logEntry), Encoding.UTF8, "application/json");
                    
                    await GetPolicy().ExecuteAsync(async () =>
                    {
                        var result = await client.PostAsync(_options.Endpoint, content, cancellationToken);
                        result.EnsureSuccessStatusCode();
                    });
                }
            }
        }
        
        
        private AsyncRetryPolicy GetPolicy()
            => Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    _ => TimeSpan.FromSeconds(DelayInSeconds),
                    (exception, _) => _logger.LogCritical(exception, "Error when sending a log entry to Fukuoka")
                    );
        

        private const int DelayInSeconds = 5;

        
        private readonly ChannelReader<HttpRequestAuditLogEntry> _channel;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RequestAuditLoggerOptions _options;
        private readonly ILogger<LogSendingService> _logger;
    }
}