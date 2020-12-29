using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace HappyTravel.SupplierRequestLogger.Extensions
{
    public static class HeadersExtensions
    {
        public static Dictionary<string, string> ToDictionary(this HttpHeaders headers)
        {
            return headers.ToDictionary(h => h.Key, 
                h => string.Join(";", h.Value));
        }
    }
}