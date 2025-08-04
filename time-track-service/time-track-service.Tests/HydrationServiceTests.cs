using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Services;
using time_track_service.Tests.Mocks;
using Xunit;

namespace time_track_service.Tests
{
    public class HydrationServiceTests
    {
        private readonly Mock<ILogger<HydrationService>> _logger;
        private readonly Mock<ITimeTrackRepository> _timeTrackRepository;
        private readonly Mock<ISyncBackNotificationService> _updateNotificationService;
        private readonly IHydrationService _hydrationService;
        public HydrationServiceTests()
        {
            _logger = new Mock<ILogger<HydrationService>>();
            _timeTrackRepository = new Mock<ITimeTrackRepository>();
            _updateNotificationService = new Mock<ISyncBackNotificationService>();
            _hydrationService = new HydrationService(
                _logger.Object, 
                _timeTrackRepository.Object, 
                _updateNotificationService.Object);

            MockTimeTrackRepos.MockTimeTrackRepository(_timeTrackRepository);
            _updateNotificationService.Setup(m => m.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<Object>(), It.IsAny<Object>())).ReturnsAsync(true);
        }
        [Fact]
        public async Task InsertTimeTracksAsyncTest_Inserted()
        {
            List<TimeTrack> TimeTracks = new List<TimeTrack>();
            TimeTracks.Add(new TimeTrack() { Id = Guid.NewGuid() });
            var result = await _hydrationService.InsertTimeTracksAsync(TimeTracks);
            Assert.Equal(DbResponse.Inserted, result);
        }
        [Fact]
        public async Task UpdateTimeTracksAsyncTest_Invalid()
        {
            var result = await _hydrationService.UpdateTimeTrackAsync(Guid.NewGuid(), MockTimeTrackRepos.MockTimeTrack(Guid.NewGuid()));
            Assert.Equal(DbResponse.Invalid, result);
        }

        [Fact]
        public async Task UpdateTimeTracksAsyncTest_Updated()
        {
            var id = Guid.NewGuid();
            var result = await _hydrationService.UpdateTimeTrackAsync(id, MockTimeTrackRepos.MockTimeTrack(id));
            Assert.Equal(DbResponse.Updated, result);
        }

        [Fact]
        public async Task UpdateTimeTracksAsyncTest_NullId()
        {
            var result = await _hydrationService.UpdateTimeTrackAsync(Guid.Empty, MockTimeTrackRepos.MockTimeTrack(Guid.Empty));
            Assert.Equal(DbResponse.Invalid, result);
        }
    }
}
