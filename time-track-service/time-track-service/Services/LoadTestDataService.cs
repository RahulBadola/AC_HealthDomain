using Bogus;
using System;
using System.Collections.Generic;
using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;

namespace time_track_service.Services
{
    public class LoadTestDataService : ILoadTestDataService
    {
        private readonly ITestRepository _testRepository;
        public LoadTestDataService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }
        public DbResponse CleanCollection()
        {
            return _testRepository.CleanTimeTrackCollection();
        }
        public DbResponse PopulateTestData()
        {
            PopulateHardcodedTestData();
            var TimeTrackActivityTypeKey = new[] { "1", "2", "3", "A1", "4S" };
            var ServiceTypeKey = new[] { "S", "SD", "SLM", "SR", "UD" };
            var ProgramTypeKey = new[] { "BHUM", "CM", "MD", "MI", "UM" };
            var ServicePlanFundingSourceTypeKey = new[] { "AAAAA", "EI", "EO", "F2F", "FI" };
            var TimeTrackSubActivityTypeKey = new[] { "AAAAA", "AdmP", "AP", "CL", "CP" };
            var SyncLockStateTypeKey = new[] { "DNUPN", "FTF", "IN", "INC", "MSDNC" };


            var testTimeTrackFaker = new Faker<TimeTrack>()
                .RuleFor(m => m.SecurityUserId, f => Guid.NewGuid())
                .RuleFor(m => m.TimeTrackActivityTypeKey, f => f.PickRandom(TimeTrackActivityTypeKey))
                .RuleFor(m => m.ServiceTypeKey, f => f.PickRandom(ServiceTypeKey))
                .RuleFor(m => m.ProgramTypeKey, f => f.PickRandom(ProgramTypeKey))
                .RuleFor(m => m.ServicePlanFundingSourceTypeKey, f => f.PickRandom(ServicePlanFundingSourceTypeKey))
                .RuleFor(m => m.TimeTrackSubActivityTypeKey, f => f.PickRandom(TimeTrackSubActivityTypeKey))
                .RuleFor(m => m.ServiceAuthId, f => Guid.NewGuid())
                .RuleFor(m => m.MemberId, f => Guid.NewGuid())
                .RuleFor(m => m.StartDate, DateTime.Now)
                .RuleFor(m => m.EndDate, DateTime.Now)
                .RuleFor(m => m.GenerateUnit, f => f.Random.Bool())
                .RuleFor(m => m.CaseRecording, f => f.Random.Bool())
                .RuleFor(m => m.VolunteerDriver, f => f.Random.Bool())
                .RuleFor(m => m.IsSystemTime, f => f.Random.Bool())
                .RuleFor(m => m.SyncLockedBy, Guid.NewGuid())
                .RuleFor(m => m.SyncLockedOn, DateTime.Now)
                .RuleFor(m => m.RowFilterId, Guid.NewGuid())
                .RuleFor(m => m.SyncLockedById, Guid.NewGuid())
                .RuleFor(m => m.MemberProgramId, f => Guid.NewGuid())
                .RuleFor(m => m.SyncLockedByName, f => f.Name.FullName())
                .RuleFor(m => m.SyncLockStateTypeKey, f => f.PickRandom(SyncLockStateTypeKey));

            var testTimeTracks = testTimeTrackFaker.Generate(100);

            return _testRepository.InsertTestTimeTrackCollection(testTimeTracks);
        }
        private void PopulateHardcodedTestData()
        {
            var insertedUpdatedUser = Guid.NewGuid();
            var insertedUpdatedDate = DateTime.Now;

            var timeTrack = new TimeTrack
            {
                Id = Guid.NewGuid(),
                SecurityUserId = Guid.NewGuid(),
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                TimeTrackActivityTypeKey = "EI",
                ServiceTypeKey = "AdmP",
                ProgramTypeKey = "DNUPN",
                ServicePlanFundingSourceTypeKey = "APREV",
                TimeTrackSubActivityTypeKey = "AAAAA",
                OtherDescription = "test",
                Reason = "test",
                Comment = "test",
                ConversionId = Guid.NewGuid().ToString(),
                MemberId = Guid.NewGuid(),
                MemberProgramId = Guid.NewGuid(),
                ServiceAuthId = Guid.NewGuid(),
                TravelDuration = 10,
                RowFilterId = Guid.NewGuid(),
                TravelMiles = 1,
                ServiceUnits = 1,
                InsertedBy = insertedUpdatedUser,
                InsertedById = insertedUpdatedUser,
                InsertedOn = insertedUpdatedDate,
                UpdatedBy = insertedUpdatedUser,
                UpdatedById = insertedUpdatedUser,
                UpdatedOn = insertedUpdatedDate,
                EffectiveDate = insertedUpdatedDate,
                SegmentId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                TenantId = Guid.Parse("d5a28601-2b37-41dc-abd6-2b0d940c7938"),
                Version = 0
            };
            _testRepository.InsertTestTimeTrackCollection(new List<TimeTrack> { timeTrack });
        }


    }
}
