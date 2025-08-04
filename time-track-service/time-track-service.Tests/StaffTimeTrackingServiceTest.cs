using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Model.Dto.Legacy;
using time_track_service.Model.ServiceDataObject;
using time_track_service.Services;
using time_track_service.Tests.Mocks;
using time_track_service.Utils;
using Xunit;
using System.Threading;

namespace time_track_service.Tests
{
    public class StaffTimeTrackingServiceTest
    {
        private readonly ContextLogger<StaffTimeTrackingService> contextLogger;
        private readonly Mock<ITimeTrackRepository> timeTrackRepository;
        private readonly Mock<ISyncBackNotificationService> updateNotificationService;
        private readonly Mock<ISharedService> sharedService;
        private readonly MockRequestContextAccessor requestContextAccessor;
        private readonly Mock<IOtherServices> otherServices;
        private readonly StaffTimeTrackingService timetrackingervice;
        public StaffTimeTrackingServiceTest()
        {
            contextLogger = new Mock<ContextLogger<StaffTimeTrackingService>>(new Mock<IRequestContextAccessor>().Object, new Mock<ILogger<StaffTimeTrackingService>>().Object).Object;
            timeTrackRepository = new Mock<ITimeTrackRepository>();
            requestContextAccessor = new MockRequestContextAccessor();
            updateNotificationService = new Mock<ISyncBackNotificationService>();
            sharedService = new Mock<ISharedService>();
            otherServices = new Mock<IOtherServices>();
            timetrackingervice=new StaffTimeTrackingService
                (contextLogger,
                new MockFieldAuthorizationService(),
                new MockOperationAuthorizationService(),
                requestContextAccessor.Object, 
                updateNotificationService.Object, sharedService.Object,
                timeTrackRepository.Object, 
                otherServices.Object);
        }

        [Fact]
        public async Task GetTimeTrackingsAsync_DataFound()
        {
            var staffId = Guid.Empty;

            otherServices.Setup(c => c.GetLookUpDataAsync(It.IsAny<List<string>>())).ReturnsAsync(MockTypeLookupData());
            otherServices.Setup(c => c.ReadStaffAsync(It.IsAny<Guid>())).ReturnsAsync((new Staff()));
            timeTrackRepository.Setup(c=> c.GetTimeTrackBySecurityUserIdAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.Found, MockTimeTrackData()));
            otherServices.Setup(c => c.GetMemberDetailsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Member>());
            var result = await timetrackingervice.GetTimeTrackingsAsync(staffId);
            Assert.Equal(DbResponse.Found, result.response);
        }
        [Fact]
        public async Task GetTimeTrackingAsync_DataFound()
        {
            Guid id = Guid.Empty;

            otherServices.Setup(c => c.GetLookUpDataAsync(It.IsAny<List<string>>())).ReturnsAsync(MockTypeLookupData());
            timeTrackRepository.Setup(c => c.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync(MockTimeTrack());
            otherServices.Setup(c => c.GetMemberDetailsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Member>());
            otherServices.Setup(c => c.GetMemberProgramAsync(It.IsAny<Guid>())).ReturnsAsync(CreateMemberProgramData());
            var result = await timetrackingervice.GetTimeTrackingAsync(id);
            Assert.Equal(DbResponse.Found, result.response);
        }

        [Fact]
        public async Task GetTimeTrackingWithoutMemberIdAsync_DataFound()
        {
            Guid id = Guid.Empty;

            otherServices.Setup(c => c.GetLookUpDataAsync(It.IsAny<List<string>>())).ReturnsAsync(MockTypeLookupData());
            timeTrackRepository.Setup(c => c.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync(MockTimeTrackWithoutMemberId());
            otherServices.Setup(c => c.GetMemberDetailsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Member>());
            otherServices.Setup(c => c.GetMemberProgramAsync(It.IsAny<Guid>())).ReturnsAsync(CreateMemberProgramData());
            var result = await timetrackingervice.GetTimeTrackingAsync(id);
            Assert.Equal(DbResponse.Found, result.response);
        }
        [Fact]
        public async Task SaveTimeTrackAsync()
        {
            sharedService.Setup(c => c.SetInsertDefaults(It.IsAny<TimeTrack>()));
            otherServices.Setup(c => c.ReadStaffAsync(It.IsAny<Guid>())).ReturnsAsync(new Staff() { SecurityUserId = Guid.NewGuid() });
            timeTrackRepository.Setup(c => c.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.NotFound,( new TimeTrack() { Id=Guid.NewGuid()})));
            timeTrackRepository.Setup(c => c.InsertTimeTrackAsync(It.IsAny<TimeTrack>())).ReturnsAsync((DbResponse.Inserted));
            updateNotificationService.Setup(c => c.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<TimeTrack>(), It.IsAny<TimeTrack>())).ReturnsAsync(true);
            var result = await timetrackingervice.SaveTimeTrackingAsync(Guid.NewGuid(), CreateLegacyTimeTrack());
            Assert.Equal(DbResponse.Inserted, result.response);
        }

