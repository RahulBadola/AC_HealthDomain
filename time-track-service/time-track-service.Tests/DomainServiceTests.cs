using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Services;
using time_track_service.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Threading;

namespace time_track_service.Tests
{
    public class DomainServiceTests
    {
        private readonly Mock<ILogger<DomainService>> _logger;
        private readonly Mock<ITimeTrackRepository> _timeTrackRepository;
        private readonly Mock<ISyncBackNotificationService> _updateNotificationService;
        private readonly IDomainService _domainService;
        private readonly Mock<ISharedService> _sharedService;
        public DomainServiceTests()
        {
            var mockRequestContextAccessor = new MockRequestContextAccessor();
            _logger = new Mock<ILogger<DomainService>>();
            _timeTrackRepository = new Mock<ITimeTrackRepository>();
            _sharedService = new Mock<ISharedService>();
            _updateNotificationService = new Mock<ISyncBackNotificationService>();
            _domainService = new DomainService(
                new MockFieldAuthorizationService(),
                _logger.Object,
                new MockOperationAuthorizationService(),
                _timeTrackRepository.Object,
                mockRequestContextAccessor.Object,
                _sharedService.Object, _updateNotificationService.Object);

            MockTimeTrackRepos.MockTimeTrackRepository(_timeTrackRepository);
            _updateNotificationService.Setup(m => m.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<Object>(), It.IsAny<Object>())).ReturnsAsync(true);
        }
        [Fact]
        public async Task InserttimeTrackAsync_ShouldInsert()
        {
            var result = await _domainService.InsertTimeTrackAsync(new TimeTrack());
            Assert.Equal(DbResponse.Inserted, result.response);
        }
        [Fact]
        public async Task ReadTimeTracksAsync_ShouldGetList()
        {
            var result = await _domainService.ReadTimeTracksAsync(true);
            Assert.Equal(DbResponse.Found, result.response);
        }
        [Fact]
        public async Task ReadTimeTrackByIdAsync_ShouldGetList()
        {
            var result = await _domainService.ReadTimeTrackByIdAsync(Guid.NewGuid());
            Assert.Equal(DbResponse.Found, result.response);
        }

        [Fact]
        public async Task UpdateTimeTrackByIdAsync_ShouldUpdate()
        {
            Guid Id = Guid.NewGuid();
            _sharedService.Setup(m => m.UpdateTimeTrackAsync(It.IsAny<TimeTrack>(), It.IsAny<CancellationToken>())).
               ReturnsAsync((DbResponse.Updated, new TimeTrack()));
            var result = await _domainService.UpdateTimeTrackByIdAsync(Id, MockTimeTrackRepos.MockTimeTrack(Id));
            Assert.Equal(DbResponse.Updated, result.response);
        }
        [Fact]
        public async Task UpdateTimeTrackByIdAsync_IDNull()
        {
            _sharedService.Setup(m => m.UpdateTimeTrackAsync(It.IsAny<TimeTrack>(), It.IsAny<CancellationToken>())).
               ReturnsAsync((DbResponse.Updated, new TimeTrack()));
            var result = await _domainService.UpdateTimeTrackByIdAsync(Guid.Empty, MockTimeTrackRepos.MockTimeTrack(Guid.NewGuid()));
            Assert.Equal(DbResponse.Invalid, result.response);
        }
        [Fact]
        public async Task UpdateTimeTrackByIdAsync_IDNotEqual()
        {
            _sharedService.Setup(m => m.UpdateTimeTrackAsync(It.IsAny<TimeTrack>(), It.IsAny<CancellationToken>())).
               ReturnsAsync((DbResponse.Updated, new TimeTrack()));
            var result = await _domainService.UpdateTimeTrackByIdAsync(Guid.NewGuid(), MockTimeTrackRepos.MockTimeTrack(Guid.NewGuid()));
            Assert.Equal(DbResponse.Invalid, result.response);
        }
        [Fact]
        public async Task VoidTimeTrackByIdAsync_ShouldVoidData()
        {
            var result = await _domainService.VoidTimeTrackByIdAsync(Guid.NewGuid(), Guid.NewGuid());
            Assert.Equal(DbResponse.Updated, result);
        }
    }
}
