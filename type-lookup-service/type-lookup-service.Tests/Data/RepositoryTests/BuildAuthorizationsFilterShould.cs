using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Linq;
using type_lookup_service.Data;
using type_lookup_service.Model;
using Xunit;

namespace type_lookup_service.Tests.Data.RepositoryTests
{
    public class BuildAuthorizationsFilterShould
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(3, 3)]
        [InlineData(5, 5)]
        public void ReturnFilterDefinition(int roleCount = 0, int objectCount = 0)
        {
            var securityRoleIds = Enumerable.Range(0, roleCount).Select(i => Guid.NewGuid());
            var objects = Enumerable.Range(0, objectCount).Select(i => $"Object-{i}");

            var actual = Repository.BuildAuthorizationsFilter(securityRoleIds, objects);

            Assert.NotNull(actual);

            var actualBson = actual.Render(
                documentSerializer: BsonSerializer.LookupSerializer<Authorization>(),
                serializerRegistry: BsonSerializer.SerializerRegistry);
            Assert.NotNull(actualBson);

            var actualJson = actualBson.ToJson();
            Assert.NotNull(actualJson);
        }
    }
}
