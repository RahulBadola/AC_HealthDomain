using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_service.Utils.DeepCopy;
using AssureCare.MedCompass.DataAuthorization.Services;
using time_track_service.Extensions;
using System.Threading;
using AssureCare.MedCompass.DataAuthorization.Models;

namespace time_track_service.Services
{
    public class DomainService : IDomainService
    {
        private readonly ITimeTrackRepository _repository;
        private readonly ILogger<DomainService> _logger;
        private readonly IRequestContextAccessor _requestContextAccessor;
        private readonly ISharedService _sharedService;
        private readonly ISyncBackNotificationService _updateNotificationService;
        private readonly IFieldAuthorizationService _fieldAuthorizationService;
        private readonly IOperationAuthorizationService _operationAuthorizationService;

        public DomainService(
            IFieldAuthorizationService fieldAuthorizationService,
            ILogger<DomainService> logger,
            IOperationAuthorizationService operationAuthorizationService,
            ITimeTrackRepository repository,
            IRequestContextAccessor requestContextAccessor, 
            ISharedService sharedService,
            ISyncBackNotificationService updateNotificationService)
        {
            _fieldAuthorizationService = fieldAuthorizationService;
            _logger = logger;
            _operationAuthorizationService = operationAuthorizationService;
            _repository = repository;
            _requestContextAccessor = requestContextAccessor;
            _sharedService = sharedService;
            _updateNotificationService = updateNotificationService;
        }
        
        public async Task<(DbResponse response, TimeTrack data)> InsertTimeTrackAsync(TimeTrack timeTrack, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("DomainService - InsertTimeTrackAsync Begin");

            _ = _fieldAuthorizationService.SecureForInsertAsync(timeTrack, cancellationToken);
            var permitted = await this._operationAuthorizationService.CheckPermissionAsync(timeTrack, null, cancellationToken);
            if (!permitted) return (DbResponse.Forbidden, null);


            _sharedService.SetInsertDefaults(timeTrack);
            var result = await _repository.InsertTimeTrackAsync(timeTrack);

            // Send an update back to the DSHP processor
            var updateResult = await _updateNotificationService.SyncBackAsync(SyncBackOperations.Insert, timeTrack, null);
            if (!updateResult)
            {
                _logger.LogError("InsertTimeTrackAsync - Failed sync back - reverting timeTrack");

                // Revert the original Mongo change and return an error
                await _repository.DeleteTimeTrackAsync(timeTrack.Id);
                return (DbResponse.Reverted, null);
            }

            return (result, timeTrack);
        }

        public async Task<(DbResponse response, IEnumerable<TimeTrack> data)> ReadTimeTracksAsync(bool includeInactive, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("DomainService - ReadTimeTracksAsync Begin");

            return await _repository.GetTimeTracksAsync(includeInactive);
        }

        public async Task<(DbResponse response, TimeTrack data)> ReadTimeTrackByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("DomainService - ReadTimeTrackByIdAsync Begin");

            return await _repository.GetTimeTrackAsync(id);
        }

        public async Task<(DbResponse response, TimeTrack data)> UpdateTimeTrackByIdAsync(Guid id, TimeTrack timeTrack, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("DomainService - UpdateTimeTrackByIdAsync Begin");

            if (id==Guid.Empty)
            {
                _logger.LogError($"DomainService - Object Id {id} in route should not be null");
                return (DbResponse.Invalid, null);
            }

            if (timeTrack.Id != Guid.Empty && timeTrack.Id != id)
            {
                _logger.LogError($"DomainService - Object Primary Id {timeTrack.Id} should match {id} in route or be null");
                return (DbResponse.Invalid, null);
            }
            timeTrack.Id = id;
            
            return await _sharedService.UpdateTimeTrackAsync(timeTrack);
        }

        public async Task<DbResponse> VoidTimeTrackByIdAsync(Guid id, Guid voidReasonId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("DomainService - VoidTimeTrackByIdAsync Begin");

            var original = await ReadTimeTrackByIdAsync(id);
            var updated = original.data.Copy();
            updated.VoidedReasonId = voidReasonId;
            updated.VoidedBy = _requestContextAccessor.RequestContext.UserId;
            updated.VoidedById = _requestContextAccessor.RequestContext.UserContextId;
            updated.VoidedOn = DateTime.UtcNow;
            var permitted = await _operationAuthorizationService.CheckOperationAsync(
               original.data, Operation.Void, original.data.SegmentId, cancellationToken);
            if (!permitted) return DbResponse.Forbidden;

            var result = await _repository.VoidTimeTrackAsync(id, updated.VoidedReasonId, updated.VoidedBy, updated.VoidedById, (DateTime)updated.VoidedOn);
            
            // Send an update back to the DSHP processor
            var updateResult = await _updateNotificationService.SyncBackAsync(SyncBackOperations.Update, updated, original.data);
            if (!updateResult)
            {
                _logger.LogError("InsertTimeTrackAsync - Failed sync back - reverting timeTrack");

                // Revert the original Mongo change and return an error
                await _repository.OverwriteTimeTrackAsync(original.data);
                return DbResponse.Reverted;
            }

            return result;
        }

        
    }
}
