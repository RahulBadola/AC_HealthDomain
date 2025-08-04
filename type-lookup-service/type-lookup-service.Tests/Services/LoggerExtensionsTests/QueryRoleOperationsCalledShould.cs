using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using type_lookup_service.Model;
using type_lookup_service.Services.Internal;
using Xunit;

namespace type_lookup_service.Tests.Services.LoggerExtensionsTests
{
    public class QueryRoleOperationsCalledShould
    {
        [Fact]
        public void WriteToLogger()
        {
            var query = new OperationQuery()
            {
                Objects = Enumerable.Range(0, 3).Select(i => $"TestObject-{i}").ToArray(),
                SecurityRoleIds = Enumerable.Range(0, 3).Select(i => Guid.NewGuid()).ToArray()
            };

            var logger = new Mock<ILogger>();
            logger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            _ = logger.Object.QueryRoleOperationsCalled(query);

            logger.Verify(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
