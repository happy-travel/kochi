using System;
using HappyTravel.SupplierRequestLogger.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HappyTravel.SupplierRequestLogger.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSupplierRequestLogSender(this IServiceCollection collection, IOptions<SenderOptions> options)
        {
            collection.AddHttpClient<ISender, Sender>(o =>
            {
                o.BaseAddress = new Uri(options.Value.Url);
            });
        }
    }
}