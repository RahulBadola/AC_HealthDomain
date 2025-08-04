using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Utils;
using AssureCare.MedCompass.DataAuthorization.Services;
using time_track_service.Extensions;
using System.Threading;

namespace time_track_service.Services
{
    public class SharedService : ISharedService
    {
        private readonly ContextLogger<SharedService> _logger;
        private readonly IFieldAuthorizationService _fieldAuthorizationService;
        private readonly IOperationAuthorizationService _operationAuthorizationService;
        private readonly ITimeTrackRepository _repository;
        private readonly IRequestContextAccessor _requestContextAccessor;
        private readonly ISyncBackNotificationService _syncBackNotificationService;

        public SharedService(
            ContextLogger<SharedService> logger,
            IFieldAuthorizationService fieldAuthorizationService,
            IOperationAuthorizationService operationAuthorizationService,
            ITimeTrackRepository repository,
            IRequestContextAccessor requestContextAccessor,
            ISyncBackNotificationService syncBackNotificationService
            )
        {
            _logger = logger;
            _fieldAuthorizationService = fieldAuthorizationService;
            _operationAuthorizationService = operationAuthorizationService;
            _repository = repository;
            _requestContextAccessor = requestContextAccessor;
            _syncBackNotificationService = syncBackNotificationService;
        }

        public void SetInsertDefaults(MedCompassBase medCompassBase)
        {
            var now = DateTime.UtcNow;
            medCompassBase.TenantId = _requestContextAccessor.RequestContext.TenantId;
            medCompassBase.SegmentId = _requestContextAccessor.RequestContext.SegmentId;
            medCompassBase.InsertedById = _requestContextAccessor.RequestContext.UserId;
            medCompassBase.InsertedBy = _requestContextAccessor.RequestContext.UserContextId;
            medCompassBase.InsertedByName = _requestContextAccessor.RequestContext.FullName;
            medCompassBase.UpdatedById = _requestContextAccessor.RequestContext.UserId;
            medCompassBase.UpdatedBy = _requestContextAccessor.RequestContext.UserContextId;
            medCompassBase.UpdatedByName = _requestContextAccessor.RequestContext.FullName;
            medCompassBase.InsertedOn = now;
            medCompassBase.UpdatedOn = now;
            medCompassBase.Version = 0;
        }

        public void SetUpdateDefaults(MedCompassBase medCompassBase)
        {
            var now = DateTime.UtcNow;
            medCompassBase.TenantId = _requestContextAccessor.RequestContext.TenantId;
            medCompassBase.SegmentId = _requestContextAccessor.RequestContext.SegmentId;
            medCompassBase.UpdatedById = _requestContextAccessor.RequestContext.UserId;
            medCompassBase.UpdatedBy = _requestContextAccessor.RequestContext.UserContextId;
            medCompassBase.UpdatedByName = _requestContextAccessor.RequestContext.FullName;
            medCompassBase.UpdatedOn = now;
        }

        public async Task<(DbResponse response, TimeTrack data)> UpdateTimeTrackAsync(TimeTrack timeTrack, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug($"SharedService - UpdateTimeTrackAsync Begin");

            // Pull the current record to be updated
            var persisted = await _repository.GetTimeTrackAsync(timeTrack.Id);

            // Abort if there isn't a match
            _logger.LogDebug("UpdateTimeTrackAsync no matching timeTrack to update");
            if (persisted.response != DbResponse.Found) return (DbResponse.NotFound, null);

            _ = _fieldAuthorizationService.SecureForUpdateAsync(timeTrack, persisted.data, cancellationToken);
            var permitted = await this._operationAuthorizationService.CheckPermissionAsync(timeTrack, persisted.data, cancellationToken);
            if (!permitted) return (DbResponse.Forbidden, null);

            // If version found and deosn't match with the incoming version then there's a conflict
            if (timeTrack.Version.HasValue && timeTrack.Version != persisted.data.Version) return (DbResponse.Conflict, null);

            // Ensure that existing values are kept
            timeTrack.TenantId = persisted.data.TenantId;
            timeTrack.SegmentId = persisted.data.SegmentId;
            timeTrack.InsertedById = persisted.data.InsertedById;
            timeTrack.InsertedBy = persisted.data.InsertedBy;
            timeTrack.InsertedOn = persisted.data.InsertedOn;

            SetUpdateDefaults(timeTrack);

            (DbResponse response, TimeTrack timeTrack) result = await ProcessTimeTrackUpdateAsync(timeTrack, persisted.data);

            // Send an update back to the DSHP processor
            var updateResult = await _syncBackNotificationService.SyncBackAsync(SyncBackOperations.Update, result.timeTrack, persisted.data);
            if (!updateResult)
            {
                _logger.LogError("UpdateTimeTrackAsync - Failed sync back - reverting timeTrack");

                // Revert the original Mongo change and return an error
                await _repository.OverwriteTimeTrackAsync(persisted.data);
                return (DbResponse.Reverted, persisted.data);
            }

            return (result.response, result.timeTrack);
        }



        private async Task<(DbResponse, TimeTrack)> ProcessTimeTrackUpdateAsync(TimeTrack submittedTimeTrack, TimeTrack persistedTimeTrack)
        {
            _logger.LogDebug("TimeTrackService - ProcessUpdateAsync Begin");

            // Each timeTrack record in Mongo contains a version. This method is essentially here to ensure that if the same record is being written from another source,
            // we don't squash it with this version right after it is written
            _logger.LogDebug($"ProcessUpdateAsync - Current Version:{persistedTimeTrack.Version}");
            var currentVersion = persistedTimeTrack.Version ?? 0;
            var newVersion = currentVersion + 1;

            submittedTimeTrack.Version = newVersion;

            // Attempt an update by finding and updating the current version.
            // If the current version is not found to update, then the outer loop of this request re-pulls current version
            var response = await _repository.UpdateTimeTrackAsync(submittedTimeTrack, currentVersion);

            if (response == DbResponse.Updated)
            {
                _logger.LogDebug("ProcessUpdateAsync - Successful Update");
                return (response, submittedTimeTrack);
            }

            return (response, null);
        }

    }
}
