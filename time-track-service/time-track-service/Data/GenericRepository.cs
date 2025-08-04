using time_track_service.Context;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Utils;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using time_track_service.Data;

namespace member_allergy_service.Data
{
    public class GenericRepository : IGenericRepository
    {
        private readonly IDBContext _dbContext;
        private readonly ContextLogger<GenericRepository> _contextLogger;
        private readonly IRequestContextAccessor _requestContextAccessor;

        public GenericRepository(IDBContext dbContext, ContextLogger<GenericRepository> contextLogger, IRequestContextAccessor requestContextAccessor)
        {
            _dbContext = dbContext;
            _contextLogger = contextLogger;
            _requestContextAccessor = requestContextAccessor;
        }

        public async Task<DbResponse> InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : MedCompassBase
        {
            _contextLogger.LogInformation($"Repository Service - InsertAsync entity:{typeof(T).Name} id:{entity.Id}");

            try
            {
                await _dbContext.GetCollection<T>().InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
                return DbResponse.Inserted;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in Repository - InsertAsync");
                return DbResponse.Error;
            }
        }

        public async Task<DbResponse> InsertManyAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : MedCompassBase
        {
            var idsText = entities.Count() > 100 ? entities.Count().ToString() : string.Join(',', entities.Select(x => x.Id));
            _contextLogger.LogInformation($"Repository Service - InsertManyAsync entity:{typeof(T).Name} ids:{idsText}");

            try
            {
                await _dbContext.GetCollection<T>().InsertManyAsync(entities, new InsertManyOptions(), cancellationToken);
                return DbResponse.Inserted;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in Repository - InsertManyAsync");
                return DbResponse.Error;
            }
        }

        public async Task<DbResponse> UpdateAsync<T>(T entity, int currentVersion, CancellationToken cancellationToken) where T : MedCompassBase
        {
            _contextLogger.LogInformation($"Repository Service - UpdateAsync - entity:{typeof(T).Name} id: {entity.Id} version: {currentVersion}");

            var builder = Builders<T>.Filter;
            var filter = builder.Eq(a => a.Id, entity.Id)
                        & GetTenantSegmentFilterDefinition<T>();

            try
            {
                var result = await _dbContext.GetCollection<T>().ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);

                if (!result.IsAcknowledged)
                {
                    _contextLogger.LogError("Error in RepositoryService - UpdateSecurityRoleAsync - Request not acknowledged");
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
                _contextLogger.LogError(e, "Error in RepositoryService - UpdateAsync");
                return DbResponse.Error;
            }
        }


        public async Task<DbResponse> OverwriteAsync<T>(T entity, CancellationToken cancellationToken) where T : MedCompassBase
        {
            _contextLogger.LogInformation($"Repository Service - OverwriteAsync Begin - entity: {typeof(T).Name}  id: {entity.Id}");

            var builder = Builders<T>.Filter;
            var filter = builder.Eq(a => a.Id, entity.Id)
                & GetTenantSegmentFilterDefinition<T>();

            try
            {
                var result = await _dbContext.GetCollection<T>().ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);

                if (!result.IsAcknowledged)
                {
                    _contextLogger.LogError("Error in RepositoryService - OverwriteAsync - Request not acknowledged");
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
                _contextLogger.LogError(e, "Error in RepositoryService - OverwriteAsync");
                return DbResponse.Error;
            }
        }

        public async Task<DbResponse> OverwriteManyAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : MedCompassBase
        {
            var idsText = entities.Count() > 100 ? entities.Count().ToString() : string.Join(',', entities.Select(x => x.Id));
            _contextLogger.LogInformation($"Repository Service - OverwriteManyAsync Begin - ids:{idsText}");

            var bulkWrites = new List<WriteModel<T>>();
            foreach (var entity in entities)
            {
                var filter = Builders<T>.Filter.Eq(a => a.Id, entity.Id) & GetTenantSegmentFilterDefinition<T>();
                bulkWrites.Add(new ReplaceOneModel<T>(filter, entity));
            }

            try
            {

                var result = await _dbContext.GetCollection<T>().BulkWriteAsync(bulkWrites, new BulkWriteOptions(), cancellationToken);

                if (!result.IsAcknowledged)
                {
                    _contextLogger.LogError("Error in RepositoryService - OverwriteAsync - Request not acknowledged");
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
                _contextLogger.LogError(e, "Error in RepositoryService - OverwriteManyAsync");
                return DbResponse.Error;
            }
        }

        public async Task<DbResponse> VoidAsync<T>(Guid id, Guid? voidReasonId, Guid? voidedBy, Guid? voidedById, DateTime voidedOn, CancellationToken cancellationToken) where T : MedCompassBase
        {
            _contextLogger.LogInformation($"Repository Service - VoidAsync - entity {typeof(T).Name} id:{id}");

            var builder = Builders<T>.Filter;
            var filter = builder.Eq(a => a.Id, id)
                & GetTenantSegmentFilterDefinition<T>();

            var updateBuilder = Builders<T>.Update;
            var update = updateBuilder
                    .Set(item => item.VoidedReasonId, voidReasonId)
                    .Set(item => item.VoidedBy, voidedBy)
                    .Set(item => item.VoidedById, voidedById)
                    .Set(item => item.VoidedOn, voidedOn);

            try
            {
                var updateResult = await _dbContext.GetCollection<T>().UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false }, cancellationToken);

                if (updateResult.IsAcknowledged && updateResult.ModifiedCount == 1)
                {
                    return DbResponse.Updated;
                }

                return DbResponse.NotFound;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - VoidMemberSecurityProfileAsync");
                return DbResponse.Error;
            }
        }


