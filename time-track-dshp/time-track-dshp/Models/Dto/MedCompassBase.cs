using System;
using System.ComponentModel.DataAnnotations;
using time_track_dshp.Attributes;

namespace time_track_dshp.Models.Dto
{
    public class MedCompassBase : IBaseEntity
    {
        [Required]
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        [Required]
        public Guid InsertedBy { get; set; } // SecurityContextUserId
        [Required]
        public Guid InsertedById { get; set; } // SecurityUserId
        [Required]
        public DateTime InsertedOn { get; set; }
        [NonSqlProperty]
        public string InsertedByName { get; set; }
        [Required]
        public Guid UpdatedBy { get; set; }
        [Required]
        public Guid UpdatedById { get; set; }
        [Required]
        public DateTime UpdatedOn { get; set; }
        [NonSqlProperty]
        public string UpdatedByName { get; set; }

        public Guid? VoidedBy { get; set; }
        public Guid? VoidedById { get; set; }
        public DateTime? VoidedOn { get; set; }
        [NonSqlProperty]
        public string VoidedByName { get; set; }
        public Guid? VoidedReasonId { get; set; }
        public string ConversionId { get; set; }
        public Guid SegmentId { get; set; }

        [NonSqlProperty]
        public string TenantId { get; set; }
        [NonSqlProperty]
        public int Version { get; set; }

        internal int GetActiveFlag()
        {
            if (!VoidedOn.HasValue)
            {
                return 3;
            }
            else if (ExpirationDate.HasValue && ExpirationDate <= DateTime.UtcNow)
            {
                return 0;
            }
            else if (EffectiveDate > DateTime.UtcNow)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }
}
