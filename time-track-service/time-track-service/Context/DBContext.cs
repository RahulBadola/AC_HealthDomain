using MongoDB.Driver;
using System;
using time_track_service.Model.Dto;

namespace time_track_service.Context
{
    public class DBContext : IDBContext
    {

        private readonly MongoClientConnectionSettings _settings;
         public IMongoDatabase Database { get; private set; }

        private IMongoDatabase _database;

        public IMongoCollection<TimeTrack> TimeTrackCollection => _database.GetCollection<TimeTrack>(_settings.TimeTrackCollection);

        public DBContext(MongoClientConnectionSettings settings)
        {
            _settings = settings;
        }
        public IMongoCollection<T> GetCollection<T>()
        {

            switch (typeof(T))
            {
                case Type t when t == typeof(TimeTrack):
                    return _database.GetCollection<T>(_settings.TimeTrackCollection);
                default:
                    return null;
            }
        }
        public void Initialize()
        {
            var dbClient = new MongoClient(_settings.GenerateSettings());
            _database = dbClient.GetDatabase(_settings.Database);

            var databaseList = dbClient.ListDatabaseNames().ToList();
            var databaseExists = databaseList.Contains(_settings.Database);

            if (!databaseExists)
            {
                _database.CreateCollection(_settings.TimeTrackCollection);
            }
        }
    }

}

