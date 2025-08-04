using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using type_lookup_service.Repositories;
using type_lookup_service.Utils;
using Xunit;

namespace type_lookup_service.Tests.Repositories.JsonRepositoryTests
{
    public class GetFileDataShould
    {
        [Fact]
        public void HandleMissingFile()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "TypeLookup:FileLocation", "" }
                })
                .Build();

            var repository = new JsonRepository(
                config: config,
                logger: new Mock<IContextLogger<JsonRepository>>().Object,
                memoryCache: new MemoryCache(new MemoryCacheOptions()),
                requestContextAccessor: new Mock<IRequestContextAccessor>().Object);

            var repositoryAccessor = new JsonRepositoryAccessor(repository);

            var actual = repositoryAccessor.GetFileData("./Data/00000000-0000-0000-0000-000000000000/00000000-0000-0000-0000-000000000000/does-not-exist.json");
            Assert.Empty(actual);
        }

        [Fact]
        public void Read()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "TypeLookup:FileLocation", "" }
                })
                .Build();

            var repository = new JsonRepository(
                config: config,
                logger: new Mock<IContextLogger<JsonRepository>>().Object,
                memoryCache: new MemoryCache(new MemoryCacheOptions()),
                requestContextAccessor: new Mock<IRequestContextAccessor>().Object);

            var repositoryAccessor = new JsonRepositoryAccessor(repository);

            var actual = repositoryAccessor.GetFileData("./Data/00000000-0000-0000-0000-000000000000/00000000-0000-0000-0000-000000000000/countryType.json");
            Assert.Equal(247, actual.Count);
        }

        [Fact]
        public void ShareOnRead()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "TypeLookup:FileLocation", "" }
                })
                .Build();

            var repository = new JsonRepository(
                config: config,
                logger: new Mock<IContextLogger<JsonRepository>>().Object,
                memoryCache: new MemoryCache(new MemoryCacheOptions()),
                requestContextAccessor: new Mock<IRequestContextAccessor>().Object);

            var repositoryAccessor = new JsonRepositoryAccessor(repository);

            Parallel.For(0, 100, (_idx) =>
            {
                var actual = repositoryAccessor.GetFileData("./Data/00000000-0000-0000-0000-000000000000/00000000-0000-0000-0000-000000000000/countryType.json");
                Assert.Equal(247, actual.Count);
            });
        }

        private sealed class JsonRepositoryAccessor
        {
            private readonly JsonRepository _target;

            public JsonRepositoryAccessor(JsonRepository target)
            {
                _target = target;
            }

            public JArray GetFileData(string filePath)
            {
                var getFileDataMethod = typeof(JsonRepository).GetMethod("GetFileData", BindingFlags.Instance | BindingFlags.NonPublic);
                var boxedResult = getFileDataMethod.Invoke(_target, new object[] { filePath });
                return (JArray)boxedResult;
            }
        }
    }
}
