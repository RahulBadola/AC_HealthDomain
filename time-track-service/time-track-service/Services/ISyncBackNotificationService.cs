using time_track_service.Model;
using System.Threading.Tasks;

namespace time_track_service.Services
{
    public interface ISyncBackNotificationService
    {
        Task<bool> SyncBackAsync<T>(SyncBackOperations operation, T newObject, T originalObject);
    }
}