        private FilterDefinition<T> GetActiveFilterDefinition<T>() where T : MedCompassBase
        {
            var builder = Builders<T>.Filter;

            // Filter out voided items
            var filter = !builder.Exists(securityRole => securityRole.VoidedOn) | builder.Eq("VoidedOn", BsonNull.Value);

            // Filter out expired items
            filter &= !builder.Exists(securityRole => securityRole.ExpirationDate) | builder.Eq("ExpirationDate", BsonNull.Value) | builder.Gt(memberSecurityRole => memberSecurityRole.ExpirationDate, DateTime.UtcNow);

            // Filter out not yet effective items
            filter &= builder.Lte(securityRole => securityRole.EffectiveDate, DateTime.UtcNow);

            return filter;
        }

        private FilterDefinition<T> GetTenantSegmentFilterDefinition<T>() where T : MedCompassBase
        {
            var builder = Builders<T>.Filter;
            return builder.Eq(entity => entity.SegmentId, RequestContext.SegmentId)
                 & builder.Eq(entity => entity.TenantId, RequestContext.TenantId);
        }
        public async Task<(DbResponse response, T data)> ReadByIdAsync<T>(Guid id, CancellationToken cancellationToken, bool includeInactive = false) where T : MedCompassBase
        {
            _contextLogger.LogInformation($"Repository Service - ReadByIdAsync entity:{typeof(T).Name} id:{id} - IncludeInactive:{includeInactive}");
            try
            {
                var builder = Builders<T>.Filter;
                var filter = builder.Eq(item => item.Id, id)
                            & GetTenantSegmentFilterDefinition<T>();

                if (!includeInactive)
                {
                    filter &= GetActiveFilterDefinition<T>();
                }

                var result = await _dbContext.GetCollection<T>().Find(filter).ToListAsync(cancellationToken);
                if (result == null || !result.Any())
                {
                    return (DbResponse.NotFound, null);
                }

                return (DbResponse.Found, result.FirstOrDefault());

            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - ReadByIdAsync");
                return (DbResponse.Error, null);
            }
        }

        public async Task<(DbResponse response, List<T> data)> ReadByIdAsync<T>(IEnumerable<Guid> ids, CancellationToken cancellationToken, bool includeInactive = false) where T : MedCompassBase
        {
            var idsText = ids.Count() > 100 ? ids.Count().ToString() : string.Join(',', ids);
            _contextLogger.LogInformation($"Repository Service - ReadByIdAsync entity:{idsText} - IncludeInactive:{includeInactive}");
            try
            {
                var builder = Builders<T>.Filter;
                var filter = builder.In(item => item.Id, ids)
                            & GetTenantSegmentFilterDefinition<T>();

                if (!includeInactive)
                {
                    filter &= GetActiveFilterDefinition<T>();
                }

                var result = await _dbContext.GetCollection<T>().Find(filter).ToListAsync(cancellationToken);
                if (result == null || !result.Any())
                {
                    return (DbResponse.NotFound, null);
                }

                return (DbResponse.Found, result);

            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - ReadByIdAsync");
                return (DbResponse.Error, null);
            }
        }

        public async Task<DbResponse> UpsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : MedCompassBase
        {
            try
            {
                var bulkOps = new List<WriteModel<T>>();
                foreach (var record in entities)
                {
                    var upsertOne = new ReplaceOneModel<T>(
                        Builders<T>.Filter.Where(x => x.Id == record.Id),
                        record)
                    { IsUpsert = true };
                    bulkOps.Add(upsertOne);
                }
                var result = await _dbContext.GetCollection<T>().BulkWriteAsync(bulkOps, new BulkWriteOptions { IsOrdered = true });
                _contextLogger.LogDebug($"matched count {result.MatchedCount} modified count: {result.ModifiedCount} inserted count: {result.InsertedCount}");
                return DbResponse.Inserted;
            }
            catch (MongoException e)
            {
                _contextLogger.LogError(e, "Error in RepositoryService - UpsertAsync");
                return DbResponse.Error;
            }
        }

        private RequestContext RequestContext => _requestContextAccessor.RequestContext;
    }
}
