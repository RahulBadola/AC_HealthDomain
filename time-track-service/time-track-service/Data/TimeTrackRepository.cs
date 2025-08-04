using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using time_track_service.Context;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Utils;

namespace time_track_service.Data
{
    public class TimeTrackRepository : ITimeTrackRepository
    {
        private readonly IDBContext _dbContext;
        private readonly ContextLogger<TimeTrack> _contextLogger;
        private readonly IRequestContextAccessor _requestContextAccessor;

        public TimeTrackRepository(IDBContext dbContext, ContextLogger<TimeTrack> contextLogger, IRequestContextAccessor requestContextAccessor)
        {
            _dbContext = dbContext;
            _contextLogger = contextLogger;
            _requestContextAccessor = requestContextAccessor;
        }
        public async Task<DbResponse> InsertTimeTrackAsync(TimeTrack timeTrack)
        {
            _contextLogger.LogInformation($"Repository Service - InsertTimeTrackAsync Begin - TimeTrackId:{timeTrack.Id}");
            timeTrack.TenantId = _requestContextAccessor.RequestContext.TenantId;
            timeTrack.SegmentId = _requestContextAccessor.RequestContext.SegmentId;

            try
            {
                await _dbContext.TimeTrackCollection.InsertOneAsync(timeTrack);
                return DbResponse.Inserted;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - InsertTimeTrackAsync");
                return DbResponse.Error;
            }
        }

        public async Task<DbResponse> InsertTimeTracksAsync(IList<TimeTrack> timeTracks)
        {
            _contextLogger.LogInformation($"Repository Service - InsertTimeTracksAsync Begin - Count:{timeTracks.Count}");
            foreach (var timeTrack in timeTracks)
            {
                timeTrack.TenantId = _requestContextAccessor.RequestContext.TenantId;
                timeTrack.SegmentId = _requestContextAccessor.RequestContext.SegmentId;
            }
            
            try
            {
                await _dbContext.TimeTrackCollection.InsertManyAsync(timeTracks, new InsertManyOptions { IsOrdered = true });
                return DbResponse.Inserted;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - InsertTimeTracksAsync");
                return DbResponse.Error;
            }
        }

        public async Task<(DbResponse response, IEnumerable<TimeTrack> data)> GetTimeTracksAsync(bool includeInactive = false)
        {
            _contextLogger.LogInformation($"Repository Service - GetTimeTracksAsync Begin - IncludeInactive:{includeInactive}");

            var builder = Builders<TimeTrack>.Filter;
            var filter = builder.Eq(timeTrack => timeTrack.SegmentId, _requestContextAccessor.RequestContext.SegmentId)
                         & builder.Eq(timeTrack => timeTrack.TenantId, _requestContextAccessor.RequestContext.TenantId);

            if (!includeInactive)
            {
                filter &= Common.GetActiveFilterDefinition<TimeTrack>();
            }

            try
            {
                var result = await _dbContext.TimeTrackCollection.Find(filter).ToListAsync();
                if (result == null || !result.Any())
                {
                    return (DbResponse.NotFound, null);
                }

                return (DbResponse.Found, result);
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - GetTimeTracksAsync");
                return (DbResponse.Error, null);
            }
        }

