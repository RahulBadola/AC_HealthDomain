using System;
using System.Collections.Generic;

namespace time_track_service.Model.Dto.Legacy
{
    public class LegacyTimeTrackingsModel
    {
        public LegacyTimeTrackingsModel()
        {
            Items = new List<LegacyTimeTrackingsItemModel>();
        }
        public string ByActivity { get; set; }
        public Guid? StaffGuid { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TotalTime { get; set; }
        public DateTime? StartDate { get; set; }
        public List<LegacyTimeTrackingsItemModel> Items { get; set; }
    }
}
