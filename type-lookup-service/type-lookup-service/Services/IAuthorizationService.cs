using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using type_lookup_service.Model;

namespace type_lookup_service.Services
{
    public interface IAuthorizationService
    {
        public Task<IEnumerable<RoleOperation>> QueryRoleOperationsAsync(OperationQuery query, CancellationToken cancellationToken = default);
    }
}