        public async Task<(DbResponse response, TimeTrack data)> GetTimeTrackAsync(Guid id)
        {
            _contextLogger.LogInformation($"Repository Service - GetTimeTrackAsync Begin - TimeTrackId:{id}");

            var builder = Builders<TimeTrack>.Filter;
            var filter = builder.Eq(timeTrack => timeTrack.Id, id)
                         & builder.Eq(timeTrack => timeTrack.SegmentId, _requestContextAccessor.RequestContext.SegmentId)
                         & builder.Eq(timeTrack => timeTrack.TenantId, _requestContextAccessor.RequestContext.TenantId);

            try
            {
                var result = await _dbContext.TimeTrackCollection.Find(filter).FirstOrDefaultAsync();

                if (result == null)
                {
                    return (DbResponse.NotFound, null);
                }

                return (DbResponse.Found, result);
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - GetTimeTrackAsync");
                return (DbResponse.Error, null);
            }
        }
        public async Task<(DbResponse response, IEnumerable<TimeTrack> data)> GetTimeTrackBySecurityUserIdAsync(Guid securityUserId)
        {
            _contextLogger.LogInformation($"Repository Service - GetTimeTrackAsync Begin - SecurityUserId:{securityUserId}");

            var builder = Builders<TimeTrack>.Filter;
            var filter = builder.Eq(timeTrack => timeTrack.SecurityUserId, securityUserId)
                         & builder.Eq(timeTrack => timeTrack.SegmentId, _requestContextAccessor.RequestContext.SegmentId)
                         & builder.Eq(timeTrack => timeTrack.TenantId, _requestContextAccessor.RequestContext.TenantId);
            
            filter &= Common.GetActiveFilterDefinition<TimeTrack>();
            try
            {
                var result = await _dbContext.TimeTrackCollection.Find(filter).ToListAsync();
                if (result == null || !result.Any())
                {
                    return (DbResponse.NotFound, null);
                }

                return (DbResponse.Found, result);
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - GetTimeTrackBySecurityUserIdAsync");
                return (DbResponse.Error, null);
            }
        }
        public async Task<DbResponse> UpdateTimeTrackAsync(TimeTrack timeTrack, int currentVersion)
        {
            _contextLogger.LogInformation($"Repository Service - UpdateTimeTrackAsync Begin - TimeTrackId:{timeTrack.Id}");

            var builder = Builders<TimeTrack>.Filter;
            var filter = builder.Eq(updateTimeTrack => updateTimeTrack.Id, timeTrack.Id)
                         & builder.Eq(updateTimeTrack => updateTimeTrack.Version, currentVersion)
                         & builder.Eq(updateTimeTrack => updateTimeTrack.SegmentId, _requestContextAccessor.RequestContext.SegmentId)
                         & builder.Eq(updateTimeTrack => updateTimeTrack.TenantId, _requestContextAccessor.RequestContext.TenantId);

            try
            {
                var result = await _dbContext.TimeTrackCollection.ReplaceOneAsync(filter, timeTrack, new ReplaceOptions { IsUpsert = false });

                if (!result.IsAcknowledged)
                {
                    _contextLogger.LogError("Error in RepositoryService - UpdateTimeTrackAsync - Request not acknowledged");
                    return DbResponse.Error;
                }

                if (result.IsAcknowledged && result.ModifiedCount == 0)
                {
                    return DbResponse.NotFound;
                }

                return DbResponse.Updated;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - UpdateTimeTrackAsync");
                return DbResponse.Error;
            }
        }

        public async Task<DbResponse> OverwriteTimeTrackAsync(TimeTrack timeTrack)
        {
            _contextLogger.LogInformation($"Repository Service - OverwriteTimeTrackAsync Begin - TimeTrackId:{timeTrack.Id}");

            var builder = Builders<TimeTrack>.Filter;
            var filter = builder.Eq(updateTimeTrack => updateTimeTrack.Id, timeTrack.Id)
                         & builder.Eq(updateTimeTrack => updateTimeTrack.SegmentId, _requestContextAccessor.RequestContext.SegmentId)
                         & builder.Eq(updateTimeTrack => updateTimeTrack.TenantId, _requestContextAccessor.RequestContext.TenantId);

            try
            {
                var result = await _dbContext.TimeTrackCollection.ReplaceOneAsync(filter, timeTrack, new ReplaceOptions { IsUpsert = false });

                if (!result.IsAcknowledged)
                {
                    _contextLogger.LogError("Error in RepositoryService - OverwriteTimeTrackAsync - Request not acknowledged");
                    return DbResponse.Error;
                }

                if (result.IsAcknowledged && result.ModifiedCount == 0)
                {
                    return DbResponse.NotFound;
                }

                return DbResponse.Updated;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - OverwriteTimeTrackAsync");
                return DbResponse.Error;
            }
        }

