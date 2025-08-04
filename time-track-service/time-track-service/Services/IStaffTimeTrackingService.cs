using System;
using System.Threading;
using System.Threading.Tasks;
using time_track_service.Model;
using time_track_service.Model.Dto.Legacy;

namespace time_track_service.Services
{
    public interface IStaffTimeTrackingService
    {
        Task<(DbResponse response, LegacyTimeTrackingsModel data)> GetTimeTrackingsAsync(Guid staffId, CancellationToken cancellationToken = default);
        Task<(DbResponse response, LegacyTimeTrackingModel data)> GetTimeTrackingAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(DbResponse response, LegacyTimeTrackingModel data)> SaveTimeTrackingAsync(Guid staffGuid, LegacyTimeTrackingModel legacyTimeTrack, CancellationToken cancellationToken = default);
        Task<(DbResponse response, bool data)> IsRequireStartAndEndDatesAsync(CancellationToken cancellationToken = default);
        Task<(DbResponse response, int data)> SelectServiceUnitMaxValueAsync(CancellationToken cancellationToken = default);
    }
}
