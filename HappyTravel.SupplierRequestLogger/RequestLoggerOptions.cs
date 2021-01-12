using JetBrains.Annotations;

namespace HappyTravel.SupplierRequestLogger
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class RequestLoggerOptions
    {
        public string Endpoint { get; set; } = string.Empty;
    }
}