        public async Task<DbResponse> DeleteTimeTrackAsync(Guid id)
        {
            _contextLogger.LogInformation($"Repository Service - DeleteTimeTrackAsync Begin - Id:{id}");

            var builder = Builders<TimeTrack>.Filter;
            var filter = builder.Eq(updateTimeTrack => updateTimeTrack.Id, id)
                         & builder.Eq(updateTimeTrack => updateTimeTrack.SegmentId, _requestContextAccessor.RequestContext.SegmentId)
                         & builder.Eq(updateTimeTrack => updateTimeTrack.TenantId, _requestContextAccessor.RequestContext.TenantId);

            try
            {
                var result = await _dbContext.TimeTrackCollection.DeleteOneAsync(filter);

                if (result.IsAcknowledged && result.DeletedCount == 1)
                {
                    return DbResponse.Deleted;
                }

                return DbResponse.NotFound;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - DeleteTimeTrackAsync");
                return DbResponse.Error;
            }
        }

        public async Task<DbResponse> VoidTimeTrackAsync(Guid id, Guid? voidReasonId, Guid? voidedBy, Guid? voidedById, DateTime voidedOn)
        {
            _contextLogger.LogInformation($"Repository Service - VoidTimeTrackAsync Begin - Id:{id}");

            var builder = Builders<TimeTrack>.Filter;
            var filter = builder.Eq(updateTimeTrack => updateTimeTrack.Id, id)
                         & builder.Eq(updateTimeTrack => updateTimeTrack.SegmentId, _requestContextAccessor.RequestContext.SegmentId)
                         & builder.Eq(updateTimeTrack => updateTimeTrack.TenantId, _requestContextAccessor.RequestContext.TenantId);
            var updateBuilder = Builders<TimeTrack>.Update;
            var update = updateBuilder.Set(updateTimeTrack => updateTimeTrack.VoidedReasonId, voidReasonId)
                    .Set(updateTimeTrack => updateTimeTrack.VoidedBy, voidedBy)
                    .Set(updateTimeTrack => updateTimeTrack.VoidedById, voidedById)
                    .Set(updateTimeTrack => updateTimeTrack.VoidedOn, voidedOn);

            try
            {
                var updateResult = await _dbContext.TimeTrackCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });

                if (updateResult.IsAcknowledged && updateResult.ModifiedCount == 1)
                {
                    return DbResponse.Updated;
                }

                return DbResponse.NotFound;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - VoidTimeTrackAsync");
                return DbResponse.Error;
            }
        }
        public async Task<(DbResponse response, List<TimeTrack> data)> GetTimeTrackByMemberIdAsync(Guid memberId)
        {
            _contextLogger.LogInformation($"Repository Service - GetTimeTrackByMemberIdAsync Begin - memberId:{memberId}");

            var builder = Builders<TimeTrack>.Filter;
            var filter = builder.Eq(timeTrack => timeTrack.MemberId, memberId)
                         & builder.Eq(timeTrack => timeTrack.SegmentId, _requestContextAccessor.RequestContext.SegmentId)
                         & builder.Eq(timeTrack => timeTrack.TenantId, _requestContextAccessor.RequestContext.TenantId);
               
            filter &= Common.GetActiveFilterDefinition<TimeTrack>();

            try
            {
                var result = await _dbContext.TimeTrackCollection.Find(filter).ToListAsync();

                if (result == null && result.Any())
                {
                    return (DbResponse.NotFound, null);
                }

                return (DbResponse.Found, result);
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - GetTimeTrackByMemberIdAsync");
                return (DbResponse.Error, null);
            }
        }
    }
}