        [Fact]
        public async Task SaveNewTimeTrackAsync()
        {
            sharedService.Setup(c => c.SetInsertDefaults(It.IsAny<TimeTrack>()));
            otherServices.Setup(c => c.ReadStaffAsync(It.IsAny<Guid>())).ReturnsAsync(new Staff() { SecurityUserId = Guid.NewGuid() });
            timeTrackRepository.Setup(c => c.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.NotFound, (new TimeTrack() { })));
            timeTrackRepository.Setup(c => c.InsertTimeTrackAsync(It.IsAny<TimeTrack>())).ReturnsAsync((DbResponse.Inserted));
            updateNotificationService.Setup(c => c.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<TimeTrack>(), It.IsAny<TimeTrack>())).ReturnsAsync(true);
            var result = await timetrackingervice.SaveTimeTrackingAsync(Guid.NewGuid(), CreateNewLegacyTimeTrack());
            Assert.Equal(DbResponse.Inserted, result.response);
        }
        [Fact]
        public async Task SaveTimeTrackAsync_FailedSyncBackTest()
        {
            sharedService.Setup(c => c.SetInsertDefaults(It.IsAny<TimeTrack>()));
            otherServices.Setup(c => c.ReadStaffAsync(It.IsAny<Guid>())).ReturnsAsync(new Staff() { SecurityUserId = Guid.NewGuid() });
            timeTrackRepository.Setup(c => c.InsertTimeTrackAsync(It.IsAny<TimeTrack>())).ReturnsAsync((DbResponse.Inserted));
            updateNotificationService.Setup(c => c.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<TimeTrack>(), It.IsAny<TimeTrack>())).ReturnsAsync(false);
            var result = await timetrackingervice.SaveTimeTrackingAsync(Guid.NewGuid(), CreateLegacyTimeTrack());
            Assert.Equal(DbResponse.Error, result.response);
        }
        [Fact]
        public async Task UpdateTimeTrackAsync()
        {
            timeTrackRepository.Setup(c => c.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.Found, new TimeTrack()));
            sharedService.Setup(c => c.SetUpdateDefaults(It.IsAny<TimeTrack>()));
            otherServices.Setup(c => c.ReadStaffAsync(It.IsAny<Guid>())).ReturnsAsync(new Staff() { SecurityUserId = Guid.NewGuid() });
            sharedService.Setup(c => c.UpdateTimeTrackAsync(It.IsAny<TimeTrack>(), It.IsAny<CancellationToken>())).ReturnsAsync((DbResponse.Updated, new TimeTrack()));
            updateNotificationService.Setup(c => c.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<TimeTrack>(), It.IsAny<TimeTrack>())).ReturnsAsync(true);
            var result = await timetrackingervice.SaveTimeTrackingAsync(Guid.NewGuid(), CreateLegacyTimeTrack());       
            Assert.Equal(DbResponse.Updated, result.response);
        }
      

        [Fact]
        public async Task IsRequireStartAndEndDatesAsync_Positive()
        {
            otherServices.Setup(c => c.GetLookUpDataAsync(It.IsAny<List<string>>())).ReturnsAsync(MockTypeLookupData());

            var result = await timetrackingervice.IsRequireStartAndEndDatesAsync();
            Assert.True(result.data);
        }
        [Fact]
        public async Task SelectServiceUnitMaxValueAsync_DataFound()
        {
            otherServices.Setup(c => c.GetLookUpDataAsync(It.IsAny<List<string>>())).ReturnsAsync(MockTypeLookupData());

            var result = await timetrackingervice.SelectServiceUnitMaxValueAsync();
            Assert.Equal(DbResponse.Found, result.response);
        }
        private LegacyTimeTrackingModel CreateLegacyTimeTrack()
        {
            var legacyTimeTrackingModel = new LegacyTimeTrackingModel();
            legacyTimeTrackingModel.Activity = "TimeTrackActivityTypeKey";
            legacyTimeTrackingModel.AdditionalComments = "Comment";
            legacyTimeTrackingModel.StartDate = DateTime.Now;
            legacyTimeTrackingModel.EndDate = DateTime.Now;
            legacyTimeTrackingModel.FundingSource = "ServicePlanFundingSourceTypeKey";
            legacyTimeTrackingModel.Id = Guid.Parse("23c5ffab-76b9-4eb7-aca0-a80be497fe04");
            legacyTimeTrackingModel.MemberId = Guid.Parse("23c5ffab-76b9-4eb7-aca0-a80be497fe04");
            legacyTimeTrackingModel.MemberProgramId = Guid.Parse("23c5ffab-76b9-4eb7-aca0-a80be497fe04");
            legacyTimeTrackingModel.ProgramType = "ProgramTypeKey";
            legacyTimeTrackingModel.Reason = "Reason";
            legacyTimeTrackingModel.Service = "ServiceTypeKey";
            legacyTimeTrackingModel.ServiceUnits = 20;
            legacyTimeTrackingModel.SecurityUserId = Guid.Parse("23c5ffab-76b9-4eb7-aca0-a80be497fe04");
            legacyTimeTrackingModel.SubActivity ="TimeTrackSubActivityTypeKey";
            legacyTimeTrackingModel.TotalTime =20;
            legacyTimeTrackingModel.TravelDuration = 20;
            legacyTimeTrackingModel.TravelMiles = 20;
            legacyTimeTrackingModel.VolunteerDriver = true;
            return legacyTimeTrackingModel;
        }

        private LegacyTimeTrackingModel CreateNewLegacyTimeTrack()
        {
            var legacyTimeTrackingModel = new LegacyTimeTrackingModel();
            legacyTimeTrackingModel.Activity = "TimeTrackActivityTypeKey";
            legacyTimeTrackingModel.AdditionalComments = "Comment";
            legacyTimeTrackingModel.StartDate = DateTime.Now;
            legacyTimeTrackingModel.EndDate = DateTime.Now;
            legacyTimeTrackingModel.FundingSource = "ServicePlanFundingSourceTypeKey";
            legacyTimeTrackingModel.Id = null;
            legacyTimeTrackingModel.MemberId = Guid.Parse("23c5ffab-76b9-4eb7-aca0-a80be497fe04");
            legacyTimeTrackingModel.MemberProgramId = Guid.Parse("23c5ffab-76b9-4eb7-aca0-a80be497fe04");
            legacyTimeTrackingModel.ProgramType = "ProgramTypeKey";
            legacyTimeTrackingModel.Reason = "Reason";
            legacyTimeTrackingModel.Service = "ServiceTypeKey";
            legacyTimeTrackingModel.ServiceUnits = 20;
            legacyTimeTrackingModel.SecurityUserId = Guid.Parse("23c5ffab-76b9-4eb7-aca0-a80be497fe04");
            legacyTimeTrackingModel.SubActivity = "TimeTrackSubActivityTypeKey";
            legacyTimeTrackingModel.TotalTime = 20;
            legacyTimeTrackingModel.TravelDuration = 20;
            legacyTimeTrackingModel.TravelMiles = 20;
            legacyTimeTrackingModel.VolunteerDriver = true;
            return legacyTimeTrackingModel;
        }
        public GenericLookup MockTypeLookupData()
        {

            GenericLookup typeTables = new GenericLookup();

            typeTables.TimeTrackActivityType.Add(new Datum()
            {
                sequenceNumber = 1,
                key = "TimeTrackActivityType",
                typeDescription = "TimeSubTrack",
            });
            typeTables.ProgramStatusType.Add(new Datum()
            {
                sequenceNumber = 1,
                key = "ProgramStatusType",
                typeDescription = "TimeSubTrack",
            });
            typeTables.TimeTrackSubActivityType.Add(new Datum()
            {
                sequenceNumber = 1,
                key = "TimeTrackSubActivityType",
                typeDescription = "TimeSubTrack" ,
            }); 
            typeTables.siteConfiguration.Add(new SiteConfiguration()
            { key = "TT_ServiceUnitMax", configKey = "TT_ServiceUnitMax", configValue = "130", activeFlag = 1 });
            typeTables.siteConfiguration.Add(new SiteConfiguration()
            { key = "TIME_AutoCalcTime", configKey = "TIME_AutoCalcTime", configValue = "Yes", activeFlag = 1 });
            return typeTables;
        }
        private MemberProgram CreateMemberProgramData()
        {
            return new MemberProgram()
            {
                Id = Guid.Parse("23c5ffab-76b9-4eb7-aca0-a80be497fe04"),
                MemberId = Guid.NewGuid(),
                ProgramTypeKey = "ProgramType",
                ProgramSubProgramTypeKey = "ProgramSubProgramType",
                ProgramStatusTypeKey = "ProgramStatusType",
                ClosureDate = DateTime.Now.AddYears(10),
                EnrollmentDate = DateTime.Now.AddYears(-5),

            };
        }
        public List<TimeTrack> MockTimeTrackData()
        {
            List<TimeTrack> timetrack = new List<TimeTrack>();
            timetrack.Add(new TimeTrack()
            {
                Id = Guid.NewGuid(),
                SecurityUserId = Guid.NewGuid(),
                TimeTrackActivityTypeKey = "TT",
                ServiceTypeKey = "SER",
                ProgramTypeKey = "PK",
                ServicePlanFundingSourceTypeKey = "SP",
                TimeTrackSubActivityTypeKey = "TTS",
                ServiceAuthId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                StartDate = DateTime.Now.AddDays(-4),
                EndDate = DateTime.Now.AddDays(30),
                TotalTime = 23,
                GenerateUnit = false,
                CaseRecording = true,
                VolunteerDriver = true,
                IsSystemTime = true,
                ServiceUnits = 8,
                TravelDuration = 23,
                Reason = "reason",
                OtherDescription = "description",
                Comment = "comment",
                MemberProgramId = Guid.NewGuid()
            });
            return timetrack;
        }

        private (DbResponse response, TimeTrack data) MockTimeTrack()
        {
            return (DbResponse.Found, new TimeTrack() {
                Id = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                MemberProgramId = Guid.NewGuid(),
                SecurityUserId = Guid.NewGuid()

            });
        }

        private (DbResponse response, TimeTrack data) MockTimeTrackWithoutMemberId()
        {
            return (DbResponse.Found, new TimeTrack()
            {
                MemberId =null,
                MemberProgramId = null,
                SecurityUserId = Guid.NewGuid()

            });
        }
    }
}
