using AssureCare.MedCompass.DataAuthorization.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace time_track_service.Tests.Mocks
{
    internal class MockOperationAuthorizationService : IOperationAuthorizationService
    {
        public Task<bool> CheckOperationAsync(string objectName, string operation, Guid? segmentId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> CheckOperationAsync(Type objectType, string operation, Guid? segmentId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> CheckOperationAsync<T>(string operation, Guid? segmentId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> CheckOperationAsync<T>(T obj, string operation, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> CheckOperationAsync<T>(T obj, string operation, Guid? segmentId, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<object> FilterByOperationAsync(object obj, CancellationToken cancellationToken = default)
            => Task.FromResult(obj);

        public Task<T> FilterByOperationAsync<T>(T obj, CancellationToken cancellationToken = default)
            => Task.FromResult(obj);
    }
}
