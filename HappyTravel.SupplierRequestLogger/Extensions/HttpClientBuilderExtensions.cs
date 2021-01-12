using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.SupplierRequestLogger.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static void AddHttpRequestLoggingHandler(this IHttpClientBuilder builder)
        {
            builder.Services.AddOptions<RequestLoggerOptions>();
            builder.Services.AddTransient<LoggingHandler>();
            builder.AddHttpMessageHandler<LoggingHandler>();
        }
    }
}