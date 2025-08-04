using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Model.Legacy;
using time_track_service.Model.ServiceDataObject;
using time_track_service.Services;
using time_track_service.Tests.Mocks;
using time_track_service.Utils;
using Xunit;

namespace time_track_service.Tests
{
    public class MemberTimeTrackingServiceTest
    {

        private readonly ContextLogger<MemberTimeTrackingService> contextLogger;
        private readonly Mock<ITimeTrackRepository> timeTrackRepository;
        private readonly Mock<ISyncBackNotificationService> updateNotificationService;
        private readonly Mock<ISharedService> sharedService;
        private readonly MockRequestContextAccessor requestContextAccessor;
        private readonly Mock<IOtherServices> otherServices;
        private readonly MemberTimeTrackingService memberTimetrackingervice;
        public MemberTimeTrackingServiceTest()
        {
            contextLogger = new Mock<ContextLogger<MemberTimeTrackingService>>(new Mock<IRequestContextAccessor>().Object, new Mock<ILogger<MemberTimeTrackingService>>().Object).Object;
            timeTrackRepository = new Mock<ITimeTrackRepository>();
            requestContextAccessor = new MockRequestContextAccessor();
            updateNotificationService = new Mock<ISyncBackNotificationService>();
            sharedService = new Mock<ISharedService>();
            otherServices = new Mock<IOtherServices>();
            memberTimetrackingervice = new MemberTimeTrackingService
                (contextLogger,
                new MockFieldAuthorizationService(),
                new MockOperationAuthorizationService(),
                timeTrackRepository.Object,
                requestContextAccessor.Object,
                updateNotificationService.Object,
                sharedService.Object,
                otherServices.Object);
        }
        [Fact]
        public async Task GetTimeTrackingsAsync_DataFound()
        {
            otherServices.Setup(m => m.GetMemberDetailsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(CreateMembersData());
            otherServices.Setup(m => m.GetMemberProgramsAsync(It.IsAny<Guid>())).ReturnsAsync(new List<MemberProgram>());
            timeTrackRepository.Setup(m => m.GetTimeTrackByMemberIdAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.Found, new List<Model.Dto.TimeTrack>()));
            var result = await memberTimetrackingervice.GetTimeTrackingsAsync(Guid.NewGuid());
            Assert.Equal(DbResponse.Found, result.response);
        }
        [Fact]
        public async Task GetTimeTrackingsAsync_DataNotFound()
        {
            otherServices.Setup(m => m.GetMemberDetailsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Member>());
            otherServices.Setup(m => m.GetMemberProgramsAsync(It.IsAny<Guid>())).ReturnsAsync(new List<MemberProgram>());
            timeTrackRepository.Setup(m => m.GetTimeTrackByMemberIdAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.NotFound, new List<Model.Dto.TimeTrack>()));
            var result = await memberTimetrackingervice.GetTimeTrackingsAsync(Guid.NewGuid());
            Assert.Equal(DbResponse.NotFound, result.response);
        }
        [Fact]
        public async Task GetTimeTrackingAsync_DataFound()
        {
            otherServices.Setup(m => m.GetMemberProgramAsync(It.IsAny<Guid>())).ReturnsAsync(new MemberProgram());
            otherServices.Setup(m => m.GetMemberDetailAsync(It.IsAny<Guid>())).ReturnsAsync(CreateMemberData());
            otherServices.Setup(c => c.GetLookUpDataAsync(It.IsAny<List<string>>())).ReturnsAsync(MockTypeLookupData());
            timeTrackRepository.Setup(m => m.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.Found, MockTimeTrack()));
            var result = await memberTimetrackingervice.GetTimeTrackingAsync(Guid.NewGuid());
            Assert.Equal(DbResponse.Found, result.response);
        }
        [Fact]
        public async Task GetTimeTrackingAsync_DataNotFound()
        {
            otherServices.Setup(m => m.GetMemberDetailsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Member>());
            otherServices.Setup(m => m.GetMemberProgramsAsync(It.IsAny<Guid>())).ReturnsAsync(new List<MemberProgram>());
            timeTrackRepository.Setup(m => m.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.NotFound, MockTimeTrack()));
            var result = await memberTimetrackingervice.GetTimeTrackingAsync(Guid.NewGuid());
            Assert.Equal(DbResponse.NotFound, result.response);
        }
        [Fact]
        public async Task SaveTimeTrackAsync()
        {
            sharedService.Setup(c => c.SetInsertDefaults(It.IsAny<TimeTrack>()));
            otherServices.Setup(c => c.ReadStaffAsync(It.IsAny<Guid>())).ReturnsAsync(new Staff() { SecurityUserId = Guid.NewGuid() });
            timeTrackRepository.Setup(c => c.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.NotFound, (new TimeTrack() { Id = Guid.NewGuid() })));
            timeTrackRepository.Setup(c => c.InsertTimeTrackAsync(It.IsAny<TimeTrack>())).ReturnsAsync((DbResponse.Inserted));
            updateNotificationService.Setup(c => c.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<TimeTrack>(), It.IsAny<TimeTrack>())).ReturnsAsync(true);
            var result = await memberTimetrackingervice.SaveTimeTrackingAsync(Guid.NewGuid(), MockLegacyTimeTrack());
            Assert.Equal(DbResponse.Inserted, result.response);
        }
        [Fact]
        public async Task SaveTimeTrackAsync_FailedSyncBackTest()
        {
            sharedService.Setup(c => c.SetInsertDefaults(It.IsAny<TimeTrack>()));
            otherServices.Setup(c => c.ReadStaffAsync(It.IsAny<Guid>())).ReturnsAsync(new Staff() { SecurityUserId = Guid.NewGuid() });
            timeTrackRepository.Setup(c => c.InsertTimeTrackAsync(It.IsAny<TimeTrack>())).ReturnsAsync((DbResponse.Inserted));
            updateNotificationService.Setup(c => c.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<TimeTrack>(), It.IsAny<TimeTrack>())).ReturnsAsync(false);
            var result = await memberTimetrackingervice.SaveTimeTrackingAsync(Guid.NewGuid(), MockLegacyTimeTrack());
            Assert.Equal(DbResponse.Error, result.response);
        }
        [Fact]
        public async Task UpdateTimeTrackAsync()
        {
            timeTrackRepository.Setup(c => c.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.Found, new TimeTrack()));
            sharedService.Setup(c => c.SetUpdateDefaults(It.IsAny<TimeTrack>()));
            sharedService.Setup(c => c.UpdateTimeTrackAsync(It.IsAny<TimeTrack>(), It.IsAny<CancellationToken>())).ReturnsAsync((DbResponse.Updated, new TimeTrack()));
            updateNotificationService.Setup(c => c.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<TimeTrack>(), It.IsAny<TimeTrack>())).ReturnsAsync(true);
            var result = await memberTimetrackingervice.SaveTimeTrackingAsync(Guid.NewGuid(), MockLegacyTimeTrack());
            Assert.Equal(DbResponse.Updated, result.response);
        }
        [Fact]
        public async Task SaveTimeTrackAsync_ErrorTest()
        {
            sharedService.Setup(c => c.SetInsertDefaults(It.IsAny<TimeTrack>()));
            timeTrackRepository.Setup(c => c.GetTimeTrackAsync(It.IsAny<Guid>())).ReturnsAsync((DbResponse.Found, new TimeTrack()));
            timeTrackRepository.Setup(c => c.InsertTimeTrackAsync(It.IsAny<TimeTrack>())).ReturnsAsync((DbResponse.Error));
            updateNotificationService.Setup(c => c.SyncBackAsync(It.IsAny<SyncBackOperations>(), It.IsAny<TimeTrack>(), It.IsAny<TimeTrack>())).ReturnsAsync(false);
            var result = await memberTimetrackingervice.SaveTimeTrackingAsync(Guid.NewGuid(), MockLegacyTimeTrack());
            Assert.Equal(DbResponse.Error, result.response);
        }
        private TimeTrack MockTimeTrack()
        {
            return new TimeTrack()
            {
                MemberId = Guid.NewGuid(),
                MemberProgramId = Guid.NewGuid()

            };
        }
        private LegacyMemberTimeTrackingModel MockLegacyTimeTrack()
        {
            return new LegacyMemberTimeTrackingModel()
            {
                StartDate = DateTime.Now.AddDays(-5),
                EndDate =DateTime.Now.AddDays(-5),

            };
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
                typeDescription = "TimeSubTrack",
            });
            typeTables.siteConfiguration.Add(new SiteConfiguration()
            { key = "TT_ServiceUnitMax", configKey = "TT_ServiceUnitMax", configValue = "130", activeFlag = 1 });
            typeTables.siteConfiguration.Add(new SiteConfiguration()
            { key = "Require_CaseProgram_Selection_On_MemberObjects", configKey = "TIME_AutoCalcTime", configValue = "Yes", activeFlag = 1 });
            return typeTables;
        }

        private List<Member> CreateMembersData()
        {
            List<Member> member = new List<Member>();
            member.Add(new Member
            {
                FirstName = "MFirstName"
            });
            return member;
        }

        private Member CreateMemberData()
        {
            Member member = new Member();
            member.FirstName = "MFirstName";
            return member;
        }
    }
}
