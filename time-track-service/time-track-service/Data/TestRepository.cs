using System.Collections.Generic;
using time_track_service.Context;
using time_track_service.Model;
using time_track_service.Model.Dto;

namespace time_track_service.Data
{
    public class TestRepository : ITestRepository
    {
        private readonly IDBContext _dbContext;
        private readonly MongoClientConnectionSettings _mongoClientConnectionSettings;

        public TestRepository(IDBContext dbContext, MongoClientConnectionSettings mongoClientConnectionSettings)
        {
            _dbContext = dbContext;
            _mongoClientConnectionSettings = mongoClientConnectionSettings;
        }


        public DbResponse CleanTimeTrackCollection()
        {
            _dbContext.Database.DropCollection(_mongoClientConnectionSettings.TimeTrackCollection);

            return DbResponse.Deleted;
        }
        public DbResponse CleanTimeTrackConfigurationDetailCollection()
        {
            _dbContext.Database.DropCollection(_mongoClientConnectionSettings.TimeTrackConfigurationDetailCollection);

            return DbResponse.Deleted;
        }

        public DbResponse InsertTestTimeTrackCollection(ICollection<TimeTrack> callLogs)
        {
            _dbContext.TimeTrackCollection.InsertManyAsync(callLogs);

            return DbResponse.Inserted;
        }

    }
}
