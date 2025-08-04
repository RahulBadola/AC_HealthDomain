using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Services;
using time_track_service.Tests.Mocks;
using time_track_service.Utils;
using Xunit;

namespace time_track_service.Tests
{
    public class SharedServiceTests
    {
        private readonly Mock<ContextLogger<SharedService>> _logger;
        private readonly Mock<ITimeTrackRepository> _timeTrackRepository;
        private readonly Mock<ISyncBackNotificationService> _updateNotificationService;
        private readonly SharedService _sharedService;
        public SharedServiceTests()
        {
            var mockRequestContextAccessor = new MockRequestContextAccessor();
            _logger = new Mock<ContextLogger<SharedService>>(new Mock<IRequestContextAccessor>().Object, new Mock<ILogger<SharedService>>().Object);
            _timeTrackRepository = new Mock<ITimeTrackRepository>();
            _updateNotificationService = new Mock<ISyncBackNotificationService>();
            _sharedService = new SharedService(
                _logger.Object,
                new MockFieldAuthorizationService(),
                new MockOperationAuthorizationService(),
                _timeTrackRepository.Object, 
                mockRequestContextAccessor.Object,
                _updateNotificationService.Object);

            MockTimeTrackRepos.MockTimeTrackRepository(_timeTrackRepository);
            _updateNotificationService.Setup(m => m.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<Object>(), It.IsAny<Object>())).ReturnsAsync(true);
        }
        [Fact]
        public async Task UpdateTimeTrackAsync_shouldUpdate()
        {
            var result = await _sharedService.UpdateTimeTrackAsync(MockTimeTrackRepos.MockTimeTrack(Guid.NewGuid()));
            Assert.Equal(DbResponse.Updated, result.response);
        }
        [Fact]
        public async Task UpdateTimeTrackAsync_SyncBackFail()
        {
            _updateNotificationService.Setup(m => m.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<Object>(), It.IsAny<Object>())).ReturnsAsync(false);
            var result = await _sharedService.UpdateTimeTrackAsync(MockTimeTrackRepos.MockTimeTrack(Guid.NewGuid()));
            Assert.Equal(DbResponse.Reverted, result.response);
        }
    }
}
