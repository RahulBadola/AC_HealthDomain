using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;

namespace time_track_service.Services
{
    public class SyncService : ISyncService
    {
        private readonly ILogger<HydrationService> _logger;
        private readonly IGenericRepository _repository;

        public SyncService(ILogger<HydrationService> logger, IGenericRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<bool> UpsertAsync(List<TimeTrack> entities, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"{nameof(SyncService)} - {nameof(UpsertAsync)} Begin");

            // Pull the current record to be updated
            var persisted = await _repository.ReadByIdAsync<TimeTrack>(entities.Select(x => x.Id).ToList(), cancellationToken, true);

            _logger.LogDebug($"UpdateAsync - {persisted.data?.Count ?? 0} Persisted Found of {entities.Count} searched");

            // If the record in the system is newer, do not update
            var leaveAlone = new List<TimeTrack>();
            var toUpdate = new List<TimeTrack>();
            var toInsert = new List<TimeTrack>();

            foreach (var entity in entities)
            {
                var dbRecord = persisted.data?.FirstOrDefault(x => x.Id == entity.Id);
                if (dbRecord != null)
                {
                    if (dbRecord.UpdatedOn > entity.UpdatedOn)
                    {
                        leaveAlone.Add(entity);
                    }
                    else
                    {
                        entity.Version = dbRecord.Version + 1;
                        entity.InsertedByName = dbRecord.InsertedByName;
                        toUpdate.Add(entity);
                    }
                }
                else
                {
                    //record doesnt exist in the table. lets add it.
                    entity.Version = 1;
                    toInsert.Add(entity);
                }
            }

            toUpdate.RemoveAll(x => leaveAlone.Select(y => y.Id).Contains(x.Id));

            _logger.LogDebug($"{nameof(UpsertAsync)} - {toInsert.Count} to Insert, {toUpdate.Count} to Update, and {leaveAlone.Count} to leave alone.");

            bool returnValue = true;

            if (toInsert.Any())
            {
                var insertResponse = await _repository.UpsertAsync<TimeTrack>(toInsert, cancellationToken);
                if (insertResponse == DbResponse.Error) returnValue = false;
            }

            if (toUpdate.Any())
            {
                var updateResponse = await _repository.UpsertAsync<TimeTrack>(toUpdate, cancellationToken);
                if (updateResponse == DbResponse.Error) returnValue = false;
            }

            return returnValue;
        }
    }
}
