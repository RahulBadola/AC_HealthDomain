using time_track_service.Model;
using time_track_service.Model.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace time_track_service.Services
{
    public interface IDomainService
    {
        
        Task<(DbResponse response, TimeTrack data)> InsertTimeTrackAsync(TimeTrack timeTrack, CancellationToken cancellationToken = default);
		Task<(DbResponse response, IEnumerable<TimeTrack> data)> ReadTimeTracksAsync(bool includeInactive, CancellationToken cancellationToken = default);
		Task<(DbResponse response, TimeTrack data)> ReadTimeTrackByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<(DbResponse response, TimeTrack data)> UpdateTimeTrackByIdAsync(Guid id, TimeTrack timeTrack, CancellationToken cancellationToken = default);
		Task<DbResponse> VoidTimeTrackByIdAsync(Guid id, Guid voidReasonId, CancellationToken cancellationToken = default);

    }
}
