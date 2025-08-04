using System.Threading.Tasks;
using time_track_dshp.Models.Dto;

namespace time_track_dshp.Services
{
    public interface ISyncBackService
    {
        public Task PublishSyncBackRecordAsync(string objectType, string objectJson);
        public Task SyncBackToSqlServerAsync(SyncBackRecord record);
        Task SyncBackTimeTrackAsync(TimeTrack timeTrack, bool isInsert = false);
        Task SyncBackTimeTrackActivityMapAsync(TimeTrackActivityMap timeTrackActivityMap, bool isInsert = false);
    }
}
