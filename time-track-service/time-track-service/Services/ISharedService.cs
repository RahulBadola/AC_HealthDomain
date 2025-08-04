using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using time_track_service.Model;
using time_track_service.Model.Dto;

namespace time_track_service.Services
{
    public interface ISharedService
    {
        void SetInsertDefaults(MedCompassBase medCompassBase);
        void SetUpdateDefaults(MedCompassBase medCompassBase);

        Task<(DbResponse response, TimeTrack data)> UpdateTimeTrackAsync(TimeTrack timeTrack, CancellationToken cancellationToken = default);

    }
}
