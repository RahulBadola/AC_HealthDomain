using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;
using Moq;
using System;
using System.Collections.Generic;

namespace time_track_service.Tests.Mocks
{
    public static class MockTimeTrackRepos
    {
        public static void MockTimeTrackRepository(Mock<ITimeTrackRepository> _repository)
        {
            _repository.Setup(m => m.InsertTimeTrackAsync(It.IsAny<TimeTrack>())).ReturnsAsync(DbResponse.Inserted);
            _repository.Setup(m => m.InsertTimeTracksAsync(It.IsAny<List<TimeTrack>>())).ReturnsAsync(DbResponse.Inserted);
            _repository.Setup(m => m.GetTimeTracksAsync(It.IsAny<Boolean>())).ReturnsAsync((DbResponse.Found, new List<TimeTrack>() { MockTimeTrack(Guid.NewGuid()) }));
            _repository.Setup(m => m.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.Found, MockTimeTrack(Guid.NewGuid())));
            _repository.Setup(m => m.OverwriteTimeTrackAsync(It.IsAny<TimeTrack>())).ReturnsAsync(DbResponse.Updated);
            _repository.Setup(m => m.VoidTimeTrackAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(DbResponse.Updated);
            _repository.Setup(m => m.DeleteTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync(DbResponse.Deleted);
            _repository.Setup(m => m.UpdateTimeTrackAsync(It.IsAny<TimeTrack>(), It.IsAny<Int32>())).ReturnsAsync(DbResponse.Updated);
        }
        public static TimeTrack MockTimeTrack(Guid Id)
        {
            return new TimeTrack()
            {
                Id = Id,
                UpdatedOn = DateTime.UtcNow.AddDays(-3)
            };
        }
    }
       
}
