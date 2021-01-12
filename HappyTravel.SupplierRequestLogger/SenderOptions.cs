using JetBrains.Annotations;

namespace HappyTravel.SupplierRequestLogger
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class SenderOptions
    {
        public string Endpoint { get; set; } = string.Empty;
    }
}