﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace HappyTravel.SupplierRequestLogger
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public record HttpRequestAuditLogEntry
    {
        public string Url { get; init; }
        public Dictionary<string, string>? RequestHeaders { get; init; }
        public Dictionary<string, string>? ResponseHeaders { get; init; }
        public string? RequestBody { get; init; }
        public string? ResponseBody { get; init; }
        public int StatusCode { get; init; }
        public string? ReferenceCode { get; init; }
        public string? Error { get; init; }
    }
}