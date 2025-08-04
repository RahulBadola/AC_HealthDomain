using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_service.Model;
using time_track_service.Model.Dto;

namespace time_track_service.Data
{
    public interface ITimeTrackRepository
    {
        Task<DbResponse> DeleteTimeTrackAsync(Guid id);
        Task<(DbResponse response, TimeTrack data)> GetTimeTrackAsync(Guid id);
        Task<(DbResponse response, IEnumerable<TimeTrack> data)> GetTimeTrackBySecurityUserIdAsync(Guid securityUserId);
        Task<(DbResponse response, IEnumerable<TimeTrack> data)> GetTimeTracksAsync(bool includeInactive = false);
        Task<DbResponse> InsertTimeTrackAsync(TimeTrack timeTrack);
        Task<DbResponse> InsertTimeTracksAsync(IList<TimeTrack> timeTracks);
        Task<DbResponse> OverwriteTimeTrackAsync(TimeTrack timeTrack);
        Task<DbResponse> UpdateTimeTrackAsync(TimeTrack timeTrack, int currentVersion);
        Task<DbResponse> VoidTimeTrackAsync(Guid id, Guid? voidReasonId, Guid? voidedBy, Guid? voidedById, DateTime voidedOn);
        Task<(DbResponse response, List<TimeTrack> data)> GetTimeTrackByMemberIdAsync(Guid memberId);
    }
}