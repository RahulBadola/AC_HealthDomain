using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace time_track_service.Model.ServiceDataObject
{
    public class MemberCaseModel
    {
        public string Case { get; set; }
        public Guid? MemberProgramId { get; set; }
        public string ProgramTypeKey { get; set; }
    }
}
