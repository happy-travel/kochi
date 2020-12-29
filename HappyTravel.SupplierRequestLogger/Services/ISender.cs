using System.Threading.Tasks;

namespace HappyTravel.SupplierRequestLogger.Services
{
    public interface ISender
    {
        Task Send(HttpRequestAuditLogEntry logEntry);
    }
}