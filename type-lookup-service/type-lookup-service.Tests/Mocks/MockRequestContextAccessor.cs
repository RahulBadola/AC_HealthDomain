using Moq;
using System;
using type_lookup_service.Model;
using type_lookup_service.Utils;

namespace type_lookup_service.Tests.Mocks
{
    public class MockRequestContextAccessor
    {
        public IRequestContextAccessor Object => CreateMock().Object;

        public Guid? SegmentId = Guid.NewGuid();

        public Guid? TenantId = Guid.NewGuid();

        public Guid UserContextId = Guid.NewGuid();

        public Guid UserId = Guid.NewGuid();

        private Mock<IRequestContextAccessor> CreateMock()
        {
            var returnValue = new Mock<IRequestContextAccessor>();

            var requestContext = new RequestContext
            {
                SegmentId = SegmentId,
                TenantId = TenantId,
                UserContextId = UserContextId,
                UserId = UserId,
            };

            returnValue
                .SetupGet(m => m.RequestContext)
                .Returns(requestContext);

            return returnValue;
        }
    }
}
