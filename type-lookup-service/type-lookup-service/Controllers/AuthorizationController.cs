using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using type_lookup_service.Model;
using type_lookup_service.Services;

namespace type_lookup_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Route("operations/query")]
        public async Task<IEnumerable<RoleOperation>> QueryRoleOperations(OperationQuery query, CancellationToken cancellationToken = default)
            => await _authorizationService.QueryRoleOperationsAsync(query, cancellationToken);
    }
}