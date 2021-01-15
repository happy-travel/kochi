using System;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.SupplierRequestLogger.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static void AddHttpRequestLoggingHandler(this IHttpClientBuilder builder, Action<RequestLoggerOptions> options)
        {
            builder.Services.Configure(options);
            builder.Services.AddTransient<LoggingHandler>();
            builder.AddHttpMessageHandler<LoggingHandler>();
        }
    }
}