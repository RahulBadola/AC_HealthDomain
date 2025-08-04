using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using type_lookup_service.Data;
using type_lookup_service.Model;
using type_lookup_service.Services.Internal;
using type_lookup_service.Utils;
using Xunit;

namespace type_lookup_service.Tests.Services.AuthorizationServiceTests
{
    public class QueryRoleOperationsAsyncShould
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public async Task ReturnNoResults(int count = 0)
        {
            var repository = new Mock<IRepository>();
            repository
                .Setup(m => m.GetAuthorizationsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<Authorization>());

            var service = new AuthorizationService(
                logger: new Mock<IContextLogger<AuthorizationService>>().Object,
                repository: repository.Object);

            var query = new OperationQuery
            {
                Objects = new[] { "TestObject" },
                SecurityRoleIds = Enumerable.Range(0, count).Select(i => Guid.NewGuid()).ToArray()
            };

            var results = await service.QueryRoleOperationsAsync(query);

            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task ReturnNoResults_GivenEmptyObjects()
        {
            var repository = new Mock<IRepository>();

            var service = new AuthorizationService(
                logger: new Mock<IContextLogger<AuthorizationService>>().Object,
                repository: repository.Object);

            var query = new OperationQuery
            {
                Objects = new string[0],
                SecurityRoleIds = Enumerable.Range(0, 5).Select(i => Guid.NewGuid()).ToArray()
            };

            var results = await service.QueryRoleOperationsAsync(query);

            Assert.NotNull(results);
            Assert.Empty(results);

            repository
                .Verify(
                    m => m.GetAuthorizationsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ReturnNoResults_GivenEmptySecurityRoleIds()
        {
            var repository = new Mock<IRepository>();

            var service = new AuthorizationService(
                logger: new Mock<IContextLogger<AuthorizationService>>().Object,
                repository: repository.Object);

            var query = new OperationQuery
            {
                Objects = new[] { "TestObject" },
                SecurityRoleIds = new Guid[0]
            };

            var results = await service.QueryRoleOperationsAsync(query);

            Assert.NotNull(results);
            Assert.Empty(results);

            repository
                .Verify(
                    m => m.GetAuthorizationsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ReturnNoResults_GivenNullObjects()
        {
            var repository = new Mock<IRepository>();

            var service = new AuthorizationService(
                logger: new Mock<IContextLogger<AuthorizationService>>().Object,
                repository: repository.Object);

            var query = new OperationQuery
            {
                Objects = null,
                SecurityRoleIds = Enumerable.Range(0, 5).Select(i => Guid.NewGuid()).ToArray()
            };

            var results = await service.QueryRoleOperationsAsync(query);

            Assert.NotNull(results);
            Assert.Empty(results);

            repository
                .Verify(
                    m => m.GetAuthorizationsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ReturnNoResults_GivenNullSecurityRoleIds()
        {
            var repository = new Mock<IRepository>();

            var service = new AuthorizationService(
                logger: new Mock<IContextLogger<AuthorizationService>>().Object,
                repository: repository.Object);

            var query = new OperationQuery
            {
                Objects = new[] { "TestObject" },
                SecurityRoleIds = null
            };

            var results = await service.QueryRoleOperationsAsync(query);

            Assert.NotNull(results);
            Assert.Empty(results);

            repository
                .Verify(
                    m => m.GetAuthorizationsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public async Task ReturnResults(int count = 0)
        {
            var authorizationId = 0;
            var repository = new Mock<IRepository>();
            repository
                .Setup(m => m.GetAuthorizationsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<Guid> a, IEnumerable<string> b, CancellationToken c) =>
                    // cross-join SecurityRoleIds and Objects (1 result for each combination)
                    a.SelectMany(
                        _ => b,
                        (x, y) => new Authorization()
                        {
                            AuthorizationId = Interlocked.Increment(ref authorizationId),
                            AuthorizationType = 1,
                            ItemId = 1001,
                            ObjectSid = x.ToString(),
                            Object = y,
                            Operation = "Add"
                        }));

            var service = new AuthorizationService(
                logger: new Mock<IContextLogger<AuthorizationService>>().Object,
                repository: repository.Object);

            var query = new OperationQuery
            {
                Objects = new[] { "TestObject" },
                SecurityRoleIds = Enumerable.Range(0, count).Select(i => Guid.NewGuid()).ToArray()
            };

            var results = await service.QueryRoleOperationsAsync(query);

            Assert.NotNull(results);
            Assert.Equal(count, results.Count());

            var expectedSecurityRoleIds = query.SecurityRoleIds.Select(g => g.ToString()).ToArray();
            Assert.All(results, result =>
            {
                Assert.Contains(result.SecurityRoleId, expectedSecurityRoleIds);
                Assert.Equal(query.Objects[0], result.Object, ignoreCase: true);
            });
        }
    }
}
