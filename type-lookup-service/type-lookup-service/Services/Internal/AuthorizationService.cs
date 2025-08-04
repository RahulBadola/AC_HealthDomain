using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using type_lookup_service.Data;
using type_lookup_service.Model;
using type_lookup_service.Utils;

namespace type_lookup_service.Services.Internal
{
    internal class AuthorizationService : IAuthorizationService
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;

        public AuthorizationService(
            IContextLogger<AuthorizationService> logger,
            IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public Task<IEnumerable<RoleOperation>> QueryRoleOperationsAsync(OperationQuery query, CancellationToken cancellationToken = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));
            return InnerQueryRoleOperationsAsync(query, cancellationToken);
        }

        private async Task<IEnumerable<RoleOperation>> InnerQueryRoleOperationsAsync(OperationQuery query, CancellationToken cancellationToken = default)
        {
            _logger.QueryRoleOperationsCalled(query);

            if (query.Objects is null || !query.Objects.Any()) return new List<RoleOperation>();
            if (query.SecurityRoleIds == null || !query.SecurityRoleIds.Any()) return new List<RoleOperation>();

            var authorizationTable = await _repository.GetAuthorizationsAsync(query.SecurityRoleIds, query.Objects, cancellationToken);
            var returnValue = authorizationTable
                .Select(auth => new RoleOperation
                {
                    Object = auth.Object,
                    Operation = auth.Operation,
                    SecurityRoleId = auth.ObjectSid
                })
                .ToList();

            _logger.QueryRoleOperationsCompleted(returnValue);

            return returnValue;
        }
    }
}
