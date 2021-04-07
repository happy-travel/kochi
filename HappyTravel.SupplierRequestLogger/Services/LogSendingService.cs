using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using HappyTravel.SupplierRequestLogger.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace HappyTravel.SupplierRequestLogger.Services
{
    public class LogSendingService : BackgroundService
    {
        public LogSendingService(
            ChannelReader<RequestLoggerOptions> channel, 
            IHttpClientFactory httpClientFactory, 
            IOptions<RequestLoggerOptions> options,
            ILogger<LogSendingService> logger)
        {
            _channel = channel;
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _logger = logger;
        }
        
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var policy = Policy
                .Handle<Exception>()
                .RetryForeverAsync(exception =>
                {
                    _logger.LogCritical(exception, "Error when sending log entry to Fukuoka");
                });

            while (await _channel.WaitToReadAsync(cancellationToken))
            {
                while (_channel.TryRead(out var logEntry))
                {
                    await policy.ExecuteAsync(async () =>
                    {
                        using var client = _httpClientFactory.CreateClient();
                        var content = new StringContent(JsonSerializer.Serialize(logEntry), Encoding.UTF8, "application/json");
                        await client.PostAsync(_options.Endpoint, content, cancellationToken);
                    });
                }
            }
        }

        
        private readonly ChannelReader<RequestLoggerOptions> _channel;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RequestLoggerOptions _options;
        private readonly ILogger<LogSendingService> _logger;
    }
}