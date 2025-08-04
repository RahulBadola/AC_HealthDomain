using time_track_service.Model;

namespace time_track_service.Services
{
    public interface ILoadTestDataService
    {
        DbResponse CleanCollection();
        DbResponse PopulateTestData();
    }
}