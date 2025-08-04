using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using time_track_dshp.Attributes;
using time_track_dshp.Utils;

namespace time_track_dshp.Models.Dto.Debezium
{
    public class McBase : CdcBase
    {
        public string ConversionId { get; set; }

        [Required]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime EffectiveDate { get; set; }

        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? ExpirationDate { get; set; }

        [Required]
        public Guid InsertedBy { get; set; }
        
        [Required]
        public Guid InsertedById { get; set; }

        [NonSqlProperty]
        public string InsertedByName { get; set; }

        [Required]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime InsertedOn { get; set; }

        public Guid SegmentId { get; set; }

        [NonSqlProperty]
        public string TenantId { get; set; }

        [Required]
        public Guid UpdatedBy { get; set; }
        
        [Required]
        public Guid UpdatedById { get; set; }

        [NonSqlProperty]
        public string UpdatedByName { get; set; }

        [Required]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime UpdatedOn { get; set; }

        [NonSqlProperty]
        public int Version { get; set; }

        public Guid? VoidedBy { get; set; }
        
        public Guid? VoidedById { get; set; }

        [NonSqlProperty]
        public string VoidedByName { get; set; }

        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? VoidedOn { get; set; }
        
        public Guid? VoidedReasonId { get; set; }        
    }
}
