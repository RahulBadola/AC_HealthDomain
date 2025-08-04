using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace type_lookup_service.Model
{
    public class AssessmentTab
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid AssessmentTabId { get; set; }

        public Guid AssessmentPageId { get; set; }

        public int? Sequence { get; set; }

        public string TabName { get; set; }

        public string TabDescription { get; set; }

        public string Instructions { get; set; }

        public string AssessmentInitialStateTypeKey { get; set; }

        public string HelpText { get; set; }

        public string Score { get; set; }

        public bool UseXYCoorinate { get; set; }

        public string EffectiveDate { get; set; }

        public string ExpirationDate { get; set; }

        public Guid InsertedBy { get; set; }

        public Guid InsertedById { get; set; }

        public string InsertedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public Guid UpdatedById { get; set; }

        public string UpdatedOn { get; set; }

        public Guid? VoidedBy { get; set; }

        public Guid? VoidedById { get; set; }

        public string VoidedOn { get; set; }

        public Guid? VoidedReasonId { get; set; }

        public string ConversionId { get; set; }

        public int ActiveFlag { get; set; }

        public Guid SegmentId { get; set; }

        [BsonIgnore]
        public IEnumerable<AssessmentSection> Sections { get; set; }
    }

}
