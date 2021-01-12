using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HappyTravel.SupplierRequestLogger.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static void AddHttpRequestLoggingHandler(this IHttpClientBuilder builder, Func<HttpRequestMessage, bool> predicate)
        {
            builder.Services.AddOptions<RequestLoggerOptions>();
            builder.Services.AddTransient(provider 
                => new LoggingHandler(
                    provider.GetRequiredService<IHttpClientFactory>(), 
                    provider.GetRequiredService<IOptions<RequestLoggerOptions>>(),
                    predicate)
            );
            builder.AddHttpMessageHandler<LoggingHandler>();
        }
    }
}