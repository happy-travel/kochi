using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.SupplierRequestLogger.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static void AddHttpRequestLoggingHandler(this IHttpClientBuilder builder)
        {
            builder.Services.AddTransient<LoggingHandler>();
            builder.Services.AddOptions<SenderOptions>();
            builder.AddHttpMessageHandler<LoggingHandler>();
        }
    }
}