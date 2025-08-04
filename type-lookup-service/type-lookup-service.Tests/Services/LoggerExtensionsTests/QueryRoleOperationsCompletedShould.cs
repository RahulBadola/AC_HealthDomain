using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using type_lookup_service.Model;
using type_lookup_service.Services.Internal;
using Xunit;

namespace type_lookup_service.Tests.Services.LoggerExtensionsTests
{
    public class QueryRoleOperationsCompletedShould
    {
        [Fact]
        public void WriteToLogger()
        {
            var results = Enumerable
                .Range(0, 10)
                .Select(i => new RoleOperation()
                {
                    Object = $"Object-{i}",
                    Operation = "Get",
                    SecurityRoleId = Guid.NewGuid().ToString()
                });

            var logger = new Mock<ILogger>();
            logger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            _ = logger.Object.QueryRoleOperationsCompleted(results);

            logger.Verify(x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);

            logger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
