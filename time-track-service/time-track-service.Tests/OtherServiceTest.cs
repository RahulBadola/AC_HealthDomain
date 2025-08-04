using time_track_service.Model;
using time_track_service.Model.ServiceDataObject;
using time_track_service.Services;
using time_track_service.Tests.Mocks;
using time_track_service.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Moq.Protected;
using System.Threading;
using System.Net;
using System.Collections.Generic;

namespace time_track_service.Tests
{
    public class OtherServiceTest
    {
        private readonly Mock<ContextLogger<IOtherServices>> contextlogger;
        private readonly MockRequestContextAccessor requestContextAccessor;
        private readonly Mock<HttpMessageHandler> mockHttpMessageHandler;
        private readonly HttpClient httpClient;
        private readonly Mock<ServicesSettings> servicesSettings;
        private readonly OtherServices otherServices;

        public OtherServiceTest()
        {
            contextlogger = new Mock<ContextLogger<IOtherServices>>(new Mock<IRequestContextAccessor>().Object, new Mock<ILogger<IOtherServices>>().Object);
            requestContextAccessor = new MockRequestContextAccessor();
            mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            httpClient = new HttpClient(mockHttpMessageHandler.Object);
            servicesSettings = new Mock<ServicesSettings>();

            otherServices = new OtherServices(httpClient, requestContextAccessor.Object, contextlogger.Object, servicesSettings.Object);
        }

        [Fact]
        public async Task GetMemberByIdAsyncynchould()
        {
            List<Guid> ids = new List<Guid>() { Guid.NewGuid() };
            servicesSettings.Object.MemberServiceUri = "http://localhost:5003/api/member/";

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(CreateMembersData())
                });

            var result = await otherServices.GetMemberDetailsAsync(ids);
            Assert.Equal("MFirstName", result[0].FirstName);
        }
        private string CreateMembersData()
        {
            Member member = new Member();
            member.FirstName = "MFirstName";
            return JsonConvert.SerializeObject(member);
        }
    }
}
