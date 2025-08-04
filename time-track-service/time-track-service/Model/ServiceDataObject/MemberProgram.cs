using System;

namespace time_track_service.Model.ServiceDataObject
{
    public class MemberProgram
    {
        public Guid MemberId { get; set; }
        public Guid Id { get; set; }
        public string ProgramTypeKey { get; set; }
        public string ProgramSubProgramTypeKey { get; set; }
        public string ProgramStatusTypeKey { get; set; }
        public DateTime? ClosureDate { get; set; }
        public DateTime? EnrollmentDate { get; set; }
    }
}
