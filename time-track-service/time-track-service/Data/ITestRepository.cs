using System.Collections.Generic;
using time_track_service.Model;
using time_track_service.Model.Dto;

namespace time_track_service.Data
{
    public interface ITestRepository
    {
        DbResponse CleanTimeTrackCollection();
        DbResponse InsertTestTimeTrackCollection(ICollection<TimeTrack> callLogs);
    }
}