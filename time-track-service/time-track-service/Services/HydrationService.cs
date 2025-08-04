using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;

namespace time_track_service.Services
{
    public class HydrationService : IHydrationService
    {
        private readonly ILogger<HydrationService> _logger;
        private readonly ITimeTrackRepository _repository;
        private readonly ISyncBackNotificationService _updateNotificationService;

        public HydrationService(
            ILogger<HydrationService> logger,
            ITimeTrackRepository repository, 
            ISyncBackNotificationService updateNotificationService)
        {
            _logger = logger;
            _repository = repository;
            _updateNotificationService = updateNotificationService;
        }
 
        public async Task<DbResponse> InsertTimeTracksAsync(IList<TimeTrack> timeTracks)
        {
            _logger.LogDebug("HydrationService - InsertTimeTracksAsync Begin");

            return await _repository.InsertTimeTracksAsync(timeTracks);
        }

        public async Task<DbResponse> UpdateTimeTrackAsync(Guid id, TimeTrack timeTrack)
        {
            _logger.LogDebug("HydrationService - UpdateTimeTrackAsync Begin");

            if (id == Guid.Empty)
            {
                _logger.LogError($"HydrationService - Object Id {id} in route should not be null");
                return DbResponse.Invalid;
            }

            if (timeTrack.Id != Guid.Empty && timeTrack.Id != id)
            {
                _logger.LogError($"HydrationService - Object Primary Id {timeTrack.Id} should match {id} in route or be null");
                return DbResponse.Invalid;
            }

            // Pull the current record to be updated
            var persisted = await _repository.GetTimeTrackAsync(timeTrack.Id);
            if (persisted.response == DbResponse.NotFound) return persisted.response;

            _logger.LogDebug("UpdateTimeTrackAsync - Persisted TimeTrack Found");

            // If the persisted record is newer, do not update
            if (persisted.data.UpdatedOn > timeTrack.UpdatedOn) return DbResponse.Invalid;

            _logger.LogDebug("UpdateTimeTrackAsync - Persisted TimeTrack Is older");

            var response = await _repository.OverwriteTimeTrackAsync(timeTrack);
            return response;
        }
        
    }
}
