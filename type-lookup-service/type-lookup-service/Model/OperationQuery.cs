using System;

namespace type_lookup_service.Model
{
    public class OperationQuery
    {
        public Guid[] SecurityRoleIds { get; set; }
        public string[] Objects { get; set; }
    }
}
