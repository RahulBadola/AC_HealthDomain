using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace time_track_service.Model.Legacy
{
    public class LegacyMemberTimeTrackingsModel
    {
        public int? TotalTime { get; set; }
        public Guid MemberGuid { get; set; }
        public List<LegacyMemberTimeTrackingsItemModel> Items { get; set; }
        
    }
}
