using time_track_service.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace time_track_service.Model.Dto
{
    [BsonIgnoreExtraElements]
    public abstract class MedCompassBase
    {
        [BsonId]
        public Guid Id { get; set; }

        public DateTime? EffectiveDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpirationDate { get; set; }

        public Guid InsertedById { get; set; }


        public Guid InsertedBy { get; set; }

        public string InsertedByName { get; set; }

        public DateTime? InsertedOn { get; set; } = DateTime.UtcNow;

        public Guid UpdatedById { get; set; }

        public Guid UpdatedBy { get; set; }

        public string UpdatedByName { get; set; }

        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

        public Guid? VoidedById { get; set; }

        public Guid? VoidedBy { get; set; }

        public string VoidedByName { get; set; }

        public DateTime? VoidedOn { get; set; }

        public Guid? VoidedReasonId { get; set; }

        public string ConversionId { get; set; }

        public Guid TenantId { get; set; } = Guid.Empty;

        public Guid SegmentId { get; set; } = Guid.Empty;

        public int? Version { get; set; }

        public int ActiveFlag
        {
            get
            {
                if (VoidedOn.HasValue)
                    return ActiveStatus.Deleted;
                else if (ExpirationDate.HasValue && ExpirationDate <= DateTime.UtcNow)
                    return ActiveStatus.Inactive;
                else if (EffectiveDate.HasValue && EffectiveDate > DateTime.UtcNow)
                    return ActiveStatus.Pending;
                else
                    return ActiveStatus.Active;
            }
        }
        
        public bool IsActive => ((ExpirationDate >= DateTime.UtcNow || ExpirationDate == null) && !VoidedOn.HasValue && (VoidedBy == Guid.Empty || !VoidedBy.HasValue));
    }
}