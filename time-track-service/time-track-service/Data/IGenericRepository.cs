using time_track_service.Model;
using time_track_service.Model.Dto;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace time_track_service.Data
{
    public interface IGenericRepository
    {
        Task<DbResponse> InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : MedCompassBase;

        Task<DbResponse> OverwriteManyAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : MedCompassBase;

        Task<DbResponse> InsertManyAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : MedCompassBase;

        Task<DbResponse> UpdateAsync<T>(T entity, int currentVersion, CancellationToken cancellationToken) where T : MedCompassBase;

        Task<DbResponse> VoidAsync<T>(Guid id, Guid? voidReasonId, Guid? voidedBy, Guid? voidedById, DateTime voidedOn, CancellationToken cancellationToken) where T : MedCompassBase;

        Task<(DbResponse response, T data)> ReadByIdAsync<T>(Guid id, CancellationToken cancellationToken, bool includeInactive = false) where T : MedCompassBase;

        Task<(DbResponse response, List<T> data)> ReadByIdAsync<T>(IEnumerable<Guid> ids, CancellationToken cancellationToken, bool includeInactive = false) where T : MedCompassBase;

        Task<DbResponse> OverwriteAsync<T>(T entity, CancellationToken cancellationToken) where T : MedCompassBase;

        Task<DbResponse> UpsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : MedCompassBase;
    }
}
