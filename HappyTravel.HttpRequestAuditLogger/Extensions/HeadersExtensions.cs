﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace HappyTravel.HttpRequestAuditLogger.Extensions
{
    public static class HeadersExtensions
    {
        internal static Dictionary<string, string> ToDictionary(this HttpHeaders headers)
        {
            return headers.ToDictionary(h => h.Key, 
                h => string.Join(";", h.Value));
        }
    }
}