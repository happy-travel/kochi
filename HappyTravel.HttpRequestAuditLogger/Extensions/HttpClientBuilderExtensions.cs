using System;
using System.Threading.Channels;
using HappyTravel.HttpRequestAuditLogger.Models;
using HappyTravel.HttpRequestAuditLogger.Options;
using HappyTravel.HttpRequestAuditLogger.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.HttpRequestAuditLogger.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static void AddHttpRequestAudit(this IHttpClientBuilder builder, Action<RequestAuditLoggerOptions> options)
        {
            builder.Services.Configure(options);
            builder.Services.AddHostedService<LogSendingService>();
            builder.Services.AddSingleton(Channel.CreateUnbounded<HttpRequestAuditLogEntry>(new UnboundedChannelOptions { SingleReader = true }));
            builder.Services.AddSingleton(svc => svc.GetRequiredService<Channel<HttpRequestAuditLogEntry>>().Reader);
            builder.Services.AddSingleton(svc => svc.GetRequiredService<Channel<HttpRequestAuditLogEntry>>().Writer);
            builder.Services.AddTransient<AuditLoggingHandler>();
            builder.AddHttpMessageHandler<AuditLoggingHandler>();
        }
    }
}