using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_service.Model;
using time_track_service.Model.Dto;

namespace time_track_service.Services
{
    public interface IHydrationService
    {

        Task<DbResponse> InsertTimeTracksAsync(IList<TimeTrack> timeTracks);

        Task<DbResponse> UpdateTimeTrackAsync(Guid id, TimeTrack timeTrack);
        
    }
}
