using time_track_service.Model;
using time_track_service.Utils;
using Moq;
using System;

namespace time_track_service.Tests.Mocks
{
    public class MockRequestContextAccessor
    {
        private Guid SegmentId = Guid.NewGuid();

        private Guid TenantId = Guid.NewGuid();

        private Guid UserContextId = Guid.NewGuid();

        private Guid UserId = Guid.Parse("23c5ffab-76b9-4eb7-aca0-a80be497fe04");

        public IRequestContextAccessor Object => Accessor.Object;

        private Mock<IRequestContextAccessor> Accessor { get; }

        public MockRequestContextAccessor()
        {
            Accessor = new Mock<IRequestContextAccessor>();

            var requestContext = new RequestContext
            {
                SegmentId = SegmentId,
                TenantId = TenantId,
                UserContextId = UserContextId,
                UserId = UserId

            };
            Accessor.SetupGet(accessor => accessor.RequestContext).Returns(requestContext);
        }

    }
}
