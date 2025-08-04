using System;

namespace time_track_service.Model.Dto.Legacy
{
    public class LegacyTimeTrackingsItemModel
    {
        public virtual Guid? Id { get; set; }
        public string Activity { get; set; }

        public DateTime? Date { get; set; }

        public string FundingSource { get; set; }

        public string Member { get; set; }

        public int? ServiceUnits { get; set; }

        public string SubActivity { get; set; }

        public string InsertedByName { get; set; }
        public int? TotalTime { get; set; }
    }
}
