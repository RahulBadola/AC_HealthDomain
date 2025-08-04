using MongoDB.Driver;
using time_track_service.Model.Dto;

namespace time_track_service.Context
{
    public interface IDBContext
    {
        IMongoDatabase Database { get; }
        void Initialize();

        IMongoCollection<TimeTrack> TimeTrackCollection { get; }

        IMongoCollection<T> GetCollection<T>();


    }
}
