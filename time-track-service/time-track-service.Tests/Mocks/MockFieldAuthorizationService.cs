using AssureCare.MedCompass.DataAuthorization.Services;
using System.Threading;
using System.Threading.Tasks;

namespace time_track_service.Tests.Mocks
{
    internal class MockFieldAuthorizationService : IFieldAuthorizationService
    {
        public Task<object> NullifyHiddenPropertiesAsync(object obj, bool recurseChildren = true, CancellationToken cancellationToken = default)
            => Task.FromResult(obj);

        public Task<T> NullifyHiddenPropertiesAsync<T>(T obj, bool recurseChildren = true, CancellationToken cancellationToken = default)
            => Task.FromResult(obj);

        public Task<object> NullifyReadOnlyPropertiesAsync(object obj, bool recurseChildren = true, CancellationToken cancellationToken = default)
            => Task.FromResult(obj);

        public Task<T> NullifyReadOnlyPropertiesAsync<T>(T obj, bool recurseChildren = true, CancellationToken cancellationToken = default)
            => Task.FromResult(obj);

        public Task<object> RevertReadOnlyPropertiesAsync(object obj, object original, CancellationToken cancellationToken = default)
            => Task.FromResult(obj);

        public Task<T> RevertReadOnlyPropertiesAsync<T>(T obj, T original, CancellationToken cancellationToken = default)
            => Task.FromResult(obj);
    }
}
