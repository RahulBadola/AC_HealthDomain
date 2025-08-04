using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace type_lookup_service.Model
{
    public class AssessmentPage
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public Guid AssessmentPageId { get; set; }

        public Guid AssessmentTemplateId { get; set; }

        public int? Sequence { get; set; }

        public string PageName { get; set; }

        public string PageDescription { get; set; }

        public string Instructions { get; set; }

        public string AssessmentInitialStateTypeKey { get; set; }

        public string HelpText { get; set; }

        public string Score { get; set; }

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
        public IEnumerable<AssessmentTab> Tabs { get; set; }
    }
}
