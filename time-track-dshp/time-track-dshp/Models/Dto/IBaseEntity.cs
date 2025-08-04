using System;

namespace time_track_dshp.Models.Dto
{
    public interface IBaseEntity
    {
        public Guid InsertedBy { get; set; } // SecurityContextUserId
        public Guid InsertedById { get; set; } // SecurityUserId
        public DateTime InsertedOn { get; set; }
        public string InsertedByName { get; set; }
        public Guid UpdatedBy { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedByName { get; set; }
        public Guid? VoidedBy { get; set; }
        public Guid? VoidedById { get; set; }
        public DateTime? VoidedOn { get; set; }
        public string VoidedByName { get; set; }
        public Guid? VoidedReasonId { get; set; }

        public Guid SegmentId { get; set; }
        public string TenantId { get; set; }
    }
}
