using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace type_lookup_service.Model
{
    public class MedicationSearchRequest
    {
        public List<Guid> MedicationIds { get; set; }
        public bool IncludeProdDesc { get; set; }
        public bool IncludePackDesc { get; set; }
        public bool IncludeManfacturer { get; set; }
    }
}
