using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using time_track_service.Model;
using time_track_service.Model.Dto.Legacy;
using time_track_service.Model.Legacy;

namespace time_track_service.Services
{
    public interface IMemberTimeTrackingService
    {
        Task<(DbResponse response, LegacyMemberTimeTrackingsModel data)> GetTimeTrackingsAsync(Guid memberGuid, CancellationToken cancellationToken = default);
        Task<(DbResponse response, LegacyMemberTimeTrackingModel data)> GetTimeTrackingAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(DbResponse response, LegacyMemberTimeTrackingModel data)> SaveTimeTrackingAsync(Guid memberGuid, LegacyMemberTimeTrackingModel legacyTimeTrack, CancellationToken cancellationToken = default);
    }
}
