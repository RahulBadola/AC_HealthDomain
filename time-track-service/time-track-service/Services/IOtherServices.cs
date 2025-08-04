using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_service.Model.ServiceDataObject;

namespace time_track_service.Services
{
    public interface IOtherServices
    {
        Task<GenericLookup> GetLookUpDataAsync(List<string> typeNames);
        Task<MemberProgram> GetMemberProgramAsync(Guid id);
        Task<List<MemberProgram>> GetMemberProgramsAsync(Guid memberId);
        Task<Staff> ReadStaffAsync(Guid id);
        Task<List<Member>> GetMemberDetailsAsync(IList<Guid> memberIds);
        Task<Member> GetMemberDetailAsync(Guid memberId);
        Task<string> RequestTypeLookupDataAsync(List<string> typeNames, string url, string logPrefix);
    }
}